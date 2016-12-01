using Newtonsoft.Json;

namespace RunKeeper.WinRT.HealthGraph.Activities
{
    public class TrackItemDto
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }

        [JsonProperty("altitude")]
        public double Altitude { get; set; }

        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        [JsonProperty("latitude")]
        public double Latitude { get; set; }
    }
}