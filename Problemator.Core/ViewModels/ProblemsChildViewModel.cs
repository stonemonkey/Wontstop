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
using Problemator.Core.Models;
using Problemator.Core.Utils;

namespace Problemator.Core.ViewModels
{
    public class ProblemsChildViewModel : IHandle<Tick>, INotifyPropertyChanged
    {
        #pragma warning disable CS0067
        // Is used by Fody to add NotifyPropertyChanged on properties.
        public event PropertyChangedEventHandler PropertyChanged;

        public string Title => "Problems";

        public bool Busy { get; private set; }
        public bool Empty { get; private set; }

        public string SelectedLocation { get; set; }
        public IDictionary<string, string> Locations { get; set; }

        private static bool _isDaySaved;
        public static DateTimeOffset Day { get; set; }
        private static DateTime SelectedDate => Day.Date;

        public IList<DateTimeOffset> TickDates { get; private set; }

        public IList<WallSection> Sections { get; private set; }
        public ObservableCollection<Tick> Ticks { get; private set; }

        private readonly ITimeService _timeService;
        private readonly IStorageService _storageService;
        private readonly IEventAggregator _eventAggregator;
        private readonly Sections _sections;
        private readonly ProblematorRequestsFactory _requestsFactory;

        public ProblemsChildViewModel(
            ITimeService timeService,
            IStorageService storageService,
            IEventAggregator eventAggregator,
            Sections sections,
            ProblematorRequestsFactory requestsFactory)
        {
            _timeService = timeService;
            _storageService = storageService;
            _eventAggregator = eventAggregator;
            _sections = sections;
            _requestsFactory = requestsFactory;

            if (!_isDaySaved)
            {
                Day = _timeService.Now;
                _isDaySaved = true;
            }
        }

        private RelayCommand _loadComand;
        public RelayCommand LoadCommand => _loadComand ??
            (_loadComand = new RelayCommand(async () => await LoadAsync()));

        private UserContext _context;

        protected virtual async Task LoadAsync()
        {
            _context = _storageService.ReadLocal<UserContext>(Settings.ContextKey);

            await RefreshAsync();

            _eventAggregator.Subscribe(this);
        }

        private async Task RefreshAsync()
        {
            Busy = true;

            await LoadDashboardAsync();
            await _sections.LoadAsync();
            await LoadTickDatesAsync();
            await LoadTicks(SelectedDate);

            Busy = false;
            Sections = _sections.Get();
            Empty = !_sections.HasProblems();
        }

        private async Task LoadDashboardAsync()
        {
            (await _requestsFactory.CreateDashboardRequest()
                    .RunAsync<ProblematorJsonParser>())
                .OnSuccess(HandleDashboardResponse)
                .PublishErrorOnHttpFailure(_eventAggregator);
        }

        private void HandleDashboardResponse(ProblematorJsonParser parser)
        {
            if (parser.PublishMessageOnInternalServerError(_eventAggregator))
            {
                return;
            }

            var dashboard = parser.To<Dashboard>();
            Locations = dashboard.Locations.ToDictionary(x => x.Name, x => x.Id);
            SelectedLocation = dashboard.Locations
                .SingleOrDefault(x => string.Equals(x.Id,_context.GymId, StringComparison.Ordinal))?
                .Name;
        }

        private async Task LoadTicks(DateTime day)
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
            _eventAggregator.Unsubscribe(this);
        }

        private RelayCommand _changeLocationComand;

        public RelayCommand ChangeLocationCommand => _changeLocationComand ??
            (_changeLocationComand = new RelayCommand(
                async () => await ChangeLocationAsync(), () => !Busy));

        private async Task ChangeLocationAsync()
        {
            var gymId = Locations[SelectedLocation];
            if (gymId == _context.GymId)
            {
                return;
            }

            (await _requestsFactory.CreateChangeGymRequest(gymId)
                    .RunAsync<ProblematorJsonParser>())
                .OnSuccess(x => HandleChangeGymRequest(x, gymId))
                .PublishErrorOnHttpFailure(_eventAggregator);

            await RefreshAsync();
        }

        private void HandleChangeGymRequest(ProblematorJsonParser parser, string gymId)
        {
            var context = parser.To<UserContext>();
            _context.GymId = gymId;
            _context.Jwt = context.Jwt;
            _context.Message = context.Message;
            _requestsFactory.SetUserContext(_context);
            _storageService.SaveLocal(Settings.ContextKey, _context);
        }

        private RelayCommand _changeDateComand;

        public RelayCommand ChangeDateCommand => _changeDateComand ??
            (_changeDateComand = new RelayCommand(async () => await RefreshAsync(), () => !Busy));

        public async void Handle(Tick message)
        {
            Ticks.Remove(message);

            await RefreshAsync();
        }
    }
}
