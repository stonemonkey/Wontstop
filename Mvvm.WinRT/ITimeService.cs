// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Mvvm.WinRT
{
    /// <summary>
    /// Service handling time.
    /// </summary>
    public interface ITimeService
    {
        DateTime Now { get; }
        DateTime UtcNow{ get; }
    }
}