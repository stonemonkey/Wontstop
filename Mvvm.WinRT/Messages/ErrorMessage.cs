// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Mvvm.WinRT.Messages
{
    public class ErrorMessage
    {
        public string Text { get; private set; }

        public ErrorMessage(string text)
        {
            Text = text;
        }
    }
}