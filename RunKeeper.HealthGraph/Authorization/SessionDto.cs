// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Newtonsoft.Json;

namespace RunKeeper.WinRT.HealthGraph.Authorization
{
    /// <summary>
    /// Session data received from RunKeeper in the authorization process.
    /// </summary>
    public class SessionDto
    {
        /// <summary>
        /// Unique identifier for the association of your application to the user's Health 
        /// Graph/Runkeeper account.
        /// </summary>
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
}