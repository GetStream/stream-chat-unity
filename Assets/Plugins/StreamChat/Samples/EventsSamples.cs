using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <summary>
        /// https://getstream.io/chat/docs/unity/event-object/?language=unity#listening-for-events
        /// </summary>
        public async Task ListeningForEvents()
        {
            // 1. Client-level events
            // Fire for the local user, regardless of which channels/messages/etc. are loaded.
            Client.Connected += OnConnected;
            Client.Disconnected += OnDisconnected;
            Client.ConnectionStateChanged += OnConnectionStateChanged;
            Client.AddedToChannelAsMember += OnAddedToChannelAsMember;
            Client.RemovedFromChannelAsMember += OnRemovedFromChannelAsMember;
            Client.ChannelDeleted += OnChannelDeleted;
            Client.ChannelInviteReceived += OnChannelInviteReceived;
            Client.ChannelInviteAccepted += OnChannelInviteAccepted;
            Client.ChannelInviteRejected += OnChannelInviteRejected;
            Client.ThreadTracked += OnThreadTracked;
            Client.ThreadUntracked += OnThreadUntracked;

            // 2. Get a channel (or many) you want to listen on
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, "my-channel-id");

            var channels = await Client.QueryChannelsAsync(new List<IFieldFilterRule>
            {
                ChannelFilter.Members.In(Client.LocalUserData.User)
            });

            // 3. Channel-level events
            // Fire only for channels you are watching (loaded via GetOrCreate* / QueryChannelsAsync).
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

            // 4. Per-message events
            // Reaction events fire on the specific IStreamMessage instance.
            var message = channel.Messages.First();
            message.ReactionAdded += OnReactionAdded;
            message.ReactionUpdated += OnReactionUpdated;
            message.ReactionRemoved += OnReactionRemoved;

            // 5. Per-thread events
            // Load a thread via GetThreadAsync / QueryThreadsAsync (or via a channel that has tracked threads).
            var thread = await Client.GetThreadAsync(message.Id);
            thread.Updated += OnThreadUpdated;
            thread.ReplyReceived += OnThreadReplyReceived;
            thread.ReadStateChanged += OnThreadReadStateChanged;

            // 6. Per-user events
            // Any IStreamUser instance (e.g. from channel.Members) exposes presence updates.
            var user = channel.Members.First().User;
            user.PresenceChanged += OnUserPresenceChanged;

            // 7. Per-poll events
            // Load a poll via Client.Polls.GetPollAsync, or take it from a message that has one attached.
            var poll = await Client.Polls.GetPollAsync("poll-id");
            poll.Closed += OnPollClosed;
            poll.Updated += OnPollUpdated;
            poll.VoteCasted += OnPollVoteCasted;
            poll.VoteChanged += OnPollVoteChanged;
            poll.VoteRemoved += OnPollVoteRemoved;
        }

        // ---- Client-level handlers ----

        private void OnConnected(IStreamLocalUserData localUserData)
        {
        }

        private void OnDisconnected()
        {
        }

        private void OnConnectionStateChanged(ConnectionState previous, ConnectionState current)
        {
        }

        private void OnAddedToChannelAsMember(IStreamChannel channel, IStreamChannelMember member)
        {
            // Fires for channels the local user was just added to and that are not yet watched locally.
            // For watched channels, use channel.MemberAdded instead.
        }

        private void OnRemovedFromChannelAsMember(IStreamChannel channel, IStreamChannelMember member)
        {
            // Fires for channels the local user was just removed from and that are not yet watched locally.
            // For watched channels, use channel.MemberRemoved instead.
        }

        private void OnChannelDeleted(string channelCid, string channelId, ChannelType channelType)
        {
        }

        private void OnChannelInviteReceived(IStreamChannel channel, IStreamUser invitee)
        {
        }

        private void OnChannelInviteAccepted(IStreamChannel channel, IStreamUser invitee)
        {
        }

        private void OnChannelInviteRejected(IStreamChannel channel, IStreamUser invitee)
        {
        }

        private void OnThreadTracked(IStreamThread thread)
        {
            // Fires when an IStreamThread becomes available locally (after GetThreadAsync, QueryThreadsAsync,
            // or when watching a channel that contains threads). Use this to bind per-thread UI.
        }

        private void OnThreadUntracked(IStreamThread thread)
        {
            // Fires when a tracked thread is no longer available (e.g. the parent message was hard-deleted).
        }

        // ---- Channel-level handlers ----

        private void OnMessageReceived(IStreamChannel channel, IStreamMessage message)
        {
        }

        private void OnMessageUpdated(IStreamChannel channel, IStreamMessage message)
        {
        }

        private void OnMessageDeleted(IStreamChannel channel, IStreamMessage message, bool isHardDelete)
        {
        }

        // Reused for both channel.Reaction* and message.Reaction* (same delegate signature).
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

        private void OnVisibilityChanged(IStreamChannel channel, bool isHidden)
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

        // ---- Thread-level handlers ----

        private void OnThreadUpdated(IStreamThread thread)
        {
        }

        private void OnThreadReplyReceived(IStreamThread thread, IStreamMessage reply)
        {
        }

        private void OnThreadReadStateChanged(IStreamThread thread)
        {
        }

        // ---- User-level handlers ----

        private void OnUserPresenceChanged(IStreamUser user, bool isOnline, DateTimeOffset? lastActive)
        {
        }

        // ---- Poll-level handlers ----

        private void OnPollClosed(IStreamPoll poll)
        {
        }

        private void OnPollUpdated(IStreamPoll poll)
        {
        }

        private void OnPollVoteCasted(IStreamPoll poll, StreamPollVote vote)
        {
        }

        private void OnPollVoteChanged(IStreamPoll poll, StreamPollVote vote)
        {
        }

        private void OnPollVoteRemoved(IStreamPoll poll, StreamPollVote vote)
        {
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/event-object/?language=unity#listening-for-events
        /// (the "You can also listen to all events at once" Unity tab)
        /// </summary>
        public void ListenToAllEventsAtOnce()
        {
            // Not supported in the Unity SDK
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/event-object/?language=unity#event-types
        /// </summary>
        public async Task ListenForUserPresenceEvents()
        {
            // Get a channel
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, "my-channel-id");

            // Each user object exposes the PresenceChanged event
            foreach (var member in channel.Members)
            {
                member.User.PresenceChanged += (userObj, isOnline, isActive) =>
                {
                    // Handle presence change
                };
            }
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/event-object/?language=unity#connection-events
        /// </summary>
        public void SubscribeToConnectionEvents()
        {
            Client.Connected += OnConnected;
            Client.Disconnected += OnDisconnected;
            Client.ConnectionStateChanged += OnConnectionStateChanged;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/event-object/?language=unity#stop-listening-for-events
        /// </summary>
        public void Unsubscribe()
        {
            Client.Connected -= OnConnected;
            Client.Disconnected -= OnDisconnected;
            Client.ConnectionStateChanged -= OnConnectionStateChanged;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/event-object/?language=unity#to-a-channel
        /// </summary>
        public void SendCustomEventToChannel()
        {
            // Not yet supported in the Unity SDK
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/event-object/?language=unity#to-a-user
        /// </summary>
        public void SendCustomEventToUser()
        {
            // Not yet supported in the Unity SDK
        }

        private IStreamChatClient Client { get; } = StreamChatClient.CreateDefaultClient();
    }
}
