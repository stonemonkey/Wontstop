// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace RunKeeper.WinRT.HealthGraph.Infrastructure
{
    public interface IReadOnlyBusinessModel
    {
        Task LoadAsync(IModelRepository repository);
    }
}