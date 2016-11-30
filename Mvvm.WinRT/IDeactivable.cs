// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Mvvm.WinRT
{
    /// <summary>
    /// Contract for deactivable types, usualy it is implemented by view models to handle 
    /// deactivation on navigation e.g. cleanup internal state
    /// </summary>
    public interface IDeactivable
    {
        void Deactivate();
    }
}