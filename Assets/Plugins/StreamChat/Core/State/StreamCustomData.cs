using System;
using System.Collections.Generic;
using StreamChat.Libs.Serialization;

namespace StreamChat.Core.State
{
    /// <inheritdoc cref="IStreamCustomData"/>
    internal class StreamCustomData : IStreamCustomData
    {
        public int Count => _customData.Count;
        public IReadOnlyCollection<string> Keys => _customData.Keys;

        public bool ContainsKey(string key) => _customData.ContainsKey(key);

        public TType Get<TType>(string key) => _serializer.TryConvertTo<TType>(_customData[key]);

        public bool TryGet<TType>(string key, out TType value)
        {
            if (!_customData.ContainsKey(key))
            {
                value = default;
                return false;
            }

            value = _serializer.TryConvertTo<TType>(_customData[key]);
            return true;
        }

        internal StreamCustomData(Dictionary<string, object> customData, ISerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _customData = customData;
        }

        private readonly Dictionary<string, object> _customData;
        private readonly ISerializer _serializer;
    }
}