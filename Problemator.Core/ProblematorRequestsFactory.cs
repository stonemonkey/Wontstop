// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using HttpApiClient;
using HttpApiClient.Configurations;
using HttpApiClient.Requests;
using MvvmToolkit;
using MvvmToolkit.Utils;
using Problemator.Core.Dtos;

namespace Problemator.Core
{
    /// <summary>
    /// Exposes factory methods for Problemator API requests.
    /// </summary>
    public class ProblematorRequestsFactory
    {
        public bool IsSecure { get; set; } = true;
        public string Server { get; set; } = "www.problemator.fi/t/problematorapi/v02";
        public string ClientId { get; set; } = "fi.problemator.mobileapp";
        public string UserAgent { get; set; } = 
            "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36";

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

        protected void AddLocationParam(ConfigBase config, string location)
        {
            if (string.IsNullOrEmpty(location))
            {
                throw new ArgumentException(_invalidArgumentMessage, nameof(location));
            }

            config.AddParam("problematorlocation", location);
        }

        protected void AddGymIdParam(ConfigBase config)
        {
            if (_context == null)
            {
                throw new InvalidOperationException(_missingUserContextMessage);
            }

            config.AddParam("gymid", _context.GymId);
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
        /// <param name="email">Account name.</param>
        /// <param name="password">Account password.</param>
        /// <param name="location">Gym id (optional).</param>
        /// <returns>GET request.</returns>
        public GetRequest CreateLoginRequest(string email, string password, string location = null)
        {
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
            if (location != null)
            {
                AddLocationParam(config, location);
            }
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
        public GetRequest CreateWallSectionsRequest()
        {
            var urn = $"{Server}/problems";
            var config = new Config(urn, IsSecure);
            AddApiAuthTokenParam(config);
            AddClientTimestampParam(config);
            AddUserAgentHeader(config);
            AddApplicationKindHeader(config);

            return new GetRequest(config, _responseLogger);
        }

        /// <summary>
        /// Creates request for adding problem ticks.
        /// </summary>
        /// <returns>GET request.</returns>
        public GetRequest CreateSaveTicksRequest(string ticks)
        {
            if (string.IsNullOrWhiteSpace(ticks))
            {
                throw new ArgumentException(_invalidArgumentMessage, nameof(ticks));
            }

            var urn = $"{Server}/saveticks/";
            var config = new Config(urn, IsSecure);
            config.AddParam("ticks", ticks);
            AddGymIdParam(config);
            AddApiAuthTokenParam(config);
            AddClientTimestampParam(config);
            AddUserAgentHeader(config);
            AddApplicationKindHeader(config);

            return new GetRequest(config, _responseLogger);
        }

        /// <summary>
        /// Creates request for fetching problem ticks for today.
        /// </summary>
        /// <returns>GET request.</returns>
        public GetRequest CreateDayTicksRequest(DateTime date)
        {
            var urn = $"{Server}/tickarchive";
            var config = new Config(urn, IsSecure);
            config.AddParam("date", date.ToString("yyyy-MM-dd"));
            AddApiAuthTokenParam(config);
            AddClientTimestampParam(config);
            AddUserAgentHeader(config);
            AddApplicationKindHeader(config);

            return new GetRequest(config, _responseLogger);
        }

        public string ProblemIdParamKey => "problemid";

        /// <summary>
        /// Creates request for updating a problem tick.
        /// </summary>
        /// <returns>GET request.</returns>
        public GetRequest CreateUpdateTickRequest(Tick tick)
        {
            if (tick == null)
            {
                throw new ArgumentNullException(nameof(tick));
            }

            var urn = $"{Server}/savetick";
            var config = new Config(urn, IsSecure);
            config.AddParam(ProblemIdParamKey, tick.ProblemId);
            config.AddParam("grade_opinion", tick.GradeOpinionId);
            config.AddParam("tries", tick.Tries.ToString());
            config.AddParam("ascent_type", tick.AscentType.ToString());
            config.AddParam("tickdate", tick.Timestamp.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
            AddApiAuthTokenParam(config);
            AddClientTimestampParam(config);
            AddUserAgentHeader(config);
            AddApplicationKindHeader(config);

            return new GetRequest(config, _responseLogger);
        }
        
        /// <summary>
        /// Creates request for deleting a problem tick.
        /// </summary>
        /// <returns>GET request.</returns>
        public GetRequest CreateDeleteTickRequest(string tickId)
        {
            if (string.IsNullOrWhiteSpace(tickId))
            {
                throw new ArgumentException(_invalidArgumentMessage, nameof(tickId));
            }

            var urn = $"{Server}/untick";
            var config = new Config(urn, IsSecure);
            config.AddParam("tickid", tickId);
            AddApiAuthTokenParam(config);
            AddClientTimestampParam(config);
            AddUserAgentHeader(config);
            AddApplicationKindHeader(config);

            return new GetRequest(config, _responseLogger);
        }

        /// <summary>
        /// Creates request for fetching dates for which ticks have been added.
        /// </summary>
        /// <returns>GET request.</returns>
        public GetRequest CreateTickDatesRequest()
        {
            var urn = $"{Server}/tickarchive_tickdates";
            var config = new Config(urn, IsSecure);
            AddApiAuthTokenParam(config);
            AddClientTimestampParam(config);
            AddUserAgentHeader(config);
            AddApplicationKindHeader(config);

            return new GetRequest(config, _responseLogger);
        }

        /// <summary>
        /// Creates request for fetching dashboard data. It includes statistics, user setings and 
        /// application settings (gyms, grades).
        /// </summary>
        /// <returns>GET request.</returns>
        public GetRequest CreateDashboardRequest()
        {
            var urn = $"{Server}/dashinfo";
            var config = new Config(urn, IsSecure);
            var location = _context?.GymId;
            if (location != null)
            {
                AddLocationParam(config, location);
            }
            AddApiAuthTokenParam(config);
            AddClientTimestampParam(config);
            AddUserAgentHeader(config);
            AddApplicationKindHeader(config);

            return new GetRequest(config, _responseLogger);
        }

        /// <summary>
        /// Creates request for changing default gym.
        /// </summary>
        /// <returns>GET request.</returns>
        public GetRequest CreateChangeGymRequest(string gymId)
        {
            var urn = $"{Server}/changegym";
            var config = new Config(urn, IsSecure);
            config.AddParam("id", gymId);
            AddApiAuthTokenParam(config);
            AddClientTimestampParam(config);
            AddUserAgentHeader(config);
            AddApplicationKindHeader(config);

            return new GetRequest(config, _responseLogger);
        }
    }
}