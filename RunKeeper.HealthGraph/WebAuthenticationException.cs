using System;
using Windows.Security.Authentication.Web;

namespace RunKeeper.WinRT.HealthGraph
{
    public class WebAuthenticationException : Exception
    {
        public WebAuthenticationResult Result { get; }

        public WebAuthenticationException(WebAuthenticationResult result)
        {
            Result = result;
        }
    }
}
