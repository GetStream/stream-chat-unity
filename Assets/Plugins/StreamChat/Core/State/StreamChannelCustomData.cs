using System.Collections.Generic;
using System.Linq;

namespace StreamChat.Core.State
{
    public class StreamChannelCustomData : IStreamChannelCustomData
    {
        public int Count => _internalDictionary.Count;
        public IEnumerable<(string Key, object Value)> Items => _internalDictionary.Select(_ => (_.Key, _.Value));

        public StreamChannelCustomData()
        {
            _internalDictionary = new Dictionary<string, object>();
        }

        public StreamChannelCustomData(Dictionary<string, object> internalDictionary)
        {
            _internalDictionary = internalDictionary ?? new Dictionary<string, object>();
        }

        public void Add(string key, object value) => _internalDictionary.Add(key, value);

        public bool Remove(string key) => _internalDictionary.Remove(key);

        public bool ContainsKey(string key) => _internalDictionary.ContainsKey(key);

        public bool TryGetValue(string key, out object value) => _internalDictionary.TryGetValue(key, out value);

        public object this[string key]
        {
            get => _internalDictionary[key];
            set => _internalDictionary[key] = value;
        }

        public void Clear() => _internalDictionary.Clear();

        private readonly Dictionary<string, object> _internalDictionary;
    }
}