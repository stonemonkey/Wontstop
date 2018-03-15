// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Threading.Tasks;
using MvvmToolkit.Attributes;
using MvvmToolkit.Commands;
using MvvmToolkit.Messages;
using MvvmToolkit.Services;
using Problemator.Core.Dtos;
using Problemator.Core.Messages;
using Problemator.Core.Models;

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

        private WallProblem _wallProblem;
        [Model]
        public WallProblem Problem
        {
            get { return _wallProblem; }
            set
            {
                _wallProblem = value;
                UpdateHasTick();
            }
        }

        public ProblemDetails Details { get; private set; }

        private bool _busy;

        private readonly Ticks _ticks;
        private readonly Problem _problem;
        private readonly IEventAggregator _eventAggregator;
        private readonly INavigationService _navigationService;

        public ProblemItemViewModel(
            Ticks ticks,
            Problem problem,
            IEventAggregator eventAggregator,
            INavigationService navigationService)
        {
            _ticks = ticks;
            _problem = problem;
            _eventAggregator = eventAggregator;
            _navigationService = navigationService;
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
            Details = await _problem.GetDetailsAsync(Problem.Id);
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

            await _ticks.DeleteTickAsync(Problem.Tick);
            UpdateHasTick();

            _eventAggregator.PublishHideBusy();
        }

        private void UpdateHasTick()
        {
            HasTick = _wallProblem?.Tick != null;
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