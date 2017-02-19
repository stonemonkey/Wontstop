// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Windows.Storage;
using Newtonsoft.Json;

namespace Mvvm.WinRT
{
    /// <summary>
    /// Provides wrappers over WinRT settings storage removing boiler plate code otherwise needed for 
    /// CRUD operations.
    /// </summary>
    public class StorageService : IStorageService
    {
        private readonly ApplicationData _applicationData = ApplicationData.Current;

        #region Local settings

        public T ReadLocal<T>(string key)
        {
            return LocalExists(key) ?
                JsonConvert.DeserializeObject<T>((string)_applicationData.LocalSettings.Values[key]) :
                default(T);
        }

        public void SaveLocal<T>(string key, T value)
        {
            if (LocalExists(key))
            {
                _applicationData.LocalSettings.Values[key] = JsonConvert.SerializeObject(value);
            }
            else
            {
                _applicationData.LocalSettings.Values.Add(key, JsonConvert.SerializeObject(value));
            }
        }

        public void DeleteLocal(string key)
        {
            if (LocalExists(key))
            {
                _applicationData.LocalSettings.Values.Remove(key);
            }
        }

        public bool LocalExists(string key)
        {
            return _applicationData.LocalSettings.Values.ContainsKey(key);
        }

        #endregion

        #region Roaming settings

        public T ReadRoaming<T>(string key)
        {
            return _applicationData.RoamingSettings.Values.ContainsKey(key) ?
                JsonConvert.DeserializeObject<T>((string)_applicationData.RoamingSettings.Values[key]) :
                default(T);
        }

        public void SaveRoaming<T>(string key, T value)
        {
            if (RoamingExists(key))
            {
                _applicationData.RoamingSettings.Values[key] = JsonConvert.SerializeObject(value);
            }
            else
            {
                _applicationData.RoamingSettings.Values.Add(key, JsonConvert.SerializeObject(value));
            }
        }

        public void DeleteRoaming(string key)
        {
            if (RoamingExists(key))
            {
                _applicationData.RoamingSettings.Values.Remove(key);
            }
        }

        public bool RoamingExists(string key)
        {
            return _applicationData.RoamingSettings.Values.ContainsKey(key);
        }

        #endregion
    }
}