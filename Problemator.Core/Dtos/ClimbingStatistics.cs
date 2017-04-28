// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Newtonsoft.Json;

namespace Problemator.Core.Dtos
{
    public class ClimbingStatistics
    {
        [JsonProperty(PropertyName = "today")]
        public AscentsStatistics Today { get; set; }

        [JsonProperty(PropertyName = "month")]
        public AscentsStatistics Month { get; set; }

        [JsonProperty(PropertyName = "alltime")]
        public AscentsStatistics AllTime { get; set; }

        [JsonProperty(PropertyName = "ascentsingyms")]
        public AscentsInGyms AscentsInGyms { get; set; }
    }
}