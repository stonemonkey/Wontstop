// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Newtonsoft.Json;

namespace Problemator.Core.Dtos
{
    public class GradeDist
    {
        [JsonProperty(PropertyName = "gradename")]
        public string Grade { get; set; }
    }
}