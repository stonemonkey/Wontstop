// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Problemator.Core.Dtos;
using System;

namespace Problemator.Core.Messages
{
    public class TickRemovedMessage
    {
        public Tick Tick { get; private set; }

        public TickRemovedMessage(Tick tick)
        {
            Tick = tick ?? throw new ArgumentNullException(nameof(tick));
        }
    }
}