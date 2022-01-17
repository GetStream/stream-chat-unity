using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.GetStreamIO.Core;
using Plugins.GetStreamIO.Core.Events.DTO;
using Plugins.GetStreamIO.Core.Models;
using Plugins.GetStreamIO.Core.Requests.V2;
using UnityEngine;

namespace Plugins.GetStreamIO.Unity
{
    /// <summary>
    /// Implementation of <see cref="IChatState"/>
    /// </summary>
    public class ChatState : IChatState
    {
        public const string MessageDeletedInfo = "This message was deleted...";

        public event Action<ChannelState> ActiveChanelChanged;
        public event Action ChannelsUpdated;

        public event Action<Message> MessageEditRequested;

        public ChannelState ActiveChannelDeprecated
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

        public IReadOnlyList<ChannelState> Channels => _channels;

        public ChatState(IGetStreamChatClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));

            _client.Connected += OnClientConnected;
            _client.MessageReceived += OnMessageReceived;
            _client.MessageDeleted += OnMessageDeleted;
            _client.MessageUpdated += OnMessageUpdated;
        }

        public void Dispose()
        {
            _client.Connected -= OnClientConnected;
            _client.MessageReceived -= OnMessageReceived;
            _client.MessageDeleted -= OnMessageDeleted;
            _client.MessageUpdated -= OnMessageUpdated;

            _client.Dispose();
        }

        public void OpenChannel(ChannelState channel) => ActiveChannelDeprecated = channel;

        public void EditMessage(Message message) => MessageEditRequested?.Invoke(message);

        private readonly IGetStreamChatClient _client;
        private readonly List<ChannelState> _channels = new List<ChannelState>();

        private ChannelState _activeChannel;

        private async void OnClientConnected()
        {
            var request = new QueryChannelsRequest
            {
                Sort = new List<SortParamRequest>
                {
                    new SortParamRequest
                    {
                        Field = nameof(Message.CreatedAt),
                        Direction = -1,
                    }
                }
            };

            try
            {
                var response = await _client.QueryChannelsAsync(request);

                _channels.Clear();
                _channels.AddRange(response.Channels);
            }
            catch (StreamApiException e)
            {
                e.LogStreamApiExceptionDetails();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            if (ActiveChannelDeprecated == null && _channels.Count > 0)
            {
                ActiveChannelDeprecated = _channels.First();
            }

            ChannelsUpdated?.Invoke();
        }

        private void OnMessageReceived(MessageNewEvent messageNewEvent)
        {
            var channel = _channels.First(_ => _.Channel.Id == messageNewEvent.ChannelId);
            channel.AddMessage(messageNewEvent.Message);
        }

        private void OnMessageDeleted(MessageDeletedEvent messageDeletedEvent)
        {
            var channel = _channels.First(_ => _.Channel.Id == messageDeletedEvent.ChannelId);
            var message = channel.Messages.First(_ => _.Id == messageDeletedEvent.Message.Id);
            message.Text = MessageDeletedInfo;

            ActiveChanelChanged?.Invoke(ActiveChannelDeprecated);
        }

        private void OnMessageUpdated(MessageUpdated messageUpdatedEvent)
        {
            var channel = _channels.First(_ => _.Channel.Id == messageUpdatedEvent.Channel_id);
            ActiveChanelChanged?.Invoke(channel);
        }
    }
}