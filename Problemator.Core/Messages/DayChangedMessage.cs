﻿// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Problemator.Core.Messages
{
    public class DayChangedMessage
    {
        public DateTimeOffset NewDay { get; }

        public DayChangedMessage(DateTimeOffset newDay)
        {
            NewDay = newDay;
        }
    }
}