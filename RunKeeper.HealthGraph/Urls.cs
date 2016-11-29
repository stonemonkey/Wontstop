// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace RunKeeper.WinRT.HealthGraph
{
    /// <summary>
    /// RunKeeper urls used across the application
    /// </summary>
    public class Urls
    {
        // Fixed URLs
        public static string ApiUrl = "https://api.runkeeper.com";

        // Obtained RunKeeper portal when registering the application
        public static string AppAuthorizeUrl = "https://runkeeper.com/apps/authorize";
        public static string AppTokenUrl = "https://runkeeper.com/apps/token";
        public static string AppDeauthorizeUrl = "https://runkeeper.com/apps/de-authorize";
    }
}
