// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Wontstop.Climb.Ui.Uwp.Dtos
{
    public class AscentsInGyms
    {
        /// <summary>
        /// Key is gymId, value is number of ticks.
        /// </summary>
        [JsonProperty(PropertyName = "boulder")]
        public IDictionary<string, string> Boulder { get; set; }

        /// <summary>
        /// Key is gymId, value is number of ticks.
        /// </summary>
        [JsonProperty(PropertyName = "sport")]
        public IDictionary<string, string> Sport { get; set; } // <gymId, ticks>
    }
}