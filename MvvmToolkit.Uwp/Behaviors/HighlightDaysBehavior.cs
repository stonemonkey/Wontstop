// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Microsoft.Xaml.Interactivity;

namespace MvvmToolkit.Uwp.Behaviors
{
    /// <summary>
    /// Attaches 'highlighted' background color to specified items.
    /// </summary>
    public class HighlightDaysBehavior : Behavior<CalendarDatePicker>
    {
        public static readonly DependencyProperty HighlightedDaysProperty =
            DependencyProperty.Register(
                "HighlightedDays",
                typeof(IList<DateTimeOffset>),
                typeof(HighlightDaysBehavior),
                new PropertyMetadata(null));

        public IList<DateTimeOffset> HighlightedDays
        {
            get { return (IList<DateTimeOffset>)GetValue(HighlightedDaysProperty); }
            set { SetValue(HighlightedDaysProperty, value); }
        }

        public static readonly DependencyProperty HighlightedDayBackgroundProperty =
            DependencyProperty.Register(
                "HighlightedDayBackground",
                typeof(Brush),
                typeof(HighlightDaysBehavior),
                new PropertyMetadata(null));

        public Brush HighlightedDayBackground
        {
            get { return (Brush)GetValue(HighlightedDayBackgroundProperty); }
            set { SetValue(HighlightedDayBackgroundProperty, value); }
        }

        protected override void OnAttached()
        {
            AssociatedObject.CalendarViewDayItemChanging += OnCalendarViewDayItemChanging;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.CalendarViewDayItemChanging -= OnCalendarViewDayItemChanging;
        }

        private void OnCalendarViewDayItemChanging(
            CalendarView sender, CalendarViewDayItemChangingEventArgs e)
        {
            var item = e.Item;
            if (Highlight(item))
            {
                item.Background = HighlightedDayBackground;
            }
        }

        private bool Highlight(CalendarViewDayItem item)
        {
            return HighlightedDays.Any(x => x.Date == item.Date.Date);
        }
    }
}