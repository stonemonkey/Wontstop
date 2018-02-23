// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using MvvmToolkit.Services;

namespace Wontstop.Climb.Ui.Uwp
{
    public class NavigationService : INavigationService
    {
        public bool CanGoBack => throw new NotImplementedException();

        public bool CanGoForward => throw new NotImplementedException();

        public void ClearBackStack()
        {
            throw new NotImplementedException();
        }

        public void GoBack()
        {
            throw new NotImplementedException();
        }

        public void GoForward()
        {
            throw new NotImplementedException();
        }

        public void Navigate(Type pageType)
        {
            throw new NotImplementedException();
        }

        public void Navigate(string pageType)
        {
            throw new NotImplementedException();
        }

        public void Navigate(Type pageType, object parameter)
        {
            throw new NotImplementedException();
        }

        public void Navigate(string pageType, object parameter)
        {
            throw new NotImplementedException();
        }

        public void Navigate<TViewModel>()
        {
            throw new NotImplementedException();
        }

        public void Navigate<TViewModel>(object parameter)
        {
            throw new NotImplementedException();
        }
    }
}