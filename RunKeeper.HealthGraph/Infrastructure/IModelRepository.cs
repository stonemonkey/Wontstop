// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace RunKeeper.WinRT.HealthGraph.Infrastructure
{
    public interface IModelRepository
    {
        Task CreateAsyc<T>(T obj, string resource);

        Task<T> ReadAsyc<T>(string resource);

        Task UpdateAsyc<T>(T obj, string resource);

        Task DeleteAsync(string resource);
    }
}