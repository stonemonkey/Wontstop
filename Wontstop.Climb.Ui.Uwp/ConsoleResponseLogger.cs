// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using HttpApiClient;
using HttpApiClient.Requests;

namespace Wontstop.Climb.Ui.Uwp
{
    public class ConsoleResponseLogger : IResponseLogger
    {
        public Task LogAsync(Response response, TimeSpan duration)
        {
            var request = response.Request;
            var name = request.Config.BuildUrl();

            var parser = response.Parser;
            if (parser != null)
            {
                Debug.WriteLine($"{name} {duration} {parser.GetStatusCode()}");
            }
            else if (response.IsRequestTimeout())
            {
                // client timeout, no response from server or some tool is eating the traffic (NetLimiter kind)
                Debug.WriteLine($"{name} {duration} timeout");
            }
            else if (response.IsRequestFailure())
            {
                // client exception, airplain mode or wrong url (the server name or address could not be resolved)
                Debug.WriteLine($"{name} {duration} server address could not be resolved");
            }
            else
            {
                // client cancel
                Debug.WriteLine($"{name} {duration} client cancel");
            }

            return Task.FromResult(true);
        }
    }
}