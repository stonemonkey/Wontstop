// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Wontstop.Climb.Ui.Uwp.Dtos
{
    public class Dashboard
    {
        [JsonProperty(PropertyName = "climber")]
        public Climber Climber { get; set; }

        [JsonProperty(PropertyName = "locations")]
        public IList<Location> Locations { get; set; }

        [JsonProperty(PropertyName = "grades")]
        public IDictionary<string, Grade> Grades { get; set; }

        [JsonProperty(PropertyName = "mysettings")]
        public UserSettings UserSettings { get; set; }

        [JsonProperty(PropertyName = "climbinfo")]
        public ClimbingStatistics ClimbingStatistics { get; set; }
    }
}