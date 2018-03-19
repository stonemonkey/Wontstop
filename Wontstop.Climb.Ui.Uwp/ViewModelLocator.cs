// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MvvmToolkit;
using MvvmToolkit.Messages;
using MvvmToolkit.Services;
using MvvmToolkit.WinRT;
using Problemator.Core;
using Problemator.Core.ViewModels;
using SimpleInjector;
using HttpApiClient;
using System.Reflection;
using Problemator.Core.Models;

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
                    new [] { Assembly.GetAssembly(GetType()) },
                    Container.GetInstance<IEventAggregator>()));

            Container.RegisterSingleton<Session>();
            Container.RegisterSingleton<Sections>();
            Container.RegisterSingleton<UserContext>();
            Container.RegisterSingleton<ProblematorRequestsFactory>();
            Container.RegisterSingleton<ITimeService, TimeService>();
            Container.RegisterSingleton<IStorageService, StorageService>();
            Container.RegisterSingleton<IResponseLogger, ConsoleResponseLogger>();
        }

        public static LoginViewModel LoginViewModel => Get<LoginViewModel>();
        public static MainViewModel MainViewModel => Get<MainViewModel>();
        public static TicksChildViewModel TicksViewModel => Get<TicksChildViewModel>();
        public static ProblemsChildViewModel ProblemsViewModel => Get<ProblemsChildViewModel>();
        public static TagProblemsViewModel TagProblemsViewModel => Get<TagProblemsViewModel>();
        public static TickDetailsViewModel TickDetailsViewModel => Get<TickDetailsViewModel>();
        public static TickItemViewModel TickItemViewModel => Get<TickItemViewModel>();
        public static ProblemItemViewModel ProblemItemViewModel => Get<ProblemItemViewModel>();
        public static ProblemDetailesViewModel ProblemDetailesViewModel => Get<ProblemDetailesViewModel>();
        public static ProblemItemTickViewModel ProblemItemTickViewModel => Get<ProblemItemTickViewModel>();
    }
}