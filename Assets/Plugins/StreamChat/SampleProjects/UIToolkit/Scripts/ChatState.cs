using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Core.Exceptions;
using StreamChat.Core.Models;
using StreamChat.Core.Requests;
using StreamChat.Libs.Auth;
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
        public event Action<ChannelState> ActiveChanelChanged;

        public OwnUser OwnUser { get; private set; }

        public IReadOnlyList<ChannelState> Channels => _channels;

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

        public ChatState(AuthCredentials chatCredentials)
        {
            _client = StreamChatClient.CreateDefaultClient(chatCredentials);
            _client.Connect();

            SubscribeToEvents();
        }

        public void Update(float deltaTime)
            => _client.Update(deltaTime);

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
            _client?.Dispose();
        }

        private readonly ILogs _unityLogger = new UnityLogs();

        private readonly IStreamChatClient _client;
        private readonly List<ChannelState> _channels = new List<ChannelState>();

        private ChannelState _activeChannel;

        private void SubscribeToEvents()
        {
            _client.Connected += OnConnected;
        }

        private void UnsubscribeFromEvents()
        {
            _client.Connected -= OnConnected;
        }

        private async void OnConnected(OwnUser ownUser)
        {
            OwnUser = ownUser;
            OwnUserUpdated?.Invoke();

            var channels = await FetchChannelsAsync();

            _channels.Clear();
            _channels.AddRange(channels);

            ChannelsUpdated?.Invoke();
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
                            { "$in", new string[] { _client.UserId } }
                        }
                    }
                }
            };

            try
            {
                var queryChannelsResponse = await _client.ChannelApi.QueryChannelsAsync(request);

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
    }
}