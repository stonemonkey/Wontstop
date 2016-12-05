// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
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
                typeof(IBasicGeoposition[]),
                typeof(Polyline),
                new PropertyMetadata(null, OnPathChanged));

        public static void SetPath(UIElement element, IBasicGeoposition[] value)
        {
            element.SetValue(PathProperty, value);
        }

        public static IBasicGeoposition[] GetPath(UIElement element)
        {
            return (IBasicGeoposition[]) element.GetValue(PathProperty);
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
            mapControl.MapElements.Add(CreateMapPolyline(path));

            var startPin = CreateStartPin(path);
            mapControl.MapElements.Add(startPin);

            var stopPin = CreateStopPin(path);
            mapControl.MapElements.Add(stopPin);

            var point = CreateGeoboundingBox(path);
            await mapControl.TrySetViewBoundsAsync(point, new Thickness(0), MapAnimationKind.Default);
        }

        private static MapIcon CreateStartPin(IBasicGeoposition[] path)
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

        private static MapIcon CreateStopPin(IBasicGeoposition[] path)
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

        private static GeoboundingBox CreateGeoboundingBox(IBasicGeoposition[] path)
        {
            var nw = new BasicGeoposition // NW
            {
                Latitude = path.Max(l => l.Latitude),
                Longitude = path.Min(l => l.Longitude)
            };
            var se = new BasicGeoposition // SE
            {
                Latitude = path.Min(l => l.Latitude),
                Longitude = path.Max(l => l.Longitude)
            };

            return new GeoboundingBox(nw, se);
        }

        private static MapPolyline CreateMapPolyline(IEnumerable<IBasicGeoposition> track)
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