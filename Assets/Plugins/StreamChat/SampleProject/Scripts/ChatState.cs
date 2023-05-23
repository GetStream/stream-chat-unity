using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Core.Exceptions;
using StreamChat.Core.LowLevelClient.Events;
using StreamChat.Core.LowLevelClient.Models;
using StreamChat.Core.StatefulModels;
using StreamChat.Core.Helpers;
using StreamChat.Core.QueryBuilders.Filters;
using StreamChat.Core.QueryBuilders.Filters.Channels;
using StreamChat.Core.QueryBuilders.Sort;
using StreamChat.Libs.Logs;
using StreamChat.SampleProject.Popups;
using StreamChat.SampleProject.Views;
using UnityEngine;
using Object = UnityEngine.Object;

namespace StreamChat.SampleProject
{
    /// <summary>
    /// Implementation of <see cref="IChatState"/>
    /// </summary>
    public class ChatState : IChatState
    {
        public const string MessageDeletedInfo = "This message was deleted...";

        public event Action<IStreamChannel> ActiveChanelChanged;
        public event Action ChannelsUpdated;

        public event Action<IStreamMessage> MessageEditRequested;

        public IStreamChannel ActiveChannel
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

        public IReadOnlyList<IStreamChannel> Channels => _channels;

        public IStreamChatClient Client { get; }

        public ChatState(IStreamChatClient client, IViewFactory viewFactory)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
            _viewFactory = viewFactory ?? throw new ArgumentNullException(nameof(viewFactory));

            Client.Connected += OnClientConnected;
            Client.ConnectionStateChanged += OnClientConnectionStateChanged;
            
            Client.ChannelInviteReceived += OnClientChannelInviteReceived;
            Client.ChannelInviteAccepted += ClientOnChannelInviteAccepted;
            Client.ChannelInviteRejected += ClientOnChannelInviteRejected;

            //StreamTodo: handle this
            // Client.MessageRead += OnMessageRead;
            //
            // Client.NotificationMarkRead += OnNotificationMarkRead;
        }

        public void Dispose()
        {
            Client.Connected -= OnClientConnected;
            Client.ConnectionStateChanged -= OnClientConnectionStateChanged;
            
            Client.ChannelInviteReceived -= OnClientChannelInviteReceived;
            Client.ChannelInviteAccepted -= ClientOnChannelInviteAccepted;
            Client.ChannelInviteRejected -= ClientOnChannelInviteRejected;
            
            // Client.MessageRead -= OnMessageRead;
            //
            // Client.NotificationMarkRead -= OnNotificationMarkRead;

            Client.Dispose();
        }

        public TPopup ShowPopup<TPopup>()
            where TPopup : BaseFullscreenPopup
        {
            return _viewFactory.CreateFullscreenPopup<TPopup>();
        }

        public void HidePopup<TPopup>(TPopup instance)
            where TPopup : BaseFullscreenPopup
        {
            Object.Destroy(instance.gameObject);
        }

        public Task<IStreamChannel> CreateNewChannelAsync(string channelName)
            => Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: Guid.NewGuid().ToString(),
                channelName);

        public void OpenChannel(IStreamChannel channel) => ActiveChannel = channel;

        public void EditMessage(IStreamMessage message) => MessageEditRequested?.Invoke(message);

        public async Task UpdateChannelsAsync()
        {
            var filter = new List<IFieldFilterRule>
            {
                ChannelFilter.Members.In(Client.LocalUserData.User)
            };
            var sort = ChannelSort.OrderByAscending(ChannelSortFieldName.CreatedAt);

            try
            {
                var channels = await Client.QueryChannelsAsync(filter, sort);

                _channels.Clear();
                _channels.AddRange(channels);

                if (ActiveChannel == null)
                {
                    ActiveChannel = _channels.FirstOrDefault();
                }
            }
            catch (StreamApiException e)
            {
                e.LogStreamExceptionDetails();
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

        private readonly List<IStreamChannel> _channels = new List<IStreamChannel>();

        private readonly IViewFactory _viewFactory;
        private readonly ILogs _unityLogger = new UnityLogs();

        //StreamTodo: get it initially from health check event
        private OwnUser _localUser;
        private IStreamChannel _activeChannel;

        private Task _activeLoadPreviousMessagesTask;
        private bool _restoreStateAfterReconnect;

        private async void OnClientConnected(IStreamLocalUserData localUserData)
        {
            await UpdateChannelsAsync();

            if (ActiveChannel == null && _channels.Count > 0)
            {
                ActiveChannel = _channels.First();
            }
        }

        private void OnClientChannelInviteReceived(IStreamChannel channel, IStreamUser invitee)
        {
            var popup = ShowPopup<InviteReceivedPopup>();
            popup.SetData(channel);
        }

        private void ClientOnChannelInviteAccepted(IStreamChannel channel, IStreamUser invitee)
        {
            Debug.LogError("ClientOnChannelInviteAccepted");
        }

        private void ClientOnChannelInviteRejected(IStreamChannel channel, IStreamUser invitee)
        {
            Debug.LogError("ClientOnChannelInviteRejected");
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