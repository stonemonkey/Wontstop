using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;

namespace Wontstop.Climb.Ui.Uwp.Converters
{
    public class IntToAscentTypeStringConverter : IValueConverter
    {
        private IDictionary<int, string> _ascentTypesMap = new Dictionary<int, string>
        {
            { 0, "lead" },
            { 1, "toprope" },
        };

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return null;
            }

            var args = (int) value;
            if (_ascentTypesMap.TryGetValue(args, out string ascentTypeText))
            {
                return ascentTypeText;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}