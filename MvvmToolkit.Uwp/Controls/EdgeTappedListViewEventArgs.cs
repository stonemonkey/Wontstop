// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Windows.UI.Xaml.Controls;

namespace MvvmToolkit.Uwp.Controls
{
    public class EdgeTappedListViewEventArgs : EventArgs
    {
        public ListViewItem ListViewItem { get; }

        public EdgeTappedListViewEventArgs(ListViewItem listViewItem)
        {
            ListViewItem = listViewItem;
        }
    }
}