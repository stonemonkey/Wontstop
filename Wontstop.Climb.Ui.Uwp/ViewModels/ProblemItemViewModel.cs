// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using HttpApiClient;
using PropertyChanged;
using System.Threading.Tasks;
using MvvmToolkit.Commands;
using MvvmToolkit.Messages;
using MvvmToolkit.WinRT.AttachedProperties;
using Wontstop.Climb.Ui.Uwp.Dtos;
using Wontstop.Climb.Ui.Uwp.Utils;

namespace Wontstop.Climb.Ui.Uwp.ViewModels
{
    [ImplementPropertyChanged]
    public class ProblemItemViewModel
    {
        public bool Busy { get; private set; }
     
        public bool HasTick { get; private set; }

        private Problem _problem;

        [Model]
        public Problem Problem
        {
            get { return _problem; }
            set
            {
                _problem = value;
                UpdateHasTick();
            }
        }

        private readonly IEventAggregator _eventAggregator;
        private readonly ProblematorRequestsFactory _requestFactory;

        public ProblemItemViewModel(
            IEventAggregator eventAggregator,
            ProblematorRequestsFactory requestFactory)
        {
            _eventAggregator = eventAggregator;
            _requestFactory = requestFactory;
        }

        private RelayCommand _loadComand;
        public RelayCommand LoadCommand => _loadComand ??
            (_loadComand = new RelayCommand(Load));

        private void Load()
        {
            _eventAggregator.Subscribe(this);
        }

        private RelayCommand _unloadComand;
        public RelayCommand UnloadCommand => _unloadComand ??
            (_unloadComand = new RelayCommand(Unload));

        private void Unload()
        {
            _eventAggregator.Unsubscribe(this);
        }

        private RelayCommand _addTickCommand;
        public RelayCommand AddTickCommand => _addTickCommand ??
            (_addTickCommand = new RelayCommand(AddTick, () => !Busy));

        private void AddTick()
        {
            Busy = true;

            // TODO: implement
            UpdateHasTick();

            Busy = false;
        }

        private RelayCommand _deleteTickCommand;
        public RelayCommand DeleteTickCommand => _deleteTickCommand ??
            (_deleteTickCommand = new RelayCommand(
                async () => await DeleteTickAsync(), () => !Busy));

        private async Task DeleteTickAsync()
        {
            Busy = true;

            await RemoveTickAsync();
            UpdateHasTick();

            Busy = false;
        }

        private void UpdateHasTick()
        {
            HasTick = _problem?.Tick != null;
        }

        private async Task RemoveTickAsync()
        {
            (await _requestFactory.CreateDeleteTickRequest(Problem.Tick.Id)
                .RunAsync<ProblematorJsonParser>())
                    .OnSuccess(HandleRemoveResponse)
                    .PublishErrorOnHttpFailure(_eventAggregator);
        }

        private void HandleRemoveResponse(ProblematorJsonParser parser)
        {
            if (parser.PublishMessageOnInternalServerError(_eventAggregator))
            {
                return;
            }

            _eventAggregator.PublishOnCurrentThread(Problem.Tick);
        }

        //private RelayCommand _tickComand;

        //public RelayCommand TickCommand => _tickComand ??
        //    (_tickComand = new RelayCommand(
        //        async () => await TickAsync(), () => !Busy));

        //// TODO: resolve tries and ascent type
        //private const int DefaultNoTries = 1;
        //private const int DefaultAscentType = 0;

        //private async Task TickAsync()
        //{
        //    Busy = true;

        //    //await SaveTickAsync();

        //    await LoadProblemsAsync();
        //    await LoadTicks(SelectedDate);

        //    Busy = false;
        //    Empty = _problems == null || !_problems.Any();
        //}

        //private async Task<bool> SaveTickAsync(Problem problem)
        //{
        //    if (IsSelectedDayToday())
        //    {
        //        return await SaveTickForTodayAsync(problem.TagShort);
        //    }

        //    return await SaveTickForDateAsync(SelectedDate, DefaultNoTries, DefaultAscentType, problem);
        //}

        //private bool IsSelectedDayToday()
        //{
        //    return SelectedDate == _timeService.Now.Date;
        //}

        //private async Task<bool> SaveTickForTodayAsync(string tag)
        //{
        //    var successfull = false;

        //    (await _requestsFactory.CreateSaveTicksRequest(tag.ToUpper())
        //        .RunAsync<ProblematorJsonParser>())
        //            .PublishErrorOnHttpFailure(_eventAggregator)
        //            .OnSuccess(x => successfull = x.PublishMessageOnInternalServerError(_eventAggregator));

        //    return successfull;
        //}

        //private async Task<bool> SaveTickForDateAsync(DateTime day, int tries, int ascentType, Problem problem)
        //{
        //    var successfull = false;

        //    (await _requestsFactory.CreateUpdateTickRequest(CreateTick(problem, tries, day, ascentType))
        //        .RunAsync<ProblematorJsonParser>())
        //            .PublishErrorOnHttpFailure(_eventAggregator)
        //            .OnSuccess(x => successfull = x.PublishMessageOnInternalServerError(_eventAggregator));

        //    return successfull;
        //}

        //private static Tick CreateTick(Problem problem, int tries, DateTime timestamp, int ascentType)
        //{
        //    return new Tick
        //    {
        //        Tries = tries,
        //        Timestamp = timestamp,
        //        ProblemId = problem.Id,
        //        AscentType = ascentType,
        //        GradeOpinionId = problem.GradeId,
        //    };
        //}
    }
}