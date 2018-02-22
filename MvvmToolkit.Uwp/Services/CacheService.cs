// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace MvvmToolkit.Uwp.Services
{
    /// <summary>
    /// Provides object caching on local folder storage.
    /// </summary>
    public class CacheService : ICacheService
    {
        private ITimeService _timeService;
        private IFolderStorageService _storage;

        public CacheService(
            ITimeService timeService, 
            IFolderStorageService folderStorageService)
        {
            _timeService = timeService;
            _storage = folderStorageService;
        }

        /// <summary>
        /// Retrieves an object from cache or generates it.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="key">The key uniquely indentifying the object.</param>
        /// <param name="generate">The function to generate the object in case is expired or is 
        /// missing from cache.</param>
        /// <param name="expireDate">The time after which the object will be regenerated.</param>
        /// <param name="forceRefresh">Forces regeneration even the expireDate is valid.</param>
        /// <returns>The instance of the object.</returns>
        public async Task<T> GetAsync<T>(
            string key, Func<Task<T>> generate, DateTime? expireDate = null, bool forceRefresh = false)
        {
            T value;

            var cacheKey = AddCacheFolder(key);

            if (!forceRefresh)
            {
                value = await GetFromCacheAsync<T>(cacheKey).ConfigureAwait(false);
                if (!EqualityComparer<T>.Default.Equals(value, default(T)))
                {
                    return value;
                }
            }

            value = await generate().ConfigureAwait(false);
            await AddAsync(cacheKey, value, expireDate).ConfigureAwait(false);

            return value;
        }

        /// <summary>
        /// Retrieves an object from cache.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="key">The key uniquely indentifying the object.</param>
        /// <returns>The object or defult(T).</returns>
        public async Task<T> GetFromCacheAsync<T>(string key)
        {
            var value = await _storage.ReadAsync<CacheObject<T>>(key).ConfigureAwait(false);
            if (value == null)
            {
                return default(T);
            }
            else if (value.IsValidAt(_timeService.Now))
            {
                return value.Obj;
            }
            else
            {
                // do not await
                #pragma warning disable CS4014
                DeleteAsync(AddCacheFolder(key));
            }

            return default(T);
        }

        /// <summary>
        /// Adds an object into cache.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="key">The key uniquely indentifying the object.</param>
        /// <param name="obj">The instance of the object.</param>
        /// <param name="expireDate"></param>
        public Task AddAsync<T>(string key, T obj, DateTime? expireDate = null)
        {
            CacheObject<T> cacheFile = new CacheObject<T>() { Obj = obj, ExpireDateTime = expireDate };

            return _storage.SaveAsync(cacheFile, AddCacheFolder(key));
        }

        /// <summary>
        /// Delete cache object if existent.
        /// </summary>
        /// <param name="key">The key uniquely indentifying the object.</param>
        public Task DeleteAsync(string key)
        {
            return _storage.DeleteAsync(AddCacheFolder(key));
        }

        //private readonly string CacheFolder = "_cache";

        private string AddCacheFolder(string key)
        {
            return key; // $"{CacheFolder}/{key}";
        }

        /// <summary>
        /// Delete expired cache objects.
        /// </summary>
        public async Task DeleteExpiredAsync()
        {
            var folder = await _storage.GetFolderAsync().ConfigureAwait(false);
            var files = await folder.GetFilesAsync();
            foreach (var file in files)
            {
                var loadedFile = await _storage.ReadAsync<CacheObject<object>>(file.DisplayName).ConfigureAwait(false);
                if (loadedFile != null && !loadedFile.IsValidAt(_timeService.Now))
                {
                    await file.DeleteAsync();
                }
            }
        }

        /// <summary>
        /// Clears completely the cache.
        /// </summary>
        public Task ClearAsync()
        {
            return _storage.DeleteFolderAsync();
        }

        /// <summary>
        /// Clears the cache by removing all the objects which haven't been modified for a given 
        /// time.
        /// </summary>
        /// <param name="maxAge">The time span over which the objects that haven't been modified 
        /// will be deleted.</param>
        public async Task ClearAsync(TimeSpan maxAge)
        {
            var folder = await _storage.GetFolderAsync();

            await ClearAsync(folder, maxAge);
        }

        private async Task ClearAsync(StorageFolder folder, TimeSpan maxAge)
        {
            var files = await folder.GetFilesAsync();
            foreach (var file in files)
            {
                var props = await file.GetBasicPropertiesAsync();
                var age = DateTimeOffset.UtcNow - props.DateModified;
                if (age >= maxAge)
                {
                    try
                    {
                        await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
                    }
                    catch (FileNotFoundException)
                    {
                        // already deleted
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // file might be in use. ignore it and continue with the other files.
                    }
                }
            }
        }

        /// <summary>
        /// Clears the cache until the total size is below the maxSize. Older objects are cleared 
        /// first.
        /// </summary>
        /// <param name="maxSize">MaxSize in bytes.</param>
        public async Task ClearAsync(ulong maxSize)
        {
            var folder = await _storage.GetFolderAsync();

            await ClearAsync(folder, maxSize);
        }

        private async Task ClearAsync(StorageFolder folder, ulong maxSize)
        {
            var list = new List<CacheFile>();

            var files = await folder.GetFilesAsync();
            foreach (var file in files)
            {
                var properties = await file.GetBasicPropertiesAsync();
                list.Add(new CacheFile
                {
                    Name = file.Name,
                    Size = properties.Size,
                    Modified = properties.DateModified
                });
            }

            // order list by Modified date so it deletes old files first
            list = list.OrderBy(x => x.Modified).ToList();

            await ClearAsync(folder, list, maxSize).ConfigureAwait(false);
        }

        private static async Task ClearAsync(
            StorageFolder folder, List<CacheFile> list, ulong maxSize)
        {
            if (list == null || !list.Any())
            {
                return;
            }
            var total = (ulong) list.Sum(x => (long) x.Size);
            if (total > maxSize)
            {
                try
                {
                    var file = await folder.GetFileAsync(list.First().Name);
                    await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
                }
                catch (FileNotFoundException)
                {
                    // already deleted
                }
                catch (UnauthorizedAccessException)
                {
                    // file might be in use. ignore it and continue with the other files.
                }
                finally
                {
                    list.RemoveAt(0);
                }

                // recursive
                await ClearAsync(folder, list, maxSize).ConfigureAwait(false);
            }
        }
    }
}