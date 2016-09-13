using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RunKeeper.WinRT.HealthGraph
{
    /// <summary>
    /// Represents a model exposing readonly data organized internally as key-value collection.
    /// </summary>
    public class ReadOnlyKeyValueModel
    {
        protected readonly Dictionary<string, string> Data = new Dictionary<string, string>();

        /// <summary>
        /// Retrives value for specified key.
        /// </summary>
        /// <param name="key">The key identifier for the value</param>
        /// <returns>The value</returns>
        /// <exception cref="ArgumentOutOfRangeException">Null or empty key</exception>
        public string GetValue(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentOutOfRangeException(nameof(key));
            }

            string value = null;
            Data.TryGetValue(key, out value);

            return value;
        }

        /// <summary>
        /// Loads the internal content of the model from an external source.
        /// </summary>
        /// <param name="reader">The reader used to load internal data.</param>
        /// <returns>Awaitable task</returns>
        /// <exception cref="ArgumentNullException">Null reader</exception>
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