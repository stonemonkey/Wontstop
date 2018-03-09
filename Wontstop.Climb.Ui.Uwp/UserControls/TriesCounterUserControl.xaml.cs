// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Wontstop.Climb.Ui.Uwp.UserControls
{
    public sealed partial class TriesCounterUserControl : UserControl
    {
        private const int MinNoTries = 1;
        private const int MaxNoTries = 50;

        public static readonly DependencyProperty CountProperty = DependencyProperty.Register(
            "Count", typeof(int), typeof(TriesCounterUserControl), new PropertyMetadata(MinNoTries, CountChanged));

        private static void CountChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (TriesCounterUserControl) d;
            control.UpdateTriesCountText();
        }

        public int Count
        {
            get { return (int) GetValue(CountProperty); }
            set { SetValue(CountProperty, value); }
        }

        public TriesCounterUserControl()
        {
            InitializeComponent();

            IncrementTriesButton.Click += OnClickIncrementTriesButton;
            DecrementTriesButton.Click += OnClickDecrementTriesButton;
        }

        private void OnClickIncrementTriesButton(object sender, RoutedEventArgs e)
        {
            if (Count < MaxNoTries)
            {
                Count++;
                UpdateTriesCountText();
            }
        }

        private void OnClickDecrementTriesButton(object sender, RoutedEventArgs e)
        {
            if (Count > MinNoTries)
            {
                Count--;
                UpdateTriesCountText();
            }
        }

        private void UpdateTriesCountText()
        {
            TriesCountTextBlock.Text = Count.ToString();
        }
    }
}
