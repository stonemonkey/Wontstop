// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Mvvm.WinRT
{
    /// <summary>
    /// Contracts for the service handling Page navigation
    /// </summary>
    public interface INavigationService
    {
        void Navigate(Type pageType);
        void Navigate(Type pageType, object parameter);

        bool CanGoBack { get; }
        void GoBack();

        bool CanGoForward { get; }
        void GoForward();
    }
}