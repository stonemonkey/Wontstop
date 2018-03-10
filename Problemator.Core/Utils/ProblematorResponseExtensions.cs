// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using HttpApiClient;
using MvvmToolkit.Messages;
using System;
using System.Threading.Tasks;

namespace Problemator.Core.Utils
{
    public static class ProblematorResponseExtensions
    {
        /// <summary>
        /// Determines if the response was successfuly executed by Problemator server. 
        /// </summary>
        /// <param name="response">Response instance.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public static bool Succeded(this Response<ProblematorJsonParser> response)
        {
            return response.IsSuccessfull<ProblematorJsonParser>() &&
                !response.IsProblematorError();
        }

        public static Response<ProblematorJsonParser> OnSuccess(
            this Response<ProblematorJsonParser> response, Action<ProblematorJsonParser> action)
        {
            if (response.Succeded())
            {
                action(response.TypedParser);
            }

            return response;
        }

        public static async Task<Response<ProblematorJsonParser>> OnSuccessAsync(
            this Response<ProblematorJsonParser> response, Func<ProblematorJsonParser, Task> func)
        {
            if (response.Succeded())
            {
                await func(response.TypedParser);
            }

            return response;
        }

        /// <summary>
        /// Determines if the response failed for any of the following reasons: request failure (e.g. 
        /// timeout, invalid URL) or response failure (e.g. HTTP error) or Problemator internal error
        /// (e.g. Login failed).
        /// </summary>
        /// <param name="response">Response instance.</param>
        /// <returns>True if on any failure, otherwise false.</returns>
        public static bool Failed(this Response<ProblematorJsonParser> response)
        {
            return !response.IsSuccessfull<ProblematorJsonParser>() ||
                response.IsProblematorError();
        }

        /// <summary>
        /// Determines if the response failed because of Problemator internal error
        /// (e.g. Login failed).
        /// </summary>
        /// <param name="response">Response instance.</param>
        /// <returns>True if response contains Problemator internal error, otherwise false.</returns>
        public static bool IsProblematorError(this Response<ProblematorJsonParser> response)
        {
            return response.TypedParser != null && 
                response.TypedParser.ContainsProblematorError();
        }

        public static Response<ProblematorJsonParser> OnProblematorError(
            this Response<ProblematorJsonParser> response, Action<ProblematorJsonParser> action)
        {
            if (response.IsProblematorError())
            {
                action(response.TypedParser);
            }

            return response;
        }

        public static async Task<Response<ProblematorJsonParser>> OnProblematorErrorAsync(
            this Response<ProblematorJsonParser> response, Func<ProblematorJsonParser, Task> func)
        {
            if (response.IsProblematorError())
            {
                await func(response.TypedParser);
            }

            return response;
        }

        /// <summary>
        /// Publishes error if the response failed for any of the folowing reasons: request failure 
        /// (e.g. timeout, invalid URL) or the response failed (e.g. HTTP error) or Problemator 
        /// internal error (e.g. Login failed).
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="eventAggregator">The aggregator used to publish error.</param>
        /// <returns>The response received as input for fluent calls.</returns>
        public static Response<ProblematorJsonParser> PublishErrorOnAnyFailure(
            this Response<ProblematorJsonParser> response, IEventAggregator eventAggregator)
        {
            return response
                .OnRequestFailure(x => eventAggregator.PublishOnCurrentThread(x.Exception))
                .OnResponseFailure(x => eventAggregator.PublishErrorMessageOnCurrentThread(x.GetContent()))
                .OnProblematorError(x => eventAggregator.PublishErrorMessageOnCurrentThread(x.GetErrorMessage()));
        }
    }
}