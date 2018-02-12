﻿// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using HttpApiClient;
using MvvmToolkit;
using MvvmToolkit.Commands;
using MvvmToolkit.Messages;
using Problemator.Core.Dtos;
using Problemator.Core.Messages;
using Problemator.Core.Utils;
using PropertyChanged;

namespace Problemator.Core.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class TicksViewModel : IHandle<Tick>, IHandle<BusyMessage>, IHandle<TickProblemsMesage>
    {
        public string Title => "Ticks";

        public bool Busy { get; private set; }
        public bool Empty { get; private set; }
        public bool CanTick { get; private set; }

        public string Tags { get; set; }

        public string SelectedLocation { get; set; }
        public IDictionary<string, string> Locations { get; set; }

        private static bool _isDaySaved;
        public static DateTimeOffset Day { get; set; }
        private static DateTime SelectedDate => Day.Date;

        public TickDetailsViewModel Details { get; }

        public IList<Problem> SuggestedProblems { get; set; }

        public IList<DateTimeOffset> TickDates { get; private set; }

        public ObservableCollection<Tick> Ticks { get; private set; }
    

        private readonly ITimeService _timeService;
        private readonly IStorageService _storageService;
        private readonly IEventAggregator _eventAggregator;
        private readonly ProblematorRequestsFactory _requestsFactory;

        public TicksViewModel(
            ITimeService timeService,
            IStorageService storageService,
            IEventAggregator eventAggregator,
            TickDetailsViewModel tickDetailsViewModel,
            ProblematorRequestsFactory requestsFactory)
        {
            _timeService = timeService;
            _storageService = storageService;
            _eventAggregator = eventAggregator;
            _requestsFactory = requestsFactory;

            Details = tickDetailsViewModel;
            Details.Problems = _taggedProblems;

            if (!_isDaySaved)
            {
                Day = _timeService.Now;
                _isDaySaved = true;
            }
        }

        private RelayCommand _loadComand;
        public RelayCommand LoadCommand => _loadComand ??
            (_loadComand = new RelayCommand(async () => await LoadAsync()));

        private UserContext _context;

        protected virtual async Task LoadAsync()
        {
            _context = _storageService.ReadLocal<UserContext>(Settings.ContextKey);

            await RefreshAsync();

            _eventAggregator.Subscribe(this);
        }

        private List<string> _tags;
        private List<Problem> _problems;

        private async Task RefreshAsync()
        {
            Busy = true;
            Details.SelectedDate = SelectedDate;

            ClearTaggedProblems();

            await LoadDashboardAsync();
            await LoadProblemsAsync();
            await LoadTickDatesAsync();
            await LoadTicks(SelectedDate);

            Busy = false;
            UpdateEmpty();
        }

        private async Task LoadDashboardAsync()
        {
            (await _requestsFactory.CreateDashboardRequest()
                    .RunAsync<ProblematorJsonParser>())
                .OnSuccess(HandleDashboardResponse)
                .PublishErrorOnHttpFailure(_eventAggregator);
        }

        private void HandleDashboardResponse(ProblematorJsonParser parser)
        {
            if (parser.PublishMessageOnInternalServerError(_eventAggregator))
            {
                return;
            }

            var dashboard = parser.To<Dashboard>();
            Details.SelectedAscentIdx = dashboard.UserSettings.AscentType;
            Locations = dashboard.Locations.ToDictionary(x => x.Name, x => x.Id);
            SelectedLocation = dashboard.Locations
                .SingleOrDefault(x => string.Equals(x.Id,_context.GymId, StringComparison.Ordinal))?
                .Name;
        }

        private async Task LoadProblemsAsync()
        {
            (await _requestsFactory.CreateWallSectionsRequest()
                .RunAsync<ProblematorJsonParser>())
                    .OnSuccess(HandleWallSectionsResponse)
                    .PublishErrorOnHttpFailure(_eventAggregator);
        }

        private void HandleWallSectionsResponse(ProblematorJsonParser parser)
        {
            if (parser.PublishMessageOnInternalServerError(_eventAggregator))
            {
                return;
            }

            var sections = new ObservableCollection<WallSection>((parser.To<IDictionary<string, WallSection>>() ?? 
                    new Dictionary<string, WallSection>())
                .Values);
            _problems = sections
                .SelectMany(x => x.Problems)
                .ToList();
            _tags = _problems
                .Select(x => x.TagShort)
                .ToList();
        }

        private async Task LoadTicks(DateTime day)
        {
            (await _requestsFactory.CreateDayTicksRequest(day)
                .RunAsync<ProblematorJsonParser>())
                    .OnSuccess(HandleTicksResponse)
                    .PublishErrorOnHttpFailure(_eventAggregator);
        }

        private void HandleTicksResponse(ProblematorJsonParser parser)
        {
            if (parser.PublishMessageOnInternalServerError(_eventAggregator))
            {
                return;
            }

            var day = parser.To<DayTicks>();
            Ticks = new ObservableCollection<Tick>(day.Ticks);
        }

        private async Task LoadTickDatesAsync()
        {
            (await _requestsFactory.CreateTickDatesRequest()
                .RunAsync<ProblematorJsonParser>())
                    .OnSuccess(HandleTickDatesResponse)
                    .PublishErrorOnHttpFailure(_eventAggregator);
        }

        private void HandleTickDatesResponse(ProblematorJsonParser parser)
        {
            if (parser.PublishMessageOnInternalServerError(_eventAggregator))
            {
                return;
            }

            TickDates = parser.To<IList<DateTimeOffset>>(); 
        }

        private RelayCommand _unloadComand;
        public RelayCommand UnloadCommand => _unloadComand ??
            (_unloadComand = new RelayCommand(Unload));

        private void Unload()
        {
            _eventAggregator.Unsubscribe(this);
        }

        private RelayCommand _changeLocationComand;

        public RelayCommand ChangeLocationCommand => _changeLocationComand ??
            (_changeLocationComand = new RelayCommand(
                async () => await ChangeLocationAsync(), () => !Busy));

        private async Task ChangeLocationAsync()
        {
            var gymId = Locations[SelectedLocation];
            if (gymId == _context.GymId)
            {
                return;
            }

            (await _requestsFactory.CreateChangeGymRequest(gymId)
                    .RunAsync<ProblematorJsonParser>())
                .OnSuccess(x => HandleChangeGymRequest(x, gymId))
                .PublishErrorOnHttpFailure(_eventAggregator);

            await RefreshAsync();
        }

        private void HandleChangeGymRequest(ProblematorJsonParser parser, string gymId)
        {
            var context = parser.To<UserContext>();
            _context.GymId = gymId;
            _context.Jwt = context.Jwt;
            _context.Message = context.Message;
            _requestsFactory.SetUserContext(_context);
            _storageService.SaveLocal(Settings.ContextKey, _context);
        }

        private RelayCommand _changeDateComand;

        public RelayCommand ChangeDateCommand => _changeDateComand ??
            (_changeDateComand = new RelayCommand(async () => await RefreshAsync(), () => !Busy));

        private RelayCommand _publisTagsComand;
        public RelayCommand PublishTagsCommand => _publisTagsComand ??
            (_publisTagsComand = new RelayCommand(PublishTags));

        private void PublishTags()
        {
            _eventAggregator.PublishOnCurrentThread(Tags ?? string.Empty);
        }

        private RelayCommand<bool> _tagsChangedComand;

        public RelayCommand<bool> TagsChangedCommand => _tagsChangedComand ??
            (_tagsChangedComand = new RelayCommand<bool>(
                TagsChanged));

        private const int TagMinLength = 2;

        private void TagsChanged(bool byUser)
        {
            if (!byUser || Busy || string.IsNullOrWhiteSpace(Tags))
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
            if (_tags.Contains(lastTag))
            {
                AddTaggedProblem(lastTag);
                PublishTags();
                return;
            }

            SuggestedProblems = _problems
                .Where(x => 
                    IsAvailaleAt(x, SelectedDate) &&
                    x.TagShort.StartsWith(lastTag, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        private RelayCommand<string> _suggestionChosenComand;

        public RelayCommand<string> SuggestionChosenCommand => _suggestionChosenComand ??
            (_suggestionChosenComand = new RelayCommand<string>(
                SuggestionChosen, tag => !Busy && !string.IsNullOrWhiteSpace(tag)));

        private void SuggestionChosen(string tag)
        {
            var lastTag = GetLastTagFrom(Tags);
            var tags = Tags.Substring(0, Tags.Length - lastTag.Length);
            Tags = tags + tag;

            AddTaggedProblem(tag);
            PublishTags();
        }

        private readonly IList<Problem> _taggedProblems = new List<Problem>();

        private void ClearTaggedProblems()
        {
            _taggedProblems.Clear();
        }

        private void AddTaggedProblem(string tag)
        {
            if (_taggedProblems.Any(x => string.Equals(tag, x.TagShort, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            var problem = _problems.First(x => 
                IsAvailaleAt(x, SelectedDate) && x.TagShort.Equals(tag, StringComparison.OrdinalIgnoreCase));

            _taggedProblems.Add(problem);
        }

        private bool IsAvailaleAt(Problem problem, DateTime date)
        {
            if (problem.Removed == null)
            {
                return true;
            }

            var removed = DateTime.Parse(problem.Removed);

            return removed > date;
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

        private void UpdateEmpty()
        {
            Empty = _problems == null || !_problems.Any();
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

            return tags.Where(x => !_tags.Contains(x))
                .ToList();
        }

        private string[] SplitTags()
        {
            return Tags.Replace(" ", null)
                .Split(new[] { TicksSeparator }, StringSplitOptions.RemoveEmptyEntries);
        }

        public async void Handle(TickProblemsMesage message)
        {
            if (message.Successfull)
            {
                Tags = null;
                ClearTaggedProblems();
            }
            else
            {
                ShowErrorForFailedToTickTags(message.FailedToTickTags);
            }

            await LoadProblemsAsync();
            await LoadTicks(SelectedDate);

            UpdateEmpty();
            SuggestedProblems = null;
        }

        private void ShowErrorForFailedToTickTags(IEnumerable<string> tags)
        {
            _eventAggregator.PublishErrorMessageOnCurrentThread(
                $"Unable to save: {string.Join(TicksSeparator, tags)}");
        }

        public void Handle(BusyMessage message)
        {
            Busy = message.Show;
        }

        public void Handle(Tick message)
        {
            Ticks.Remove(message);
        }
    }
}
