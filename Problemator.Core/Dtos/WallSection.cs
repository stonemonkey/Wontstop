// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Problemator.Core.Dtos
{
    /// <summary>
    /// Represents a section of a climbing wall. It is returned by Problemator API as a result of  
    /// problems request.
    /// </summary>
    public class WallSection
    {
        [JsonProperty(PropertyName = "wallchar")]
        public string WallChar { get; set; }

        [JsonProperty(PropertyName = "wallname")]
        public string WallName { get; set; }

        [JsonProperty(PropertyName = "problems")]
        public IList<Problem> Problems { get; set; }

        [JsonIgnore]
        public string FullName => $"{WallChar} {WallName}";
    }
}