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
        public const string MessageDeletedInfo = "This message was deleted...";

        public event Action<Channel> ActiveChanelChanged;
        public event Action ChannelsUpdated;

        public event Action<Message> MessageEditRequested;

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
            _client.MessageDeleted += OnMessageDeleted;
        }

        public void Dispose()
        {
            _client.Connected -= OnClientConnected;
            _client.MessageReceived -= OnMessageReceived;
            _client.MessageDeleted -= OnMessageDeleted;

            _client.Dispose();
        }

        public void OpenChannel(Channel channel) => ActiveChannel = channel;

        public void EditMessage(Message message) => MessageEditRequested?.Invoke(message);

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

        private void OnMessageReceived(MessageNewEvent messageNewEvent)
        {
            var channel = _channels.First(_ => _.Details.Id == messageNewEvent.ChannelId);
            channel.AppendMessage(messageNewEvent.Message);
        }

        private void OnMessageDeleted(MessageDeletedEvent messageDeletedEvent)
        {
            var channel = _channels.First(_ => _.Details.Id == messageDeletedEvent.ChannelId);
            var message = channel.Messages.First(_ => _.Id == messageDeletedEvent.Message.Id);
            message.Text = MessageDeletedInfo;

            ActiveChanelChanged?.Invoke(ActiveChannel);
        }
    }
}