// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MvvmToolkit;
using MvvmToolkit.Messages;
using MvvmToolkit.WinRT.Utils;

namespace Wontstop.Climb.Ui.Uwp.Views
{
    public abstract class MainChildPageBase : Page, IHandle<ErrorMessage>, IHandle<NavigationMessage>
    {
        private readonly IEventAggregator _eventAggregator;

        public MainChildPageBase()
        {
            _eventAggregator = ServiceLocator.Get<IEventAggregator>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _eventAggregator.Subscribe(this);
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            _eventAggregator.Unsubscribe(this);
        }

        public async void Handle(ErrorMessage message)
        {
            await message.ShowErrorAsync();
        }

        public void Handle(NavigationMessage message)
        {
            _eventAggregator.Unsubscribe(this);
        }
    }
}
