// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MvvmToolkit.Utils;

namespace Problemator.Core.Messages
{
    public class TickAddedMesage
    {
        public string TagShort { get; }

        public TickAddedMesage(string tagShort)
        {
            tagShort.ValidateNotNullEmptyWhiteSpace(nameof(tagShort));

            TagShort = tagShort;
        }
    }
}