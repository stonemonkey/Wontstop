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
using HttpApiClient.Requests;
using System.Diagnostics;

namespace Wontstop.Climb.Ui.Uwp.ViewModels
{
    [ImplementPropertyChanged]
    public class ProblemsViewModel : IHandle<Problem>
    {
        public string Title => "Ticks";

        public bool Busy { get; private set; }
        public bool Empty { get; private set; }

        public string Tags { get; set; }

        private static bool _isDaySaved;
        public static DateTimeOffset Day { get; set; }

        public IList<Problem> SuggestedProblems { get; set; }

        public IList<DateTime> TickDates { get; private set; }

        public ObservableCollection<Problem> TickedProblems { get; private set; }

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

        private List<string> _tags;
        private List<Problem> _problems;

        private async Task RefreshAsync()
        {
            Busy = true;

            ClearTaggedProblems();

            await LoadProblemsAsync();
            await LoadTicksForDay(Day);
            await LoadTickDatesAsync();

            Busy = false;
            Empty = (_problems == null) || !_problems.Any();
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

            var sections = parser.To<IDictionary<string, WallSection>>()
                .Values.ToList();
            _problems = sections
                .SelectMany(x => x.Problems)
                .ToList();
            _tags = _problems
                .Select(x => x.TagShort)
                .ToList();
        }

        private async Task LoadTicksForDay(DateTimeOffset day)
        {
            (await _requestsFactory.CreateDayTicksRequest(day.Date)
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
            TickedProblems = new ObservableCollection<Problem>(_problems
                .Where(x => day.Ticks.Any(tick => 
                    string.Equals(x.Id, tick.ProblemId, StringComparison.OrdinalIgnoreCase)))
                .ToList());
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

            TickDates = parser.To<IList<DateTime>>();
        }

        private RelayCommand _unloadComand;
        public RelayCommand UnloadCommand => _unloadComand ??
            (_unloadComand = new RelayCommand(Unload));

        private void Unload()
        {
            _eventAggregator.Unsubscribe(this);
        }

        private RelayCommand _dateChangedComand;

        public RelayCommand DateChangedCommand => _dateChangedComand ??
            (_dateChangedComand = new RelayCommand(async () => await RefreshAsync(), () => !Busy));

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
                TagsChanged, byUser => !Busy && byUser && !string.IsNullOrWhiteSpace(Tags)));

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
                AddTaggedProblem(lastTag);
                PublishTags();
                return;
            }

            SuggestedProblems = _problems
                .Where(x => 
                    IsAvailaleAt(x, Day.Date) &&
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

        private IList<Problem> _taggedProblems = new List<Problem>();

        private void ClearTaggedProblems() // TODO: clear tagged problems when Tags is cleared
        {
            _taggedProblems.Clear();
        }

        private void AddTaggedProblem(string tag)
        {
            if (_taggedProblems.Any(x => string.Equals(tag, x.TagShort, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            var problem = _problems
                .Where(x =>
                    IsAvailaleAt(x, Day.Date) &&
                    x.TagShort.Equals(tag, StringComparison.OrdinalIgnoreCase))
                .First();

            _taggedProblems.Add(problem);
        }

        private List<Problem> GetTaggedProblems()
        {
            var tags = SplitTags();

            return _taggedProblems
                .Where(x => tags.Any(tag => string.Equals(tag, x.TagShort, StringComparison.OrdinalIgnoreCase)))
                .ToList();
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

        private RelayCommand _tickComand;

        public RelayCommand TickCommand => _tickComand ??
            (_tickComand = new RelayCommand(
                async () => await TickAsync(), () => !Busy && !string.IsNullOrWhiteSpace(Tags)));

        // TODO: resolve tries and ascent type
        private const int DefaultNoTries = 1;
        private const int DefaultAscentType = 0;

        private async Task TickAsync()
        {
            Busy = true;

            if (!ShowErrorForInexistentTags())
            {
                if (await SaveTaggedTicksAsync())
                {
                    Tags = null;
                    ClearTaggedProblems();
                }

                await LoadProblemsAsync();
                await LoadTicksForDay(Day);
            }

            Busy = false;
            SuggestedProblems = null;
            Empty = (_problems == null) || !_problems.Any();
        }

        private async Task<bool> SaveTaggedTicksAsync()
        {
            if (IsSelectedDayToday())
            {
                return await SaveTicksForTodayAsync(Tags);
            }

            return await SaveTicksForDayAsync(Day, DefaultNoTries, DefaultAscentType);
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

            var tags = SplitTags();

            return tags.Where(x => !_tags.Contains(x))
                .ToList();
        }

        private string[] SplitTags()
        {
            return Tags.Replace(" ", null)
                .Split(new[] { TicksSeparator }, StringSplitOptions.RemoveEmptyEntries);
        }

        private bool IsSelectedDayToday()
        {
            return Day.Date == _timeService.Now.Date;
        }

        private async Task<bool> SaveTicksForTodayAsync(string tags)
        {
            var successfull = false;

            var response = (await _requestsFactory.CreateSaveTicksRequest(tags.ToUpper())
                .RunAsync<ProblematorJsonParser>())
                    .PublishErrorOnHttpFailure(_eventAggregator)
                    .OnSuccess(x => successfull = x.PublishMessageOnInternalServerError(_eventAggregator));

            return successfull;
        }

        private async Task<bool> SaveTicksForDayAsync(DateTimeOffset day, int tries, int ascentType)
        {
            var problems = GetTaggedProblems();

            var responses = await Task.WhenAll(problems
                .Select(x => _requestsFactory.CreateUpdateTickRequest(
                        CreateTick(x, tries, Day.Date, ascentType))
                    .RunAsync<ProblematorJsonParser>()));

            return !ShowErrorForUnsavedTicks(responses);
        }

        private bool ShowErrorForUnsavedTicks(IEnumerable<Response<ProblematorJsonParser>> responses)
        {
            var failedRequests = GetFailedRequests(responses);
            if (!failedRequests.Any())
            {
                return false;
            }

            var ids = failedRequests.Select(x =>
                x.Config.Params[_requestsFactory.ProblemIdParamKey]);

            var tags = _problems.Where(x => ids.Any(id =>
                    string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase)))
                .Select(x => x.TagShort);

            _eventAggregator.PublishErrorMessageOnCurrentThread(
                $"Unable to save: {string.Join(TicksSeparator, tags)}");

            return true;
        }

        public IEnumerable<IRequest> GetFailedRequests(
            IEnumerable<Response<ProblematorJsonParser>> responses)
        {
            return responses
                .Where(x => x.Failed())
                .Select(x => x.Request);
        }

        private Tick CreateTick(Problem problem, int tries, DateTime timestamp, int ascentType)
        {
            return new Tick
            {
                Tries = tries,
                Timestamp = timestamp,
                ProblemId = problem.Id,
                AscentType = ascentType,
                GradeOpinionId = problem.GradeId,
            };
        }

        public void Handle(Problem message)
        {
            TickedProblems.Remove(message);
        }
    }
}
