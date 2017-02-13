// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using HttpApiClient;
using Mvvm.WinRT.Messages;

namespace Wontstop.Climb.Ui.Uwp.Utils
{
    public static class ProblematorJsonParserExtensions
    {
        public static T To<T>(this ProblematorJsonParser parser)
        {
            return parser.GetData().ToObject<T>();
        }

        private const string ErrorKey = "error";
        private const string ErrorValue = "true";

        public static bool IsError(this ProblematorJsonParser parser)
        {
            return string.Equals(
                ErrorValue, 
                parser.GetValue(ErrorKey), 
                StringComparison.OrdinalIgnoreCase);
        }

        private const string ErrorMessageKey = "message";

        public static string GetErrorMessage(this ProblematorJsonParser parser)
        {
            return parser.GetValue(ErrorMessageKey);
        }

        public static bool PublishMessageOnError(
            this ProblematorJsonParser parser, IEventAggregator eventAggregator)
        {
            var isError = parser.IsInternalError();
            if (isError)
            {
                eventAggregator.PublishErrorMessageOnCurrentThread(parser.GetErrorMessage());
            }

            return isError;
        }

        public static bool IsInternalError(this ProblematorJsonParser parser)
        {
            return string.Equals(
                ErrorValue,
                parser.GetValue(ErrorKey),
                StringComparison.OrdinalIgnoreCase);
        }

        public static Response<ProblematorJsonParser> PublishErrorOnFailure(
            this Response<ProblematorJsonParser> response, IEventAggregator eventAggregator)
        {
            return response
                .OnRequestFailure(x => eventAggregator.PublishOnCurrentThread(x.Exception))
                .OnResponseFailure(x => eventAggregator.PublishErrorMessageOnCurrentThread(x.GetContent()));
        }
    }
}