// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MvvmToolkit.Services
{ 
    /// <summary>
    /// Provides localization operations.
    /// </summary>
    public interface ITranslationService
    {
        /// <summary>
        /// Retrives localized string resource.
        /// </summary>
        /// <param name="key">String resource key.</param>
        /// <returns>Culture specific string resource.</returns>
        string Localize(string key);
    }
}
