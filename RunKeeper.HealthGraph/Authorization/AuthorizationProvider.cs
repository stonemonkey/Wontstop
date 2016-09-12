using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Newtonsoft.Json;

namespace RunKeeper.WinRT.HealthGraph.Authorization
{
    /// <summary>
    /// Provides operations for performing authorization against RunKeeper SSO.
    /// </summary>
    public class AuthorizationProvider : IAuthorizationProvider
    {
        private readonly Uri _callbackUri;
        private readonly string _clientId;
        private readonly string _clientSecret;

        /// <summary>
        /// Creates the instance with RunKeeper 
        /// </summary>
        /// <param name="clientId">The client id received for your application when registred on 
        /// RunKeeper applications portal.</param>
        /// <param name="clientSecret">The client secred received for your application when 
        /// registred on RunKeeper applications portal.</param>
        /// <exception cref="ArgumentOutOfRangeException">Null or empty clientId/clientSecret</exception>
        public AuthorizationProvider(string clientId, string clientSecret)
        {
            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new ArgumentOutOfRangeException(nameof(clientId));
            }
            if (string.IsNullOrWhiteSpace(clientSecret))
            {
                throw new ArgumentOutOfRangeException(nameof(clientSecret));
            }

            _clientId = clientId;
            _clientSecret = clientSecret;
            _callbackUri = WebAuthenticationBroker.GetCurrentApplicationCallbackUri();
        }

        private const string AuthorizeEndpoint = "https://runkeeper.com/apps/authorize";
        /// <summary>
        /// Starts the authorization process.
        /// </summary>
        /// <typeparam name="T">The type of data instance returned as the result of a successfull 
        /// authorization</typeparam>
        /// <returns>Awaitable task containing successfull data received from RunKeeper</returns>
        /// <exception cref="WebAuthenticationResult">In case the authorization fails or the access 
        /// is denied.</exception>
        public async Task<T> AuthorizeAsync<T>() where T : class
        {
            var facebookUrl = AuthorizeEndpoint +
                $"?client_id={Uri.EscapeDataString(_clientId)}" +
                $"&redirect_uri={Uri.EscapeDataString(_callbackUri.ToString())}" +
                "&response_type=code";

            var startUri = new Uri(facebookUrl);
            var endUri = _callbackUri;
            try
            {
                var webAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(
                    WebAuthenticationOptions.None, startUri, endUri);
                var code = Parse(webAuthenticationResult);
                return await RequestToken<T>(code);

            }
            catch (FileNotFoundException)
            {
                // [cosmo 2015/6/10] this is silly but WebAuthenticationBroker throws FileNotFound in airplain mode
                return null;
            }
        }

        private static string Parse(WebAuthenticationResult result)
        {
            if ((result.ResponseStatus != WebAuthenticationStatus.Success) || IsAccessDenied(result))
            {
                throw new WebAuthenticationException(result);
            }

            return GetParameterValue(result.ResponseData, "code");
        }

        private static bool IsAccessDenied(WebAuthenticationResult result)
        {
            var error = GetParameterValue(result.ResponseData, "error");

            return string.Equals(error, "access_denied", StringComparison.Ordinal);
        }

        private static string GetParameterValue(string webAuthResultResponseData, string key)
        {
            if (string.IsNullOrWhiteSpace(webAuthResultResponseData))
            {
                throw new ArgumentOutOfRangeException(nameof(webAuthResultResponseData));
            }

            var parameters = webAuthResultResponseData.GetUrlParameters();

            return parameters.GetUrlParameterValue(key);
        }

        private const string TokenEndpoint = "https://runkeeper.com/apps/token";
        private async Task<T> RequestToken<T>(string code) where T : class
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentOutOfRangeException(nameof(code));
            }

            var body = $"&code={code}" +
                "&grant_type=authorization_code" +
                $"&client_id={Uri.EscapeDataString(_clientId)}" +
                $"&client_secret={Uri.EscapeDataString(_clientSecret)}" +
                $"&redirect_uri={Uri.EscapeDataString(_callbackUri.ToString())}";

            using (var client = new HttpClient())
            { 
                var request = new HttpRequestMessage(HttpMethod.Post, new Uri(TokenEndpoint))
                {
                    Content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded"),
                };

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var token = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<T>(token);
            }
        }

        private const string DeauthorizeEndpoint = "https://runkeeper.com/apps/de-authorize";
        /// <summary>
        /// Removes authorization.
        /// </summary>
        /// <param name="accessToken">The token obtained authorizing successfull session that needs
        /// to be unauthorized.</param>
        /// <returns>Awaitable task</returns>
        public async Task DeauthorizeAsync(string accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new ArgumentOutOfRangeException(nameof(accessToken));
            }

            var body = $"&access_token={accessToken}";

            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, new Uri(DeauthorizeEndpoint))
                {
                    Content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded"),
                };

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
            }
        }     
    }
}