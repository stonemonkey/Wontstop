// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Mvvm.WinRT.Messages
{
    /// <summary>
    /// Message send on Page navigation
    /// </summary>
    public class NavigationMessage
    {
        /// <summary>
        /// The instance sending the message, usualy is INavigationService
        /// </summary>
        public object Sender { get; set; }

        /// <summary>
        /// The parameter transmitted on navigation
        /// </summary>
        public object Parameter { get; set; }

        /// <summary>
        /// Navigation operation mode <see cref="NavigationModeExtensions"/>
        /// </summary>
        public int NavigationMode { get; set; }
    }
}