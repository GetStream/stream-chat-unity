using System.Collections.Generic;
using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Core.Models;
using StreamChat.Core.QueryBuilders.Filters;
using StreamChat.Core.QueryBuilders.Filters.Channels;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Samples
{
    internal class EventsSamples
    {
        public async Task QueryChannelsEvents()
        {
            // Get a single channel
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, "my-channel-id");

            // Or multiple with optional filters
            var channels = await Client.QueryChannelsAsync(new List<IFieldFilterRule>()
            {
                ChannelFilter.Members.In(Client.LocalUserData.User)
            });

            // Subscribe to events
            channel.MessageReceived += OnMessageReceived;
            channel.MessageUpdated += OnMessageUpdated;
            channel.MessageDeleted += OnMessageDeleted;

            channel.ReactionAdded += OnReactionAdded;
            channel.ReactionUpdated += OnReactionUpdated;
            channel.ReactionRemoved += OnReactionRemoved;

            channel.MemberAdded += OnMemberAdded;
            channel.MemberRemoved += OnMemberRemoved;
            channel.MemberUpdated += OnMemberUpdated;

            channel.MembersChanged += OnMembersChanged;
            channel.VisibilityChanged += OnVisibilityChanged;
            channel.MuteChanged += OnMuteChanged;
            channel.Truncated += OnTruncated;
            channel.Updated += OnUpdated;
            channel.WatcherAdded += OnWatcherAdded;
            channel.WatcherRemoved += OnWatcherRemoved;
            channel.UserStartedTyping += OnUserStartedTyping;
            channel.UserStoppedTyping += OnUserStoppedTyping;
            channel.TypingUsersChanged += OnTypingUsersChanged;
        }

        private void OnMessageReceived(IStreamChannel channel, IStreamMessage message)
        {
        }

        private void OnMessageUpdated(IStreamChannel channel, IStreamMessage message)
        {
        }

        private void OnMessageDeleted(IStreamChannel channel, IStreamMessage message, bool isharddelete)
        {
        }

        private void OnReactionAdded(IStreamChannel channel, IStreamMessage message, StreamReaction reaction)
        {
        }

        private void OnReactionUpdated(IStreamChannel channel, IStreamMessage message, StreamReaction reaction)
        {
        }

        private void OnReactionRemoved(IStreamChannel channel, IStreamMessage message, StreamReaction reaction)
        {
        }

        private void OnMemberAdded(IStreamChannel channel, IStreamChannelMember member)
        {
        }

        private void OnMemberRemoved(IStreamChannel channel, IStreamChannelMember member)
        {
        }

        private void OnMemberUpdated(IStreamChannel channel, IStreamChannelMember member)
        {
        }

        private void OnMembersChanged(IStreamChannel channel, IStreamChannelMember member, OperationType operationType)
        {
        }

        private void OnVisibilityChanged(IStreamChannel channel, bool isVisible)
        {
        }

        private void OnMuteChanged(IStreamChannel channel, bool isMuted)
        {
        }

        private void OnTruncated(IStreamChannel channel)
        {
        }

        private void OnUpdated(IStreamChannel channel)
        {
        }

        private void OnWatcherAdded(IStreamChannel channel, IStreamUser user)
        {
        }

        private void OnWatcherRemoved(IStreamChannel channel, IStreamUser user)
        {
        }

        private void OnUserStartedTyping(IStreamChannel channel, IStreamUser user)
        {
        }

        private void OnUserStoppedTyping(IStreamChannel channel, IStreamUser user)
        {
        }

        private void OnTypingUsersChanged(IStreamChannel channel)
        {
        }

        public void SubscribeToClientEvents()
        {
            Client.AddedToChannelAsMember += OnAddedToChannelAsMember;
            Client.RemovedFromChannelAsMember += OnRemovedFromChannel;
        }

        private void OnAddedToChannelAsMember(IStreamChannel channel, IStreamChannelMember member)
        {
            // channel - new channel to which local user was just added
            // member - object containing channel membership information
        }

        private void OnRemovedFromChannel(IStreamChannel channel, IStreamChannelMember member)
        {
            // channel - channel from which local user was removed
            // member - object containing channel membership information
        }

        public void SubscribeToConnectionEvents()
        {
            Client.Connected += OnConnected;
            Client.Disconnected += OnDisconnected;
            Client.ConnectionStateChanged += OnConnectionStateChanged;
        }

        private void OnConnected(IStreamLocalUserData localUserData)
        {
        }

        private void OnDisconnected()
        {
        }

        private void OnConnectionStateChanged(ConnectionState previous, ConnectionState current)
        {
        }

        public void Unsubscribe()
        {
            Client.Connected -= OnConnected;
            Client.Disconnected -= OnDisconnected;
            Client.ConnectionStateChanged -= OnConnectionStateChanged;
        }

        private IStreamChatClient Client { get; } = StreamChatClient.CreateDefaultClient();
    }
}