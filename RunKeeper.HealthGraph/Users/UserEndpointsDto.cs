using Newtonsoft.Json;

namespace RunKeeper.WinRT.HealthGraph.Users
{
    public class UserEndpointsDto
    {
        [JsonProperty("userID")]
        public string UserId { get; set; }
        [JsonProperty("profile")]
        public string ProfileUrl { get; set; }
        [JsonProperty("settings")]
        public string SettingsUrl { get; set; }
        [JsonProperty("fitness_activities")]
        public string FitnessActivitiesUrl { get; set; }
    }
}