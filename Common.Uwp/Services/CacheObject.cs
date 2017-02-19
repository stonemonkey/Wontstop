// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Common.Uwp.Services
{
    /// <summary>
    /// Used as a wrapper around the stored object to keep metadata.
    /// </summary>
    internal class CacheObject
    {
        /// <summary>
        /// Expire date of cached object.
        /// </summary>
        public DateTime? ExpireDateTime { get; set; }

        /// <summary>
        /// Determines if the cache object is valid at specified date.
        /// </summary>
        public bool IsValidAt(DateTime date)
        {
            return (ExpireDateTime == null || ExpireDateTime.Value > date);
        }
    }

    /// <summary>
    /// Used as a wrapper around the stored object to keep metadata.
    /// </summary>
    /// <typeparam name="T">The type of the stored object.</typeparam>
    internal class CacheObject<T> : CacheObject
    {
        /// <summary>
        /// Actual object data being stored.
        /// </summary>
        public T Obj { get; set; }
    }
}