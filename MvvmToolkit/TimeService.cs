// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace MvvmToolkit
{
    /// <summary>
    /// Provides wrappers over time getters so that we get rid of hard dependencier usefull to sync 
    /// later the time with backend or test time dependent code.
    /// </summary>
    public class TimeService : ITimeService
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
    }
}