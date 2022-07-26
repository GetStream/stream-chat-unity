using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Core.Requests;
using UnityEngine;

namespace Plugins.StreamChat.Samples.ClientDocs
{
    /// <summary>
    /// Code samples for Channels sections: https://getstream.io/chat/docs/unity/creating_channels/?language=unity
    /// </summary>
    public class ChannelsCodeSamples
    {
        private async Task CreatingChannelUsingChannelId()
        {
            var channelState = await Client.ChannelApi.GetOrCreateChannelAsync(
                channelType: "messaging", channelId: "channel-id-1", new ChannelGetOrCreateRequest());
        }

        private async void CreatingChannelForListOfMembers()
        {
            var requestBody = new ChannelGetOrCreateRequest
            {
                Data = new ChannelRequest
                {
                    Members = new List<ChannelMemberRequest>
                    {
                        new ChannelMemberRequest
                        {
                            UserId = "tommaso",
                        },
                        new ChannelMemberRequest
                        {
                            UserId = "thierry",
                        },
                    }
                }
            };

            var channelState = await Client.ChannelApi.GetOrCreateChannelAsync(channelType: "messaging", requestBody);
        }

        private async Task WatchingChannel()
        {
            await Client.ChannelApi.GetOrCreateChannelAsync(channelType: "messaging", channelId: "channel-id-1",
                new ChannelGetOrCreateRequest
                {
                    Watch = true,
                });
        }

        private async Task GetReadStatesForChannel()
        {
            var channelState = await Client.ChannelApi.GetOrCreateChannelAsync(
                channelType: "messaging", channelId: "channel-id-1", new ChannelGetOrCreateRequest());

            foreach (var read in channelState.Read)
            {
                Debug.Log(read.User.Id);
                Debug.Log(read.UnreadMessages); //Total unread messages
                Debug.Log(read.LastRead); //Date of the last read message
            }
        }

        private async Task GetMessagesBasedOnLastRead()
        {
            var lastReadTime = new DateTimeOffset(); //Take it from channelState.Read
            var getOrCreateRequest = new ChannelGetOrCreateRequest()
            {
                Messages = new MessagePaginationParamsRequest()
                {
                    CreatedAtAfterOrEqual = lastReadTime,
                    Limit = 25
                }
            };

            var channelState = await Client.ChannelApi.GetOrCreateChannelAsync(
                channelType: "messaging", channelId: "channel-id-1", getOrCreateRequest);
        }

        private async Task WatchingMultipleChannels()
        {
            var user_id = "specific-user-id";

            var request = new QueryChannelsRequest
            {
                //Sort results by created_at in descending order
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
                            { "$in", new[] { user_id } }
                        }
                    }
                },

                //Channels are watched by default when queried
                //Watch = true
            };

            var channelsResponse = await Client.ChannelApi.QueryChannelsAsync(request);
        }

        private async Task StopWatchingChannel()
        {
            var stopWatchingResponse = await Client.ChannelApi.StopWatchingChannelAsync(channelType: "messaging",
                channelId: "channel-id-1", new ChannelStopWatchingRequest());
        }

        private async Task WatcherCount()
        {
            var user_id = "specific-user-id";

            var request = new QueryChannelsRequest
            {
                //Sort results by created_at in descending order
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
                            { "$in", new[] { user_id } }
                        }
                    }
                }
            };

            //WatcherCount is a property of ChannelState which you get either from channels query or from GetOrCreateChannel request
            var channelsResponse = await Client.ChannelApi.QueryChannelsAsync(request);

            foreach (var channelState in channelsResponse.Channels)
            {
                var watcherCount = channelState.WatcherCount;
            }
        }

        public void PaginatingChannelWatchersWithChannelQuery()
        {
            //Not possible yet
        }

        public async Task DeleteChannel()
        {
            var deleteChannelResponse =
                await Client.ChannelApi.DeleteChannelAsync(channelType: "messaging", channelId: "channel-id-1");
        }

        public async Task DeletingManyChannels()
        {
            var deleteChannelsResponse = await Client.ChannelApi.DeleteChannelsAsync(new DeleteChannelsRequest
            {
                Cids = new List<string>
                {
                    "messaging:channel-id-1",
                    "messaging:channel-id-2",
                    "messaging:channel-id-3",
                },
                HardDelete = true
            });
        }

        public async Task PartialChannelUpdate()
        {
            var channelState = await Client.ChannelApi.GetOrCreateChannelAsync(
                channelType: "messaging", channelId: "channel-id-1", new ChannelGetOrCreateRequest());

            var updateChannelResponse = await Client.ChannelApi.UpdateChannelPartialAsync(channelType: "messaging",
                channelId: "channel-id-1", new UpdateChannelPartialRequest
                {
                    Set = new Dictionary<string, object>
                    {
                        { "owned_dogs", 5 },
                        {
                            "breakfast", new string[]
                            {
                                "donuts"
                            }
                        }
                    },
                    Unset = new List<string>
                    {
                        //must be previously set before trying to unset
                        "owned_hamsters"
                    }
                });
        }

        private async Task UpdateChannelToHideHistoryForNewMembers()
        {
            var channelState = await Client.ChannelApi.GetOrCreateChannelAsync(
                channelType: "messaging", channelId: "channel-id-1", new ChannelGetOrCreateRequest());

            var updateChannelResponse = await Client.ChannelApi.UpdateChannelAsync(channelType: "messaging",
                channelId: "channel-id-1", new UpdateChannelRequest()
                {
                    HideHistory = true,
                });
        }

        public void FullChannelUpdate()
        {
        }

        public async Task TruncateChannel()
        {
//default truncating
            var truncateChannelResponse = await Client.ChannelApi.TruncateChannelAsync(channelType: "messaging",
                channelId: "channel-id-1", new TruncateChannelRequest());

//or with parameters
            var truncateChannelResponse2 = await Client.ChannelApi.TruncateChannelAsync(channelType: "messaging",
                channelId: "channel-id-1", new TruncateChannelRequest()
                {
                    HardDelete = true,
                    SkipPush = true,
                    Message = new MessageRequest
                    {
                        Text = "This channel has been truncated"
                    }
                });
        }

        private IStreamChatClient Client;
    }
}