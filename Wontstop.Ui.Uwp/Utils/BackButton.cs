using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Wontstop.Ui.Uwp.Utils
{
    public class BackButton
    {
        public static bool IsVisible()
        {
            return SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility == 
                AppViewBackButtonVisibility.Visible;
        }

        public static void TryShow()
        {
            var frame = (Frame) Window.Current.Content;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = 
                frame.CanGoBack ?
                AppViewBackButtonVisibility.Visible : 
                AppViewBackButtonVisibility.Collapsed;
        }

        public static void Hide()
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                AppViewBackButtonVisibility.Collapsed;
        }
    }
}