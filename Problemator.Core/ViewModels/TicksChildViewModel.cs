﻿// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using HttpApiClient;
using MvvmToolkit;
using MvvmToolkit.Commands;
using MvvmToolkit.Messages;
using Problemator.Core.Dtos;
using Problemator.Core.Messages;
using Problemator.Core.Models;
using Problemator.Core.Utils;

namespace Problemator.Core.ViewModels
{
    public class TicksChildViewModel : IHandle<Tick>, IHandle<BusyMessage>, IHandle<TickProblemsMesage>, INotifyPropertyChanged
    {
        #pragma warning disable CS0067
        // Is used by Fody to add NotifyPropertyChanged on properties.
        public event PropertyChangedEventHandler PropertyChanged;

        public string Title => "Ticks";

        public bool Busy { get; private set; }
        public bool Empty { get; private set; }
        public bool CanTick { get; private set; }
        public bool NoProblemsAvailable { get; private set; }

        public string Tags { get; set; }

        public string SelectedLocation { get; set; }
        public IList<string> Locations { get; private set; }

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
        private readonly Session _session;
        private readonly Sections _sections;

        private readonly ProblematorRequestsFactory _requestsFactory;

        public TicksChildViewModel(
            ITimeService timeService,
            IStorageService storageService,
            IEventAggregator eventAggregator,
            Session session,
            Sections sections,
            TickDetailsViewModel tickDetailsViewModel,
            ProblematorRequestsFactory requestsFactory)
        {
            _timeService = timeService;
            _storageService = storageService;
            _eventAggregator = eventAggregator;
            _session = session;
            _sections = sections;
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

        protected virtual async Task LoadAsync()
        {
            await RefreshAsync();

            _eventAggregator.Subscribe(this);
        }

        private async Task RefreshAsync()
        {
            Busy = true;
            // TODO: move to session
            Details.SelectedDate = SelectedDate;

            ClearTaggedProblems();

            await _sections.LoadAsync();
            await LoadTickDatesAsync();
            await LoadTicksAsync(SelectedDate);

            Locations = await _session.GetLocationNames();
            SelectedLocation = await _session.GetCurrentLocationName();

            Busy = false;
            UpdateEmpty();
            NoProblemsAvailable = !_sections.HasProblems();
        }

        private async Task LoadTicksAsync(DateTime day)
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
            Ticks = null;
            TickDates = null;

            _eventAggregator.Unsubscribe(this);
        }

        private RelayCommand _changeLocationComand;

        public RelayCommand ChangeLocationCommand => _changeLocationComand ??
            (_changeLocationComand = new RelayCommand(
                async () => await ChangeLocationAsync(), () => !Busy));

        private async Task ChangeLocationAsync()
        {
            await _session.SetCurrentLocationAsync(SelectedLocation);
            await RefreshAsync();
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
            if (_sections.ContainsProblem(lastTag))
            {
                AddTaggedProblem(lastTag);
                PublishTags();
                return;
            }

            SuggestedProblems = _sections.GetAvailableProblems(lastTag, SelectedDate);
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

            _taggedProblems.Add(_sections.GetFirstAvailableProblem(tag, SelectedDate));
            Details.IsSingleSelection = _taggedProblems.Count == 1;
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
            Empty = Ticks == null || !Ticks.Any();
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

            await _sections.LoadAsync();
            await LoadTicksAsync(SelectedDate);

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
