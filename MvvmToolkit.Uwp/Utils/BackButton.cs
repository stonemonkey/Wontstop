using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MvvmToolkit.Uwp.Utils
{
    /// <summary>
    /// Back button helpers
    /// </summary>
    public static class BackButton
    {
        /// <summary>
        /// Determines if back button is visible.
        /// </summary>
        /// <returns>True if back button is visible, otherwise false.</returns>
        public static bool IsVisible()
        {
            return SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility == 
                AppViewBackButtonVisibility.Visible;
        }

        /// <summary>
        /// Shows back button if application can go back.
        /// </summary>
        public static void TryShow()
        {
            var frame = (Frame) Window.Current.Content;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = 
                frame.CanGoBack ?
                AppViewBackButtonVisibility.Visible : 
                AppViewBackButtonVisibility.Collapsed;
        }

        /// <summary>
        /// Hides back button if is visible.
        /// </summary>
        public static void Hide()
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                AppViewBackButtonVisibility.Collapsed;
        }
    }
}