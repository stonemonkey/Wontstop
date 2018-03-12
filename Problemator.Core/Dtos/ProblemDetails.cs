// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Problemator.Core.Dtos
{
    /// <summary>
    /// Represents an indoor climbing route instance holding details.
    /// </summary>
    public class ProblemDetails
    {
        [JsonProperty(PropertyName = "ascentcount")]
        public int AscentCount { get; set; }

        [JsonProperty(PropertyName = "gradedist")]
        public IList<GradeDist> GradeDist { get; set; }

        [JsonProperty(PropertyName = "mytickcount")]
        public int TickCount { get; set; }

        [JsonProperty(PropertyName = "opinions")]
        public IDictionary<string, int> Opinions { get; set; }
    }
}