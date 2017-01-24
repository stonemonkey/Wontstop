// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;

namespace Mvvm.WinRT.Messages
{
    /// <summary>
    /// Extensions for <see cref="IEventAggregator"/>.
    /// </summary>
    public static class EventAggregatorExtensions
    {
        /// <summary>
        /// Publishes a message on the current thread (synchrone).
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name = "message">The message instance.</param>
        public static void PublishOnCurrentThread(
            this IEventAggregator eventAggregator, object message)
        {
            eventAggregator.Publish(message, action => action());
        }

        /// <summary>
        /// Publishes a message on a background thread (async).
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name = "message">The message instance.</param>
        public static void PublishOnBackgroundThread(
            this IEventAggregator eventAggregator, object message)
        {
            eventAggregator.Publish(message, action => Task.Factory.StartNew(
                action, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default));
        }

        /// <summary>
        /// Publishes an error message on a current thread (synchrone).
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name = "errorText">The message text.</param>
        public static void PublishErrorMessageOnCurrentThread(
            this IEventAggregator eventAggregator, string errorText)
        {
            var errorMessage = new ErrorMessage(errorText);
            eventAggregator.Publish(errorMessage, action => action());
        }
    }
}