// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Newtonsoft.Json;

namespace Problemator.Core.Dtos
{
    /// <summary>
    /// Flags a problem as done with associated information about the attempt.
    /// </summary>
    public class Tick
    {
        [JsonProperty(PropertyName = "routetype")]
        public string RouteType { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "userid")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "problemid")]
        public string ProblemId { get; set; }

        [JsonProperty(PropertyName = "tstamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty(PropertyName = "grade_opinion")]
        public string GradeOpinionId { get; set; }

        [JsonProperty(PropertyName = "tries")]
        public int Tries { get; set; }
        
        [JsonProperty(PropertyName = "ascent_type")]
        public int AscentTypeId { get; set; }

        [JsonProperty(PropertyName = "a_like")]
        public int Like { get; set; }

        [JsonProperty(PropertyName = "a_love")]
        public int Love { get; set; }

        [JsonProperty(PropertyName = "a_dislike")]
        public int Dislike { get; set; }

        [JsonProperty(PropertyName = "gradeid")]
        public string GradeId { get; set; }

        [JsonProperty(PropertyName = "gradename")]
        public string GradeName { get; set; }

        [JsonProperty(PropertyName = "tag")]
        public string Tag { get; set; }

        [JsonIgnore]
        public string TagShort => Tag?.Substring(7);

        [JsonProperty(PropertyName = "colour")]
        public string ColorName { get; set; }
    }
}