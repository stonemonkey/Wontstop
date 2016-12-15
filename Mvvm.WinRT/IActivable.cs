// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Mvvm.WinRT
{
    /// <summary>
    /// Activable type, usualy it is implemented by view models to handle activation on navigation 
    /// e.g. parsing and/or saving parameter for later use when the view model is loading data.
    /// </summary>
    public interface IActivable
    {
        void Activate(object parameter);
    }
}