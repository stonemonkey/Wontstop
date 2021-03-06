﻿// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using HttpApiClient;
using MvvmToolkit;
using MvvmToolkit.Messages;
using MvvmToolkit.Utils;
using Problemator.Core.Dtos;
using Problemator.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Problemator.Core.Models
{
    public class Session
    {
        private readonly UserContext _userContext;
        private readonly ITimeService _timeService;
        private readonly IStorageService _storageService;
        private readonly IEventAggregator _eventAggregator;
        private readonly ProblematorRequestsFactory _requestsFactory;

        public Session(
            UserContext userContext,
            ITimeService timeService,
            IStorageService storageService,
            IEventAggregator eventAggregator,
            ProblematorRequestsFactory requestsFactory)
        {
            _userContext = userContext;
            _timeService = timeService;
            _storageService = storageService;
            _eventAggregator = eventAggregator;
            _requestsFactory = requestsFactory;
        }

        private Dashboard _dashboard;
        private Task<Response<ProblematorJsonParser>> _dashboardRequestTask;

        public async Task LoadAsync(bool forceReload)
        {
            if (forceReload || _dashboard == null)
            {
                _dashboardRequestTask = _requestsFactory.CreateDashboardRequest()
                    .RunAsync<ProblematorJsonParser>();

                var response = await _dashboardRequestTask;

                _dashboardRequestTask = null;

                response.OnSuccess(p => _dashboard = p.To<Dashboard>())
                    .PublishErrorOnAnyFailure(_eventAggregator);
            }
        }

        #region SelectedDate

        public DateTime GetSelectedDate()
        {
            if (_storageService.LocalExists(Settings.SelectedDateKey))
            {
                return _storageService.ReadLocal<DateTime>(Settings.SelectedDateKey);
            }

            return _timeService.Now;
        }

        public void SetSelectedDate(DateTime date)
        {
            _storageService.SaveLocal(Settings.SelectedDateKey, date);
        }

        public async Task<IList<Grade>> GetGradesAsync()
        {
            await ValidateDashboardLoaded();

            return _dashboard.Grades.Values.ToList();
        }

        #endregion

        #region AscentType

        private class AscentType
        {
            public int Id { get; private set; }
            public string Name { get; private set; }

            public AscentType(int id, string name)
            {
                Id = id;
                Name = name;
            }
        }

        private IList<AscentType> _ascentTypes =
        new List<AscentType>
        {
            new AscentType(0, "lead"),
            new AscentType(1, "toprope"),
        };

        public IList<string> GetSportAscentTypes()
        {
            return _ascentTypes.Select(x => x.Name).ToList();
        }

        public int GetSportAscentTypeId(string name)
        {
            var type = _ascentTypes.SingleOrDefault(x => 
                string.Equals(name, x.Name, StringComparison.OrdinalIgnoreCase));
            if (type == null)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(name), $"Coudn't find ascent type '{name}'!");
            }

            return type.Id;
        }

        public string GetSportAscentType(int id)
        {
            var type = _ascentTypes.SingleOrDefault(x => id == x.Id);
            if (type == null)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(id), $"Coudn't find ascent type id '{id}'!");
            }

            return type.Name;
        }

        public async Task<string> GetUserSportAscentType()
        {
            await ValidateDashboardLoaded();

            var userSettings = _dashboard.UserSettings;
            var ascentTypeId = int.Parse(userSettings.SportTickAscentType);
            var type = _ascentTypes.Single(x => x.Id == ascentTypeId);

            return type.Name;
        }

        #endregion

        #region Location

        public async Task<string> GetCurrentLocationName()
        {
            await ValidateDashboardLoaded();

            var identity = _userContext.GetUserIdentity();
            var gymId = identity.GymId;
            if (gymId == null)
            {
                return null;
            }

            var location = _dashboard.Locations
                .Single(x => string.Equals(x.Id, gymId, StringComparison.Ordinal));

            return location.Name;
        }

        public async Task SetCurrentLocationAsync(string name)
        {
            name.ValidateNotNullEmptyWhiteSpace(nameof(name));
            await ValidateDashboardLoaded();

            var location = GetLocation(name);
            if (location == null)
            {
                throw new ArgumentOutOfRangeException(
                    $"Couldn't find location with name '{name}'!", nameof(name));
            }

            var identity = _userContext.GetUserIdentity();
            if (string.Equals(location.Id, identity.GymId, StringComparison.Ordinal))
            {
                return;
            }

            await ChangeGymAsync(location.Id);
        }

        public async Task<IList<string>> GetLocationNames()
        {
            await ValidateDashboardLoaded();

            return _dashboard.Locations
                .Select(x => x.Name)
                .ToList();
        }

        private Location GetLocation(string name)
        {
            return _dashboard.Locations
                .SingleOrDefault(x => string.Equals(name, x.Name, StringComparison.Ordinal));
        }

        private async Task ChangeGymAsync(string gymId)
        {
            (await _requestsFactory.CreateChangeGymRequest(gymId)
                .RunAsync<ProblematorJsonParser>())
                    .OnSuccess(p => 
                    {
                        var confirmation = p.To<GymChangeConfirmation>();
                        _userContext.UpdateUserIdentity(gymId, confirmation);
                    })
                    .PublishErrorOnAnyFailure(_eventAggregator);
        }

        #endregion

        public bool IsLoaded()
        {
            return _dashboard != null;
        }

        private async Task ValidateDashboardLoaded()
        {
            while (_dashboardRequestTask != null)
            {
                await Task.Delay(100);
            }

            if (_dashboard == null)
            {
                throw new InvalidOperationException(
                    "Dashboard must be loaded first! Call LoadAsync().");
            }
        }
    }
}
