﻿// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Windows.UI.Xaml.Data;
using MvvmToolkit.Uwp.Utils;

namespace Wontstop.Climb.Ui.Uwp.Converters
{
    public class IntOneToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var boolValue = int.Parse(value.ToString()) == 1;
            if (parameter.TryParseBool())
            {
                boolValue = !boolValue;
            }
            return boolValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
