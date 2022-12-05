using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Core.Models;
using StreamChat.Core.StatefulModels;
using UnityEngine;

namespace StreamChat.Samples
{
    internal sealed class ChannelsCodeSamples
    {
        /// <summary>
        /// https://getstream.io/chat/docs/unity/creating_channels/?language=unity#1.-creating-a-channel-using-a-channel-id
        /// </summary>
        public async Task CreateChannelUsingId()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, "my-channel-id");

// Once you get or query a channel it is also added to Client.WatchedChannels list
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/creating_channels/?language=unreal#2.-creating-a-channel-for-a-list-of-members
        /// </summary>
        public async Task CreateChannelForListOfMembers()
        {
// Find user you want to start a chat with
            var users = await Client.QueryUsersAsync(new Dictionary<string, object>
            {
                {
                    "id", new Dictionary<string, object>
                    {
                        {
                            "$in", new List<string>
                            {
                                "other-user-id"
                            }
                        }
                    }
                }
            });

            var otherUser = users.First();
            var localUser = Client.LocalUserData.User;

// Start direct channel between 2 users
            var channel =
                await Client.GetOrCreateChannelWithMembersAsync(ChannelType.Messaging, new[] { localUser, otherUser });
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/watch_channel/?language=unity#to-start-watching-a-channel
        /// </summary>
        public async Task StartWatchingChannel()
        {
            // find user you want to start chat with
            var users = await Client.QueryUsersAsync(new Dictionary<string, object>
            {
                {
                    "id", new Dictionary<string, object>
                    {
                        {
                            "$in", new List<string>
                            {
                                "other-user-id"
                            }
                        }
                    }
                }
            });

            var otherUser = users.First();
            var localUser = Client.LocalUserData.User;

// Get channel by ID
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");

// Get channel with users combination
            var channelWithUsers
                = await Client.GetOrCreateChannelWithMembersAsync(ChannelType.Messaging,
                    new[] { localUser, otherUser });

// Access properties
            Debug.Log(channel.Name);
            Debug.Log(channel.Members);
            Debug.Log(channel.Name);
            Debug.Log(channel.Name);

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
            throw new System.NotImplementedException();
        }

        private void OnReactionUpdated(IStreamChannel channel, IStreamMessage message, StreamReaction reaction)
        {
            throw new System.NotImplementedException();
        }

        private void OnReactionAdded(IStreamChannel channel, IStreamMessage message, StreamReaction reaction)
        {
            throw new System.NotImplementedException();
        }

        private void OnMessageDeleted(IStreamChannel channel, IStreamMessage message, bool isharddelete)
        {
            throw new System.NotImplementedException();
        }

        private void OnMessageUpdated(IStreamChannel channel, IStreamMessage message)
        {
            throw new System.NotImplementedException();
        }

        private void OnMessageReceived(IStreamChannel channel, IStreamMessage message)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/watch_channel/?language=unity#watching-multiple-channels
        /// </summary>
        public async Task WatchingMultipleChannels()
        {
            var localUser = Client.LocalUserData.User;

// Get channels where local user is a member of
            var channels = await Client.QueryChannelsAsync(new Dictionary<string, object>
            {
                {
                    "members", new Dictionary<string, object>
                    {
                        { "$in", new[] { localUser.Id } }
                    }
                }
            });

// Get all currently watched channels
            foreach (var channel in channels)
            {
                // Access properties
                Debug.Log(channel.Name);
                Debug.Log(channel.Members);
                Debug.Log(channel.Name);
                Debug.Log(channel.Name);

                // Subscribe to events so you can react to updates
                channel.MessageReceived += OnMessageReceived;
                channel.MessageUpdated += OnMessageUpdated;
                channel.MessageDeleted += OnMessageDeleted;
                channel.ReactionAdded += OnReactionAdded;
                channel.ReactionUpdated += OnReactionUpdated;
                channel.ReactionRemoved += OnReactionRemoved;
            }
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/watch_channel/?language=unity#stop-watching-a-channel
        /// </summary>
        public async Task StopWatchingChannel()
        {
            var channel = Client.WatchedChannels.First();

            await channel.StopWatchingAsync();
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/watch_channel/?language=unity#watcher-count
        /// </summary>
        public async Task WatcherCount()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");

            Debug.Log(channel.WatcherCount);
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/watch_channel/?language=unity#paginating-channel-watchers-with-channel.query
        /// </summary>
        public async Task PaginateChannelWatchers()
        {
            //StreamTodo: IMPLEMENT watchers pagination
            await Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/watch_channel/?language=unity#listening-to-changes-in-watchers
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
                    "Legends",
                }
            };

            var setFields = new Dictionary<string, object>();

// Set custom values
            setFields.Add("owned_dogs", 5);
// Set custom arrays
            setFields.Add("breakfast", new[] { "donuts" });
// Set custom class objects
            setFields.Add("clan_info", setClanInfo);

// Send data
            await channel.UpdatePartialAsync(setFields);

// Data is now available via CustomData property
            var ownedDogs = channel.CustomData.Get<int>("owned_dogs");
            var breakfast = channel.CustomData.Get<List<string>>("breakfast");
            var clanInfo = channel.CustomData.Get<ClanData>("clan_info");
        }

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
//StreamTodo: IMPLEMENT channel full update
            await Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/channel_members/?language=unity
        /// </summary>
        public async Task AddingAndRemovingChannelMembers()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");

            var users = await Client.QueryUsersAsync(new Dictionary<string, object>
            {
                {
                    "id", new Dictionary<string, object>
                    {
                        {
                            "$in", new List<string>
                            {
                                "other-user-id-1",
                                "other-user-id-2",
                                "other-user-id-3"
                            }
                        }
                    }
                }
            });

// Add user as a member
            await channel.AddMembersAsync(users);

// Access channel members via channel.Members, let's remove the first member as an example
            var member = channel.Members.First();
            await channel.RemoveMembersAsync(member);
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/channel_members/?language=unity#hide-history
        /// </summary>
        public async Task AddMembersAndHideHistory()
        {
//StreamTodo: IMPLEMENT add members and hide history
            await Task.CompletedTask;
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

        /// <summary>
        /// https://getstream.io/chat/docs/unity/query_channels/?language=unity
        /// </summary>
        public async Task QueryChannels()
        {
            var filters = new Dictionary<string, object>
            {
                {
                    // Get channels that contain a member user with any of the provided ids
                    "members", new Dictionary<string, object>
                    {
                        // you can provide multiple ids
                        { "$in", new[] { Client.LocalUserData.UserId } }
                    }

                    // You can query by many other fields
                }
            };

            var channels = await Client.QueryChannelsAsync(filters);
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/query_channels/?language=unity#messaging-and-team
        /// </summary>
        public async Task MessagingAndTeam()
        {
            var filters = new Dictionary<string, object>
            {
                {
                    // Get channels that contain a member user with any of the provided ids
                    "members", new Dictionary<string, object>
                    {
                        // you can provide multiple ids
                        { "$in", new[] { Client.LocalUserData.UserId } }
                    }

                    // You can query by many other fields
                }
            };

            var channels = await Client.QueryChannelsAsync(filters);
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/query_channels/?language=unity#support
        /// </summary>
        public async Task Support()
        {
            //StreamTodo: IMPLEMENT query support filter example
            await Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/query_channels/?language=unity#pagination
        /// </summary>
        public async Task QueryChannelsPagination()
        {
            var filters = new Dictionary<string, object>
            {
                {
                    // Get channels that contain a member user with any of the provided ids
                    "members", new Dictionary<string, object>
                    {
                        // you can provide multiple ids
                        { "$in", new[] { Client.LocalUserData.UserId } }
                    }

                    // You can query by many other fields
                }
            };

// Pass limit and offset to control the page or results returned
// Limit - how many records per page
// Offset - how many records to skip
            var channels = await Client.QueryChannelsAsync(filters, limit: 30, offset: 60);
        }

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
            var membersResult = await channel.QueryMembers(filters, limit: 30, offset: 0);
        }

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
        /// https://getstream.io/chat/docs/unity/channel_capabilities/?language=unity
        /// </summary>
        public async Task ChannelCapabilities()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");

            if (channel.OwnCapabilities.Contains("update-own-message"))
            {
                // User can update own message
            }

            // Check action keys here https://getstream.io/chat/docs/unity/permissions_reference/?language=unity
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/channel_invites/?language=unity
        /// </summary>
        public async Task Invite()
        {
            //StreamTodo: IMPLEMENT send invite
            await Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/channel_invites/?language=unity#accepting-an-invite
        /// </summary>
        public async Task AcceptInvite()
        {
            //StreamTodo: IMPLEMENT accept invite
            await Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/channel_invites/?language=unity#rejecting-an-invite
        /// </summary>
        public async Task RejectInvite()
        {
            //StreamTodo: IMPLEMENT reject invite
            await Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/channel_invites/?language=unity#query-for-accepted-invites
        /// </summary>
        public async Task QueryAcceptedInvites()
        {
            //StreamTodo: IMPLEMENT query accepted invites
            await Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/channel_invites/?language=unity#query-for-rejected-invites
        /// </summary>
        public async Task QueryRejectedInvites()
        {
            //StreamTodo: IMPLEMENT query rejected invites
            await Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/channel_invites/?language=unity#query-for-pending-invites
        /// </summary>
        public async Task QueryPendingInvites()
        {
            //StreamTodo: IMPLEMENT query pending invites
            await Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/muting_channels/?language=unity#muting-channels
        /// </summary>
        public async Task MuteChannel()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");
            await channel.MuteChannelAsync();
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/muting_channels/?language=unity#query-muted-channels
        /// </summary>
        public async Task QueryMutedChannels()
        {
            //StreamTodo: IMPLEMENT query muted channels
            await Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/muting_channels/?language=unity#remove-a-channel-mute
        /// </summary>
        public async Task RemoveChannelMute()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");
            await channel.UnmuteChannelAsync();
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/muting_channels/?language=unity#hiding-a-channel
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
            //StreamTodo: IMPLEMENT disable channel
            await Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/disabling_channels/?language=unity#freeze-a-channel
        /// </summary>
        public async Task FreezeChannel()
        {
            //StreamTodo: IMPLEMENT freeze channel
            await Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/disabling_channels/?language=unity#unfreeze-a-channel
        /// </summary>
        public async Task UnfreezeChannel()
        {
            //StreamTodo: IMPLEMENT unfreeze channel
            await Task.CompletedTask;
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
        /// https://getstream.io/chat/docs/unity/slow_mode/?language=unity
        /// </summary>
        public async Task ThrottleAndSlowMode()
        {
            //StreamTodo: IMPLEMENT Throttle and slow mode
            await Task.CompletedTask;
        }

        private IStreamChatClient Client { get; } = StreamChatClient.CreateDefaultClient();
    }
}