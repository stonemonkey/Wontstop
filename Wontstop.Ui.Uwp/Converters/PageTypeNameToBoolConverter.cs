// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Windows.UI.Xaml.Data;

namespace Wontstop.Ui.Uwp.Converters
{
    public class PageTypeNameToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((value == null) || (parameter == null))
            {
                return false;
            }

            var controlName = parameter.ToString();
            var viewModelName = value.GetType().Name;

            return string.Equals(viewModelName, controlName, StringComparison.Ordinal);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}