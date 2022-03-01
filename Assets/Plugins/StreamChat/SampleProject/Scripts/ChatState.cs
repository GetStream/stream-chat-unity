using System;
using System.Collections.Generic;
using System.Linq;
using StreamChat.Core;
using StreamChat.Core.Events;
using StreamChat.Core.Exceptions;
using StreamChat.Core.Models;
using StreamChat.Core.Requests;
using UnityEngine;

namespace StreamChat.SampleProject
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

        public ChannelState ActiveChannel
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

        public ChatState(IStreamChatClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));

            _client.Connected += OnClientConnected;
            _client.MessageReceived += OnMessageReceived;
            _client.MessageDeleted += OnMessageDeleted;
            _client.MessageUpdated += OnMessageUpdated;
            _client.ReactionReceived += OnReactionReceived;
            _client.ReactionUpdated += OnReactionUpdated;
            _client.ReactionDeleted += OnReactionDeleted;
        }

        public void Dispose()
        {
            _client.Connected -= OnClientConnected;
            _client.MessageReceived -= OnMessageReceived;
            _client.MessageDeleted -= OnMessageDeleted;
            _client.MessageUpdated -= OnMessageUpdated;
            _client.ReactionReceived -= OnReactionReceived;
            _client.ReactionUpdated -= OnReactionUpdated;
            _client.ReactionDeleted -= OnReactionDeleted;

            _client.Dispose();
        }

        public void OpenChannel(ChannelState channel) => ActiveChannel = channel;

        public void EditMessage(Message message) => MessageEditRequested?.Invoke(message);

        private readonly IStreamChatClient _client;
        private readonly List<ChannelState> _channels = new List<ChannelState>();

        private ChannelState _activeChannel;

        //Todo: get it initially from health check event
        private OwnUser _localUser;

        private async void OnClientConnected()
        {
            var request = new QueryChannelsRequest
            {
                Sort = new List<SortParamRequest>
                {
                    new SortParamRequest
                    {
                        Field = "created_at",
                        Direction = -1,
                    }
                },

                // Example on to Limit & Offset results
                // Limit = 30,
                // Offset = 0,

                // Example on how to get only channels containing a specific member
                // FilterConditions = new Dictionary<string, object>
                // {
                //     {"members", new Dictionary<string, object>
                //     {
                //         {"$in", new string[]{_client.UserId}}
                //     }}
                // }
            };

            try
            {
                var queryChannelsResponse = await _client.ChannelApi.QueryChannelsAsync(request);

                _channels.Clear();
                _channels.AddRange(queryChannelsResponse.Channels);
            }
            catch (StreamApiException e)
            {
                e.LogStreamApiExceptionDetails();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            if (ActiveChannel == null && _channels.Count > 0)
            {
                ActiveChannel = _channels.First();
            }

            ChannelsUpdated?.Invoke();
        }

        private void OnMessageReceived(EventMessageNew messageNewEvent)
        {
            var channel = GetChannel(messageNewEvent.ChannelId);
            channel.AddMessage(messageNewEvent.Message);
        }

        private void OnMessageDeleted(EventMessageDeleted messageDeletedEvent)
        {
            var channel = GetChannel(messageDeletedEvent.ChannelId);
            var message = channel.Messages.First(_ => _.Id == messageDeletedEvent.Message.Id);
            message.Text = MessageDeletedInfo;

            if (channel == ActiveChannel)
            {
                ActiveChanelChanged?.Invoke(ActiveChannel);
            }
        }

        private void OnMessageUpdated(EventMessageUpdated messageUpdatedEvent)
        {
            var channel = GetChannel(messageUpdatedEvent.ChannelId);

            if (channel == ActiveChannel)
            {
                ActiveChanelChanged?.Invoke(ActiveChannel);
            }
        }

        private void OnReactionReceived(EventReactionNew eventReactionNew) =>
            UpdateChannelMessage(eventReactionNew.Message);

        private void OnReactionDeleted(EventReactionDeleted eventReactionDeleted) =>
            UpdateChannelMessage(eventReactionDeleted.Message);

        private void OnReactionUpdated(EventReactionUpdated eventReactionUpdated) =>
            UpdateChannelMessage(eventReactionUpdated.Message);

        private ChannelState GetChannel(string id) => _channels.First(_ => _.Channel.Id == id);

        private void UpdateChannelMessage(Message message)
        {
            var channelCid = message.Cid;

            var channel = _channels.FirstOrDefault(_ => _.Channel.Cid == channelCid);

            if (channel == null)
            {
                return;
            }

            if (channel.Messages is IList<Message> channelMessages)
            {
                for (var i = channelMessages.Count - 1; i >= 0; i--)
                {
                    if (channelMessages[i].Id == message.Id)
                    {
                        channelMessages.RemoveAt(i);
                        channelMessages.Insert(i, message);
                        break;
                    }
                }

                if (channel == ActiveChannel)
                {
                    ActiveChanelChanged?.Invoke(ActiveChannel);
                }
            }
            else
            {
                Debug.LogError("Failed to update channel message");
            }
        }
    }
}