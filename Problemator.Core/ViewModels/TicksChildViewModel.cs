// Copyright (c) Costin Morariu. All rights reserved.
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
        IHandle<TickRemoveMessage>, 
        IHandle<TickAddMesage>, 
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

        private readonly Session _session;
        private readonly Sections _sections;
        private readonly ITimeService _timeService;
        private readonly IStorageService _storageService;
        private readonly IEventAggregator _eventAggregator;
        private readonly ProblematorRequestsFactory _requestsFactory;

        public TicksChildViewModel(
            Session session,
            Sections sections,
            ITimeService timeService,
            IStorageService storageService,
            IEventAggregator eventAggregator,
            ProblematorRequestsFactory requestsFactory)
        {
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

            await _session.LoadAsync(refresh);
            Locations = await _session.GetLocationNames();
            SelectedLocation = await _session.GetCurrentLocationName();

            _eventAggregator.PublishHideBusy();

            UpdateEmpty();
        }

        private async Task LoadTicksAsync(DateTime day)
        {
            (await _requestsFactory.CreateDayTicksRequest(day)
                .RunAsync<ProblematorJsonParser>())
                    .OnSuccess(p =>
                    {
                        var dayTicks = p.To<DayTicks>();
                        Ticks = new ObservableCollection<Tick>(dayTicks.Ticks);
                    })
                    .PublishErrorOnAnyFailure(_eventAggregator);
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

        private void UpdateEmpty()
        {
            Empty = Ticks == null || !Ticks.Any();
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

        public async void Handle(TickRemoveMessage message)
        {
            await LoadTicksAsync(SelectedDate);

            UpdateEmpty();
        }

        public async void Handle(TickAddMesage message)
        {
            if (!message.Successfull)
            {
                ShowErrorForFailedToTickTags(message.FailedToTickTags);
            }

            await LoadTicksAsync(SelectedDate);

            UpdateEmpty();
        }

        private const string TicksSeparator = ",";

        private void ShowErrorForFailedToTickTags(IEnumerable<string> tags)
        {
            _eventAggregator.PublishErrorMessageOnCurrentThread(
                $"Unable to save: {string.Join(TicksSeparator, tags)}");
        }

        public void Handle(BusyMessage message)
        {
            Busy = message.Show;
        }
    }
}
