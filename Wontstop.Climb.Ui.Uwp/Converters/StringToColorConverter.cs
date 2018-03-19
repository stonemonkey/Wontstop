// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Wontstop.Climb.Ui.Uwp.Converters
{
    public class StringToColorConverter : IValueConverter
    {
        private static Dictionary<string, Color> _colorNames;

        private static readonly Color DefaultColor = Colors.Transparent;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var colorName = value as string;
            if (colorName == null)
            {
                return new SolidColorBrush(DefaultColor);
            }

            if (_colorNames == null)
            {
                _colorNames = GetAllColorsByName();
            }

            var name = colorName.ToLower().Replace(" ", null);

            return new SolidColorBrush(
                _colorNames.TryGetValue(name, out Color color) ? color : DefaultColor);
        }

        private Dictionary<string, Color> GetAllColorsByName()
        {
            var colorNames = new Dictionary<string, Color>();

            foreach (var color in typeof (Colors).GetRuntimeProperties())
            {
                var name = color.Name.ToLower();
                colorNames[name] = (Color) color.GetValue(null);
            }

            // add some custom hardcoded colors
            colorNames["lightpurple"] = Colors.MediumPurple;
            colorNames["neonyellow"] = Colors.GreenYellow;
            colorNames["terracotta"] = Colors.DeepPink;
            colorNames["grey"] = Colors.Gray;

            return colorNames;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
