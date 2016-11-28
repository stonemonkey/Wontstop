using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RunKeeper.WinRT.HealthGraph.Infrastructure
{
    public static class DictionaryExtensions
    {
        public static string GetValue(this IDictionary<string, string> dictionary, string key)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentOutOfRangeException(nameof(key));
            }

            string value;
            dictionary.TryGetValue(key, out value);

            return value;
        }

        public static bool IsNullOrEmpty(this IDictionary<string, string> dictionary)
        {
            return (dictionary == null) || !dictionary.Any();
        }

        public static async Task<IDictionary<string, string>> FatchCachedAsync(this string resource, 
            IModelRepository cacheRepository, IModelRepository serverRepository)
        {
            if (string.IsNullOrWhiteSpace(resource))
            {
                throw new ArgumentOutOfRangeException(nameof(resource));
            }
            if (cacheRepository == null)
            {
                throw new ArgumentNullException(nameof(cacheRepository));
            }
            if (serverRepository == null)
            {
                throw new ArgumentNullException(nameof(serverRepository));
            }

            var data = await cacheRepository.ReadAsyc<IDictionary<string, string>>(resource);
            if (data.IsNullOrEmpty())
            {
                data = await serverRepository.ReadAsyc<IDictionary<string, string>>(resource);
                await cacheRepository.CreateAsyc(data, resource);
            }

            return data;
        }
    }
}