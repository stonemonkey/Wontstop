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

namespace Wontstop.Climb.Ui.Uwp.ViewModels
{
    [ImplementPropertyChanged]
    public class ProblemsViewModel
    {
        public string Title => "Problems";

        public bool Busy { get; private set; }
        public bool Empty { get; private set; }

        public string Tags { get; set; }

        public IList<Problem> SuggestedProblems { get; set; }

        public IList<WallSection> Sections { get; private set; }

        private readonly IEventAggregator _eventAggregator;
        private readonly ProblematorRequestsFactory _requestsFactory;

        public ProblemsViewModel(
            IEventAggregator eventAggregator,
            ProblematorRequestsFactory requestsFactory)
        {
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

            Busy = false;
            Empty = (Sections == null) || !Sections.Any();
        }

        private async Task LoadSectionsAsync()
        {
            (await _requestsFactory.CreateProblemsRequest()
                .RunAsync<ProblematorJsonParser>())
                    .OnSuccess(HandleResponse)
                    .OnRequestFailure(x =>
                        _eventAggregator.PublishOnCurrentThread(x.Exception))
                    .OnResponseFailure(x =>
                        _eventAggregator.PublishOnCurrentThread(new ErrorMessage(x.GetContent())));
        }

        private List<string> _tags;

        private void HandleResponse(ProblematorJsonParser parser)
        {
            if (parser.IsError())
            {
                _eventAggregator.PublishOnCurrentThread(
                    new ErrorMessage(parser.GetErrorMessage()));
            }
            else
            {
                Sections = parser.To<IDictionary<string, WallSection>>()
                    .Values.ToList();
                _tags = Sections.SelectMany(x => x.Problems)
                    .Select(x => x.TagShort)
                    .ToList();
            }
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
            //var problem = SuggestedProblems.Single(x =>
            //    string.Equals(tag, x.TagShort, StringComparison.OrdinalIgnoreCase));
            //if (problem.IsVisible)
            //{
                var lastTag = GetLastTagFrom(Tags);

                Tags = Tags.Replace(lastTag, tag);
            //}
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

            if (ValidateTicks())
            {
                // TODO: send it to Problemator ...
                await LoadSectionsAsync();

                Tags = null;
            }

            Busy = false;
            SuggestedProblems = null;
            Empty = (Sections == null) || !Sections.Any();
        }

        private bool ValidateTicks()
        {
            var inexisten = GetInexistenTags();
            if (inexisten.Any())
            {
                _eventAggregator.PublishOnCurrentThread(
                    new ErrorMessage($"Unable to find: {string.Join(TicksSeparator, inexisten)}"));

                return false;
            }

            return true;
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
    }
}
