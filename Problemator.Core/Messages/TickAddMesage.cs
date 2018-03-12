﻿// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Problemator.Core.Messages
{
    public class TickAddMesage
    {
        public string TagShort { get; }

        public TickAddMesage(string tagShort)
        {
            if (string.IsNullOrWhiteSpace(tagShort))
            {
                throw new ArgumentOutOfRangeException(nameof(tagShort));
            }

            TagShort = tagShort;
        }
    }
}