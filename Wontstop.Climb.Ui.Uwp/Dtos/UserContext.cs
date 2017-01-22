// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Newtonsoft.Json;

namespace Wontstop.Climb.Ui.Uwp.Dtos
{
    /// <summary>
    /// Represents the context of the user within the application. It is received from problmator 
    /// API as a result of authentication process (login/signup).
    /// </summary>
    public class UserContext
    {
        [JsonProperty(PropertyName = "JWT")]
        public string Jwt { get; set; }

        [JsonProperty(PropertyName = "loc")]
        public string GymId { get; set; }

        [JsonProperty(PropertyName = "uid")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}