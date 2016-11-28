using System;
using Windows.Security.Authentication.Web;

namespace RunKeeper.WinRT.HealthGraph.Infrastructure
{
    /// <summary>
    /// Exception for custom WebAuthenticationBroker errors
    /// </summary>
    public class WebAuthenticationException : Exception
    {
        /// <summary>
        /// Authentication response
        /// </summary>
        public WebAuthenticationResult Result { get; }

        /// <summary>
        /// Creates the instance attaching the response.
        /// </summary>
        /// <param name="result">The result of an authentication operation</param>
        public WebAuthenticationException(WebAuthenticationResult result)
        {
            Result = result;
        }
    }
}
