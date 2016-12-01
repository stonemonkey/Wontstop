using Newtonsoft.Json;

namespace RunKeeper.WinRT.HealthGraph.Activities
{
    public class DistanceItemDto
    {
        [JsonProperty("distance")]
        public double Distance { get; set; }

        [JsonProperty("timestamp")]
        public int Timestamp { get; set; }
    }
}