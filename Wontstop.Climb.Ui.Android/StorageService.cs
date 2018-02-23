// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MvvmToolkit;

namespace Wontstop.Climb.Ui.Uwp
{
    public class StorageService : IStorageService
    {
        public void DeleteLocal(string key)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteRoaming(string key)
        {
            throw new System.NotImplementedException();
        }

        public bool LocalExists(string key)
        {
            throw new System.NotImplementedException();
        }

        public T ReadLocal<T>(string key)
        {
            throw new System.NotImplementedException();
        }

        public T ReadRoaming<T>(string key)
        {
            throw new System.NotImplementedException();
        }

        public bool RoamingExists(string key)
        {
            throw new System.NotImplementedException();
        }

        public void SaveLocal<T>(string key, T value)
        {
            throw new System.NotImplementedException();
        }

        public void SaveRoaming<T>(string key, T value)
        {
            throw new System.NotImplementedException();
        }
    }
}