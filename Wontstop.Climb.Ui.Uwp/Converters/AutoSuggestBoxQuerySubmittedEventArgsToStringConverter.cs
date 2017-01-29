using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Wontstop.Climb.Ui.Uwp.Converters
{
    public class AutoSuggestBoxQuerySubmittedEventArgsToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var args = value as AutoSuggestBoxQuerySubmittedEventArgs;

            return args?.QueryText;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}