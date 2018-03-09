// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Problemator.Core.Dtos;
using System;

namespace Problemator.Core.Messages
{
    public class TickRemoveMessage
    {
        public Tick Tick { get; private set; }

        public TickRemoveMessage(Tick tick)
        {
            Tick = tick ?? throw new ArgumentNullException(nameof(tick));
        }
    }
}