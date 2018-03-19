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
    public class ProblemItemTickViewModel : 
        IHandle<BusyMessage>,
        INotifyPropertyChanged
    {
        #pragma warning disable CS0067
        // Is used by Fody to add NotifyPropertyChanged on properties.
        public event PropertyChangedEventHandler PropertyChanged;

        private Tick _tick;
        [Model]
        public Tick Tick
        {
            get { return _tick; }
            set
            {
                _tick = value;
                TryUpdateFieldsFromTick();
            }
        }

        public string AscentType { get; private set; }

        private bool _busy;

        private readonly Ticks _ticks;
        private readonly Session _session;
        private readonly IEventAggregator _eventAggregator;
        private readonly INavigationService _navigationService;

        public ProblemItemTickViewModel(
            Ticks ticks,
            Session session,
            IEventAggregator eventAggregator,
            INavigationService navigationService)
        {
            _ticks = ticks;
            _session = session;
            _eventAggregator = eventAggregator;
            _navigationService = navigationService;
        }

        private RelayCommand _loadComand;
        public RelayCommand LoadCommand => _loadComand ??
            (_loadComand = new RelayCommand(async () => await LoadAsync()));

        private async Task LoadAsync()
        {
            _eventAggregator.Subscribe(this);

            await _session.LoadAsync(false);
            TryUpdateFieldsFromTick();
        }

        private void TryUpdateFieldsFromTick()
        {
            if (Tick != null && _session.IsLoaded())
            {
                AscentType = _session.GetSportAscentType(Tick.AscentTypeId);
            }
        }

        private RelayCommand _unloadComand;
        public RelayCommand UnloadCommand => _unloadComand ??
            (_unloadComand = new RelayCommand(Unload));

        private void Unload()
        {
            _eventAggregator.Unsubscribe(this);
        }

        private RelayCommand _deleteTickCommand;
        public RelayCommand DeleteTickCommand => _deleteTickCommand ??
            (_deleteTickCommand = new RelayCommand(
                async () => await DeleteTickAsync(), () => !_busy));

        private async Task DeleteTickAsync()
        {
            _eventAggregator.PublishShowBusy();

            await _ticks.DeleteTickAsync(Tick);

            _eventAggregator.PublishHideBusy();
        }

        private RelayCommand _openDetailsComand;
        public RelayCommand OpenDetailsCommand => _openDetailsComand ??
            (_openDetailsComand = new RelayCommand(OpenDetails, () => !_busy));

        private void OpenDetails()
        {
            _navigationService.Navigate<TickDetailsViewModel>(Tick);
        }

        public void Handle(BusyMessage message)
        {
            _busy = message.Show;
        }
    }
}