// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MvvmToolkit.Messages;
using MvvmToolkit.Services;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace MvvmToolkit.WinRT
{
    /// <summary>
    /// Provides Page navigation operations.
    /// </summary>
    public class NavigationService : INavigationService
    {
        private readonly Frame _frame;
        private readonly Assembly[] _pageAssemblies;
        private readonly IEventAggregator _eventAggregator;

        /// <summary>
        /// Initializes new instance with the frame and the messaging aggregator.
        /// </summary>
        /// <param name="frame">Frame instance used to perform navigation operations.</param>
        /// <param name="pageAssemblies">Assemblies used to lookup pages.</param>
        /// <param name="eventAggregator">Messaging aggregator instance used to publish navigation 
        /// messages.</param>
        public NavigationService(
            Frame frame, 
            Assembly[] pageAssemblies, 
            IEventAggregator eventAggregator)
        {
            _frame = frame;
            _pageAssemblies = pageAssemblies;
            _eventAggregator = eventAggregator;
            _frame.Navigated += OnFrameNavigated;
        }

        private void OnFrameNavigated(object sender, NavigationEventArgs e)
        {
            var page = e.Content as Page;
            if (page == null)
            {
                return;
            }
  
            var navigationMessage = new NavigationMessage
            {
                Sender = this,
                Parameter = e.Parameter,
                NavigationMode = (int) e.NavigationMode
            };

            _eventAggregator.PublishOnCurrentThread(navigationMessage);

            ActivateViewModel(e.Parameter, page);
        }

        private static void ActivateViewModel(object parameter, Page view)
        {
            var viewModel = view.DataContext as IActivable;
            viewModel?.Activate(parameter);
        }

        /// <summary>
        /// Initiates navigation towards specified Page type.
        /// </summary>
        /// <param name="viewModelType">The type of the view model to navigate.</param>
        public void Navigate(Type viewModelType)
        {
            DeactivatePreviousViewModel();
            _frame.Navigate(GetViewType(viewModelType));
        }

        /// <summary>
        /// Initiates navigation towards specified Page type.
        /// </summary>
        /// <param name="viewModelType">The type of the view model to navigate.</param>
        /// <param name="parameter">The parameter instance transmitted to the page.</param>
        public void Navigate(Type viewModelType, object parameter)
        {
            DeactivatePreviousViewModel();
            _frame.Navigate(GetViewType(viewModelType), parameter);
        }

        private Type GetViewType(Type viewModelType)
        {
            var viewModelTypeName = viewModelType.Name;
            var viewTypeName = viewModelTypeName.Replace("ViewModel", "Page");

            var _types = new List<Type>();
            foreach (var assembly in _pageAssemblies)
            {
                _types.AddRange(assembly.GetTypes()
                    .Where(t => string.Equals(t.Name, viewTypeName, StringComparison.OrdinalIgnoreCase)));
            }
            if (_types.Count() == 0)
            {
                throw new InvalidOperationException(
                    $"Couldn't find page '{viewTypeName}' for view model '{viewModelTypeName}'!");
            }
            if (_types.Count() > 1)
            {
                throw new InvalidOperationException(
                    $"Found multiple pages '{viewTypeName}' for view model '{viewModelTypeName}'!");
            }

            return _types.Single();
        }

        private void DeactivatePreviousViewModel()
        {
            var viewModel = CurrentView.DataContext as IDeactivable;
            viewModel?.Deactivate();
        }

        protected Page CurrentView => _frame.Content as Page;

        /// <summary>
        /// Gets a value that indicates whether there is at least one entry in back navigation
        /// history. True if there is at least one entry in back navigation history; false if there
        /// are no entries in back navigation history or the Frame does not own its own navigation
        /// history.
        /// </summary>
        public bool CanGoBack => (_frame != null) && _frame.CanGoBack;

        /// <summary>
        /// Navigates to the most recent item in back navigation history, if a Frame manages
        /// its own navigation history.
        /// </summary>
        public void GoBack()
        {
            if (CanGoBack)
            {
                _frame.GoBack();
            }
        }

        /// <summary>
        /// Clears back navigation history.
        /// </summary>
        /// <remarks>Pressing back with empty navigation history will exit suspend the app and move 
        /// the user out of the app context.</remarks>
        public void ClearBackStack()
        {
            _frame.BackStack.Clear();
        }

        /// <summary>
        /// Gets a value that indicates whether there is at least one entry in forward navigation
        /// history. True if there is at least one entry in forward navigation history; false if 
        /// there are no entries in forward navigation history or the Frame does not own its own
        /// navigation history. 
        /// </summary>
        public bool CanGoForward => (_frame != null) && _frame.CanGoForward;

        /// <summary>
        /// Navigates to the most recent item in forward navigation history, if a Frame manages
        /// its own navigation history.
        /// </summary>
        public void GoForward()
        {
            if (CanGoForward)
            {
                _frame.GoForward();
            }
        }
    }
}