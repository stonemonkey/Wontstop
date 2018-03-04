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
        IHandle<Tick>, IHandle<BusyMessage>, IHandle<TickProblemsMesage>, INotifyPropertyChanged
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
            await RefreshAsync();
        }

        private async Task RefreshAsync()
        {
            _eventAggregator.PublishOnCurrentThread(new BusyMessage(true));

            await LoadTickDatesAsync();
            await LoadTicksAsync(SelectedDate);

            Locations = await _session.GetLocationNames();
            SelectedLocation = await _session.GetCurrentLocationName();

            _eventAggregator.PublishOnCurrentThread(new BusyMessage(false));

            UpdateEmpty();
        }

        private async Task LoadTicksAsync(DateTime day)
        {
            (await _requestsFactory.CreateDayTicksRequest(day)
                .RunAsync<ProblematorJsonParser>())
                    .OnSuccess(HandleTicksResponse)
                    .PublishErrorOnHttpFailure(_eventAggregator);
        }

        private void HandleTicksResponse(ProblematorJsonParser parser)
        {
            if (parser.PublishMessageOnInternalServerError(_eventAggregator))
            {
                return;
            }

            var day = parser.To<DayTicks>();
            Ticks = new ObservableCollection<Tick>(day.Ticks);
        }

        private async Task LoadTickDatesAsync()
        {
            (await _requestsFactory.CreateTickDatesRequest()
                .RunAsync<ProblematorJsonParser>())
                    .OnSuccess(HandleTickDatesResponse)
                    .PublishErrorOnHttpFailure(_eventAggregator);
        }

        private void UpdateEmpty()
        {
            Empty = Ticks == null || !Ticks.Any();
        }

        private void HandleTickDatesResponse(ProblematorJsonParser parser)
        {
            if (parser.PublishMessageOnInternalServerError(_eventAggregator))
            {
                return;
            }

            TickDates = parser.To<IList<DateTimeOffset>>(); 
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
            _eventAggregator.PublishOnCurrentThread(new LocationChangedMessage(SelectedLocation));
            await RefreshAsync();
        }

        private RelayCommand _changeDateComand;

        public RelayCommand ChangeDateCommand => _changeDateComand ??
            (_changeDateComand = new RelayCommand(
                async () => await ChangeDayAsync(), () => !Busy));

        private async Task ChangeDayAsync()
        {
            _eventAggregator.PublishOnCurrentThread(new DayChangedMessage(SelectedDay));
            await RefreshAsync();
        }

        public async void Handle(TickProblemsMesage message)
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

        public void Handle(Tick message)
        {
            Ticks.Remove(message);
        }
    }
}
