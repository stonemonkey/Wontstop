// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Windows.UI.Xaml;
using Microsoft.Xaml.Interactivity;
using Windows.UI.ViewManagement;

namespace MvvmToolkit.Uwp.Behaviors
{
    /// <summary>
    /// Attaches InputPane Hiding and Showing to an element so that when the virtual keyboard is
    /// shown a marging is added to push it on top of the keyboard, or removed on hiding.
    /// Based on https://msdn.microsoft.com/en-us/windows/uwp/input-and-devices/respond-to-the-presence-of-the-touch-keyboard
    /// </summary>
    public class KeepAboveInputPaneBehavior : Behavior<FrameworkElement>
    {
        private Thickness _originalMargin;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObjectLoaded;
            _originalMargin = AssociatedObject.Margin;
        }

        private void AssociatedObjectLoaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.Loaded -= AssociatedObjectLoaded;
            InputPane.GetForCurrentView().Hiding += InputPaneHiding;
            InputPane.GetForCurrentView().Showing += InputPaneShowing;
        }

        protected override void OnDetaching()
        {
            InputPane.GetForCurrentView().Hiding -= InputPaneHiding;
            InputPane.GetForCurrentView().Showing -= InputPaneShowing;
        }

        private void InputPaneShowing(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            AssociatedObject.Margin = new Thickness(
                _originalMargin.Left, _originalMargin.Top,
                _originalMargin.Right, _originalMargin.Bottom + args.OccludedRect.Height);
        }

        private void InputPaneHiding(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            AssociatedObject.Margin = _originalMargin;
        }
    }
}
