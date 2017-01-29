// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Newtonsoft.Json;

namespace Wontstop.Climb.Ui.Uwp.Dtos
{
    /// <summary>
    /// Flags a problem as done with associated information about the attempt.
    /// </summary>
    public class Tick
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "userid")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "problemid")]
        public string ProblemId { get; set; }

        [JsonProperty(PropertyName = "tstamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty(PropertyName = "tries")]
        public int Tries { get; set; }

        [JsonProperty(PropertyName = "grade_opinion")]
        public string GradeOpinion { get; set; }

        [JsonProperty(PropertyName = "ascent_type")]
        public int AscentType { get; set; }
    }
}