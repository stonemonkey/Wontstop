// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using PropertyChanged;
using RunKeeper.WinRT.HealthGraph.Infrastructure;

namespace RunKeeper.WinRT.HealthGraph.Activities
{
    [ImplementPropertyChanged]
    public class Activity
    {
        public string Type { get; private set; }
        public TimeSpan Duration { get; private set; }
        public DateTime StartTime { get; private set; }
        public double TotalCalories { get; private set; }
        public double TotalDistance { get; private set; }
        public int AverageHeartRate { get; private set; }
        public int MaximumHeartRate { get; private set; }
        public TimeSpan AveragePace { get; private set; }
        public TrackItemDto[] Track { get; private set; }
        public double MinimumAltitude { get; private set; }
        public double MaximumAltitude { get; private set; }

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
            Duration = TimeSpan.FromSeconds(_data.Duration);
            StartTime = _data.StartTime;
            TotalCalories = _data.TotalCalories;
            TotalDistance = _data.TotalDistance;
            AverageHeartRate = _data.AverageHeartRate;
            MaximumHeartRate = _data.HeartRate.Max(x => x.HeartRate);
            AveragePace = TimeSpan.FromSeconds(_data.Duration / (_data.TotalDistance / 1000));
            MinimumAltitude = Math.Round(_data.Track.Min(x => x.Altitude));
            MaximumAltitude = Math.Round(_data.Track.Max(x => x.Altitude));
            Track = _data.Track;
        }
    }
}