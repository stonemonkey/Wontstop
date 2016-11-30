using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Mvvm.WinRT;
using Mvvm.WinRT.Messages;

namespace Wontstop.Ui.Uwp
{
    public class NavigationService : INavigationService
    {
        private readonly Frame _frame;
        private readonly IEventAggregator _eventAggregator;

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

        public void Navigate(Type pageType)
        {
            DeactivatePreviousViewModel();
            _frame.Navigate(pageType);
        }

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

        public Page CurrentView => _frame.Content as Page;

        public bool CanGoBack => (_frame != null) && _frame.CanGoBack;

        public void GoBack()
        {
            if (CanGoBack)
            {
                _frame.GoBack();
            }
        }

        public bool CanGoForward => (_frame != null) && _frame.CanGoForward;

        public void GoForward()
        {
            if (CanGoForward)
            {
                _frame.GoForward();
            }
        }
    }
}