using System;
using Newtonsoft.Json;

namespace RunKeeper.WinRT.HealthGraph.Activities
{
    public class ActivityDto
    {
        [JsonProperty("userID")]
        public string UserId { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("share_map")]
        public string ShareMapMode { get; set; }

        [JsonProperty("activity")]
        public string RunKeeperUrl { get; set; }

        [JsonProperty("is_live")]
        public bool IsLive { get; set; }

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

        [JsonProperty("average_heart_rate")]
        public int AverageHeartRate { get; set; }

        [JsonProperty("climb")]
        public double TotalClimb { get; set; }

        [JsonProperty("total_calories")]
        public double TotalCalories { get; set; }

        [JsonProperty("total_distance")]
        public double TotalDistance { get; set; }

        [JsonProperty("next")]
        public string NextPath { get; set; }

        [JsonProperty("uri")]
        public string ResourcePath { get; set; }

        [JsonProperty("previous")]
        public string PreviousPath { get; set; }

        [JsonProperty("comments")]
        public string CommentsPath { get; set; }

        [JsonProperty("path")]
        public TrackItemDto[] Track { get; set; }

        [JsonProperty("distance")]
        public DistanceItemDto[] Distance { get; set; }

        [JsonProperty("heart_rate")]
        public HeartRateItemDto[] HeartRate { get; set; }
    }
}