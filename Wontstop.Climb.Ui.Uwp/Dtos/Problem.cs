// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Newtonsoft.Json;

namespace Wontstop.Climb.Ui.Uwp.Dtos
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

        [JsonProperty(PropertyName = "tagshort")]
        public string TagShort { get; set; }

        [JsonProperty(PropertyName = "font")]
        public string GradeFont { get; set; }

        [JsonProperty(PropertyName = "routetype")]
        public string RouteType { get; set; }

        [JsonProperty(PropertyName = "tick")]
        public Tick Tick { get; set; }
    }
}