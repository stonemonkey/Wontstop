// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using HttpApiClient.Configurations;
using HttpApiClient.Requests;
using Mvvm.WinRT;
using Mvvm.WinRT.Utils;
using Wontstop.Climb.Ui.Uwp.Dtos;

namespace Wontstop.Climb.Ui.Uwp
{
    /// <summary>
    /// Exposes factory methods for Problemator API requests.
    /// </summary>
    public class ProblematorRequestsFactory
    {
        public bool IsSecure { get; set; } = true;
        public string Server { get; set; } = "www.problemator.fi/t/problematorapi/v02";
        public string ClientId { get; set; } = "fi.problemator.mobileapp";
        public string UserAgent { get; set; } = "Mozilla/5.0 (Windows NT 10.0; WOW64) " +
                                                "AppleWebKit/537.36 (KHTML, like Gecko) " +
                                                "Chrome/55.0.2883.87 Safari/537.36";

        private readonly ITimeService _timeService;
        private readonly IResponseLogger _responseLogger;

        public ProblematorRequestsFactory(
            ITimeService timeService,
            IResponseLogger responseLogger)
        {
            _timeService = timeService;
            _responseLogger = responseLogger;
        }

        private static string _invalidArgumentMessage = "Invalid argument!";
        private static string _missingUserContextMessage = "Missing user context!";

        private UserContext _context;

        /// <summary>
        /// Sets the context of the user to be used for access while accessing Problemator API.
        /// </summary>
        /// <param name="context">User context instance.</param>
        public void SetUserContext(UserContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _context = context;
        }
        
        /// <summary>
        /// Removes the context of the user.
        /// </summary>
        public void ClearUserContext()
        {
            _context = null;
        }

        #region Parameter helpers

        protected void AddLocationParam(ConfigBase config, string gymId = null)
        {
            config.AddParam("problematorlocation", gymId ?? _context.GymId);
        }

        protected void AddApiAuthTokenParam(ConfigBase config)
        {
            if (_context == null)
            {
                throw new InvalidOperationException(_missingUserContextMessage);
            }

            config.AddParam("api-auth-token", _context.Jwt);
        }

        protected void AddClientTimestampParam(ConfigBase config)
        {
            config.AddParam("_", _timeService.Now.ToUnixTimestamp().ToString("F0"));
        }

        #endregion

        #region Header helpers

        protected void AddUserAgentHeader(ConfigBase config)
        {
            config.AddHeader("User-Agent", UserAgent);
        }

        protected void AddApplicationKindHeader(ConfigBase config)
        {
            config.AddHeader("X-Requested-With", ClientId);
        }

        #endregion

        /// <summary>
        /// Creates request for authenticating an existent user.
        /// </summary>
        /// <param name="gymId">Gym id.</param>
        /// <param name="email">Account name.</param>
        /// <param name="password">Account password.</param>
        /// <returns>GET request.</returns>
        public GetRequest CreateLoginRequest(string gymId, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(gymId))
            {
                throw new ArgumentException(_invalidArgumentMessage, nameof(gymId));
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException(_invalidArgumentMessage, nameof(email));
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException(_invalidArgumentMessage, nameof(password));
            }

            var urn = $"{Server}/dologin";
            var config = new Config(urn, IsSecure);
            config.AddParam("username", email);
            config.AddParam("password", password);
            config.AddParam("authenticate", "true");
            AddLocationParam(config, gymId);
            AddClientTimestampParam(config);
            AddUserAgentHeader(config);
            AddApplicationKindHeader(config);

            return new GetRequest(config, _responseLogger);
        }

        /// <summary>
        /// Creates request for revoke an existent authentication session.
        /// </summary>
        /// <returns>GET request.</returns>
        public GetRequest CreateLogoutRequest()
        {
            var urn = $"{Server}/logout";
            var config = new Config(urn, IsSecure);
            AddApiAuthTokenParam(config);
            AddClientTimestampParam(config);
            AddUserAgentHeader(config);
            AddApplicationKindHeader(config);

            return new GetRequest(config, _responseLogger);
        }

        /// <summary>
        /// Creates request for retriving problems on the gym wall sections.
        /// </summary>
        /// <returns>GET request.</returns>
        public GetRequest CreateProblemsRequest()
        {
            var urn = $"{Server}/problems";
            var config = new Config(urn, IsSecure);
            AddApiAuthTokenParam(config);
            AddClientTimestampParam(config);
            AddUserAgentHeader(config);
            AddApplicationKindHeader(config);

            return new GetRequest(config, _responseLogger);
        }
    }
}