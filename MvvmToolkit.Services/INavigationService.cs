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
        void Navigate(Type pageType);
        void Navigate(string pageType);
        void Navigate(Type pageType, object parameter);
        void Navigate(string pageType, object parameter);

        bool CanGoBack { get; }
        void GoBack();
        void ClearBackStack();

        bool CanGoForward { get; }
        void GoForward();
    }
}