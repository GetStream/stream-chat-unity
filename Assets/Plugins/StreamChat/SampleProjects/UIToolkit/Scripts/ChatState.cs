using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Core.Events;
using StreamChat.Core.Exceptions;
using StreamChat.Core.Models;
using StreamChat.Core.Requests;
using StreamChat.Libs.Logs;
using UnityEngine;

namespace StreamChat.SampleProjects.UIToolkit
{
    /// <summary>
    /// Maintains Chat State and exposes chat data to other components
    /// </summary>
    public class ChatState : IChatState
    {
        public event Action OwnUserUpdated;

        public event Action ChannelsUpdated;
        public event Action ActiveChannelChanged;
        public event Action<Message> MessageReceived;
        public event Action<Message> MessageDeleted;
        public event Action<Message> MessageUpdated;

        public OwnUser OwnUser { get; private set; }

        public IReadOnlyList<ChannelState> Channels => _channels;

        public ChannelState ActiveChannel
        {
            get => _activeChannel;
            private set
            {
                if (_activeChannel != null && _activeChannel.Channel.Cid == value?.Channel.Cid)
                {
                    return;
                }

                _activeChannel = value;

                Debug.Log("Selected channel: " + (_activeChannel?.Channel.Id ?? "None"));
                ActiveChannelChanged?.Invoke();
            }
        }

        public ChatState(IStreamChatClient streamChatClient)
        {
            _streamClient = streamChatClient ?? throw new ArgumentNullException(nameof(streamChatClient));

            SubscribeToEvents();
        }

        public void Update(float deltaTime)
            => _streamClient.Update(deltaTime);

        public void SelectChannel(ChannelState channelState)
        {
            var channel = _channels.FirstOrDefault(_ => _.Channel.Cid == channelState.Channel.Cid);

            if (channel == null)
            {
                Debug.LogError("Failed to find channel with Cid: " + channelState.Channel.Cid);
                return;
            }

            ActiveChannel = channel;
        }

        public void Dispose()
        {
            UnsubscribeFromEvents();
            _streamClient?.Dispose();
        }

        private readonly ILogs _unityLogger = new UnityLogs();

        private readonly IStreamChatClient _streamClient;
        private readonly List<ChannelState> _channels = new List<ChannelState>();

        private ChannelState _activeChannel;

        private void SubscribeToEvents()
        {
            _streamClient.Connected += OnConnected;
            _streamClient.MessageReceived += OnMessageReceived;
            _streamClient.MessageDeleted += OnMessageDeleted;
            _streamClient.MessageUpdated += OnMessageUpdated;
        }

        private void UnsubscribeFromEvents()
        {
            _streamClient.Connected -= OnConnected;
            _streamClient.MessageReceived -= OnMessageReceived;
            _streamClient.MessageDeleted -= OnMessageDeleted;
            _streamClient.MessageUpdated -= OnMessageUpdated;
        }

        private async void OnConnected(OwnUser ownUser)
        {
            OwnUser = ownUser;
            OwnUserUpdated?.Invoke();

            var channels = await FetchChannelsAsync();

            _channels.Clear();
            _channels.AddRange(channels);

            ChannelsUpdated?.Invoke();

            if (ActiveChannel == null && _channels.Count > 0)
            {
                ActiveChannel = _channels[0];
            }
        }

        private async Task<IEnumerable<ChannelState>> FetchChannelsAsync()
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

                // Limit & Offset results
                Limit = 30,
                Offset = 0,

                // Get only channels containing a specific member
                FilterConditions = new Dictionary<string, object>
                {
                    {
                        "members", new Dictionary<string, object>
                        {
                            { "$in", new string[] { _streamClient.UserId } }
                        }
                    }
                }
            };

            try
            {
                var queryChannelsResponse = await _streamClient.ChannelApi.QueryChannelsAsync(request);
                return queryChannelsResponse.Channels;

            }
            catch (StreamApiException e)
            {
                e.LogStreamApiExceptionDetails(_unityLogger);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return Enumerable.Empty<ChannelState>();
        }

        private void OnMessageUpdated(EventMessageUpdated eventMessageUpdated)
        {
            var channel = _channels.FirstOrDefault(_ => _.Channel.Cid == eventMessageUpdated.Cid);
            if (channel == null)
            {
                Debug.LogError($"Deleted message, but channel with CID `{eventMessageUpdated.Cid}` not found");
                return;
            }

            if (channel.Messages?.Count == 0)
            {
                return;
            }

            for (int i = channel.Messages.Count - 1; i >= 0; i--)
            {
                var message = channel.Messages[i];
                if (message.Id != eventMessageUpdated.Message.Id)
                {
                    continue;
                }

                channel.Messages.RemoveAt(i);
                channel.Messages.Insert(i, eventMessageUpdated.Message);
                MessageUpdated?.Invoke(eventMessageUpdated.Message);
                return;
            }
        }

        private void OnMessageDeleted(EventMessageDeleted eventMessageDeleted)
        {
            var channel = _channels.FirstOrDefault(_ => _.Channel.Cid == eventMessageDeleted.Cid);
            if (channel == null)
            {
                Debug.LogError($"Deleted message, but channel with CID `{eventMessageDeleted.Cid}` not found");
                return;
            }

            if (channel.Messages?.Count == 0)
            {
                return;
            }

            for (int i = channel.Messages.Count - 1; i >= 0; i--)
            {
                var message = channel.Messages[i];
                if (message.Id != eventMessageDeleted.Message.Id)
                {
                    continue;
                }

                channel.Messages.RemoveAt(i);
                MessageDeleted?.Invoke(message);
                return;
            }
        }

        private void OnMessageReceived(EventMessageNew eventMessageNew)
        {
            var channel = _channels.FirstOrDefault(_ => _.Channel.Cid == eventMessageNew.Cid);
            if (channel == null)
            {
                Debug.LogError($"Received message, but channel with CID `{eventMessageNew.Cid}` not found");
                return;
            }

            channel.Messages.Add(eventMessageNew.Message);
            MessageReceived?.Invoke(eventMessageNew.Message);
        }
    }
}