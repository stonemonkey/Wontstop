// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Mvvm.WinRT.Messages
{
    /// <summary>
    /// Message send from view models to request scroll operation on ListViewBase controls
    /// </summary>
    public class ScrollMessage
    {
        /// <summary>
        /// The item that should be scrolled into view
        /// </summary>
        public object Item { get; set; }
    }
}