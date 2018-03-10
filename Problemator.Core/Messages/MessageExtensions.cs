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

        public static void PublishFailedToAdd(this IEventAggregator eventAggregator, string [] tags)
        {
            eventAggregator.PublishOnCurrentThread(new TickAddMesage(tags));
        }

        public static void PublishRemove(this IEventAggregator eventAggregator, Tick tick)
        {
            eventAggregator.PublishOnCurrentThread(new TickRemoveMessage(tick));
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