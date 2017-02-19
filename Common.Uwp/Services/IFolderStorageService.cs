// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Windows.Storage;

namespace Common.Uwp.Services
{
    /// <summary>
    /// Service handling folder storage operations.
    /// </summary>
    public interface IFolderStorageService
    {
        Task SaveAsync<T>(T obj, string fileName);

        Task<T> ReadAsync<T>(string fileName);

        Task DeleteAsync(string fileName);

        Task DeleteFolderAsync();

        Task<StorageFolder> GetFolderAsync();
    }
}