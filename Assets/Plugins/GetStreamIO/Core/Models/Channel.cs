using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Plugins.GetStreamIO.Core.Models
{
    //Todo: extract DTOs - probably better to have models not polluted with json details
    /// <summary>
    /// Channel model
    /// </summary>
    public class Channel
    {
        public event Action<Channel> Updated;

        [JsonProperty("channel")]
        public ChannelDetails Details;

        [JsonProperty("messages")]
        public List<Message> Messages = new List<Message>();

        [JsonProperty("watcher_count")]
        public int WatcherCount;

        [JsonProperty("read")]
        public List<User> Read = new List<User>();

        [JsonProperty("members")]
        public List<Member> Members = new List<Member>();

        public string Name => Details.Name;

        public void AppendMessage(Message message)
        {
            Messages.Add(message);
            Updated?.Invoke(this);
        }

        public void DeleteMessage(Message message, bool hard)
        {

        }

        public bool IsDirectMessage => Details.MemberCount == 2 && Members.Any(_ => _.User.Id == Details.CreatedBy.Id);
    }
}