// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace Wontstop.Ui.Uwp.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            var mainItems = CreateMainItems();
            HamburgerMenuControl.ItemsSource = mainItems;
            HamburgerMenuControl.OptionsItemsSource = CreateOptionsItems();

            ContentFrame.Navigate(mainItems.First().PageType);
        }

        private static List<HamburgerMenuItem> CreateMainItems()
        {
            return new List<HamburgerMenuItem>
            {
                new HamburgerMenuItem {Icon = Symbol.Globe, Name = "Feeds", PageType = typeof (FeedsPage)},
                new HamburgerMenuItem {Icon = Symbol.Calendar, Name = "Activities", PageType = typeof (ActivitiesPage)}
            };
        }

        private static List<HamburgerMenuItem> CreateOptionsItems()
        {
            return new List<HamburgerMenuItem>
            {
                new HamburgerMenuItem {Icon = Symbol.Setting, Name = "Settings", PageType = typeof (SettingsPage)}
            };
        }

        private void OnMenuItemClick(object sender, ItemClickEventArgs e)
        {
            var menuItem = (HamburgerMenuItem) e.ClickedItem;
            ContentFrame.Navigate(menuItem.PageType);
        }
    }
}
