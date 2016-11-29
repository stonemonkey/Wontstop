// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;

namespace RunKeeper.WinRT.HealthGraph.Infrastructure
{
    public class LocalStorageRepository : IModelRepository
    {
        public Task CreateAsyc<T>(T obj, string resource)
        {
            return SaveAsyc(obj, resource);
        }

        public async Task<T> ReadAsyc<T>(string resource)
        {
            try
            {
                var folder = ApplicationData.Current.LocalFolder;
                var file = await folder.GetFileAsync(GetFileName(resource));
                var json = await FileIO.ReadTextAsync(file);

                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (FileNotFoundException)
            {
                return default(T);
            }
        }

        public Task UpdateAsyc<T>(T obj, string resource)
        {
            return SaveAsyc(obj, resource);
        }

        public async Task SaveAsyc<T>(T obj, string resource)
        {
            var folder = ApplicationData.Current.LocalFolder;
            var file = await folder.CreateFileAsync(
                GetFileName(resource), CreationCollisionOption.ReplaceExisting);

            var json = JsonConvert.SerializeObject(obj);

            await FileIO.WriteTextAsync(file, json);
        }

        public async Task DeleteAsync(string resource)
        {
            var folder = ApplicationData.Current.LocalFolder;
            var file = await folder.CreateFileAsync(
                GetFileName(resource), CreationCollisionOption.OpenIfExists);
            await file.DeleteAsync();
        }

        protected string GetFileName(string resource)
        {
            if (string.IsNullOrWhiteSpace(resource))
            {
                throw new ArgumentOutOfRangeException(nameof(resource));
            }

            return Uri.EscapeDataString(resource);
        }
    }
}