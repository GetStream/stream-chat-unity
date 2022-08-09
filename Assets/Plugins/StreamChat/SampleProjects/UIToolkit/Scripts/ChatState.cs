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
        public event Action ActiveChannelChanged;

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
        }

        private void UnsubscribeFromEvents()
        {
            _streamClient.Connected -= OnConnected;
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
    }
}