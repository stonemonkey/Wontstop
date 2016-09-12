using System;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;

namespace RunKeeper.WinRT.HealthGraph.Authorization
{
    /// <summary>
    /// Represents the session with RunKeeper. A session is needed in order to access HealthGraph. 
    /// </summary>
    public class AuthorizationSession
    {
        private readonly string _sessionKey;
        private readonly IAuthorizationProvider _authorizationProvider;

        /// <summary>
        /// Creates session authorization instance.
        /// </summary>
        /// <param name="sessionKey">Key used to save localy session data</param>
        /// <param name="authorizationProvider">The provider used to authorize the session</param>
        /// <exception cref="ArgumentOutOfRangeException">Null or empty sessionKey</exception>
        /// <exception cref="ArgumentNullException">Null authorizationProvider</exception>
        public AuthorizationSession(string sessionKey, IAuthorizationProvider authorizationProvider)
        {
            if (string.IsNullOrWhiteSpace(sessionKey))
            {
                throw new ArgumentOutOfRangeException(nameof(sessionKey));
            }
            if (authorizationProvider == null)
            {
                throw new ArgumentNullException(nameof(authorizationProvider));
            }

            _sessionKey = sessionKey;
            _authorizationProvider = authorizationProvider;
        }

        /// <summary>
        /// Retrives session access token obtained while initializing the session.
        /// </summary>
        /// <returns>Access token</returns>
        /// <exception cref="InvalidOperationException">When session was not previously initialized</exception>
        public string GetAccessToken()
        {
            if (!ExistLocal())
            {
                throw new InvalidOperationException("Uninitialized session!");
            }

            var localSession = ReadLocal();

            return localSession.AccessToken;
        }

        /// <summary>
        /// Retrives wheather session initialization was successfully performed and localy stored.
        /// </summary>
        /// <returns>True if the session was successfully authorized, otherwise false.</returns>
        public bool IsInitialized()
        {
            return ExistLocal();
        }

        /// <summary>
        /// Performes authorization against RunKeeper SSO.
        /// </summary>
        /// <param name="forceReconnect">If true forces reauthorization on an already initialized session.</param>
        /// <returns>Awaitable task</returns>
        public async Task InitializeAsync(bool forceReconnect = false)
        {
            if (ExistLocal() && !forceReconnect)
            {
                return;
            }

            var session = await _authorizationProvider.AuthorizeAsync<SessionDto>();
            SaveLocal(session);
        }

        /// <summary>
        /// Deauthorize existent RunKeeper session.
        /// </summary>
        /// <returns>Awaitable task</returns>
        public async Task UninitializeAsync()
        {
            if (!ExistLocal())
            {
                return;
            }

            var session = ReadLocal();
            await _authorizationProvider.DeauthorizeAsync(session.AccessToken);

            DeleteLocal();
        }

        #region Local storage

        private SessionDto ReadLocal()
        {
            var json = ApplicationData.Current.LocalSettings.Values[_sessionKey] as string;
            return JsonConvert.DeserializeObject<SessionDto>(json);
        }

        private void SaveLocal(SessionDto session)
        {
            var json = JsonConvert.SerializeObject(session);
            ApplicationData.Current.LocalSettings.Values[_sessionKey] = json;
        }

        private void DeleteLocal()
        {
            ApplicationData.Current.LocalSettings.Values.Remove(_sessionKey);
        }

        public bool ExistLocal()
        {
            return ApplicationData.Current.LocalSettings.Values.ContainsKey(_sessionKey);
        }

        #endregion
    }
}
