// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Newtonsoft.Json;

namespace Problemator.Core.Dtos
{
    /// <summary>
    /// Represents the confirmation from problmator API as a result of changing the gym.
    /// </summary>
    public class GymChangeConfirmation
    {
        [JsonProperty(PropertyName = "jwt")]
        public string Jwt { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}