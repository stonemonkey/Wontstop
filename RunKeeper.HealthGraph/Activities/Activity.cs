// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using PropertyChanged;
using RunKeeper.WinRT.HealthGraph.Infrastructure;

namespace RunKeeper.WinRT.HealthGraph.Activities
{
    [ImplementPropertyChanged]
    public class Activity
    {
        public string Type { get; private set; }
        public double Duration { get; private set; }
        public DateTime StartTime { get; private set; }
        public double TotalDistance { get; private set; }
        public int AverageHeartRate { get; private set; }
        public TrackItemDto[] Track { get; private set; }

        private readonly IModelRepository _serverRepository;

        public Activity(IModelRepository serverRepository)
        {
            _serverRepository = serverRepository;
        }

        private string _resource;

        public void SetResource(string resource)
        {
            _resource = resource;
        }

        private ActivityDto _data;

        public async Task LoadAsync()
        {
            _data = await _serverRepository.ReadAsyc<ActivityDto>(_resource);

            MapData();
        }

        private void MapData()
        {
            Type = _data.Type;
            Duration = _data.Duration;
            StartTime = _data.StartTime;
            TotalDistance = _data.TotalDistance;
            AverageHeartRate = _data.AverageHeartRate;
            Track = _data.Track;
        }
    }
}