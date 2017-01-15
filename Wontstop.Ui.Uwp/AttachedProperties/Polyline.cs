// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Maps;
using RunKeeper.WinRT.HealthGraph.Activities;
using Windows.Foundation;

namespace Wontstop.Ui.Uwp.AttachedProperties
{
    public class Polyline
    {
        public static readonly DependencyProperty PathProperty =
            DependencyProperty.RegisterAttached(
                "Path",
                typeof(ObservableCollection<TrackItemDto>),
                typeof(Polyline),
                new PropertyMetadata(null, OnPathChanged));

        public static void SetPath(UIElement element, ObservableCollection<TrackItemDto> value)
        {
            element.SetValue(PathProperty, value);
        }

        public static ObservableCollection<TrackItemDto> GetPath(UIElement element)
        {
            return (ObservableCollection<TrackItemDto>) element.GetValue(PathProperty);
        }

        private static async void OnPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var mapControl = d as MapControl;
            if (mapControl == null)
            {
                throw new InvalidOperationException(
                    "Polyline.Path property can only be attached to a MapControl!");
            }

            mapControl.MapElements.Clear();

            var path = GetPath(mapControl);
            //path.CollectionChanged += OnPathCollectionChanged;
            if (!path.Any())
            {
                return;
            }
            mapControl.MapElements.Add(CreateMapPolyline(path));

            var startPin = CreateStartPin(path);
            mapControl.MapElements.Add(startPin);

            var stopPin = CreateStopPin(path);
            mapControl.MapElements.Add(stopPin);

            var point = CreateGeoboundingBox(path);
            await mapControl.TrySetViewBoundsAsync(point, new Thickness(0), MapAnimationKind.Default);
        }

        private static void OnPathCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static MapIcon CreateStartPin(IEnumerable<TrackItemDto> path)
        {
            var first = path.First();
            return new MapIcon
            {
                ZIndex = 0,
                Title = "Start",
                NormalizedAnchorPoint = new Point(0.5, 1.0),
                Location = new Geopoint(new BasicGeoposition { Latitude = first.Latitude, Longitude = first.Longitude }),
            };
        }

        private static MapIcon CreateStopPin(IEnumerable<TrackItemDto> path)
        {
            var last = path.Last();
            return new MapIcon
            {
                ZIndex = 0,
                Title = "Stop",
                NormalizedAnchorPoint = new Point(0.5, 1.0),
                Location = new Geopoint(new BasicGeoposition { Latitude = last.Latitude, Longitude = last.Longitude }),
            };
        }

        private static GeoboundingBox CreateGeoboundingBox(IReadOnlyCollection<TrackItemDto> path)
        {
            var nw = new BasicGeoposition // NW
            {
                Latitude = path.Max(x => x.Latitude),
                Longitude = path.Min(x => x.Longitude)
            };
            var se = new BasicGeoposition // SE
            {
                Latitude = path.Min(x => x.Latitude),
                Longitude = path.Max(x => x.Longitude)
            };

            return new GeoboundingBox(nw, se);
        }

        private static MapPolyline CreateMapPolyline(IEnumerable<TrackItemDto> track)
        {
            return new MapPolyline
            {
                Path = new Geopath(track.Select(x =>
                    new BasicGeoposition
                    {
                        Altitude = x.Altitude,
                        Latitude = x.Latitude,
                        Longitude = x.Longitude,
                    })),
                StrokeColor = Colors.Red,
                StrokeThickness = 3,
                StrokeDashed = false
            };
        }
    }
}