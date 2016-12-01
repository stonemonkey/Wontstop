using System;
using Windows.UI.Xaml.Data;

namespace Wontstop.Ui.Uwp.Converters
{
    public class DoubleDurationToStringTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return null;
            }

            var durationSec = double.Parse(value.ToString());
            var timeSpan = TimeSpan.FromSeconds(durationSec);
            return timeSpan.ToString(@"hh\:mm\:ss");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}