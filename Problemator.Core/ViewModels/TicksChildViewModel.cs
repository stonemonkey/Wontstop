﻿// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using HttpApiClient;
using MvvmToolkit;
using MvvmToolkit.Commands;
using MvvmToolkit.Messages;
using Problemator.Core.Dtos;
using Problemator.Core.Messages;
using Problemator.Core.Models;
using Problemator.Core.Utils;

namespace Problemator.Core.ViewModels
{
    public class TicksChildViewModel : 
        IHandle<TickRemovedMessage>, 
        IHandle<TickAddedMesage>, 
        IHandle<BusyMessage>, 
        INotifyPropertyChanged
    {
        #pragma warning disable CS0067
        // Is used by Fody to add NotifyPropertyChanged on properties.
        public event PropertyChangedEventHandler PropertyChanged;

        public string Title => "Ticks";

        public bool Busy { get; private set; }
        public bool Empty { get; private set; }

        public string SelectedLocation { get; set; }
        public IList<string> Locations { get; private set; }

        private static bool _isSelectedDaySaved;
        public static DateTimeOffset SelectedDay { get; set; }
        private static DateTime SelectedDate => SelectedDay.Date;

        public IList<DateTimeOffset> TickDates { get; private set; }

        public ObservableCollection<Tick> Ticks { get; private set; }

        private readonly Ticks _ticks;
        private readonly Session _session;
        private readonly Sections _sections;
        private readonly ITimeService _timeService;
        private readonly IStorageService _storageService;
        private readonly IEventAggregator _eventAggregator;
        private readonly ProblematorRequestsFactory _requestsFactory;

        public TicksChildViewModel(
            Ticks ticks,
            Session session,
            Sections sections,
            ITimeService timeService,
            IStorageService storageService,
            IEventAggregator eventAggregator,
            ProblematorRequestsFactory requestsFactory)
        {
            _ticks = ticks;
            _session = session;
            _sections = sections;
            _timeService = timeService;
            _storageService = storageService;
            _eventAggregator = eventAggregator;
            _requestsFactory = requestsFactory;

            if (!_isSelectedDaySaved)
            {
                SelectedDay = _timeService.Now;
                _isSelectedDaySaved = true;
            }
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

            await LoadTickDatesAsync();
            await LoadTicksAsync(SelectedDate);
            await LoadSessionAsync(refresh);

            _eventAggregator.PublishHideBusy();
        }

        private async Task LoadTicksAsync(DateTime day)
        {
            var ticks = await _ticks.GetDayTicksAsync(day);

            Ticks = new ObservableCollection<Tick>(ticks);
            Empty = Ticks == null || !Ticks.Any();
        }

        private async Task LoadTickDatesAsync()
        {
            (await _requestsFactory.CreateTickDatesRequest()
                .RunAsync<ProblematorJsonParser>())
                    .OnSuccess(p =>
                    {
                        TickDates = p.To<IList<DateTimeOffset>>();
                    })
                    .PublishErrorOnAnyFailure(_eventAggregator);
        }

        private async Task LoadSessionAsync(bool refresh)
        {
            await _session.LoadAsync(refresh);

            Locations = await _session.GetLocationNames();
            SelectedLocation = await _session.GetCurrentLocationName();
        }

        private RelayCommand _unloadComand;
        public RelayCommand UnloadCommand => _unloadComand ??
            (_unloadComand = new RelayCommand(Unload));

        private void Unload()
        {
            Ticks = null;
            TickDates = null;

            _eventAggregator.Unsubscribe(this);
        }

        private RelayCommand _changeLocationComand;

        public RelayCommand ChangeLocationCommand => _changeLocationComand ??
            (_changeLocationComand = new RelayCommand(
                async () => await ChangeLocationAsync(), () => !Busy));

        private async Task ChangeLocationAsync()
        {
            await _session.SetCurrentLocationAsync(SelectedLocation);
            _eventAggregator.PublishLocationChanged(SelectedLocation);
            await RefreshAsync(true);
        }

        private RelayCommand _changeDateComand;

        public RelayCommand ChangeDateCommand => _changeDateComand ??
            (_changeDateComand = new RelayCommand(
                async () => await ChangeDayAsync(), () => !Busy));

        private async Task ChangeDayAsync()
        {
            await RefreshAsync(true);
            _eventAggregator.PublishDayChanged(SelectedDay);
        }

        public async void Handle(TickRemovedMessage message)
        {
            await LoadTicksAsync(SelectedDate);
        }

        public async void Handle(TickAddedMesage message)
        {
            await LoadTicksAsync(SelectedDate);
        }

        public void Handle(BusyMessage message)
        {
            Busy = message.Show;
        }
    }
}
