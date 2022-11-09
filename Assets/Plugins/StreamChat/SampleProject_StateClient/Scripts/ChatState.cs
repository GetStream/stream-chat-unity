﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Core.Events;
using StreamChat.Core.Exceptions;
using StreamChat.Core.Models;
using StreamChat.Core.Requests;
using StreamChat.Core.State;
using StreamChat.Core.State.TrackedObjects;
using StreamChat.Libs.Logs;
using StreamChat.SampleProject_StateClient.Utils;
using StreamChat.SampleProject_StateClient.Views;
using UnityEngine;

namespace StreamChat.SampleProject_StateClient
{
    /// <summary>
    /// Implementation of <see cref="IChatState"/>
    /// </summary>
    public class ChatState : IChatState
    {
        public const string MessageDeletedInfo = "This message was deleted...";

        public event Action<StreamChannel> ActiveChanelChanged;
        public event Action ChannelsUpdated;

        public event Action<StreamMessage> MessageEditRequested;

        public StreamChannel ActiveChannel
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

        public IReadOnlyList<StreamChannel> Channels => _channels;

        public IStreamChatStateClient Client { get; }

        public ChatState(IStreamChatStateClient client, IViewFactory viewFactory)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
            _viewFactory = viewFactory ?? throw new ArgumentNullException(nameof(viewFactory));

            Client.Connected += OnClientConnected;
            Client.ConnectionStateChanged += OnClientConnectionStateChanged;
            //StreamTodo: handle this
            // Client.MessageRead += OnMessageRead;
            //
            // Client.NotificationMarkRead += OnNotificationMarkRead;
        }

        public void Dispose()
        {
            Client.Connected -= OnClientConnected;
            Client.ConnectionStateChanged -= OnClientConnectionStateChanged;
            // Client.MessageRead -= OnMessageRead;
            //
            // Client.NotificationMarkRead -= OnNotificationMarkRead;

            Client.Dispose();
        }

        public void ShowPopup<TPopup>()
            where TPopup : BaseFullscreenPopup
        {
            _viewFactory.CreateFullscreenPopup<TPopup>();
        }

        public void HidePopup<TPopup>(TPopup instance)
            where TPopup : BaseFullscreenPopup
        {
            GameObject.Destroy(instance.gameObject);
        }

        public Task<StreamChannel> CreateNewChannelAsync(string channelName)
            => Client.GetOrCreateChannelAsync(ChannelType.Messaging, channelId: Guid.NewGuid().ToString(), channelName);

        public void OpenChannel(StreamChannel channel) => ActiveChannel = channel;

        public void EditMessage(StreamMessage message) => MessageEditRequested?.Invoke(message);

        public async Task UpdateChannelsAsync()
        {
            var requestOld = new QueryChannelsRequest
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
                            { "$in", new string[] { Client.LocalUserData.User.Id } }
                        }
                    }
                }
            };

            var filter = new Dictionary<string, object>
            {
                {
                    "members", new Dictionary<string, object>
                    {
                        { "$in", new string[] { Client.LocalUserData.User.Id } }
                    }
                }
            };

            try
            {
                var channels = await Client.QueryChannelsAsync(filter);

                _channels.Clear();
                _channels.AddRange(channels);

                if (ActiveChannel != null)
                {
                    var activeChannel = _channels.FirstOrDefault(_ => _ == ActiveChannel);
                    if (activeChannel != null)
                    {
                        ActiveChannel = activeChannel;
                    }
                    else
                    {
                        ActiveChannel = _channels.FirstOrDefault();
                    }
                }
            }
            catch (StreamApiException e)
            {
                e.LogStreamApiExceptionDetails(_unityLogger);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            ChannelsUpdated?.Invoke();
        }

        public Task LoadPreviousMessagesAsync()
        {
            if (ActiveChannel == null)
            {
                return Task.CompletedTask;
            }

            return ActiveChannel.LoadOlderMessagesAsync();
        }
        
        private readonly List<StreamChannel> _channels = new List<StreamChannel>();

        private readonly IViewFactory _viewFactory;
        private readonly ILogs _unityLogger = new UnityLogs();

        //StreamTodo: get it initially from health check event
        private OwnUser _localUser;
        private StreamChannel _activeChannel;

        private Task _activeLoadPreviousMessagesTask;
        private bool _restoreStateAfterReconnect;

        private async void OnClientConnected(StreamLocalUserData localUserData)
        {
            await UpdateChannelsAsync();

            if (ActiveChannel == null && _channels.Count > 0)
            {
                ActiveChannel = _channels.First();
            }
        }

        private void OnNotificationMarkRead(EventNotificationMarkRead eventNotificationMarkRead)
            => Debug.Log($"Notified mark read for channel: {eventNotificationMarkRead.Cid}, " +
                         $"TotalUnreadCount: {eventNotificationMarkRead.TotalUnreadCount}, UnreadChannels: {eventNotificationMarkRead.UnreadChannels}");

        private void OnMessageRead(EventMessageRead eventMessageRead)
            => Debug.Log("Message read received for channel: " + eventMessageRead.Cid);
        
        private void OnClientConnectionStateChanged(ConnectionState prev, ConnectionState current)
        {
            if (current == ConnectionState.Disconnected)
            {
                _restoreStateAfterReconnect = true;
            }

            if (current == ConnectionState.Connected && _restoreStateAfterReconnect)
            {
                _restoreStateAfterReconnect = false;
                //StreamTodo: this should be handled by state client
            }
        }
    }
}