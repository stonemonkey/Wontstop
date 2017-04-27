// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace MvvmToolkit.Uwp.Services
{
    /// <summary>
    /// Service handling object caching.
    /// </summary>
    public interface ICacheService
    {
        Task<T> GetAsync<T>(string key, Func<Task<T>> generate, DateTime? expireDate = null, bool forceRefresh = false);

        Task<T> GetFromCacheAsync<T>(string key);

        Task AddAsync<T>(string key, T obj, DateTime? expireDate = null);

        Task DeleteAsync(string key);

        Task DeleteExpiredAsync();

        Task ClearAsync();

        Task ClearAsync(TimeSpan maxAge);

        Task ClearAsync(ulong maxSize);
    }
}