// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using HttpApiClient.Requests;
using Mvvm.WinRT;
using Mvvm.WinRT.Messages;
using SimpleInjector;
using Wontstop.Climb.Ui.Uwp.ViewModels;

namespace Wontstop.Climb.Ui.Uwp
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

            Container.RegisterSingleton<ProblematorRequestsFactory>();
            Container.RegisterSingleton<ITimeService, TimeService>();
            Container.RegisterSingleton<IStorageService, StorageService>();
            Container.RegisterSingleton<IResponseLogger, ConsoleResponseLogger>();
        }

        public static LoginViewModel LoginViewModel => Get<LoginViewModel>();
        public static MainViewModel MainViewModel => Get<MainViewModel>();
        public static ProblemsViewModel ProblemsViewModel => Get<ProblemsViewModel>();
        public static ProblemItemViewModel ProblemItemViewModel => Get<ProblemItemViewModel>();
    }
}