using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace MvvmToolkit.WinRT.AttachedProperties
{
    public static class ContextMenu
    {
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.RegisterAttached(
            "IsOpen", typeof(bool), typeof(ContextMenu), new PropertyMetadata(true, IsOpenChangedCallback));

        public static readonly DependencyProperty ParentProperty = DependencyProperty.RegisterAttached(
            "Parent", typeof(FrameworkElement), typeof(ContextMenu), null);

        public static void SetIsOpen(DependencyObject element, bool value)
        {
            element.SetValue(IsOpenProperty, value);
        }

        public static bool GetIsOpen(DependencyObject element)
        {
            return (bool) element.GetValue(IsOpenProperty);
        }

        private static void IsOpenChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var flyout = d as FlyoutBase;
            if (flyout == null)
            {
                return;
            }

            if ((bool) e.NewValue)
            {
                flyout.Opened -= OnFlyoutOpened;
                flyout.Closed += OnFlyoutClosed;
                flyout.ShowAt(GetParent(d));
            }
            else
            {
                flyout.Opened += OnFlyoutOpened;
                flyout.Closed -= OnFlyoutClosed;
                flyout.Hide();
            }
        }

        private static void OnFlyoutOpened(object sender, object e)
        {
            SetIsOpen(sender as DependencyObject, true);
        }

        private static void OnFlyoutClosed(object sender, object e)
        {
            SetIsOpen(sender as DependencyObject, false);
        }

        public static void SetParent(DependencyObject element, FrameworkElement value)
        {
            element.SetValue(ParentProperty, value);
        }

        public static FrameworkElement GetParent(DependencyObject element)
        {
            return (FrameworkElement) element.GetValue(ParentProperty);
        }
    }
}
