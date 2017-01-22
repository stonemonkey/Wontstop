﻿// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Mvvm.WinRT;
using Mvvm.WinRT.Messages;
using Mvvm.WinRT.Utils;

namespace Wontstop.Climb.Ui.Uwp.Views
{
    public sealed partial class LoginPage : Page, IHandle<Exception>, IHandle<ErrorMessage>
    {
        private readonly IEventAggregator _eventAggregator;

        public LoginPage()
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

        public async void Handle(ErrorMessage message)
        {
            await message.ShowErrorAsync();
        }
    }
}
