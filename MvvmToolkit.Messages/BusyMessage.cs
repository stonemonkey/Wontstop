// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MvvmToolkit.Messages
{
    /// <summary>
    /// Message send from view models to signal show/hide busy on UI.
    /// </summary>
    public class BusyMessage
    {
        /// <summary>
        /// Determines wheather show (true) or to hide (false) operation is requested.
        /// </summary>
        public bool Show { get; private set; }

        /// <summary>
        /// Text associated with show busy operation.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Create instance for toggeling busy indicator.
        /// </summary>
        /// <param name="show">Flag determining busy operation show = true, hide = false</param>
        public BusyMessage(bool show)
        {
            Show = show;
            Text = null;
        }

        /// <summary>
        /// Create instance for showing busy with text message.
        /// </summary>
        /// <param name="text">The text to be shown to the user while busy</param>
        public BusyMessage(string text)
        {
            Show = true;
            Text = text;
        }
    }
}
