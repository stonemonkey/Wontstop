// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Newtonsoft.Json;

namespace RunKeeper.WinRT.HealthGraph.Activities
{
    public class ActivityHistoryItemDto
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("has_map")]
        public bool HasMap { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("entry_mode")]
        public string EntryMode { get; set; }

        [JsonProperty("tracking_mode")]
        public string TrackingMode { get; set; }

        [JsonProperty("utc_offset")]
        public int UtcOffset { get; set; }

        [JsonProperty("duration")]
        public double Duration { get; set; }

        [JsonProperty("start_time")]
        public DateTime StartTime { get; set; }

        [JsonProperty("total_calories")]
        public int TotalCalories { get; set; }

        [JsonProperty("total_distance")]
        public double TotalDistance { get; set; }

        [JsonProperty("uri")]
        public string ResourcePath { get; set; }
    }
}