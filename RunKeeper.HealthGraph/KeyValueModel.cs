using System;
using System.Threading.Tasks;

namespace RunKeeper.WinRT.HealthGraph
{
    public class KeyValueModel : ReadOnlyKeyValueModel
    {
        protected void SetValue(string key, string value)
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

        public Task SaveAsync()
        {
            throw new NotImplementedException();
        }
    }
}