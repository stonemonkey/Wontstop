// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MvvmToolkit.WinRT.Utils
{
    /// <summary>
    /// Helper methods for managing ListViewBase (ListView and GridView) visual elements.
    /// </summary>
    public static class ListViewBaseExtensions
    {
        /// <summary>
        /// Tries to bring the list view (ListView or GridView) item into the middle of the 
        /// scrollable window.
        /// </summary>
        /// <param name="listView">The list view control containing the item.</param>
        /// <param name="itemSourceObject">The item source object to be centred into visible 
        /// window.</param>
        /// <returns>Awaitable task</returns>
        public static async Task ScrollIntoViewCentredAsync(this ListViewBase listView, object itemSourceObject)
        {
            // TODO [cosmo 2016/21/7] Check the following link in case scroll doesn't work with virtualization 
            // http://stackoverflow.com/questions/32557216/windows-10-scrollintoview-is-not-scrolling-to-the-items-in-the-middle-of-a-lis

            await Task.Delay(10); // wait for listview to complete initialization
            listView.ScrollIntoView(itemSourceObject);

            var listViewItem = await listView.GetListViewItemAsync(itemSourceObject);
            if (listViewItem == null)
            {
                return;
            }

            var scrollViewer = listView.GetFirstScrollViewer();

            var verticalOffset = listView.GetVerticalCentredOffset(listViewItem, scrollViewer);
            var horizontalOffset = listView.GetHorizontalCentredOffset(listViewItem, scrollViewer);

            scrollViewer.ChangeView(horizontalOffset, verticalOffset, null);

            ((Control)listViewItem).Focus(FocusState.Programmatic);
        }

        /// <summary>
        /// Retrives list view (ListView or GridView) item container for an item source object.
        /// </summary>
        /// <param name="listView">The list view control continer.</param>
        /// <param name="itemSourceObject">The item source object.</param>
        /// <returns>Awaitable task returning the list view item container.</returns>
        public static async Task<FrameworkElement> GetListViewItemAsync(
            this ListViewBase listView, object itemSourceObject)
        {
            FrameworkElement listViewItem = null;
            for (var i = 0; (i < 30) && (listViewItem == null); i++)
            {
                await Task.Delay(10); // wait for scrolling to complete
                listViewItem = (FrameworkElement) listView.ContainerFromItem(itemSourceObject);
            }

            return listViewItem;
        }

        /// <summary>
        /// Retrives the first ScrollViewer control child within a parent container.
        /// </summary>
        /// <param name="container">The parent container.</param>
        /// <returns>The scroll viewer control.</returns>
        /// <exception cref="InvalidOperationException">No ScrollViewer child could be found within 
        /// parent container visual tree.</exception>
        public static ScrollViewer GetFirstScrollViewer(this DependencyObject container)
        {
            return container.FindChildrenOf<ScrollViewer>()
                .First();
        }

        /// <summary>
        /// Retrives the vertical offset for the middle of the item within the scroll viewer of 
        /// a container.
        /// </summary>
        /// <param name="container">The parent container control.</param>
        /// <param name="item">The item container control.</param>
        /// <param name="scrollViewer">The scroll viewer control.</param>
        /// <returns>The offset value relative to actual vertical offset of the scroll viewer
        /// control.</returns>
        public static double GetVerticalCentredOffset(
            this FrameworkElement container, FrameworkElement item, ScrollViewer scrollViewer)
        {
            var itemHeight = item.ActualHeight;
            var containerHeight = container.ActualHeight;
            var desiredTop = (containerHeight - itemHeight) / 2.0;

            var top = item.TransformToVisual(container)
                .TransformPoint(new Point(0, 0)).Y;
            var delta = (top - desiredTop);

            var currentOffset = scrollViewer.VerticalOffset;
            return (currentOffset + delta);
        }

        /// <summary>
        /// Retrives the horizontal offset for the middle of the item within the scroll viewer of 
        /// a container.
        /// </summary>
        /// <param name="container">The parent container control.</param>
        /// <param name="item">The item container control.</param>
        /// <param name="scrollViewer">The scroll viewer control.</param>
        /// <returns>The offset value relative to actual horizontal offset of the scroll viewer
        /// control.</returns>
        public static double GetHorizontalCentredOffset(
            this FrameworkElement container, FrameworkElement item, ScrollViewer scrollViewer)
        {
            var itemWidth = item.ActualWidth;
            var containerWidth = container.ActualWidth;
            var desiredLeft = (containerWidth - itemWidth) / 2.0;

            var left = item.TransformToVisual(container)
                .TransformPoint(new Point(0, 0)).X;
            var delta = (left - desiredLeft);

            var currentOffset = scrollViewer.HorizontalOffset;

            return (currentOffset + delta);
        }

        /// <summary>
        /// Focusess the first item within the list view control (ListView or GridView). 
        /// Doesn't bring the item into the visible window.
        /// </summary>
        /// <param name="listView">The list view control.</param>
        /// <returns>Awaitable task.</returns>
        public static async Task FocusFirstItemAsync(this ListViewBase listView)
        {
            var itemModel = listView.Items.FirstOrDefault();

            await listView.FocusItemAsync(itemModel);
        }

        /// <summary>
        /// Focusess the item within the list view control (ListView or GridView). 
        /// Doesn't bring the item into the visible window.
        /// </summary>
        /// <param name="listView">The list view control.</param>
        /// <param name="itemSourceObject">The item source object.</param>
        /// <returns>Awaitable task.</returns>
        public static async Task FocusItemAsync(this ListViewBase listView, object itemSourceObject)
        {
            if (itemSourceObject == null)
            {
                return;
            }

            var item = (Control) await listView.GetListViewItemAsync(itemSourceObject);
            item?.Focus(FocusState.Programmatic);
        }
    }
}
