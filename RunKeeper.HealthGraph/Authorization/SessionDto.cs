using Newtonsoft.Json;

namespace RunKeeper.WinRT.HealthGraph.Authorization
{
    /// <summary>
    /// Session DTO received from RunKeeper in the authorization process
    /// </summary>
    public class SessionDto
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
}