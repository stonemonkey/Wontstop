// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace MvvmToolkit
{
    /// <summary>
    /// Encapsulates the process involved in obtaining application dependencies.
    /// </summary>
    public class ServiceLocator
    {
        /// <summary>
        /// Function responsible for instance location. Encapsulates a get instance operation on a 
        /// concrete IoC container.
        /// </summary>
        public static Func<Type, object> GetInstance { private get; set; }

        /// <summary>
        /// Retrives registred or creates new instance of requested type.
        /// </summary>
        /// <typeparam name="T">The type of the requested instance.</typeparam>
        /// <returns>An instance of requested type.</returns>
        /// <exception cref="InvalidOperationException">ServiceLocator is not configured.</exception>
        public static T Get<T>() where T : class
        {
            if (GetInstance == null)
            {
                throw new InvalidOperationException("ServiceLocator is not configured!");
            }

            return (T) GetInstance(typeof(T));
        }
    }
}
