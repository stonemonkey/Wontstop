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

        private static string _missingUserIdentityMessage = "Missing user context!";

        private UserIdentity _userIdentity;

        /// <summary>
        /// Sets the context of the user to be used for access while accessing Problemator API.
        /// </summary>
        /// <param name="context">User context instance.</param>
        public void SetUserContext(UserIdentity context)
        {
            context.ValidateNotNull(nameof(context));

            _userIdentity = context;
        }
        
        /// <summary>
        /// Removes the context of the user.
        /// </summary>
        public void ClearUserContext()
        {
            _userIdentity = null;
        }

        #region Parameter helpers

        protected void AddLocationParam(ConfigBase config, string location)
        {
            location.ValidateNotNullEmptyWhiteSpace(location);
 
            config.AddParam("problematorlocation", location);
        }

        protected void AddGymIdParam(ConfigBase config)
        {
            if (_userIdentity == null)
            {
                throw new InvalidOperationException(_missingUserIdentityMessage);
            }

            config.AddParam("gymid", _userIdentity.GymId);
        }

        protected void AddApiAuthTokenParam(ConfigBase config)
        {
            if (_userIdentity == null)
            {
                throw new InvalidOperationException(_missingUserIdentityMessage);
            }

            config.AddParam("api-auth-token", _userIdentity.Jwt);
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
            email.ValidateNotNullEmptyWhiteSpace(nameof(email));
            password.ValidateNotNullEmptyWhiteSpace(nameof(password));

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
        /// Creates request for retriving problem details.
        /// </summary>
        /// <returns>GET request.</returns>
        public GetRequest CreateProblemRequest(string id)
        {
            id.ValidateNotNullEmptyWhiteSpace(nameof(id));

            var urn = $"{Server}/problem";
            var config = new Config(urn, IsSecure);
            config.AddParam("id", id);
            AddApiAuthTokenParam(config);
            AddClientTimestampParam(config);
            AddUserAgentHeader(config);
            AddApplicationKindHeader(config);

            return new GetRequest(config, _responseLogger);
        }

        /// <summary>
        /// Creates request for adding problem ticks.
        /// </summary>
        /// <param name="ticks">Comma separated string containing problem ids.</param>
        /// <returns>GET request.</returns>
        public GetRequest CreateSaveTicksRequest(string ticks)
        {
            ticks.ValidateNotNullEmptyWhiteSpace(nameof(ticks));

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
        /// Creates request for fetching problem ticks for a day.
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
        
        /// <summary>
        /// Creates request for fetching problem ticks for a problem.
        /// </summary>
        /// <returns>GET request.</returns>
        public GetRequest CreateProblemTicksRequest(string problemId)
        {
            problemId.ValidateNotNullEmptyWhiteSpace(nameof(problemId));

            var urn = $"{Server}/userticks";
            var config = new Config(urn, IsSecure);
            config.AddParam("pid", problemId);
            AddApiAuthTokenParam(config);
            AddClientTimestampParam(config);
            AddUserAgentHeader(config);
            AddApplicationKindHeader(config);

            return new GetRequest(config, _responseLogger);
        }

        /// <summary>
        /// Creates request for updating a problem tick.
        /// </summary>
        /// <returns>GET request.</returns>
        public GetRequest CreateUpdateTickRequest(Tick tick)
        {
            tick.ValidateNotNull(nameof(tick));

            var urn = $"{Server}/savetick";
            var config = new Config(urn, IsSecure);
            config.AddParam("problemid", tick.ProblemId);
            config.AddParam("grade_opinion", tick.GradeOpinionId);
            config.AddParam("tries", tick.Tries.ToString());
            config.AddParam("ascent_type", tick.AscentTypeId.ToString());
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
            tickId.ValidateNotNullEmptyWhiteSpace(nameof(tickId));

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
            var location = _userIdentity?.GymId;
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
            gymId.ValidateNotNullEmptyWhiteSpace(nameof(gymId));

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