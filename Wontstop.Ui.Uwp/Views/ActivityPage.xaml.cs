// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;

namespace Wontstop.Ui.Uwp.Views
{
    public sealed partial class ActivityPage : Page
    {
        public ActivityPage()
        {
            InitializeComponent();

            ActivityMapControl.ColorScheme = 
                (Application.Current.RequestedTheme == ApplicationTheme.Dark) ?
                MapColorScheme.Dark : MapColorScheme.Light;
        }
    }
}
