// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MvvmToolkit;
using MvvmToolkit.Messages;
using MvvmToolkit.Services;
using MvvmToolkit.WinRT;
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
                typeof(Activity),
                () => new Activity(
                    Container.GetInstance<HttpRepository>()));

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
                        GetRunKeeperClientId(), GetRunKeeperClientSecret()));

            Container.RegisterSingleton<RunKeeperAuth.AuthorizationSession>();
        }

        private static string GetRunKeeperClientId()
        {
            object runKeeperClientId = "<not set>";
            Debug.Assert(Application.Current.Resources.TryGetValue(
                "RunKeeperClientId", out runKeeperClientId),
                "RunKeeperClientId is missing from app resources!" +
                "Add <x:String x:Key=\"RunKeeperClientId\">your_id</x:String> in Config.xaml");
            return runKeeperClientId.ToString();
        }

        private static string GetRunKeeperClientSecret()
        {
            object runKeeperClientSecret = "<not set>";
            Debug.Assert(Application.Current.Resources.TryGetValue(
                "RunKeeperClientSecret", out runKeeperClientSecret),
                "RunKeeperClientSecret is missing from app resources!" +
                "Add <x:String x:Key=\"RunKeeperClientSecret\">your_secret</x:String> in Config.xaml");
            return runKeeperClientSecret.ToString();
        }

        public static MainViewModel MainViewModel => Get<MainViewModel>();
        public static AccountViewModel AccountViewModel => Get<AccountViewModel>();
        public static HistoryViewModel HistoryViewModel => Get<HistoryViewModel>();
        public static ActivityViewModel ActivityViewModel => Get<ActivityViewModel>();
        public static LiveActivityViewModel LiveActivityViewModel => Get<LiveActivityViewModel>();
    }
}