// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Newtonsoft.Json;

namespace Wontstop.Climb.Ui.Uwp.Dtos
{
    public class Grade
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "score")]
        public string Score { get; set; }

        [JsonProperty(PropertyName = "tapecolour")]
        public string TapeColour { get; set; }

        [JsonProperty(PropertyName = "chartcolor")]
        public string ChartColor { get; set; }
        
        [JsonProperty(PropertyName = "uiaa")]
        public string Uiaa { get; set; }

        [JsonProperty(PropertyName = "vscale")]
        public string VScale { get; set; }

        [JsonProperty(PropertyName = "australian")]
        public string Australian { get; set; }

        [JsonProperty(PropertyName = "font")]
        public string Fontainebleau { get; set; }

        [JsonProperty(PropertyName = "south_africa")]
        public string SouthAfrica { get; set; }

        [JsonProperty(PropertyName = "yds")]
        public string YosemiteDecimalSystem { get; set; }
    }
}