using Newtonsoft.Json;

namespace RunKeeper.WinRT.HealthGraph.Activities
{
    public class HeartRateItemDto
    {
        [JsonProperty("heart_rate")]
        public int HeartRate { get; set; }

        [JsonProperty("timestamp")]
        public int Timestamp { get; set; }
    }
}