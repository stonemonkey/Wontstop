// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Mvvm.WinRT.Utils
{
    public static class ExceptionExtensions
    {
        public static async Task ShowErrorAsync(this Exception exception)
        {
            var dialog = new MessageDialog(
                $"{exception.GetType()}\n{exception.Message}", "Error");
            await dialog.ShowAsync();
        }
    }
}