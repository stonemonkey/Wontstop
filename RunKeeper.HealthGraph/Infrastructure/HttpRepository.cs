using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RunKeeper.WinRT.HealthGraph.Authorization;

namespace RunKeeper.WinRT.HealthGraph.Infrastructure
{
    public class HttpRepository : IModelRepository
    {
        private readonly AuthorizationSession _session;

        public HttpRepository(AuthorizationSession session)
        {
            _session = session;
        }

        public Task CreateAsyc<T>(T obj, string resource)
        {
            throw new NotImplementedException();
        }

        public async Task<T> ReadAsyc<T>(string resource)
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, GetUri(resource));
                request.Headers.Add("Authorization", $"Bearer {_session.GetAccessToken()}");

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(json);
            }
        }

        public Task UpdateAsyc<T>(T obj, string resource)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(string resource)
        {
            throw new NotImplementedException();
        }

        protected Uri GetUri(string resource)
        {
            if (string.IsNullOrWhiteSpace(resource))
            {
                throw new ArgumentOutOfRangeException(nameof(resource));
            }

            return new Uri($"{Urls.ApiUrl}{resource}");
        }
    }
}