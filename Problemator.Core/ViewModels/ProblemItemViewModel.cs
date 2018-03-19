// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
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

        public bool ShowTicks { get; set; }

        public bool HasTicks { get; private set; }

        private WallProblem _wallProblem;
        [Model]
        public WallProblem Problem
        {
            get { return _wallProblem; }
            set
            {
                _wallProblem = value;
                HasTicks = _wallProblem?.Tick != null;
            }
        }

        public IList<Tick> Ticks { get; set; }

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

            if (HasTicks)
            {
                await LoadDetailsAsync();
            }
        }

        private async Task LoadDetailsAsync()
        {
            Details = await _problem.GetDetailsAsync(Problem.ProblemId);
        }

        private RelayCommand _unloadComand;
        public RelayCommand UnloadCommand => _unloadComand ??
            (_unloadComand = new RelayCommand(Unload));

        private void Unload()
        {
            _eventAggregator.Unsubscribe(this);
        }

        private RelayCommand _manageTicksCommand;
        public RelayCommand ManageTicksCommand => _manageTicksCommand ??
            (_manageTicksCommand = new RelayCommand(async () => await ManageTicksAsync(), () => !_busy));

        private async Task ManageTicksAsync()
        {
            _eventAggregator.PublishShowBusy();

            if (ShowTicks)
            {
                await LoadTicksAsync();
            }
            else
            {
                ClearTicks();
            }

            _eventAggregator.PublishHideBusy();
        }

        private void ClearTicks()
        {
            Ticks = null;
        }

        private async Task LoadTicksAsync()
        {
            Ticks = await _ticks.GetProblemTicksAsync(Problem.ProblemId);
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