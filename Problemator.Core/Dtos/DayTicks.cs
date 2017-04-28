// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Problemator.Core.Dtos
{
    public class DayTicks
    {
        [JsonProperty(PropertyName = "ticksinday")]
        public IList<Tick> Ticks { get; set; }
    }
}