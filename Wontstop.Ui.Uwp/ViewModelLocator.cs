// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Mvvm.WinRT;
using Mvvm.WinRT.Messages;
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
        /// Creates and configures internal ServiceLocator container.
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
                typeof(UserResources),
                () => new UserResources(
                    Container.GetInstance<LocalStorageRepository>(), 
                    Container.GetInstance<HttpRepository>()));

            Container.RegisterSingleton(
                typeof(UserProfile),
                () => new UserProfile(
                    Container.GetInstance<LocalStorageRepository>(), 
                    Container.GetInstance<HttpRepository>()));

            Container.RegisterSingleton(
                typeof(IAuthorizationProvider),
                () => new RunKeeperAuth.AuthorizationProvider(
                        "85588e575f7e46b0a2b8b982e00162c6", "10f5ff51664c42989f1c8e7eb6d310ac"));

            Container.RegisterSingleton<RunKeeperAuth.AuthorizationSession>();
        }

        public static SettingsViewModel SettingsViewModel => Get<SettingsViewModel>();
        public static ActivitiesViewModel ActivitiesViewModel => Get<ActivitiesViewModel>();
    }
}