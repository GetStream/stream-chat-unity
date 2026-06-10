using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Core.Models;
using StreamChat.Core.QueryBuilders.Filters;
using StreamChat.Core.QueryBuilders.Filters.Channels;
using StreamChat.Core.QueryBuilders.Filters.Users;
using StreamChat.Core.QueryBuilders.Sort;
using StreamChat.Core.Requests;
using StreamChat.Core.StatefulModels;
using UnityEngine;

namespace StreamChat.Samples
{
    internal sealed class ChannelsCodeSamples
    {
        #region Creating Channels

        /// <summary>
        /// https://getstream.io/chat/docs/unity/creating_channels/?language=unity#1.-creating-a-channel-using-a-channel-id
        /// </summary>
        public async Task CreateChannelUsingId()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, "my-channel-id");

// Once you get or query a channel it is also added to Client.WatchedChannels list
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/creating_channels/?language=unity#2.-creating-a-channel-for-a-list-of-members
        /// </summary>
        public async Task CreateChannelForListOfMembers()
        {
            var filters = new IFieldFilterRule[]
            {
                UserFilter.Id.EqualsTo("other-user-id")
            };
// Find user you want to start a chat with
            var users = await Client.QueryUsersAsync(filters);

            var otherUser = users.First();
            var localUser = Client.LocalUserData.User;

// Start direct channel between 2 users
            var channel = await Client.GetOrCreateChannelWithMembersAsync(ChannelType.Messaging,
                new[] { localUser, otherUser });
        }

        #endregion

        #region Watch a channel

        /// <summary>
        /// https://getstream.io/chat/docs/unity/creating-channels/?language=unity#watching-channels
        /// </summary>
        public async Task StartWatchingChannel()
        {
            var filters = new IFieldFilterRule[]
            {
                UserFilter.Id.EqualsTo("other-user-id")
            };

// Find user you want to start chat with
            var users = await Client.QueryUsersAsync(filters);
            var otherUser = users.First();
            var localUser = Client.LocalUserData.User;

// Get channel by ID
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");

// Get channel with users combination
            var channelWithUsers = await Client.GetOrCreateChannelWithMembersAsync(ChannelType.Messaging,
                new[] { localUser, otherUser });

// Access properties
            Debug.Log(channel.Name);
            Debug.Log(channel.Members);

// Subscribe to events so you can react to updates
            channel.MessageReceived += OnMessageReceived;
            channel.MessageUpdated += OnMessageUpdated;
            channel.MessageDeleted += OnMessageDeleted;
            channel.ReactionAdded += OnReactionAdded;
            channel.ReactionUpdated += OnReactionUpdated;
            channel.ReactionRemoved += OnReactionRemoved;

// You can also access all currently watched channels via
            foreach (var c in Client.WatchedChannels)
            {
                // Every queried channel is automatically watched and starts receiving updates from the server
            }
        }

        private void OnReactionRemoved(IStreamChannel channel, IStreamMessage message, StreamReaction reaction)
        {
        }

        private void OnReactionUpdated(IStreamChannel channel, IStreamMessage message, StreamReaction reaction)
        {
        }

        private void OnReactionAdded(IStreamChannel channel, IStreamMessage message, StreamReaction reaction)
        {
        }

        private void OnMessageDeleted(IStreamChannel channel, IStreamMessage message, bool isharddelete)
        {
        }

        private void OnMessageUpdated(IStreamChannel channel, IStreamMessage message)
        {
        }

        private void OnMessageReceived(IStreamChannel channel, IStreamMessage message)
        {
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/query-channels/?language=unity#channel-creation-and-watching
        /// </summary>
        public async Task WatchingMultipleChannels()
        {
            var filters = new IFieldFilterRule[]
            {
                // Get channels where local user is a member
                ChannelFilter.Members.In(Client.LocalUserData.UserId)
            };

// Get channel state or create one if it doesn't exist
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, "general");

// Or get multiple channels matching a filter
            var channels = await Client.QueryChannelsAsync(filters);

            // Subscribe to per-channel events as needed
            foreach (var c in channels)
            {
                c.MessageReceived += OnMessageReceived;
                c.MessageUpdated += OnMessageUpdated;
                c.MessageDeleted += OnMessageDeleted;
                c.ReactionAdded += OnReactionAdded;
                c.ReactionUpdated += OnReactionUpdated;
                c.ReactionRemoved += OnReactionRemoved;
            }
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/query-channels/?language=unity#sort-best-practices
        /// </summary>
        public async Task WatchingMultipleChannels2()
        {
            var filters = new IFieldFilterRule[]
            {
                // Get channels where local user is a member
                ChannelFilter.Members.In(Client.LocalUserData.UserId)
            };

            var sort = ChannelSort.OrderByDescending(ChannelSortFieldName.LastMessageAt);
            var channels = await Client.QueryChannelsAsync(filters, sort);
        }

        /// <summary>
        /// Code-only reference: no live Unity docs page demonstrates chained
        /// channel sort (<c>ThenByDescending</c>) for any SDK, so this snippet
        /// has no published home. Kept as a code-only example of how to chain
        /// multiple sort fields against <see cref="IStreamChatClient.QueryChannelsAsync"/>.
        /// </summary>
        public async Task WatchingMultipleChannels3()
        {
            var filters = new IFieldFilterRule[]
            {
                // Get channels where local user is a member
                ChannelFilter.Members.In(Client.LocalUserData.UserId)
            };

            // You can sort by multiple fields and chain as many ThenByDescending as you need
            var sort = ChannelSort.OrderByDescending(ChannelSortFieldName.MemberCount)
                .ThenByDescending(ChannelSortFieldName.CreatedAt);

            var channels = await Client.QueryChannelsAsync(filters, sort);
        }

        /// <summary>
        /// Code-only reference: <c>creating-channels/?language=unity</c> and
        /// <c>query-channels/?language=unity</c> (the live targets of the old
        /// <c>watch_channel/</c> redirect) do not expose a "Stop watching a
        /// channel" section for any SDK, so this snippet has no published home.
        /// Kept as a code-only example of <see cref="IStreamChannel.StopWatchingAsync"/>.
        /// </summary>
        public async Task StopWatchingChannel()
        {
            var channel = Client.WatchedChannels.First();

            await channel.StopWatchingAsync();
        }

        /// <summary>
        /// Code-only reference: <c>creating-channels/?language=unity</c> and
        /// <c>query-channels/?language=unity</c> (the live targets of the old
        /// <c>watch_channel/</c> redirect) do not surface a "Watcher count"
        /// section for any SDK. Kept as a code-only example of how to read
        /// <see cref="IStreamChannel.WatcherCount"/>.
        /// </summary>
        public async Task WatcherCount()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");

            Debug.Log(channel.WatcherCount);
        }

        /// <summary>
        /// Code-only reference: the old <c>watch_channel/</c> page exposed
        /// "Listening to changes in watchers" but the live targets of its
        /// redirect (<c>creating-channels/</c>, <c>query-channels/</c>) do not
        /// host a watcher-events section for any SDK. Kept as a code-only
        /// example of <see cref="IStreamChannel.WatcherAdded"/> /
        /// <see cref="IStreamChannel.WatcherRemoved"/>.
        /// </summary>
        public async Task ListenToChangesInWatchers()
        {
            // Get IStreamChannel reference by Client.GetOrCreateChannel or Client.QueryChannels
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");

            // Subscribe to events
            channel.WatcherAdded += OnWatcherAdded;
            channel.WatcherRemoved += OnWatcherRemoved;
        }

        private void OnWatcherAdded(IStreamChannel channel, IStreamUser user)
        {
        }

        private void OnWatcherRemoved(IStreamChannel channel, IStreamUser user)
        {
        }

        #endregion

        #region Updating a Channel

        /// <summary>
        /// https://getstream.io/chat/docs/unity/channel_update/?language=unity#partial-update
        /// </summary>
        public async Task PartialUpdate()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");

            var setClanInfo = new ClanData
            {
                MaxMembers = 50,
                Name = "Wild Boards",
                Tags = new List<string>
                {
                    "Competitive",
                    "Legendary",
                }
            };

            var setFields = new Dictionary<string, object>();

            // Set custom values
            setFields.Add("frags", 5);
            // Set custom arrays
            setFields.Add("items", new[] { "sword", "shield" });
            // Set custom class objects
            setFields.Add("clan_info", setClanInfo);

            // Send data
            await channel.UpdatePartialAsync(setFields);

            // Data is now available via CustomData property
            var frags = channel.CustomData.Get<int>("frags");
            var items = channel.CustomData.Get<List<string>>("items");
            var clanInfo = channel.CustomData.Get<ClanData>("clan_info");
        }

// Example class with data that you can assign as Channel custom data
        private class ClanData
        {
            public int MaxMembers;
            public string Name;
            public List<string> Tags;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/channel_update/?language=unity#full-update-(overwrite)
        /// </summary>
        public async Task FullUpdate()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");

            var updateRequest = new StreamUpdateOverwriteChannelRequest
            {
                Name = "New name",
                CustomData = new StreamCustomDataRequest
                {
                    { "my-custom-int", 12 },
                    { "my-custom-array", new string[] { "one", "two" } }
                }
            };

// This request will have all channel data removed except what is being passed in the request
            await channel.UpdateOverwriteAsync(updateRequest);

// You can also pass an instance of channel to the request constructor to have all of the date copied over
// This way you alter only the fields you wish to change
            var updateRequest2 = new StreamUpdateOverwriteChannelRequest(channel)
            {
                Name = "Bran new name"
            };

// This will update only the name because all other data was copied over
            await channel.UpdateOverwriteAsync(updateRequest2);
        }

        #endregion

        #region Updating Channel Members

        /// <summary>
        /// https://getstream.io/chat/docs/unity/channel_members/?language=unity
        /// </summary>
        public async Task AddingAndRemovingChannelMembers()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");

            var filters = new IFieldFilterRule[]
            {
                UserFilter.Id.In("other-user-id-1", "other-user-id-2", "other-user-id-3")
            };

            var users = await Client.QueryUsersAsync(filters);

// Add IStreamUser collection as a members
            await channel.AddMembersAsync(users);

// Or add by ID
            await channel.AddMembersAsync(hideHistory: default, optionalMessage: default, "some-user-id-1",
                "some-user-id-2");

// Access channel members via channel.Members, let's remove the first member as an example
            var member = channel.Members.First();
            await channel.RemoveMembersAsync(member);

// Remove local user from a channel by user ID
            var localUser = Client.LocalUserData.User;
            await channel.RemoveMembersAsync(localUser.Id);

// Remove multiple users by their ID
            await channel.RemoveMembersAsync("some-user-id-1", "some-user-id-2");
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/channel_members/?language=csharp#message-parameter
        /// </summary>
        public async Task AddMembersWithMessage()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");

            var filters = new IFieldFilterRule[]
            {
                UserFilter.Id.In("other-user-id-1", "other-user-id-2", "other-user-id-3")
            };

            var users = await Client.QueryUsersAsync(filters);

            await channel.AddMembersAsync(users, hideHistory: default, new StreamMessageRequest
            {
                Text = "John has joined the channel"
            });
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/channel_members/?language=unity#hide-history
        /// </summary>
        public async Task AddMembersAndHideHistory()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");

            var filters = new IFieldFilterRule[]
            {
                UserFilter.Id.In("other-user-id-1", "other-user-id-2", "other-user-id-3")
            };

            var users = await Client.QueryUsersAsync(filters);

            await channel.AddMembersAsync(users, hideHistory: true);
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/channel_members/?language=unity#leaving-a-channel
        /// </summary>
        public async Task LeaveChannel()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");
            var member = channel.Members.First();

            await channel.RemoveMembersAsync(member);
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/channel_members/?language=unity#adding-removing-moderators-to-a-channel
        /// </summary>
        public async Task AddAndRemoveModeratorsToChanel()
        {
            // Only Server-side SDK
            await Task.CompletedTask;
        }

        #endregion

        #region Querying Channels

        /// <summary>
        /// https://getstream.io/chat/docs/unity/query-channels/?language=unity
        /// </summary>
        public async Task QueryChannels()
        {
            var filters = new List<IFieldFilterRule>
            {
                // Return only channels where local user is a member
                ChannelFilter.Members.In(Client.LocalUserData.UserId),

                // You can define multiple filters that will all have to be satisfied
            };

            var channels = await Client.QueryChannelsAsync(filters);
        }

        /// <summary>
        /// Code-only reference: <c>query-channels/?language=unity</c> only
        /// surfaces the single-filter <c>Members.In(...)</c> example (covered
        /// by <see cref="QueryChannels"/>); the extended
        /// <c>Members + Muted + MembersCount</c> filter set is not published
        /// for any SDK. Kept as a code-only example of combining multiple
        /// <see cref="ChannelFilter"/> rules in a single query.
        /// </summary>
        public async Task QueryChannelsExtended()
        {
// Get channels where local user is a member AND channel is not muted AND channel has more than 10 members
            var filters = new List<IFieldFilterRule>
            {
                ChannelFilter.Members.In(Client.LocalUserData.UserId),
                ChannelFilter.Muted.EqualsTo(false),
                ChannelFilter.MembersCount.GreaterThan(10)
            };

            var channels = await Client.QueryChannelsAsync(filters);
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/query-channels/?language=unity#messaging-and-team-channels
        /// </summary>
        public async Task MessagingAndTeam()
        {
            var filters = new List<IFieldFilterRule>
            {
                ChannelFilter.Type.EqualsTo(ChannelType.Messaging),
                ChannelFilter.Members.In(Client.LocalUserData.UserId),
            };

            var channels = await Client.QueryChannelsAsync(filters);
        }

        /// <summary>
        /// Code-only reference: <c>query-channels/?language=unity</c> does
        /// not host a "Support" / customer-service filter example for any
        /// SDK (the <c>#support</c> anchor on the old <c>query_channels/</c>
        /// page no longer exists). Kept as a code-only example of querying
        /// channels by custom fields via <see cref="ChannelFilter.Custom(string)"/>.
        /// </summary>
        public async Task Support()
        {
            var filters = new List<IFieldFilterRule>
            {
                // Return only channels where local user is a member
                ChannelFilter.Custom("agent_id").EqualsTo(Client.LocalUserData.UserId),
                ChannelFilter.Custom("status").In("pending", "open", "new")
            };

            var channels = await Client.QueryChannelsAsync(filters);
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/query-channels/?language=unity#pagination
        /// </summary>
        public async Task QueryChannelsPagination()
        {
            var filters = new List<IFieldFilterRule>
            {
                // Return only channels where local user is a member
                ChannelFilter.Members.In(Client.LocalUserData.UserId),
            };

// Pass limit and offset to control pagination
// Limit - records per page
// Offset - records to skip
            var channels = await Client.QueryChannelsAsync(filters, limit: 30, offset: 60);
        }

        #endregion

        #region Querying Members

        /// <summary>
        /// https://getstream.io/chat/docs/unity/query_members/?language=unity#pagination-and-ordering
        /// </summary>
        public async Task QueryMembers()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");

            var filters = new Dictionary<string, object>
            {
                {
                    "id", new Dictionary<string, object>
                    {
                        { "$in", new[] { "user-id-1", "user-id-2" } }
                    }
                }
            };

// Pass limit and offset to control the page or results returned
// Limit - how many records per page
// Offset - how many records to skip
            var membersResult = await channel.QueryMembersAsync(filters, limit: 30, offset: 0);
        }

        #endregion

        #region Channel Pagination

        /// <summary>
        /// https://getstream.io/chat/docs/unity/channel_pagination/?language=unity
        /// </summary>
        public async Task ChannelPaginateMessages()
        {
// Channel is loaded with the most recent messages
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");

// Every call will load 1 more page of messages
            await channel.LoadOlderMessagesAsync();
        }

        #endregion

        /// <summary>
        /// https://getstream.io/chat/docs/unity/channel_pagination/?language=unity
        /// </summary>
        public async Task ChannelPaginateMembers()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");

            // StreamTodo: IMPLEMENT channel members pagination
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/channel_pagination/?language=unity
        /// </summary>
        public async Task ChannelPaginateWatchers()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");

            // StreamTodo: IMPLEMENT channel watchers pagination
        }

        /// <summary>
        /// Code-only reference: there is no Unity tab on
        /// https://getstream.io/chat/docs/unity/chat-permission-policies/?language=unity
        /// (or any other live docs page) that demonstrates reading
        /// <see cref="IStreamChannel.OwnCapabilities"/>. The capability strings
        /// are listed on https://getstream.io/chat/docs/unity/permissions-reference/?language=unity.
        /// </summary>
        public async Task ChannelCapabilities()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");

            if (channel.OwnCapabilities.Contains("update-own-message"))
            {
                // User can update own message
            }
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/channel_invites/?language=unity
        /// </summary>
        public async Task Invite()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");

            var filters = new IFieldFilterRule[]
            {
                UserFilter.Id.In("other-user-id-1", "other-user-id-2", "other-user-id-3")
            };

            var users = await Client.QueryUsersAsync(filters);

// Invite IStreamUser collection as new members
            await channel.InviteMembersAsync(users);

// Or add by ID
            await channel.InviteMembersAsync("some-user-id-1", "some-user-id-2");

            //StreamTodo: IMPLEMENT send invite
            await Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/channel_invites/?language=unity#accepting-an-invite
        /// </summary>
        public async Task AcceptInvite()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");
            await channel.AcceptInviteAsync();
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/channel_invites/?language=unity#rejecting-an-invite
        /// </summary>
        public async Task RejectInvite()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");
            await channel.RejectInviteAsync();
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/channel_invites/?language=unity#query-for-accepted-invites
        /// </summary>
        public async Task QueryAcceptedInvites()
        {
            var filter = new List<IFieldFilterRule>
            {
                ChannelFilter.Invite.EqualsTo(ChannelFieldInvite.Status.Accepted)
            };

            var sort = ChannelSort.OrderByDescending(ChannelSortFieldName.LastMessageAt);

            var acceptedInvites = await Client.QueryChannelsAsync(filter, sort);
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/channel_invites/?language=unity#query-for-rejected-invites
        /// </summary>
        public async Task QueryRejectedInvites()
        {
            var filter = new List<IFieldFilterRule>
            {
                ChannelFilter.Invite.EqualsTo(ChannelFieldInvite.Status.Rejected)
            };

            var sort = ChannelSort.OrderByDescending(ChannelSortFieldName.LastMessageAt);

            var rejectedInvites = await Client.QueryChannelsAsync(filter, sort);
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/channel_invites/?language=unity#query-for-pending-invites
        /// </summary>
        public async Task QueryPendingInvites()
        {
            var filter = new List<IFieldFilterRule>
            {
                ChannelFilter.Invite.EqualsTo(ChannelFieldInvite.Status.Pending)
            };

            var sort = ChannelSort.OrderByDescending(ChannelSortFieldName.LastMessageAt);

            var pendingInvites = await Client.QueryChannelsAsync(filter, sort);
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/muting-channels/?language=unity#mute-a-channel
        /// </summary>
        public async Task MuteChannel()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");

// Mute a channel for the current user
            await channel.MuteChannelAsync();

// Mute a channel for 2 weeks (duration in milliseconds)
            await channel.MuteChannelAsync(milliseconds: 14 * 24 * 60 * 60 * 1000);

// Mute a channel for 10 seconds
            await channel.MuteChannelAsync(milliseconds: 10000);

// The list of channel mutes (with expiration times) is available after connect
            var channelMutes = Client.LocalUserData.ChannelMutes;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/muting-channels/?language=unity#query-muted-channels
        /// </summary>
        public async Task QueryMutedChannels()
        {
// Retrieve all channels excluding muted ones
            var notMutedFilters = new IFieldFilterRule[]
            {
                ChannelFilter.Members.In(Client.LocalUserData.UserId),
                ChannelFilter.Muted.EqualsTo(false),
            };
            var notMutedChannels = await Client.QueryChannelsAsync(notMutedFilters);

// Retrieve all muted channels
            var mutedFilters = new IFieldFilterRule[]
            {
                ChannelFilter.Muted.EqualsTo(true),
            };
            var mutedChannels = await Client.QueryChannelsAsync(mutedFilters);
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/muting-channels/?language=unity#remove-a-channel-mute
        /// </summary>
        public async Task RemoveChannelMute()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");
            await channel.UnmuteChannelAsync();
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/hiding-channels/?language=unity#hide-a-channel
        /// </summary>
        public async Task HideAndShowChannel()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");

// Hide a channel
            await channel.HideAsync();

// Show previously hidden channel
            await channel.ShowAsync();
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/disabling_channels/?language=unity#disable-a-channel
        /// </summary>
        public async Task DisableChannel()
        {
            // Feature only available via a server-side SDK
            await Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/disabling_channels/?language=unity#freeze-a-channel
        /// </summary>
        public async Task FreezeChannel()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");
            await channel.FreezeAsync();
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/disabling_channels/?language=unity#unfreeze-a-channel
        /// </summary>
        public async Task UnfreezeChannel()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");
            await channel.UnfreezeAsync();
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/disabling_channels/?language=unity#granting-the-frozen-channel-permissions
        /// </summary>
        public async Task GrantingTheFrozenChannelPermission()
        {
            //StreamTodo: IMPLEMENT granting frozen channel permissions
            await Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/channel_delete/?language=unity#deleting-a-channel
        /// </summary>
        public async Task DeleteChannel()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");

// In Unity SDK you can only soft delete the channel. If you wish to hard delete you can only do it with server-side SDK
            await channel.DeleteAsync();
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/channel_delete/?language=unity#deleting-many-channels
        /// </summary>
        public async Task DeleteManyChannels()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");
            var channel2
                = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id-2");

// Hard delete removes the channel entirely from the database while soft delete removes the from users to see but it's still accessible via server-side SDK as an archive
            await Client.DeleteMultipleChannelsAsync(new[] { channel, channel2 }, isHardDelete: true);
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/truncate_channel/?language=unity
        /// </summary>
        public async Task TruncateChannel()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");


            await channel.TruncateAsync(systemMessage: "Clearing up the history!");
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/slow-mode/?language=unity#channel-slow-mode
        /// </summary>
        public async Task EnableDisableSlowMode()
        {
// This feature is not yet available in the Unity SDK.
// Please let us know if you'd like this feature implemented: https://github.com/GetStream/stream-chat-unity/issues
            await Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/slow-mode/?language=unity#channel-slow-mode
        /// </summary>
        public async Task ReadChannelCooldown()
        {
// This feature is not yet available in the Unity SDK.
// Please let us know if you'd like this feature implemented: https://github.com/GetStream/stream-chat-unity/issues
            await Task.CompletedTask;
        }

        private IStreamChatClient Client { get; } = StreamChatClient.CreateDefaultClient();
    }
}