// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MvvmToolkit.Messages;
using Problemator.Core.Dtos;
using System;

namespace Problemator.Core.Messages
{
    public static class MessageExtensions
    {
        public static void PublishShowBusy(this IEventAggregator eventAggregator)
        {
            eventAggregator.PublishOnCurrentThread(new BusyMessage(true));
        }

        public static void PublishHideBusy(this IEventAggregator eventAggregator)
        {
            eventAggregator.PublishOnCurrentThread(new BusyMessage(false));
        }

        public static void PublishAddedTick(this IEventAggregator eventAggregator, string tagShort)
        {
            eventAggregator.PublishOnCurrentThread(new TickAddedMesage(tagShort));
        }

        public static void PublishRemovedTick(this IEventAggregator eventAggregator, Tick tick)
        {
            eventAggregator.PublishOnCurrentThread(new TickRemovedMessage(tick));
        }

        public static void PublishDayChanged(this IEventAggregator eventAggregator, DateTimeOffset newDay)
        {
            eventAggregator.PublishOnCurrentThread(new DayChangedMessage(newDay));
        }

        public static void PublishLocationChanged(this IEventAggregator eventAggregator, string newLocation)
        {
            eventAggregator.PublishOnCurrentThread(new LocationChangedMessage(newLocation));
        }
    }
}