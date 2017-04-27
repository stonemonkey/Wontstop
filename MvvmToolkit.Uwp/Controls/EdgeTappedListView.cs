// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace MvvmToolkit.Uwp.Controls
{
    public delegate void ListViewEdgeTappedEventHandler(ListView sender, EdgeTappedListViewEventArgs e);

    public class EdgeTappedListView : ListView
    {
        #region IsItemLeftEdgeTapEnabled property

        public static readonly DependencyProperty IsItemLeftEdgeTapEnabledProperty = 
            DependencyProperty.Register(
                "IsItemLeftEdgeTapEnabled", 
                typeof(bool), 
                typeof(EdgeTappedListView), 
                new PropertyMetadata(default(bool), IsItemLeftEdgeTapEnabledChanged));

        private static void IsItemLeftEdgeTapEnabledChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var listView = (EdgeTappedListView) d;
            var enabled = (bool) args.NewValue;
            if (enabled)
            {
                listView.TryRestoreSelectionMode();
            }
            else
            {
                listView.EnableMultiSelection();
            }
        }

        private bool _isSelectionModeChanging;

        private void EnableMultiSelection()
        {
            SaveSelectionMode();

            _isSelectionModeChanging = true;
            SelectionMode = ListViewSelectionMode.Multiple;
            _isSelectionModeChanging = false;
        }

        private ListViewSelectionMode? _savedSelectionMode;

        private void SaveSelectionMode()
        {
            _savedSelectionMode = SelectionMode;
        }

        private void TryRestoreSelectionMode()
        {
            if (_savedSelectionMode.HasValue)
            {
                SelectionMode = _savedSelectionMode.Value;
                _savedSelectionMode = null;
            }
        }

        public bool IsItemLeftEdgeTapEnabled
        {
            get { return (bool) GetValue(IsItemLeftEdgeTapEnabledProperty); }
            set { SetValue(IsItemLeftEdgeTapEnabledProperty, value); }
        }

        #endregion

        public event ItemClickEventHandler ItemClickOnSingleSelection;
        public event ListViewEdgeTappedEventHandler ItemLeftEdgeTapped;

        private ListViewItem _edgeTappedItem;

        public EdgeTappedListView()
        {
            _edgeTappedItem = null;

            ItemClick += OnItemClick;
            SelectionChanged += OnSelectionChanged;
            ContainerContentChanging += OnContainerContentChanging;
        }

        private void OnItemClick(object sender, ItemClickEventArgs e)
        {
            if (SelectionMode == ListViewSelectionMode.Single)
            {
                ItemClickOnSingleSelection?.Invoke(this, e);
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((SelectedItems.Count == 0) && !_isSelectionModeChanging)
            {
                IsItemLeftEdgeTapEnabled = true;
            }
        }

        private void OnContainerContentChanging(
            ListViewBase sender, ContainerContentChangingEventArgs e)
        {
            if (e.InRecycleQueue)
            {
                var lvi = (e.ItemContainer as ListViewItem);
                if (lvi != null)
                {
                    var element = VisualTreeHelper.GetChild(lvi, 0) as UIElement;
                    if (element != null)
                    {
                        element.PointerExited -= OnPointerExited;
                        element.PointerPressed -= OnPointerPressed;
                        element.PointerReleased -= OnPointerReleased;
                        element.PointerCaptureLost -= OnPointerCaptureLost;
                    }
                }
            }
            else if (e.Phase == 0)
            {
                var lvi = (e.ItemContainer as ListViewItem);
                if (lvi != null)
                {
                    var element = VisualTreeHelper.GetChild(lvi, 0) as UIElement;
                    if (element != null)
                    {
                        element.PointerExited += OnPointerExited;
                        element.PointerPressed += OnPointerPressed;
                        element.PointerReleased += OnPointerReleased;
                        element.PointerCaptureLost += OnPointerCaptureLost;
                    }
                }
            }
        }

        void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            TryEnableMultiSelection();
        }

        private const double HitTarget = 32;
        void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (IsItemLeftEdgeTapEnabled)
            {
                if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
                {
                    var element = (sender as UIElement);
                    if (element != null)
                    {
                        var pointerPoint = e.GetCurrentPoint(element);
                        if (pointerPoint.Position.X <= HitTarget)
                        {
                            _edgeTappedItem = VisualTreeHelper.GetParent(element) as ListViewItem;
                        }
                    }
                }
            }
        }

        void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            TryEnableMultiSelection();
        }

        private void OnPointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            TryEnableMultiSelection();
        }

        private void TryEnableMultiSelection()
        {
            if (_edgeTappedItem == null)
            {
                return;
            }

            IsItemLeftEdgeTapEnabled = false;
            ItemLeftEdgeTapped?.Invoke(this, new EdgeTappedListViewEventArgs(_edgeTappedItem));

            _edgeTappedItem = null;
        } 
    }
}