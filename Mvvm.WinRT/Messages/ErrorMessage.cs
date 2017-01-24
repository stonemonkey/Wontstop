// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Mvvm.WinRT.Messages
{
    /// <summary>
    /// Message send from view models to signal error messages on UI.
    /// </summary>
    public class ErrorMessage
    {
        /// <summary>
        /// The text message to be shown.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Creates instance with associated text message.
        /// </summary>
        /// <param name="text"></param>
        public ErrorMessage(string text)
        {
            Text = text;
        }
    }
}