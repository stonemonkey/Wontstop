// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Wontstop.Climb.Ui.Uwp.Utils
{
    public static class ProblematorJsonParserExtensions
    {
        public static T To<T>(this ProblematorJsonParser parser)
        {
            return parser.GetData().ToObject<T>();
        }

        public static bool IsError(this ProblematorJsonParser parser)
        {
            return string.Equals(
                "true", 
                parser.GetValue("error"), 
                StringComparison.OrdinalIgnoreCase);
        }

        public static string GetErrorMessage(this ProblematorJsonParser parser)
        {
            return parser.GetValue("message");
        }
    }
}