// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.Devices.Geolocation;
using Windows.UI.Core;
using Mvvm.WinRT.Commands;
using Mvvm.WinRT.Messages;
using PropertyChanged;
using RunKeeper.WinRT.HealthGraph.Activities;

namespace Wontstop.Ui.Uwp.ViewModels
{
    [ImplementPropertyChanged]
    public class LiveActivityViewModel : IHandle<BusyMessage>
    {
        public bool Busy { get; private set; }

        public string Speed { get; private set; }

        public string Status { get; private set; }
        public double Altitude { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

        public Activity Activity { get; }

        private readonly IEventAggregator _eventAggregator;

        public LiveActivityViewModel(
            IEventAggregator eventAggregator, Activity activity)
        {
            _eventAggregator = eventAggregator;
            Activity = activity;
        }

        private RelayCommand _loadComand;
        public RelayCommand LoadCommand => _loadComand ??
            (_loadComand = new RelayCommand(async () => await LoadAsync()));

        private Geolocator _geolocator;

        protected virtual async Task LoadAsync()
        {
            Busy = true;

            try
            {
                // ...
            }
            catch (Exception exception)
            {
                _eventAggregator.PublishOnCurrentThread(exception);
            }
            finally
            {
                Busy = false;
            }
        }

        private RelayCommand _startComand;
        public RelayCommand StartCommand => _startComand ??
            (_startComand = new RelayCommand(async () => await StartActivityAsync()));

        private async Task StartActivityAsync()
        {
            _geolocator = await CreateGeolocatorWithExtendedSession();
            if (_geolocator != null)
            {
                Activity.Start();
                Status = "Waiting for update ... ";
                _geolocator.StatusChanged += OnStatusChanged;
                _geolocator.PositionChanged += OnPositionChanged;
            }
        }

        private async Task<Geolocator> CreateGeolocatorWithExtendedSession()
        {
            var extendedSession = new ExtendedExecutionSession
            {
                Reason = ExtendedExecutionReason.LocationTracking,
                Description = "Location tracking"
            };

            Geolocator geolocator = null;

            var result = await extendedSession.RequestExtensionAsync();
            if (result == ExtendedExecutionResult.Allowed)
            {
                geolocator = new Geolocator
                {
                    ReportInterval = 2000,
                    DesiredAccuracyInMeters = 1,
                    DesiredAccuracy = PositionAccuracy.High,
                };
            }

            return geolocator;
        }

        private async void OnStatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                switch (args.Status)
                {
                    case PositionStatus.Ready:
                        // Location platform is providing valid data.
                        Status = "Ready";
                        break;

                    case PositionStatus.Initializing:
                        // Location platform is attempting to acquire a fix. 
                        Status = "Initializing";
                        break;

                    case PositionStatus.NoData:
                        // Location platform could not obtain location data.
                        Status = "No data";
                        break;

                    case PositionStatus.Disabled:
                        // The permission to access location data is denied by the user or other policies.
                        Status = "Disabled";
                        break;

                    case PositionStatus.NotInitialized:
                        // The location platform is not initialized. This indicates that the application 
                        // has not made a request for location data.
                        Status = "Not initialized";
                        break;

                    case PositionStatus.NotAvailable:
                        // The location platform is not available on this version of the OS.
                        Status = "Not available";
                        break;

                    default:
                        Status = "Unknown";
                        break;
                }
            });
        }

        private async void OnPositionChanged(Geolocator sender, PositionChangedEventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Activity.OnPositionChanged(e.Position);

                var coordinate = e.Position.Coordinate;
                var position = coordinate.Point.Position;

                Altitude = position.Altitude;
                Latitude = position.Latitude;
                Longitude = position.Longitude;
                Speed = $"{coordinate.Speed} m/s";
            });
        }

        private RelayCommand _stopComand;
        public RelayCommand StopCommand => _stopComand ??
            (_stopComand = new RelayCommand(async () => await StopActivityAsync()));

        private async Task StopActivityAsync()
        {
            Activity.Stop();

            if (_geolocator != null)
            {
                _geolocator.PositionChanged -= OnPositionChanged;
                _geolocator.StatusChanged -= OnStatusChanged;
            }
        }

        public void Handle(BusyMessage message)
        {
            Busy = message.Show;
        }
    }
}