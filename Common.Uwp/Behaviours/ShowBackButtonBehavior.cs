// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Common.Uwp.Utils;
using Microsoft.Xaml.Interactivity;

namespace Common.Uwp.Behaviours
{
    /// <summary>
    /// Attaches back navigation showing back button on the application title bar while loading 
    /// the page.
    /// </summary>
    public class ShowBackButtonBehavior : Behavior<Page>
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