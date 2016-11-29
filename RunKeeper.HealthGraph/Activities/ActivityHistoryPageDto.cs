// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Newtonsoft.Json;

namespace RunKeeper.WinRT.HealthGraph.Activities
{
    public class ActivityHistoryPageDto
    {
        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("next")]
        public string NextPageUrl { get; set; }

        [JsonProperty("previous")]
        public string PreviousPageUrl { get; set; }

        [JsonProperty("items")]
        public IList<ActivityHistoryItemDto> Items { get; set; }
    }
}