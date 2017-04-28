// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MvvmToolkit;
using MvvmToolkit.WinRT;
using Problemator.Core;
using Problemator.Core.Dtos;
using Wontstop.Climb.Ui.Uwp.Views;

namespace Wontstop.Climb.Ui.Uwp
{
    sealed partial class App : Application
    {
        private Frame _rootFrame;

        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            _rootFrame = Window.Current.Content as Frame;
            if (_rootFrame == null)
            {
                _rootFrame = new Frame();
                _rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                Window.Current.Content = _rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (_rootFrame.Content == null)
                {
                    InitLocator();
                    SetupBackButton();
                    _rootFrame.Navigate(GetStartPage(), e.Arguments);
                }

                // Set minimum windows size
                ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(320, 400));

                Window.Current.Activate();
            }
        }

        private static Type GetStartPage()
        {
            var storageService = ServiceLocator.Get<IStorageService>();
            var context = storageService.ReadLocal<UserContext>(Settings.ContextKey);
            if (context == null)
            {
                return typeof (LoginPage);
            }

            var requestsFactory = ServiceLocator.Get<ProblematorRequestsFactory>();
            requestsFactory.SetUserContext(context);
            return typeof (MainPage);
        }

        private void SetupBackButton()
        {
            var navigationManager = SystemNavigationManager.GetForCurrentView();
            navigationManager.BackRequested += OnBackPressed;
            navigationManager.AppViewBackButtonVisibility = _rootFrame.CanGoBack ?
                AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }

        private void OnBackPressed(object sender, BackRequestedEventArgs e)
        {
            if (_rootFrame.CanGoBack && !e.Handled)
            {
                _rootFrame.GoBack();
                e.Handled = true;
            }
        }

        private void InitLocator()
        {
            // force locator to initialize otherwise using directly ServiceLocator may 
            // throw if no DataContext binding to ViewModelLocator is done before
            object locator;
            Debug.Assert(
                Resources.TryGetValue("ViewModelLocator", out locator),
                "Unable to find ViewModelLocator in App.xaml resources!");
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
