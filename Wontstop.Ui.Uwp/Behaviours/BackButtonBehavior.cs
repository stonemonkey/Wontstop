using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;
using Wontstop.Ui.Uwp.Utils;

namespace Wontstop.Ui.Uwp.Behaviours
{
    public class BackButtonBehavior : Behavior<Page>
    {
        protected override void OnAttached()
        {
            AssociatedObject.Loading += OnAssociatedObjectLoading;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Loading -= OnAssociatedObjectLoading;
        }

        private void OnAssociatedObjectLoading(FrameworkElement sender, object args)
        {
            BackButton.TryShow();
        }
    }
}