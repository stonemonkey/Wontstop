// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using MvvmToolkit.Uwp.Utils;

namespace MvvmToolkit.Uwp.Converters
{
    public class BoolToObjectConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty TrueValueProperty =
            DependencyProperty.Register(nameof(TrueValue), typeof(object), typeof(BoolToObjectConverter), new PropertyMetadata(null));

        public static readonly DependencyProperty FalseValueProperty =
            DependencyProperty.Register(nameof(FalseValue), typeof(object), typeof(BoolToObjectConverter), new PropertyMetadata(null));

        public object TrueValue
        {
            get { return GetValue(TrueValueProperty); }
            set { SetValue(TrueValueProperty, value); }
        }

        public object FalseValue
        {
            get { return GetValue(FalseValueProperty); }
            set { SetValue(FalseValueProperty, value); }
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool boolValue = value is bool && (bool) value;

            if (parameter.TryParseBool())
            {
                boolValue = !boolValue;
            }
            var objValue = boolValue ? TrueValue : FalseValue;

            return objValue.Convert(targetType);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            bool result = Equals(value, TrueValue.Convert(value.GetType()));

            if (parameter.TryParseBool())
            {
                result = !result;
            }

            return result;
        }
    }
}