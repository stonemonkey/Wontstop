// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MvvmToolkit.Utils;
using Problemator.Core.Dtos;

namespace Problemator.Core.Utils
{
    public static class TickExtensions
    {
        public static string GetTagShort(this Tick tick)
        {
            tick.ValidateNotNull(nameof(tick));

            var tag = tick.Tag;
            if (tag == null)
            {
                return null;
            }

            return tag.Length < 7 ? tag : tag.Substring(7);
        }
    }
}