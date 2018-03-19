// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using MvvmToolkit.Commands;
using MvvmToolkit.Messages;
using Problemator.Core.Dtos;
using Problemator.Core.Messages;
using Problemator.Core.Models;

namespace Problemator.Core.ViewModels
{
    public class ProblemsChildViewModel :
        IHandle<TickRemovedMessage>,
        IHandle<BusyMessage>,
        INotifyPropertyChanged
    {
        #pragma warning disable CS0067
        // Is used by Fody to add NotifyPropertyChanged on properties.
        public event PropertyChangedEventHandler PropertyChanged;

        public string Title => "Problems";

        public bool Busy { get; private set; }
        public bool Empty { get; private set; }

        public string SelectedLocation { get; set; }
        public IList<string> Locations { get; private set; }

        public IList<WallSection> Sections { get; private set; }

        private readonly Session _session;
        private readonly Sections _sections;
        private readonly IEventAggregator _eventAggregator;

        public ProblemsChildViewModel(
            Session session,
            Sections sections,
            IEventAggregator eventAggregator)
        {
            _session = session;
            _sections = sections;
            _eventAggregator = eventAggregator;
        }

        private RelayCommand _loadComand;
        public RelayCommand LoadCommand => _loadComand ??
            (_loadComand = new RelayCommand(async () => await LoadAsync()));

        protected virtual async Task LoadAsync()
        {
            _eventAggregator.Subscribe(this);
            await RefreshAsync(false);
        }

        private async Task RefreshAsync(bool refresh)
        {
            _eventAggregator.PublishShowBusy();

            await LoadSessionAsync(refresh);
            await LoadSectionsAsync();

            _eventAggregator.PublishHideBusy();
        }

        private async Task LoadSessionAsync(bool refresh)
        {
            await _session.LoadAsync(refresh);

            Locations = await _session.GetLocationNames();
            SelectedLocation = await _session.GetCurrentLocationName();
        }

        private async Task LoadSectionsAsync()
        {
            await _sections.LoadAsync();

            Sections = _sections.Get();
            Empty = !_sections.HasProblems();
        }

        private RelayCommand _unloadComand;
        public RelayCommand UnloadCommand => _unloadComand ??
            (_unloadComand = new RelayCommand(Unload));

        private void Unload()
        {
            _eventAggregator.Unsubscribe(this);
        }

        private RelayCommand _changeLocationComand;

        public RelayCommand ChangeLocationCommand => _changeLocationComand ??
            (_changeLocationComand = new RelayCommand(
                async () => await ChangeLocationAsync(), () => !Busy));

        private async Task ChangeLocationAsync()
        {
            await _session.SetCurrentLocationAsync(SelectedLocation);
            await RefreshAsync(true);
        }

        public async void Handle(TickRemovedMessage message)
        {
            await RefreshAsync(true);
        }

        public void Handle(BusyMessage message)
        {
            Busy = message.IsBusy;
        }
    }
}
