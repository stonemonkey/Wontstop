// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Problemator.Core.Dtos
{
    /// <summary>
    /// Represents an indoor climbing route instance.
    /// </summary>
    public class Problem
    {
        [JsonProperty(PropertyName = "problemId")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "colour")]
        public string ColorName { get; set; }

        [JsonProperty(PropertyName = "visible")]
        public string Visible { get; set; }

        [JsonProperty(PropertyName = "tag")]
        public string Tag { get; set; }

        [JsonProperty(PropertyName = "tagshort")]
        public string TagShort { get; set; }

        [JsonProperty(PropertyName = "gradeid")]
        public string GradeId { get; set; }

        [JsonProperty(PropertyName = "gradename")]
        public string Grade { get; set; }

        [JsonProperty(PropertyName = "author")]
        public string Author { get; set; }

        [JsonProperty(PropertyName = "hidedate")]
        public string Removed { get; set; }

        [JsonProperty(PropertyName = "added")]
        public string Added { get; set; }

        [JsonProperty(PropertyName = "addedrelative")]
        public string AddedRelative { get; set; }

        [JsonProperty(PropertyName = "addedformatted")]
        public string AddedFormatted { get; set; }

        [JsonProperty(PropertyName = "routetype")]
        public string RouteType { get; set; }

        [JsonProperty(PropertyName = "c_like")]
        public int Like { get; set; }

        [JsonProperty(PropertyName = "c_love")]
        public int Love { get; set; }

        [JsonProperty(PropertyName = "c_dislike")]
        public int Dislike { get; set; }

        [JsonProperty(PropertyName = "score")]
        public int Score { get; set; }

        [JsonProperty(PropertyName = "ascentcount")]
        public int AscentCount { get; set; }

        [JsonProperty(PropertyName = "mytickcount")]
        public int TickCount { get; set; }

        [JsonProperty(PropertyName = "gradedist")]
        public IList<GradeDist> GradeDist { get; set; }

        [JsonProperty(PropertyName = "opinions")]
        public IDictionary<string, int> Opinions { get; set; }

        [JsonProperty(PropertyName = "tick")]
        public Tick Tick { get; set; }
    }
}