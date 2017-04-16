// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Newtonsoft.Json;

namespace Wontstop.Climb.Ui.Uwp.Dtos
{
    public class UserSettings
    {
        [JsonProperty(PropertyName = "etunimi")]
        public string Firstname  { get; set; }

        [JsonProperty(PropertyName = "sukunimi")]
        public string Surname { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "gender")]
        public string Gender { get; set; }

        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }

        [JsonProperty(PropertyName = "publicascents")]
        public string PublicAscents { get; set; }

        [JsonProperty(PropertyName = "rankinglocation")]
        public string RankingLocation { get; set; }

        [JsonProperty(PropertyName = "showinranking")]
        public string ShowInRanking { get; set; }

        [JsonProperty(PropertyName = "sport_tick_ascent_type")]
        public string SportTickAscentType { get; set; }

        [JsonProperty(PropertyName = "_")]
        public string UnixTimestamp { get; set; }

        [JsonIgnore]
        public int AscentType => int.Parse(SportTickAscentType);
    }
}