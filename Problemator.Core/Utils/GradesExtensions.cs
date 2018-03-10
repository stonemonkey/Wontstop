// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Problemator.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Problemator.Core.Utils
{
    public static class GradesExtensions
    {
        public static Grade GetById(this IEnumerable<Grade> grades, string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            return grades.Single(x => string.Equals(x.Id, id, StringComparison.Ordinal));
        }

        public static Grade GetByName(this IEnumerable<Grade> grades, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentOutOfRangeException(nameof(name));
            }

            return grades.Single(x => string.Equals(x.Name, name, StringComparison.Ordinal));
        }
    }
}