// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Wontstop.Climb.Ui.Uwp.Converters
{
    public class StringEqualToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var actual = value as string;
            var expected = parameter as string;

            return string.Equals(actual, expected, StringComparison.Ordinal) ? 
                Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
