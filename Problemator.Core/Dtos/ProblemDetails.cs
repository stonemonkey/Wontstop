// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Problemator.Core.Dtos
{
    /// <summary>
    /// Represents an indoor climbing route instance holding details.
    /// </summary>
    public class ProblemDetails
    {
        [JsonProperty(PropertyName = "gradeid")]
        public string GradeId { get; set; }

        [JsonProperty(PropertyName = "problemId")]
        public string ProblemId { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "gradename")]
        public string GradeName { get; set; }

        [JsonProperty(PropertyName = "wallchar")]
        public string WallName { get; set; }

        [JsonProperty(PropertyName = "walldesc")]
        public string WallDescription { get; set; }

        [JsonProperty(PropertyName = "colour")]
        public string ColorName { get; set; }

        [JsonProperty(PropertyName = "added")]
        public DateTime AddedDate { get; set; }

        [JsonProperty(PropertyName = "tag")]
        public string Tag { get; set; }

        [JsonProperty(PropertyName = "author")]
        public string Author { get; set; }

        [JsonProperty(PropertyName = "routetype")]
        public string RouteType { get; set; }

        [JsonProperty(PropertyName = "startdefinition")]
        public string StartDefinition { get; set; }

        [JsonProperty(PropertyName = "enddefinition")]
        public string EndDefinition { get; set; }

        [JsonProperty(PropertyName = "c_like")]
        public string Like { get; set; }

        [JsonProperty(PropertyName = "c_love")]
        public string Love { get; set; }

        [JsonProperty(PropertyName = "c_dislike")]
        public string Dislike { get; set; }

        [JsonProperty(PropertyName = "ascentcount")]
        public int AscentCount { get; set; }

        [JsonProperty(PropertyName = "gradedist")]
        public IList<GradeDist> GradeDist { get; set; }

        [JsonProperty(PropertyName = "tagshort")]
        public string TagShort { get; set; }

        [JsonProperty(PropertyName = "addedformatted")]
        public string AddedFormatted { get; set; }

        [JsonProperty(PropertyName = "addedrelative")]
        public string AddedRelative { get; set; }

        [JsonProperty(PropertyName = "mytickcount")]
        public int TickCount { get; set; }

        [JsonIgnore]
        //[JsonProperty(PropertyName = "opinions")]
        public IList<GradeOpinion> Opinions { get; set; }

        [JsonExtensionData]
        private Dictionary<string, JToken> _additionalData = 
            new Dictionary<string, JToken>();

        [OnDeserialized]
        private void DeserializeOpinions(StreamingContext context)
        {
            var opinions = _additionalData["opinions"];
            if (opinions is JObject)
            {
                var properties = ToProperties(opinions);
                Opinions = new List<GradeOpinion>()
                {
                    ToOpinion(properties.Single())
                };
            }
            else if (opinions is JArray jArray)
            {
                Opinions = jArray.Select(o =>
                        ToOpinion(ToProperties(o).Single()))
                    .ToList();
            }
        }

        private IEnumerable<JProperty> ToProperties(JToken jToken)
        {
            return ((JObject) jToken).Properties();
        }

        private GradeOpinion ToOpinion(JProperty property)
        {
            return new GradeOpinion
            {
                GradeName = property.Name,
                GradeId = property.ToObject<int>()
            };
        }
    }
}