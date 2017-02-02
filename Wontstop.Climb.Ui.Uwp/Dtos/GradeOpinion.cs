// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Newtonsoft.Json;

namespace Wontstop.Climb.Ui.Uwp.Dtos
{
    /// <summary>
    /// Represents an user opinion about a problem grade.
    /// </summary>
    public class GradeOpinion
    {
        [JsonProperty(PropertyName = "gradename")]
        public string Grade { get; set; }
    }
}