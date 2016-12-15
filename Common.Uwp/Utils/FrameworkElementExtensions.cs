using System;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace Common.Uwp.Utils
{
    public static class FrameworkElementExtensions
    {
        public static double GetScreenTopToElementBottomHeight(this FrameworkElement element)
        {
            var contentLeftTopPoint = element.TransformToVisual(Window.Current.Content)
                .TransformPoint(new Point(0, 0));

            return contentLeftTopPoint.Y + element.ActualHeight;
        }

        public static double GetVirtualKeyboardOverlappingHeight(this FrameworkElement element)
        {
            var inputPane = InputPane.GetForCurrentView();
            var keyboardTop = inputPane.OccludedRect.Y;
            var screenTopToContentBottom = element.GetScreenTopToElementBottomHeight();
            var height = screenTopToContentBottom - keyboardTop;

            return (Math.Abs(inputPane.OccludedRect.Height) > 0) && (height > 0) ? height : 0;
        }
    }
}