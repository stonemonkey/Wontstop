// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;
using Mvvm.WinRT;
using Mvvm.WinRT.Messages;
using Mvvm.WinRT.Utils;

namespace Common.Uwp.Behaviours
{
    /// <summary>
    /// Attaches handler for ScrollMessage so that the ListViewBase will try to scroll to the item 
    /// encapsulated in the mesage when a view model publishes it.
    /// </summary>
    public class ScrollToItemBehavior : Behavior<ListViewBase>, IHandle<ScrollMessage>
    {
        private readonly IEventAggregator _eventAggregator = ServiceLocator.Get<IEventAggregator>();

        protected override void OnAttached()
        {
            AssociatedObject.Loaded += OnAssociatedObjectLoaded;
            AssociatedObject.Unloaded += OnAssociatedObjectUnloaded;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= OnAssociatedObjectLoaded;
            AssociatedObject.Unloaded -= OnAssociatedObjectUnloaded;
        }

        private void OnAssociatedObjectLoaded(object sender, RoutedEventArgs e)
        {
            _eventAggregator.Subscribe(this);
        }

        private void OnAssociatedObjectUnloaded(object sender, RoutedEventArgs e)
        {
            _eventAggregator.Unsubscribe(this);
        }

        public async void Handle(ScrollMessage message)
        {
            await AssociatedObject.ScrollIntoViewCentredAsync(message.Item);
        }
    }
}