using System.Collections.Concurrent;

namespace Lisa.AI.Singletons
{
    public sealed class MemDataStore
    {
        private static readonly Lazy<MemDataStore> lazy = new Lazy<MemDataStore>(() => new MemDataStore());

        public static MemDataStore Instance { get { return lazy.Value; } }

        private ConcurrentDictionary<string, object> data;

        private MemDataStore()
        {
            data = new ConcurrentDictionary<string, object>();
        }

        public void AddData(string key, object value)
        {
            data[key] = value;
        }

        public bool UpdateData(string key, object newValue, bool addIfNotExist = false)
        {
            if (!addIfNotExist && !IsExist(key)) return false;
            return data.AddOrUpdate(key, newValue, (existingKey, existingValue) => newValue) != null;
        }

        public object GetData(string key)
        {
            data.TryGetValue(key, out var value);
            return value;
        }

        public bool RemoveData(string key)
        {
            return data.TryRemove(key, out _);
        }

        public bool IsExist(string key)
        {
            return data.ContainsKey(key);
        }

        public void ClearData()
        {
            data.Clear();
        }
    }
}
