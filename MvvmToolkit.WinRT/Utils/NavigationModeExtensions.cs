// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MvvmToolkit.WinRT.Utils
{
    /// <summary>
    /// Helper methods for determining the navigation stack characteristics of a navigation operation.
    /// </summary>
    public static class NavigationModeExtensions
    {
        /// Navigation is to a new instance of a page (not going forward or backward in the stack).
        public static bool IsNew(this int mode)
        {
            return mode == 0;
        }

        /// Navigation is going backward in the stack.
        public static bool IsBack(this int mode)
        {
            return mode == 1;
        }

        /// Navigation is going forward in the stack.
        public static bool IsForward(this int mode)
        {
            return mode == 2;
        }

        /// Navigation is to the current page (perhaps with different data).
        public static bool IsRefresh(this int mode)
        {
            return mode == 3;
        }
    }
}