// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Mvvm.WinRT;
using Mvvm.WinRT.Messages;
using RunKeeper.WinRT.HealthGraph.Activities;
using RunKeeper.WinRT.HealthGraph.Infrastructure;
using RunKeeper.WinRT.HealthGraph.User;
using SimpleInjector;
using Wontstop.Ui.Uwp.ViewModels;
using RunKeeperAuth = RunKeeper.WinRT.HealthGraph.Authorization;

namespace Wontstop.Ui.Uwp
{
    /// <summary>
    /// Automates view models dependency injection.
    /// </summary>
    public class ViewModelLocator : ServiceLocator
    {
        /// <summary>
        /// Creates the instance setting up the container and registering the dependencies.
        /// </summary>
        public ViewModelLocator()
        {
            SetupIoC();
        }

        private void SetupIoC()
        {
            CreateContainer();
            SetupLocator();

            RegisterDependencies();
        }

        /// <summary>
        /// Internal IoC container
        /// </summary>
        protected Container Container;

        /// <summary>
        /// Creates and configures internal container.
        /// </summary>
        protected virtual void CreateContainer()
        {
            Container = new Container();
            Container.Options.AllowOverridingRegistrations = false;
        }

        /// <summary>
        /// Initializes the locator (ServiceLocator class) used for resolving on dependencies.
        /// </summary>
        protected virtual void SetupLocator()
        {
            GetInstance = service => Container.GetInstance(service);
        }

        /// <summary>
        /// Registers the types used as dependencies by the application.
        /// </summary>
        protected virtual void RegisterDependencies()
        {
            Container.RegisterSingleton<IEventAggregator, EventAggregator>();

            Container.RegisterSingleton(
                typeof(INavigationService),
                () => new NavigationService(
                    (Frame) Window.Current.Content,
                    Container.GetInstance<IEventAggregator>()));

            Container.Register(
                typeof(History),
                () => new History(
                    Container.GetInstance<HttpRepository>()));

            Container.Register(
                typeof(UserProfile),
                () => new UserProfile(
                    Container.GetInstance<LocalStorageRepository>(), 
                    Container.GetInstance<HttpRepository>()));

            Container.RegisterSingleton(
                typeof(UserResources),
                () => new UserResources(
                    Container.GetInstance<LocalStorageRepository>(), 
                    Container.GetInstance<HttpRepository>()));

            Container.RegisterSingleton(
                typeof(IAuthorizationProvider),
                () => new RunKeeperAuth.AuthorizationProvider(
                        "<clientId>", "<clientSecret> "));

            Container.RegisterSingleton<RunKeeperAuth.AuthorizationSession>();
        }

        public static ActivitiesViewModel ActivitiesViewModel => Get<ActivitiesViewModel>();
        public static ActivityViewModel ActivityViewModel => Get<ActivityViewModel>();
        public static SettingsViewModel SettingsViewModel => Get<SettingsViewModel>();
    }
}