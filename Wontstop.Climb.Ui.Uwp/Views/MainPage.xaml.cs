﻿// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Wontstop.Climb.Ui.Uwp.Views
{
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        private Page _activePage;
        public Page ActivePage
        {
            get { return _activePage; }
            set
            {
                _activePage = value;
                Notify();
            }
        }

        private readonly Thickness _menuItemCompactPanelThickness;
        private readonly Thickness _menuItemExpandedPanelThickness;

        public MainPage()
        {
            InitializeComponent();

            TitleTextBlock.DataContext = this;
            ProblemsButton.DataContext = this;

            _menuItemCompactPanelThickness = 
                (Thickness) Application.Current.Resources["MenuItemCompactPanelThickness"];
            _menuItemExpandedPanelThickness = 
                (Thickness) Application.Current.Resources["MenuItemExpandedPanelThickness"];
        }

        private static Page _lastActivePage;

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_lastActivePage == null)
            {
                Activate<ProblemsPage>();
            }
            else
            {
                Activate(_lastActivePage.GetType());
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            _lastActivePage = ActivePage;
        }

        private void OnHamburgerButtonClick(object sender, RoutedEventArgs e)
        {
            AppSplitView.IsPaneOpen = !AppSplitView.IsPaneOpen;
            UpdateTitlePosition();
        }

        private void OnAppSplitViewPaneClosed(SplitView sender, object args)
        {
            UpdateTitlePosition();
        }

        private void OnAppContentFrameSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateTitlePosition();
        }

        private void UpdateTitlePosition()
        {
            TitleBarPanel.Margin = 
                    (AppSplitView.IsPaneOpen) &&
                    (AppSplitView.DisplayMode == SplitViewDisplayMode.Inline ||
                     AppSplitView.DisplayMode == SplitViewDisplayMode.CompactInline) ?
                _menuItemExpandedPanelThickness :
                _menuItemCompactPanelThickness;
        }

        private void OnAppContentFrameNavigated(object sender, NavigationEventArgs e)
        {
            ActivePage = (Page) e.Content;
        }

        private void OnClickProblemsButton(object sender, RoutedEventArgs e)
        {
            Activate<ProblemsPage>();
        }

        private void Activate<T>()
        {
            Activate(typeof(T));
        }

        private void Activate(Type pageType)
        {
            TryCollapseMenu();

            AppContentFrame.Navigate(pageType);
        }

        private void TryCollapseMenu()
        {
            if (AppSplitView.DisplayMode == SplitViewDisplayMode.Overlay ||
                AppSplitView.DisplayMode == SplitViewDisplayMode.CompactOverlay)
            {
                AppSplitView.IsPaneOpen = false;
            }
        }

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
