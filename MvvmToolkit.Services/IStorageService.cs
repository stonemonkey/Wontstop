// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MvvmToolkit
{
    /// <summary>
    /// Service handling settings storage CRUD.
    /// </summary>
    public interface IStorageService
    {
        T ReadLocal<T>(string key);
        void SaveLocal<T>(string key, T value);
        void DeleteLocal(string key);
        bool LocalExists(string key);

        T ReadRoaming<T>(string key);
        void SaveRoaming<T>(string key, T value);
        void DeleteRoaming(string key);
        bool RoamingExists(string key);
    }
}