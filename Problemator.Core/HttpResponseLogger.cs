using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using HttpApiClient;

namespace Problemator.Core
{
    public class HttpResponseLogger : IResponseLogger
    {
        public Task LogAsync(Response response, TimeSpan duration)
        {
            var request = response.Request;
            var type = request.GetType().Name;
            var url = request.Config.BuildUrl();

            Debug.WriteLine($"{type}: {url}");

            var parser = response.Parser;
            if (parser != null)
            {
                var code = int.Parse(parser.GetStatusCode());
                var status = (HttpStatusCode) code;
                Debug.WriteLine($" - {status.ToString()} ({code})");
            }
            else if (response.IsRequestFailure())
            {
                Debug.WriteLine(" - Client exception, airplain mode or wrong url (the server name or address could not be resolved).");
            }
            else if (response.IsRequestTimeout())
            {
                Debug.WriteLine(" - Client timeout, no response from server or some tool is eating the traffic (NetLimiter kind).");
            }
            else
            {
                Debug.WriteLine(" - Client cancel.");
            }

            Debug.WriteLine($" - {duration.Milliseconds} ms");

            return Task.FromResult(true);
        }
    }
}
