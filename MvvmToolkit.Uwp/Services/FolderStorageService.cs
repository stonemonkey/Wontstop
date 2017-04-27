// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;

namespace MvvmToolkit.Uwp.Services
{
    /// <summary>
    /// Provides serialization/deserialization over WinRT folders storage.
    /// </summary>
    public class FolderStorageService : IFolderStorageService
    {
        private ApplicationData _applicationData = ApplicationData.Current;

        public FolderStorageService() 
            : this (null, null)
        {
        }

        public FolderStorageService(StorageFolder storageFolder) 
            : this (storageFolder, null)
        {
        }

        private string _subFolder;
        private StorageFolder _storageFolder;

        public FolderStorageService(StorageFolder storageFolder, string subFolder)
        {
            _subFolder = subFolder;
            _storageFolder = storageFolder;
        }

        /// <summary>
        /// Serializes object into a text file as JSON. Overrides existent. 
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="fileName">The name of the file.</param>
        public async Task SaveAsync<T>(T obj, string fileName)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("Invalid argument!", nameof(fileName));
            }

            var folder = await GetFolderAsync().ConfigureAwait(false);
            var file = await folder.CreateFileAsync(
                TryAddExtension(fileName), CreationCollisionOption.ReplaceExisting);

            var data = JsonConvert.SerializeObject(obj);
            await FileIO.WriteTextAsync(file, data);
        }

        /// <summary>
        /// Deserializes object from a JSON text file.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>The instance of the object or default(T) in case of inexistent file.</returns>
        public async Task<T> ReadAsync<T>(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("Invalid argument!", nameof(fileName));
            }

            var fileNameWithExtension = TryAddExtension(fileName);
            var folder = await GetFolderAsync().ConfigureAwait(false);
            if (await folder.TryGetItemAsync(fileNameWithExtension) == null)
            {
                return default(T);
            }

            var file = await folder.GetFileAsync(fileNameWithExtension);
            var data = await FileIO.ReadTextAsync(file);

            return JsonConvert.DeserializeObject<T>(data);
        }

        /// <summary>
        /// Deletes file if exists.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        public async Task DeleteAsync(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("Invalid argument!", nameof(fileName));
            }

            var folder = await GetFolderAsync().ConfigureAwait(false);
            var file = await folder.TryGetItemAsync(TryAddExtension(fileName));
            if (file != null)
            {
                await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
        }

        /// <summary>
        /// Deletes current folder if authorized.
        /// </summary>
        public async Task DeleteFolderAsync()
        {
            StorageFolder folder = await GetFolderAsync().ConfigureAwait(false);

            try
            {
                await folder.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        /// <summary>
        /// Retrieves current folder.
        /// </summary>
        /// <returns>The folder.</returns>
        public async Task<StorageFolder> GetFolderAsync()
        {
            var folder = _storageFolder ?? _applicationData.LocalFolder;
            if (string.IsNullOrEmpty(_subFolder))
            {
                return folder;
            }

            return await folder.CreateFolderAsync(_subFolder, CreationCollisionOption.OpenIfExists);
        }

        private string TryAddExtension(string fileName)
        {
            const string extension = ".json";
            if (fileName.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
            {
                return fileName;
            }

            return fileName + extension;
        }
    }
}