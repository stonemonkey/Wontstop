// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HttpApiClient;
using Mvvm.WinRT.Commands;
using Mvvm.WinRT.Messages;
using PropertyChanged;
using Wontstop.Climb.Ui.Uwp.Dtos;
using Wontstop.Climb.Ui.Uwp.Utils;
using Mvvm.WinRT;
using System.Collections.ObjectModel;

namespace Wontstop.Climb.Ui.Uwp.ViewModels
{
    [ImplementPropertyChanged]
    public class ProblemsViewModel : IHandle<Problem>
    {
        public string Title => "Ticks today";

        public bool Busy { get; private set; }
        public bool Empty { get; private set; }

        public string Tags { get; set; }

        public IList<Problem> SuggestedProblems { get; set; }

        public IList<WallSection> Sections { get; private set; }

        public ObservableCollection<Problem> Ticks { get; private set; }

        private readonly ITimeService _timeService;
        private readonly IEventAggregator _eventAggregator;
        private readonly ProblematorRequestsFactory _requestsFactory;

        public ProblemsViewModel(
            ITimeService timeService,
            IEventAggregator eventAggregator,
            ProblematorRequestsFactory requestsFactory)
        {
            _timeService = timeService;
            _eventAggregator = eventAggregator;
            _requestsFactory = requestsFactory;
        }

        private RelayCommand _loadComand;
        public RelayCommand LoadCommand => _loadComand ??
            (_loadComand = new RelayCommand(async () => await LoadAsync()));

        protected virtual async Task LoadAsync()
        {
            Busy = true;

            await LoadSectionsAsync();
            await LoadTicksForToday();

            Busy = false;
            Empty = (Sections == null) || !Sections.Any();

            _eventAggregator.Subscribe(this);
        }

        private async Task LoadSectionsAsync()
        {
            (await _requestsFactory.CreateProblemsRequest()
                .RunAsync<ProblematorJsonParser>())
                    .OnSuccess(HandleProblemsResponse)
                    .PublishErrorOnFailure(_eventAggregator);
        }

        private void HandleProblemsResponse(ProblematorJsonParser parser)
        {
            if (parser.PublishMessageOnError(_eventAggregator))
            {
                return;
            }

            Sections = parser.To<IDictionary<string, WallSection>>()
                .Values.ToList();
            _tags = Sections.SelectMany(x => x.Problems)
                .Select(x => x.TagShort)
                .ToList();
        }

        private async Task LoadTicksForToday()
        {
            (await _requestsFactory.CreateDayTicksRequest(_timeService.Now)
                .RunAsync<ProblematorJsonParser>())
                    .OnSuccess(HandleTicksResponse)
                    .PublishErrorOnFailure(_eventAggregator);
        }

        private void HandleTicksResponse(ProblematorJsonParser parser)
        {
            if (parser.PublishMessageOnError(_eventAggregator))
            {
                return;
            }

            var day = parser.To<DayTicks>();
            Ticks = new ObservableCollection<Problem>(
                Sections.SelectMany(x => x.Problems)
                .Where(x => day.Ticks.Any(t => string.Equals(x.Id, t.ProblemId)))
                .ToList());
        }
        
        private RelayCommand _unloadComand;
        public RelayCommand UnloadCommand => _unloadComand ??
            (_unloadComand = new RelayCommand(Unload));

        private void Unload()
        {
            _eventAggregator.Unsubscribe(this);
        }

        private RelayCommand _publisTagsComand;
        public RelayCommand PublishTagsCommand => _publisTagsComand ??
            (_publisTagsComand = new RelayCommand(PublishTags));

        private void PublishTags()
        {
            _eventAggregator.PublishOnCurrentThread(Tags ?? string.Empty);
        }

        //private RelayCommand<string> _tagsSelectedComand;

        //public RelayCommand<string> TagsSelectedCommand => _tagsSelectedComand ??
        //    (_tagsSelectedComand = new RelayCommand<string>(
        //        TagsSelected, tag => !Busy && !string.IsNullOrWhiteSpace(tag)));

        //private void TagsSelected(string tags)
        //{
        //    Tags = tags;

        //    PublishTags();
        //}

        private RelayCommand<bool> _tagsChangedComand;

        public RelayCommand<bool> TagsChangedCommand => _tagsChangedComand ??
            (_tagsChangedComand = new RelayCommand<bool>(
                TagsChanged, byUser => !Busy && byUser && !string.IsNullOrWhiteSpace(Tags)));

        private List<string> _tags;

        private const int TagMinLength = 2;

        private void TagsChanged(bool byUser)
        {
            var lastTag = GetLastTagFrom(Tags);
            if (lastTag.Length < TagMinLength)
            {
                SuggestedProblems = null;
                return;
            }
            if (_tags.Contains(lastTag))
            {
                PublishTags();
                return;
            }

            SuggestedProblems = Sections.SelectMany(x => x.Problems)
                .Where(x => x.TagShort.StartsWith(lastTag, StringComparison.OrdinalIgnoreCase))
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

            PublishTags();
        }

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

        private RelayCommand _tickComand;

        public RelayCommand TickCommand => _tickComand ??
            (_tickComand = new RelayCommand(
                async () => await TickAsync(), () => !Busy && !string.IsNullOrWhiteSpace(Tags)));

        private const string TicksSeparator = ",";

        private async Task TickAsync()
        {
            Busy = true;

            if (!ShowErrorForInexistentTags())
            {
                await SaveTicksAsync(Tags.ToUpper());
                await LoadTicksForToday();
                Tags = null;
            }

            Busy = false;
            SuggestedProblems = null;
            Empty = (Sections == null) || !Sections.Any();
        }

        private bool ShowErrorForInexistentTags()
        {
            var inexisten = GetInexistenTags();
            if (inexisten.Any())
            {
                _eventAggregator.PublishErrorMessageOnCurrentThread(
                    $"Unable to find: {string.Join(TicksSeparator, inexisten)}");

                return true;
            }

            return false;
        }

        private List<string> GetInexistenTags()
        {
            if (Tags == null)
            {
                return new List<string>();
            }

            var tags = Tags.Replace(" ", null)
                .Split(new[] {TicksSeparator}, StringSplitOptions.RemoveEmptyEntries);
            
            return tags.Where(x => !_tags.Contains(x))
                .ToList();
        }

        private async Task SaveTicksAsync(string ticks)
        {
            await (await _requestsFactory.CreateSaveTicksRequest(ticks)
                .RunAsync<ProblematorJsonParser>())
                    .PublishErrorOnFailure(_eventAggregator)
                    .OnSuccessAsync(HandleSaveTicksResponse);
        }

        private async Task HandleSaveTicksResponse(ProblematorJsonParser parser)
        {
            if (parser.PublishMessageOnError(_eventAggregator))
            {
                return;
            }

            await LoadSectionsAsync();
        }

        public void Handle(Problem message)
        {
            Ticks.Remove(message);
        }

        //public void Handle(Problem message)
        //{
        //    if (string.IsNullOrWhiteSpace(Tags))
        //    {
        //        Tags = message.TagShort;
        //        return;
        //    }

        //    var exists = Tags.Contains(message.TagShort);
        //    if (exists)
        //    {
        //        Tags = Tags
        //            .Replace($"{message.TagShort},", null)
        //            .Replace($",{message.TagShort}", null)
        //            .Replace(message.TagShort, null);
        //    }
        //    else
        //    {
        //        Tags = Tags.Trim(',', ' ') + $",{message.TagShort}";
        //    }
        //}
    }
}
