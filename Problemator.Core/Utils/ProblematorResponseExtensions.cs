// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using HttpApiClient;
using MvvmToolkit.Messages;

namespace Problemator.Core.Utils
{
    public static class ProblematorResponseExtensions
    {
        /// <summary>
        /// Determines if the response failed for any of the following reasons: request failure (e.g. 
        /// timeout, invalid URL) or response failure (e.g. HTTP error) or Problemator internal error.
        /// </summary>
        /// <param name="response">Response instance.</param>
        /// <returns>True if response doesn't contain , otherwise false.</returns>
        public static bool Failed(this Response<ProblematorJsonParser> response)
        {
            return !response.IsSuccessfull<ProblematorJsonParser>() ||
                response.TypedParser.ContainsServerInternalError();
        }

        /// <summary>
        /// Publishes error if the response failed for any of the folowing reasons: request failure 
        /// (e.g. timeout, invalid URL) or the response failed (e.g. HTTP error).
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="eventAggregator">The aggregator used to publish error.</param>
        /// <returns>The response received as input for fluent calls.</returns>
        public static Response<ProblematorJsonParser> PublishErrorOnHttpFailure(
            this Response<ProblematorJsonParser> response, IEventAggregator eventAggregator)
        {
            return response
                .OnRequestFailure(x => eventAggregator.PublishOnCurrentThread(x.Exception))
                .OnResponseFailure(x => eventAggregator.PublishErrorMessageOnCurrentThread(x.GetContent()));
        }
    }
}