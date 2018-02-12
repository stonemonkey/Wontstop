// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Geolocation;
using Windows.System.Threading;
using Windows.UI.Core;
using RunKeeper.WinRT.HealthGraph.Infrastructure;
using System.ComponentModel;

namespace RunKeeper.WinRT.HealthGraph.Activities
{
    public static class NumericExtensions
    {
        public static double ToRadians(this double val)
        {
            return (Math.PI / 180) * val;
        }
    }

    public class Activity : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool Live { get; private set; }
        public string Type { get; private set; }
        public TimeSpan Duration { get; private set; }
        public DateTime StartTime { get; private set; }
        public double TotalCalories { get; private set; }
        public double TotalDistance { get; private set; }
        public int AverageHeartRate { get; private set; }
        public int MaximumHeartRate { get; private set; }
        public TimeSpan AveragePace { get; private set; }
        public double MinimumAltitude { get; private set; }
        public double MaximumAltitude { get; private set; }
        public ObservableCollection<TrackItemDto> Track { get; private set; }

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
            Track = new ObservableCollection<TrackItemDto>(_data.Track);
        }

        private ThreadPoolTimer _timer;

        public void Start()
        {
            Live = true;
            StartTime = DateTime.Now;
            Track = new ObservableCollection<TrackItemDto>();

            _timer = ThreadPoolTimer.CreatePeriodicTimer(OnTimerTick, TimeSpan.FromSeconds(1));
        }

        private async void OnTimerTick(object state)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Duration = (DateTime.Now - StartTime);
            });
        }

        private TrackItemDto _lastPosition;

        public async void OnPositionChanged(Geoposition geoposition)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var coordinate = geoposition.Coordinate;
                var position = coordinate.Point.Position;
                if (coordinate.Speed <= 0)
                {
                    return;
                }

                if (_lastPosition != null)
                {
                    TotalDistance += CalculateDistance(
                        _lastPosition.Latitude, _lastPosition.Longitude, 
                        position.Latitude, position.Longitude);
                    AveragePace = TimeSpan.FromSeconds(Duration.TotalSeconds / (TotalDistance / 1000));
                }
                else
                {
                    MinimumAltitude = position.Altitude;
                    MaximumAltitude = position.Altitude;
                }

                _lastPosition = new TrackItemDto
                {
                    Altitude = position.Altitude,
                    Latitude = position.Latitude,
                    Longitude = position.Longitude
                };
                Track.Add(_lastPosition);

                if (MinimumAltitude > position.Altitude)
                {
                    MinimumAltitude = position.Altitude;
                }
                if (MaximumAltitude < position.Altitude)
                {
                    MaximumAltitude = position.Altitude;
                }
            });
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double r = 6371e3; // metres
            var l1 = lat1.ToRadians();
            var l2 = lat2.ToRadians();
            var dLat = (lat2 - lat1).ToRadians();
            var dLon = (lon2 - lon1).ToRadians();

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(l1) * Math.Cos(l2) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return r * c;
        }

        public void Stop()
        {
            if (_timer != null)
            {
                _timer.Cancel();
                _timer = null;
            }

            Live = false;
        }
    }
}