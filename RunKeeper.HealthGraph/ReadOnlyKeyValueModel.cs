using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RunKeeper.WinRT.HealthGraph
{
    public class ReadOnlyKeyValueModel
    {
        protected readonly Dictionary<string, string> Data = new Dictionary<string, string>();

        protected string GetValue(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentOutOfRangeException(nameof(key));
            }

            string value = null;
            Data.TryGetValue(key, out value);

            return value;
        }

        public async Task LoadAsync(IModelReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            var data = await reader.ReadAsyc<Dictionary<string, string>>();
            foreach (var item in data)
            {
                Data[item.Key] = item.Value;
            }
        }
    }
}