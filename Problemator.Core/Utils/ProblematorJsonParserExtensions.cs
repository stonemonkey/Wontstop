// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using HttpApiClient.Parsers;

namespace Problemator.Core.Utils
{
    public static class ProblematorJsonParserExtensions
    {
        /// <summary>
        /// Deserializes Problemator response.
        /// </summary>
        /// <typeparam name="T">The type of the instance.</typeparam>
        /// <param name="parser">Parser associated with the response.</param>
        /// <returns>An instance or default(T) in case of an empty Json.</returns>
        public static T To<T>(this ProblematorJsonParser parser)
        {
            var jToken = parser.GetData();
            if (jToken.IsNullOrEmpty())
            {
                return default(T);
            }
                        
            return jToken.ToObject<T>();
        }

        private const string ErrorKey = "error";
        private const string ErrorValue = "true";

        private const string ErrorMessageKey = "message";

        /// <summary>
        /// Retrieves Problemator error message from a response.
        /// </summary>
        /// <param name="parser">Parser associated with the response.</param>
        /// <returns>Error message or empty in case response doesn't contain error.</returns>
        public static string GetErrorMessage(this ProblematorJsonParser parser)
        {
            return parser.GetValue(ErrorMessageKey);
        }

        /// <summary>
        /// Determines if a successfull HTTP response contains Problemator error. 
        /// </summary>
        /// <param name="parser">Parser associated with the response.</param>
        /// <returns>True if the response contains an internal error, otherwise false.</returns>
        public static bool ContainsProblematorError(this ProblematorJsonParser parser)
        {
            return parser.IsResponseSuccessfull() &&
                string.Equals(
                    ErrorValue,
                    parser.GetValue(ErrorKey),
                    StringComparison.OrdinalIgnoreCase);
        }
    }
}