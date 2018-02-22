// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace MvvmToolkit.Services
{
    /// <summary>
    /// Service handling Page navigation.
    /// </summary>
    public interface INavigationService
    {
        void Navigate<TViewModel>();
        void Navigate(Type viewModelType);
        void Navigate<TViewModel>(object parameter);
        void Navigate(Type viewModelType, object parameter);

        bool CanGoBack { get; }
        void GoBack();
        void ClearBackStack();

        bool CanGoForward { get; }
        void GoForward();
    }
}