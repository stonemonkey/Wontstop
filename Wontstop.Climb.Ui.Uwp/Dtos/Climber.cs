// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Newtonsoft.Json;

namespace Wontstop.Climb.Ui.Uwp.Dtos
{
    public class Climber
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "etunimi")]
        public string Firstname { get; set; }

        [JsonProperty(PropertyName = "sukunimi")]
        public string Surname { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
    }
}