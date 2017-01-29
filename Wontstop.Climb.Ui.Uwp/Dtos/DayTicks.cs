// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Wontstop.Climb.Ui.Uwp.Dtos
{
    public class DayTicks
    {
        [JsonProperty(PropertyName = "ticksinday")]
        public IList<Tick> Ticks { get; set; }
    }
}