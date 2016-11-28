using System;
using System.Collections.Generic;
using System.Linq;

namespace RunKeeper.WinRT.HealthGraph.Infrastructure
{
    public static class HttpStringExtensions
    {
        private static readonly string[] ReserverdChars =
        {
            ":","?","#","[","]","@","!","$","&","\'","(",")","*","+",",",";","//" //,'/','='
        };

        public static IEnumerable<string> GetUrlParameters(this string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return Enumerable.Empty<string>();
            }

            var p = url.Split(ReserverdChars, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => x.Contains('='));

            return p;
        }

        public static string GetUrlParameterValue(this IEnumerable<string> parameters, string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentOutOfRangeException(nameof(key));
            }

            var v = parameters.FirstOrDefault(value =>
                value.StartsWith(key));

            return v?.Remove(0, key.Length + 1);
        }
    }
}