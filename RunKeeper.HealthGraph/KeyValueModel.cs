using System;
using System.Threading.Tasks;

namespace RunKeeper.WinRT.HealthGraph
{
    /// <summary>
    /// Represents a model exposing data organized internally as key-value collection.
    /// </summary>
    public class KeyValueModel : ReadOnlyKeyValueModel
    {
        /// <summary>
        /// Saves the key-value pair.
        /// </summary>
        /// <param name="key">The key identifier</param>
        /// <param name="value">The value associated with the key</param>
        public void SetValue(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentOutOfRangeException(nameof(key));
            }

            if (value == null)
            {
                Data.Remove(key);
            }
            else
            {
                Data[key] = value;
            }
        }
    }
}