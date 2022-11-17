using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Core.StatefulModels;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace Plugins.StreamChat.Samples
{
    internal class ChannelsCodeSamples
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

            // Get channel by user list
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
            //StreamTodo: IMPLEMENT
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/watch_channel/?language=unity#listening-to-changes-in-watchers
        /// </summary>
        public async Task ListToChangesInWatchers()
        {
            //StreamTodo: IMPLEMENT
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

            // Set values
            setFields.Add("owned_dogs", 5);
            // Set arrays
            setFields.Add("breakfast", new[] {"donuts"});
            // Set your custom class objects
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
            //StreamTodo: IMPLEMENT
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/channel_members/?language=unity
        /// </summary>
        public async Task AddingAndRemovingChannelMembers()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task PartialUpdate112()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task PartialUpdate113()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task PartialUpdate114()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task PartialUpdate115()
        {
        }

        protected IStreamChatClient Client { get; } = StreamChatClient.CreateDefaultClient();
    }
}