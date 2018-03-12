// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using HttpApiClient;
using HttpApiClient.Requests;
using MvvmToolkit;
using MvvmToolkit.Commands;
using MvvmToolkit.Messages;
using Problemator.Core.Dtos;
using Problemator.Core.Messages;
using Problemator.Core.Models;
using Problemator.Core.Utils;

namespace Problemator.Core.ViewModels
{
    public class TagProblemsViewModel : 
        IHandle<LocationChangedMessage>,
        IHandle<DayChangedMessage>,
        IHandle<TickAddMesage>,
        INotifyPropertyChanged
    {
        #pragma warning disable CS0067
        // Is used by Fody to add NotifyPropertyChanged on properties.
        public event PropertyChangedEventHandler PropertyChanged;

        public string Tags { get; set; }

        public IList<WallProblem> SuggestedProblems { get; set; }

        public bool CanTick { get; private set; }

        public bool IsSingleSelection { get; private set; }

        public bool NoProblemsAvailable { get; private set; }

        public Grade SelectedGrade { get; set; }
        public IList<Grade> Grades { get; private set; }

        public string SelectedAscentType { get; set; }
        public IList<string> AscentTypes { get; private set; }

        private bool _busy;
        private DateTime _selectedDate;

        private readonly Session _session;
        private readonly Sections _sections;
        private readonly ITimeService _timeService;
        private readonly IEventAggregator _eventAggregator;
        private readonly ProblematorRequestsFactory _requestsFactory;

        public TagProblemsViewModel(
            Session session,
            Sections sections,
            ITimeService timeService,
            IEventAggregator eventAggregator,
            ProblematorRequestsFactory requestsFactory)
        {
            _session = session;
            _sections = sections;
            _timeService = timeService;
            _eventAggregator = eventAggregator;
            _requestsFactory = requestsFactory;

            _selectedDate = _timeService.Now;
        }

        #region Load/Unload handlers

        private RelayCommand _loadComand;
        public RelayCommand LoadCommand => _loadComand ??
            (_loadComand = new RelayCommand(async () => await LoadAsync()));

        private async Task LoadAsync()
        {
            _eventAggregator.Subscribe(this);

            ClearTaggedProblems();

            await _session.LoadAsync(false);

            Grades = await _session.GetGradesAsync();
            AscentTypes = _session.GetSportAscentTypes();
            SelectedAscentType = await _session.GetUserSportAscentType();

            await _sections.LoadAsync();

            NoProblemsAvailable = !_sections.HasProblems();
        }

        private RelayCommand _unloadComand;
        public RelayCommand UnloadCommand => _unloadComand ??
            (_unloadComand = new RelayCommand(Unload));

        private void Unload()
        {
            _eventAggregator.Unsubscribe(this);
        }

        #endregion

        #region Suggestions

        private RelayCommand<bool> _tagsChangedComand;

        public RelayCommand<bool> TagsChangedCommand => _tagsChangedComand ??
            (_tagsChangedComand = new RelayCommand<bool>(TagsChanged));

        private const int TagMinLength = 2;

        private void TagsChanged(bool byUser)
        {
            if (!byUser || _busy || string.IsNullOrWhiteSpace(Tags))
            {
                CanTick = false;
                return;
            }
            CanTick = AreAllTagsValid();

            var lastTag = GetLastTagFrom(Tags);
            if (lastTag.Length < TagMinLength)
            {
                SuggestedProblems = null;
                return;
            }
            if (_sections.ContainsProblem(lastTag))
            {
                AddTaggedProblem(lastTag);
                return;
            }

            SuggestedProblems = _sections.GetAvailableProblems(lastTag, _selectedDate);
        }

        private RelayCommand<string> _suggestionChosenComand;

        public RelayCommand<string> SuggestionChosenCommand => _suggestionChosenComand ??
            (_suggestionChosenComand = new RelayCommand<string>(
                SuggestionChosen, tag => !_busy && !string.IsNullOrWhiteSpace(tag)));

        private void SuggestionChosen(string tag)
        {
            var lastTag = GetLastTagFrom(Tags);
            var tags = Tags.Substring(0, Tags.Length - lastTag.Length);
            Tags = tags + tag;

            AddTaggedProblem(tag);
        }

        private readonly IList<WallProblem> _taggedProblems = new List<WallProblem>();

        private void ClearTaggedProblems()
        {
            Tags = null;
            _taggedProblems.Clear();
            IsSingleSelection = false;
        }

        private void AddTaggedProblem(string tag)
        {
            if (_taggedProblems.Any(x => string.Equals(tag, x.TagShort, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            _taggedProblems.Add(_sections.GetFirstAvailableProblem(tag, _selectedDate));

            IsSingleSelection = _taggedProblems.Count == 1;
            if (IsSingleSelection)
            {
                var gradeId = _taggedProblems.Single().GradeId;
                SelectedGrade = Grades.GetById(gradeId);
            }
        }

        private const string TicksSeparator = ",";

        private static string GetLastTagFrom(string text)
        {
            var lastTag = text.Replace(" ", null);

            var lastTagSeparatorIdx = lastTag.LastIndexOf(TicksSeparator, StringComparison.Ordinal);
            if (lastTagSeparatorIdx >= 0)
            {
                lastTag = lastTag.Remove(0, lastTagSeparatorIdx + 1);
            }

            return lastTag;
        }

        private bool AreAllTagsValid()
        {
            var inexisten = GetInexistenTags();

            return !inexisten.Any();
        }

        private List<string> GetInexistenTags()
        {
            if (Tags == null)
            {
                return new List<string>();
            }

            var tags = SplitTags();

            return tags.Where(x => !_sections.ContainsProblem(x))
                .ToList();
        }

        private string[] SplitTags()
        {
            return Tags.Replace(" ", null)
                .Split(new[] { TicksSeparator }, StringSplitOptions.RemoveEmptyEntries);
        }

        #endregion

        #region Tick

        public int TriesCount { get; set; }

        private RelayCommand _tickComand;

        public RelayCommand TickCommand => _tickComand ??
            (_tickComand = new RelayCommand(async () => await TickAsync()));

        private async Task TickAsync()
        {
            _eventAggregator.PublishShowBusy();

            var responses = await Task.WhenAll(_taggedProblems
                .Select(x => _requestsFactory.CreateUpdateTickRequest(
                        CreateTick(x, TriesCount, _selectedDate, SelectedAscentType))
                    .RunAsync<ProblematorJsonParser>()));

            var failedToAddTags = GetFailedToTickTags(responses);
            _eventAggregator.PublishFailedToAdd(failedToAddTags);

            _eventAggregator.PublishHideBusy();
        }

        private string[] GetFailedToTickTags(
            IEnumerable<Response<ProblematorJsonParser>> responses)
        {
            var failedRequests = GetFailedRequests(responses).ToList();
            if (!failedRequests.Any())
            {
                return new string[] { };
            }

            var ids = failedRequests.Select(x =>
                x.Config.Params[_requestsFactory.ProblemIdParamKey]);

            return _taggedProblems.Where(x => ids.Any(id =>
                    string.Equals(x.ProblemId, id, StringComparison.OrdinalIgnoreCase)))
                .Select(x => x.TagShort)
                .ToArray();
        }

        public IEnumerable<IRequest> GetFailedRequests(
            IEnumerable<Response<ProblematorJsonParser>> responses)
        {
            return responses
                .Where(x => x.Failed())
                .Select(x => x.Request);
        }

        private Tick CreateTick(WallProblem problem, int tries, DateTime timestamp, string ascentType)
        {
            var gradeOpinionId = problem.GradeId;
            if (IsSingleSelection)
            {
                gradeOpinionId = SelectedGrade.Id;
            }

            var ascentTypeId = _session.GetSportAscentTypeId(ascentType);

            return new Tick
            {
                Tries = tries,
                Timestamp = timestamp,
                ProblemId = problem.ProblemId,
                AscentTypeId = ascentTypeId,
                GradeOpinionId = gradeOpinionId,
            };
        }

        #endregion

        #region Message handlers

        public async void Handle(LocationChangedMessage message)
        {
            await _sections.LoadAsync();

            ClearTaggedProblems();
            NoProblemsAvailable = !_sections.HasProblems();
        }

        public void Handle(DayChangedMessage message)
        {
            _selectedDate = message.NewDay.Date;

            ClearTaggedProblems();
        }

        public async void Handle(TickAddMesage message)
        {
            if (message.Successfull)
            {
                ClearTaggedProblems();
            }

            await _sections.LoadAsync();

            SuggestedProblems = null;
        }

        public void Handle(BusyMessage message)
        {
            _busy = message.Show;
        }

        #endregion
    }
}