using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Problemator.Core.Dtos;

namespace Wontstop.Climb.Ui.Uwp.Converters
{
    public class AutoSuggestBoxSuggestionChosenEventArgsToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var args = value as AutoSuggestBoxSuggestionChosenEventArgs;
            var problem = args?.SelectedItem as Problem;

            return problem?.TagShort;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}