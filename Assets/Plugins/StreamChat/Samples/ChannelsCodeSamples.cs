using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StreamChat.Core;
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

            //Once you obtain a channel from the server it is also added to Client.WatchedChannels list
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/creating_channels/?language=unreal#2.-creating-a-channel-for-a-list-of-members
        /// </summary>
        public async Task CreateChannelForListOfMembers()
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

            // start direct channel between 2 users
            var channel =
                await Client.GetOrCreateChannelWithMembersAsync(ChannelType.Messaging, new[] {localUser, otherUser});
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
            var channelWithUsers =
                await Client.GetOrCreateChannelWithMembersAsync(ChannelType.Messaging, new[] {localUser, otherUser});

            // Get all currently watched channels
            foreach (var watchedChannel in Client.WatchedChannels)
            {
            }
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
                        {"$in", new[] {localUser.Id}}
                    }
                }
            });

            // Get all currently watched channels
            foreach (var watchedChannel in Client.WatchedChannels)
            {
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
            //StreamTodo: IMPLEMENT listen to changes in watchers
            await Task.CompletedTask;
        }

        private class ClanData
        {
            public int MaxMembers;
            public string Name;
            public List<string> Tags;
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
            setFields.Add("breakfast", new[] {"donuts"});
            // Set custom class objects
            setFields.Add("clan_info", setClanInfo);

            // Send data
            await channel.UpdatePartialAsync(setFields);

            // Data is now available via CustomData property
            var ownedDogs = channel.CustomData.Get<int>("owned_dogs");
            var breakfast = channel.CustomData.Get<List<string>>("breakfast");
            var clanInfo = channel.CustomData.Get<ClanData>("clan_info");
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

            await channel.AddMembersAsync(users);

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
            var memberToRemove = channel.Members.First();

            await channel.RemoveMembersAsync(memberToRemove);
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
                    //Return on channels that contain a member user with any of the provided ids
                    "members", new Dictionary<string, object>
                    {
                        //you can provide multiple ids
                        {"$in", new[] {Client.LocalUserData.UserId}}
                    }
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
                    //Return on channels that contain a member user with any of the provided ids
                    "members", new Dictionary<string, object>
                    {
                        //you can provide multiple ids
                        {"$in", new[] {Client.LocalUserData.UserId}}
                    }
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
            //StreamTodo: IMPLEMENT query channels pagination
            await Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/query_channels/?language=unity#support
        /// </summary>
        public async Task QueryBasedOnAdditionalInformation()
        {
            //StreamTodo: IMPLEMENT query based on additional information
            await Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/query_channels/?language=unity#pagination
        /// </summary>
        public async Task QueryPagination()
        {
            //StreamTodo: IMPLEMENT

            var filters = new Dictionary<string, object>
            {
                {
                    //Return on channels that contain a member user with any of the provided ids
                    "members", new Dictionary<string, object>
                    {
                        //you can provide multiple ids
                        {"$in", new[] {Client.LocalUserData.UserId}}
                    }
                }
            };

            var channels = await Client.QueryChannelsAsync(filters, limit: 30, offset: 10);
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
                        {"$in", new[] {"user-id-1", "user-id-2"}}
                    }
                }
            };

            // Paginate through results by increasing offset
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
            await channel.HideAsync();

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
            await channel.DeleteAsync();
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/truncate_channel/?language=unity
        /// </summary>
        public async Task TruncateChannel()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");
            await channel.TruncateAsync();
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