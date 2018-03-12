using System;
using System.Net.Http;
using System.Threading.Tasks;
using HttpApiClient.Parsers;
using MvvmToolkit.Utils;
using Newtonsoft.Json.Linq;

namespace Problemator.Core.Utils
{
    public class ProblematorJsonParser : StringParser
    {
        protected JToken Data;

        public override async Task ParseAsync(HttpResponseMessage httpResponse)
        {
            httpResponse.ValidateNotNull(nameof(httpResponse));

            await base.ParseAsync(httpResponse);

            try
            {
                Data = string.IsNullOrWhiteSpace(Content) ? 
                    new JObject() : 
                    JToken.Parse(Content.TrimStart('(').TrimEnd(')'));
            }
            catch (Exception e)
            {
                Data = JToken.FromObject(new
                {
                    error = true,
                    message = $"{e.Message}: '{Content}'"
                });
            }
        }

        public string GetValue(string key)
        {
            key.ValidateNotNullEmptyWhiteSpace(nameof(key));

            var data = Data as JObject;
            var value = data?[key];
            return (value == null) ? string.Empty : value.ToString();
        }

        public JToken GetData()
        {
            return Data;
        }
    }
}