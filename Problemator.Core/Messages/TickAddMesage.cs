// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;

namespace Problemator.Core.Messages
{
    public class TickAddMesage
    {
        public bool Successfull { get; }
        public string[] FailedToTickTags { get; }

        public TickAddMesage(string[] failedToTickTags)
        {
            if (failedToTickTags == null)
            {
                throw new ArgumentNullException(nameof(failedToTickTags));
            }

            FailedToTickTags = failedToTickTags;
            Successfull = !failedToTickTags.Any();
        }
    }
}