// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using HttpApiClient;
using HttpApiClient.Requests;
using MvvmToolkit.Commands;
using MvvmToolkit.Messages;
using Problemator.Core.Dtos;
using Problemator.Core.Messages;
using Problemator.Core.Utils;

namespace Problemator.Core.ViewModels
{
    public class TickDetailsViewModel : INotifyPropertyChanged
    {
        #pragma warning disable CS0067
        // Is used by Fody to add NotifyPropertyChanged on properties.
        public event PropertyChangedEventHandler PropertyChanged;

        public DateTime SelectedDate { get; set; }

        public IList<Problem> Problems { get; set; }

        private bool _isSingleSelection;
        public bool IsSingleSelection
        {
            get { return _isSingleSelection; }
            set
            {
                _isSingleSelection = value;
                if (_isSingleSelection)
                {
                    SelectedGradeIdx = GradesIds.IndexOf(Problems.Single().GradeId);
                }
            }
        }

        public int SelectedGradeIdx { get; set; }
        public IDictionary<string, Grade> Grades { get; set; }
        private IList<string> GradesIds => Grades?.Keys.ToList();

        public int SelectedAscentIdx { get; set; }
        public IDictionary<int, string> AscentTypes { get; private set; } =
            new Dictionary<int, string>
            {
                { 0, "lead" },
                { 1, "toprope" },
            };

        private readonly IEventAggregator _eventAggregator;
        private readonly ProblematorRequestsFactory _requestsFactory;

        public TickDetailsViewModel(
            IEventAggregator eventAggregator,
            ProblematorRequestsFactory requestsFactory)
        {
            _eventAggregator = eventAggregator;
            _requestsFactory = requestsFactory;
        }

        #region Tick

        private RelayCommand _tickComand;

        public RelayCommand TickCommand => _tickComand ??
            (_tickComand = new RelayCommand(async () => await TickAsync()));

        private async Task TickAsync()
        {
            _eventAggregator.PublishOnCurrentThread(new BusyMessage(true));

            var responses = await Task.WhenAll(Problems
                .Select(x => _requestsFactory.CreateUpdateTickRequest(
                        CreateTick(x, NoTries, SelectedDate, SelectedAscentIdx))
                    .RunAsync<ProblematorJsonParser>()));

            var failedToTickTags = GetFailedToTickTags(responses).ToArray();
            _eventAggregator.PublishOnCurrentThread(new TickProblemsMesage(failedToTickTags));

            _eventAggregator.PublishOnCurrentThread(new BusyMessage(false));
        }

        private IEnumerable<string> GetFailedToTickTags(
            IEnumerable<Response<ProblematorJsonParser>> responses)
        {
            var failedRequests = GetFailedRequests(responses).ToList();
            if (!failedRequests.Any())
            {
                return Enumerable.Empty<string>();
            }

            var ids = failedRequests.Select(x =>
                x.Config.Params[_requestsFactory.ProblemIdParamKey]);

            return Problems.Where(x => ids.Any(id =>
                    string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase)))
                .Select(x => x.TagShort);
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
            var gradeOpinionId = problem.GradeId;
            if (IsSingleSelection)
            {
                gradeOpinionId = GradesIds.ElementAt(SelectedGradeIdx);
            }
            return new Tick
            {
                Tries = tries,
                Timestamp = timestamp,
                ProblemId = problem.Id,
                AscentType = ascentType,
                GradeOpinionId = gradeOpinionId,
            };
        }

        #endregion

        #region Tryes

        private const int MinNoTries = 1;
        private const int MaxNoTries = 50;

        public int NoTries { get; set; } = MinNoTries;

        private RelayCommand _decrementTriesCommand;

        public RelayCommand DecrementTriesCommand => _decrementTriesCommand ??
            (_decrementTriesCommand = new RelayCommand(DecrementTries, () => NoTries > MinNoTries));

        private void DecrementTries()
        {
            NoTries--;
        }

        private RelayCommand _incrementTriesCommand;

        public RelayCommand IncrementTicksCommand => _incrementTriesCommand ??
            (_incrementTriesCommand = new RelayCommand(IncrementTicks, () => NoTries < MaxNoTries));

        private void IncrementTicks()
        {
            NoTries++;
        }

        #endregion
    }
}