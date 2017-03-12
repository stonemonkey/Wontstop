// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Newtonsoft.Json;

namespace Wontstop.Climb.Ui.Uwp.Dtos
{
    public class AscentsStatistics
    {
        [JsonProperty(PropertyName = "boulder")]
        public Statistics Boulder { get; set; }

        [JsonProperty(PropertyName = "sport")]
        public Statistics Sport { get; set; }

        [JsonProperty(PropertyName = "ascentscombined")]
        public int AscentsCombined { get; set; }
    }
}