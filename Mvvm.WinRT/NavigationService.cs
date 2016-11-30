// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Mvvm.WinRT.Messages;

namespace Mvvm.WinRT
{
    /// <summary>
    /// Provides Page navigation operations
    /// </summary>
    public class NavigationService : INavigationService
    {
        private readonly Frame _frame;
        private readonly IEventAggregator _eventAggregator;

        /// <summary>
        /// Initializes new instance with the frame and the messaging aggregator
        /// </summary>
        /// <param name="frame">Frame instance used to perform navigation operations</param>
        /// <param name="eventAggregator">Messaging aggregator instance used to publish navigation 
        /// messages</param>
        public NavigationService(Frame frame, IEventAggregator eventAggregator)
        {
            _frame = frame;
            _eventAggregator = eventAggregator;
            _frame.Navigated += OnFrameNavigated;
        }

        private void OnFrameNavigated(object sender, NavigationEventArgs e)
        {
            var view = e.Content as Page;
            if (view == null)
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

            ActivateViewModel(e, view);
        }

        private static void ActivateViewModel(object parameter, Page view)
        {
            var viewModel = view.DataContext as IActivable;
            viewModel?.Activate(parameter);
        }

        /// <summary>
        /// Initiates navigation towards specified Page type
        /// </summary>
        /// <param name="pageType">The type of the page to navigate</param>
        public void Navigate(Type pageType)
        {
            DeactivatePreviousViewModel();
            _frame.Navigate(pageType);
        }

        /// <summary>
        /// Initiates navigation towards specified Page type
        /// </summary>
        /// <param name="pageType">The type of the page to navigate</param>
        /// <param name="parameter">The parameter instance transmitted to the page</param>
        public void Navigate(Type pageType, object parameter)
        {
            DeactivatePreviousViewModel();
            _frame.Navigate(pageType, parameter);
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