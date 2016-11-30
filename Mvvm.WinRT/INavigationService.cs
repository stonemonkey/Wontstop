using System;

namespace Mvvm.WinRT
{
    public interface INavigationService
    {
        void Navigate(Type type);
        void Navigate(Type type, object parameter);

        bool CanGoBack { get; }
        void GoBack();

        bool CanGoForward { get; }
        void GoForward();
    }
}