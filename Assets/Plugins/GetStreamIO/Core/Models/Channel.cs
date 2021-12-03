using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Plugins.GetStreamIO.Core.Models
{
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

        public void AppendMessage(Message message)
        {
            Messages.Add(message);
            Updated?.Invoke(this);
        }
    }

    //Todo: probably better to have models not polluted with json details
}