// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Threading.Tasks;
using HttpApiClient;
using MvvmToolkit.Attributes;
using MvvmToolkit.Commands;
using MvvmToolkit.Messages;
using MvvmToolkit.Services;
using Problemator.Core.Dtos;
using Problemator.Core.Messages;
using Problemator.Core.Utils;

namespace Problemator.Core.ViewModels
{
    public class ProblemItemViewModel : 
        IHandle<BusyMessage>,
        INotifyPropertyChanged
    {
        #pragma warning disable CS0067
        // Is used by Fody to add NotifyPropertyChanged on properties.
        public event PropertyChangedEventHandler PropertyChanged;

        public bool HasTick { get; private set; }

        private WallProblem _problem;
        [Model]
        public WallProblem Problem
        {
            get { return _problem; }
            set
            {
                _problem = value;
                UpdateHasTick();
            }
        }

        public Problem Details { get; private set; }

        private bool _busy;

        private readonly IEventAggregator _eventAggregator;
        private readonly INavigationService _navigationService;
        private readonly ProblematorRequestsFactory _requestFactory;

        public ProblemItemViewModel(
            IEventAggregator eventAggregator,
            INavigationService navigationService,
            ProblematorRequestsFactory requestFactory)
        {
            _eventAggregator = eventAggregator;
            _navigationService = navigationService;
            _requestFactory = requestFactory;
        }

        private RelayCommand _loadComand;
        public RelayCommand LoadCommand => _loadComand ??
            (_loadComand = new RelayCommand(async () => await LoadAsync()));

        private async Task LoadAsync()
        {
            _eventAggregator.Subscribe(this);

            if (HasTick)
            {
                await LoadProblemAsync();
            }
        }

        private async Task LoadProblemAsync()
        {
            (await _requestFactory.CreateProblemRequest(Problem.ProblemId)
                .RunAsync<ProblematorJsonParser>())
                    .OnSuccess(p =>
                    {
                        var data = p.GetData();
                        Details = data["problem"].ToObject<Problem>();
                    })
                    .PublishErrorOnAnyFailure(_eventAggregator);
        }

        private RelayCommand _unloadComand;
        public RelayCommand UnloadCommand => _unloadComand ??
            (_unloadComand = new RelayCommand(Unload));

        private void Unload()
        {
            _eventAggregator.Unsubscribe(this);
        }

        //private RelayCommand _addTickCommand;
        //public RelayCommand AddTickCommand => _addTickCommand ??
        //    (_addTickCommand = new RelayCommand(AddTick, () => !_busy));

        //private void AddTick()
        //{
        //    _eventAggregator.PublishShowBusy();

        //    // TODO: implement
        //    UpdateHasTick();

        //    _eventAggregator.PublishHideBusy();
        //}

        private RelayCommand _deleteTickCommand;
        public RelayCommand DeleteTickCommand => _deleteTickCommand ??
            (_deleteTickCommand = new RelayCommand(
                async () => await DeleteTickAsync(), () => !_busy));

        private async Task DeleteTickAsync()
        {
            _eventAggregator.PublishShowBusy();

            await RemoveTickAsync();
            UpdateHasTick();

            _eventAggregator.PublishHideBusy();
        }

        private void UpdateHasTick()
        {
            HasTick = _problem?.Tick != null;
        }

        private async Task RemoveTickAsync()
        {
            (await _requestFactory.CreateDeleteTickRequest(Problem.Tick.Id)
                .RunAsync<ProblematorJsonParser>())
                    .OnSuccess(p =>
                    {
                        _eventAggregator.PublishRemove(Problem.Tick);
                    })
                    .PublishErrorOnAnyFailure(_eventAggregator);
        }

        private RelayCommand _openDetailsComand;

        public RelayCommand OpenDetailsCommand => _openDetailsComand ??
            (_openDetailsComand = new RelayCommand(OpenDetails, () => !_busy));

        private void OpenDetails()
        {
            _navigationService.Navigate<ProblemDetailesViewModel>(Problem);
        }

        public void Handle(BusyMessage message)
        {
            _busy = message.Show;
        }
    }
}