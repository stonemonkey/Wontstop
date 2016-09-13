namespace RunKeeper.WinRT.HealthGraph.User
{
    /// <summary>
    /// RunKeeper user available resources (API URLs)
    /// </summary>
    public class UserResources : ReadOnlyKeyValueModel
    {
        /// <summary>
        /// Retrives user id for the active session.
        /// </summary>
        /// <returns>The id</returns>
        public string Id => GetValue("userID");

        /// <summary>
        /// Retrives user profile API url for the active session.
        /// </summary>
        /// <returns>The url</returns>
        public string ProfileUrl =>  GetUrl("profile");

        /// <summary>
        /// Retrives user settings API url for the active session.
        /// </summary>
        /// <returns>The url</returns>
        public string SettingsUrl => GetUrl("settings");

        /// <summary>
        /// Retrives user activities API url for the active session.
        /// </summary>
        /// <returns>The url</returns>
        public string ActivitiesUrl => GetUrl("fitness_activities");
        
        private string GetUrl(string key)
        {
            var value = GetValue(key);
            return value == null ? null : $"{Urls.ApiUrl}{value}";
        }
    }
}
