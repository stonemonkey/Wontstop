// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Common.Uwp.Services
{
    /// <summary>
    /// Cache storage file metadata used to clear cache based on size or touch time.
    /// </summary>
    internal class CacheFile
    {
        /// <summary>
        /// The name of the file.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The size of the file in bytes.
        /// </summary>
        public ulong Size { get; set; }

        /// <summary>
        /// Last time modified.
        /// </summary>
        public DateTimeOffset Modified { get; set; }
    }
}