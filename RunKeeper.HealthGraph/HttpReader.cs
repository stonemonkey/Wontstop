using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RunKeeper.WinRT.HealthGraph
{
    /// <summary>
    /// RunKeeper model reader over HTTP.
    /// </summary>
    public class HttpReader : IModelReader
    {
        private readonly string _apiUrl;
        private readonly string _accessToken;

        /// <summary>
        /// Initializes the instance with 
        /// </summary>
        /// <param name="apiUrl">The url of API used to get data</param>
        /// <param name="accessToken">The access token of the active session.</param>
        public HttpReader(string apiUrl, string accessToken)
        {
            _apiUrl = apiUrl;
            _accessToken = accessToken;
        }

        /// <summary>
        /// Request data from HTTP API.
        /// </summary>
        /// <typeparam name="T">Data type of the response.</typeparam>
        /// <returns>Awaitable task</returns>
        public async Task<T> ReadAsyc<T>()
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_apiUrl));
                request.Headers.Add("Authorization", $"Bearer {_accessToken}");

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(json);
            }
        }
    }
}