namespace RunKeeper.WinRT.HealthGraph.User
{
    /// <summary>
    /// RunKeeper profile info
    /// </summary>
    public class UserProfile : KeyValueModel
    {
        private const string NameKey = "name";
        /// <summary>
        /// User's full name
        /// </summary>
        public string Name => GetValue(NameKey);

        private const string LocationKey = "location";
        /// <summary>
        /// User's geographical location
        /// </summary>
        public string Location => GetValue(LocationKey);

        private const string PictureKey = "normal_picture";
        /// <summary>
        /// The URL of the small (100×100 pixels) version of the user's profile picture on the 
        /// Runkeeper Web site
        /// </summary>
        public string Picture => GetValue(PictureKey);
    }
}
