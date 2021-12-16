using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.GetStreamIO.Core;
using Plugins.GetStreamIO.Core.Events.DTO;
using Plugins.GetStreamIO.Core.Models;

namespace Plugins.GetStreamIO.Unity
{
    /// <summary>
    /// Implementation of <see cref="IChatState"/>
    /// </summary>
    public class ChatState : IChatState
    {
        public event Action<Channel> ActiveChanelChanged;
        public event Action ChannelsUpdated;

        public Channel ActiveChannel
        {
            get => _activeChannel;
            private set
            {
                var prevValue = _activeChannel;
                _activeChannel = value;

                if (prevValue != value)
                {
                    ActiveChanelChanged?.Invoke(_activeChannel);
                }
            }
        }

        public IReadOnlyList<Channel> Channels => _channels;

        public ChatState(IGetStreamChatClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));

            _client.Connected += OnClientConnected;
            _client.MessageReceived += OnMessageReceived;
        }

        public void Dispose()
        {
            _client.Connected -= OnClientConnected;
            _client.MessageReceived -= OnMessageReceived;
            _client.Dispose();
        }

        public void OpenChannel(Channel channel) => ActiveChannel = channel;

        private readonly IGetStreamChatClient _client;
        private readonly List<Channel> _channels = new List<Channel>();

        private Channel _activeChannel;

        private async void OnClientConnected()
        {
            var channels = await _client.GetChannelsAsync();

            _channels.Clear();
            _channels.AddRange(channels);

            if (ActiveChannel == null && _channels.Count > 0)
            {
                ActiveChannel = _channels.First();
            }

            ChannelsUpdated?.Invoke();
        }

        private void OnMessageReceived(NewMessageEvent newMessageEvent)
        {
            var channel = _channels.First(_ => _.Details.Id == newMessageEvent.ChannelId);
            channel.AppendMessage(newMessageEvent.Message);
        }
    }
}