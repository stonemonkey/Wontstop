// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net;
using Newtonsoft.Json;

namespace Problemator.Core.Dtos
{
    public class Location
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string NameUrlEncoded { get; set; }
        
        [JsonProperty(PropertyName = "latitude")]
        public string Latitude { get; set; }

        [JsonProperty(PropertyName = "longitude")]
        public string Longitude { get; set; }

        [JsonProperty(PropertyName = "country")]
        public string Country { get; set; }

        [JsonProperty(PropertyName = "continent")]
        public string Continent { get; set; }

        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        [JsonIgnore]
        public string Name => WebUtility.HtmlDecode(NameUrlEncoded);
    }
}