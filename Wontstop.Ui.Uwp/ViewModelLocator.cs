// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Mvvm.WinRT;
using Mvvm.WinRT.Messages;
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
            Container.Options.AllowOverridingRegistrations = true;
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

            Container.Register(() => new RunKeeperSessionViewModel(
                Get<IEventAggregator>(), CreateRunKeeperAuthorizationProvider()));
        }

        private static RunKeeperAuth.AuthorizationProvider CreateRunKeeperAuthorizationProvider()
        {
            const string clientId = "7e5cddd793d54e25a0c09a97eaa44279";
            const string clientSecret = "f1bb5680f13a4058be439860dd8cfbfa";

            return new RunKeeperAuth.AuthorizationProvider(clientId, clientSecret);
        }

        public static SettingsViewModel SettingsViewModel => Get<SettingsViewModel>();
        public static RunKeeperSessionViewModel RunKeeperSessionViewModel => Get<RunKeeperSessionViewModel>();
    }
}