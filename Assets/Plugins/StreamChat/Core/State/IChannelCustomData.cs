using System.Collections.Generic;
using StreamChat.Core.State.TrackedObjects;

namespace StreamChat.Core.State
{
    /// <summary>
    /// <see cref="StreamChannel"/>'s custom data. Use it to assign any additional data to the channel.
    ///
    /// Common examples of custom data: image_url, clan_id, description, additional stats, etc.
    /// </summary>
    public interface IChannelCustomData
    {
        int Count { get; }
        IEnumerable<(string Key, object Value)> Items { get; }

        void Add(string key, object value);

        bool Remove(string key);

        bool ContainsKey(string key);

        bool TryGetValue(string key, out object value);

        object this[string key] { get; set; }

        void Clear();
    }
}