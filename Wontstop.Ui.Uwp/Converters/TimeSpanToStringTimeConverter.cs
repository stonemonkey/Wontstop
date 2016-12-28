using System;
using Windows.UI.Xaml.Data;

namespace Wontstop.Ui.Uwp.Converters
{
    public class TimeSpanToStringTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return null;
            }

            var timeSpan = TimeSpan.Parse(value.ToString());
            return timeSpan.ToString(@"hh\:mm\:ss");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}