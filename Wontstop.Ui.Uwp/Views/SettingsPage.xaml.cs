// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Mvvm.WinRT;
using Mvvm.WinRT.Messages;
using Mvvm.WinRT.Utils;

namespace Wontstop.Ui.Uwp.Views
{
    public sealed partial class SettingsPage : Page, IHandle<Exception>
    {
        private readonly IEventAggregator _eventAggregator;

        public SettingsPage()
        {
            InitializeComponent();

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

        public async void Handle(Exception message)
        {
            await message.ShowErrorAsync();
        }
    }
}
