using System;
using System.Collections.Generic;
using StreamChat.Libs.Serialization;

namespace StreamChat.Core.State
{
    /// <summary>
    /// <inheritdoc cref="IStreamCustomData"/>
    /// </summary>
    internal class StreamCustomData : IStreamCustomData
    {
        public int Count => _customData.Count;
        public IReadOnlyCollection<string> Keys => _customData.Keys;
        public bool ContainsKey(string key) => _customData.ContainsKey(key);

        public TCastType Get<TCastType>(string key) => _serializer.TryConvertTo<TCastType>(_customData[key]);
        
        internal StreamCustomData(Dictionary<string, object> customData, ISerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _customData = customData;
        }
        
        private readonly Dictionary<string, object> _customData;
        private readonly ISerializer _serializer;
    }
}