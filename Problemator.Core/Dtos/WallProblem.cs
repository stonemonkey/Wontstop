// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Newtonsoft.Json;

namespace Problemator.Core.Dtos
{
    /// <summary>
    /// Represents an indoor climbing route instance attached to a wall section.
    /// </summary>
    public class WallProblem
    {
        [JsonProperty(PropertyName = "gradeid")]
        public string GradeId { get; set; }

        [JsonProperty(PropertyName = "author")]
        public string Author { get; set; }
        
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

        [JsonProperty(PropertyName = "gymid")]
        public string GymId { get; set; }

        [JsonProperty(PropertyName = "wallid")]
        public string WallId { get; set; }

        [JsonProperty(PropertyName = "colour")]
        public string ColorName { get; set; }

        [JsonProperty(PropertyName = "colourid")]
        public string ColorId { get; set; }

        [JsonProperty(PropertyName = "added")]
        public DateTime AddedDate { get; set; }

        [JsonProperty(PropertyName = "visible")]
        public string Visible { get; set; }

        [JsonProperty(PropertyName = "tag")]
        public string Tag { get; set; }

        [JsonProperty(PropertyName = "routetype")]
        public string RouteType { get; set; }

        [JsonProperty(PropertyName = "startdefinition")]
        public string StartDefinition { get; set; }

        [JsonProperty(PropertyName = "enddefinition")]
        public string EndDefinition { get; set; }

        [JsonProperty(PropertyName = "c_like")]
        public int Like { get; set; }

        [JsonProperty(PropertyName = "c_love")]
        public int Love { get; set; }

        [JsonProperty(PropertyName = "c_dislike")]
        public int Dislike { get; set; }

        [JsonProperty(PropertyName = "c_dirty")]
        public int Dirty { get; set; }

        [JsonProperty(PropertyName = "c_dangerous")]
        public int Dangerous { get; set; }

        [JsonProperty(PropertyName = "soontoberemoved")]
        public string SoonToBeRemoved { get; set; }

        [JsonProperty(PropertyName = "hidedate")]
        public string Removed { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "tapecolour")]
        public string TapeColorCode { get; set; }

        [JsonProperty(PropertyName = "score")]
        public int Score { get; set; }

        [JsonProperty(PropertyName = "addedformatted")]
        public string AddedFormatted { get; set; }

        [JsonProperty(PropertyName = "addedrelative")]
        public string AddedRelative { get; set; }

        [JsonProperty(PropertyName = "tagshort")]
        public string TagShort { get; set; }

        [JsonProperty(PropertyName = "colourcode")]
        public string ColourCode { get; set; }

        [JsonProperty(PropertyName = "tick")]
        public Tick Tick { get; set; }
    }
}