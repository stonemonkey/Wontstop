// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;
using PropertyChanged;
using RunKeeper.WinRT.HealthGraph.Infrastructure;

namespace RunKeeper.WinRT.HealthGraph.Authorization
{
    /// <summary>
    /// Represents the session with RunKeeper. A session is needed in order to access HealthGraph. 
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class AuthorizationSession
    {
        /// <summary>
        /// Specifies wheather session authorization was successfully performed and localy stored.
        /// </summary>
        /// <returns>True if the session was successfully authorized, otherwise false.</returns>
        public bool IsAuthorized { get; private set; }

        private readonly IAuthorizationProvider _authorizationProvider;

        /// <summary>
        /// Creates session authorization instance.
        /// </summary>
        /// <param name="authorizationProvider">The provider used to authorize the session.</param>
        /// <exception cref="ArgumentOutOfRangeException">Null or empty sessionKey.</exception>
        /// <exception cref="ArgumentNullException">Null authorizationProvider.</exception>
        public AuthorizationSession(IAuthorizationProvider authorizationProvider)
        {
            if (authorizationProvider == null)
            {
                throw new ArgumentNullException(nameof(authorizationProvider));
            }

            _authorizationProvider = authorizationProvider;
            IsAuthorized = ExistLocal();
        }

        /// <summary>
        /// Retrives session access token obtained after a successfull authorization operation. 
        /// The token unique identifies the association of your application to the user's Health 
        /// Graph/Runkeeper account.
        /// </summary>
        /// <returns>Access token</returns>
        /// <exception cref="InvalidOperationException">If session was not previously authorized.
        /// </exception>
        public string GetAccessToken()
        {
            if (!ExistLocal())
            {
                throw new InvalidOperationException("Unauthorized session!");
            }

            var localSession = ReadLocal();

            return localSession.AccessToken;
        }

        /// <summary>
        /// Performes authorization against RunKeeper Single Sign On service.
        /// </summary>
        /// <param name="forceReconnect">If true forces reauthorization on an existent authorized  
        /// session.</param>
        /// <returns>Awaitable task</returns>
        public async Task AuthorizeAsync(bool forceReconnect = false)
        {
            if (ExistLocal() && !forceReconnect)
            {
                return;
            }

            var session = await _authorizationProvider.AuthorizeAsync<SessionDto>();
            if (session != null)
            {
                SaveLocal(session);
            }

            IsAuthorized = ExistLocal();
        }

        /// <summary>
        /// Deauthorize existent RunKeeper session.
        /// </summary>
        /// <returns>Awaitable task</returns>
        public async Task UnauthorizeAsync()
        {
            if (!ExistLocal())
            {
                return;
            }

            var session = ReadLocal();
            if (session != null)
            {
                await _authorizationProvider.DeauthorizeAsync(session.AccessToken);
            }

            DeleteLocal();

            IsAuthorized = ExistLocal();
        }

        #region Local storage

        private const string SessionKey = "runKeeperSession";

        private SessionDto ReadLocal()
        {
            var json = ApplicationData.Current.LocalSettings.Values[SessionKey] as string;
            return JsonConvert.DeserializeObject<SessionDto>(json);
        }

        private void SaveLocal(SessionDto session)
        {
            var json = JsonConvert.SerializeObject(session);
            ApplicationData.Current.LocalSettings.Values[SessionKey] = json;
        }

        private void DeleteLocal()
        {
            ApplicationData.Current.LocalSettings.Values.Remove(SessionKey);
        }

        public bool ExistLocal()
        {
            return ApplicationData.Current.LocalSettings.Values.ContainsKey(SessionKey);
        }

        #endregion
    }
}
