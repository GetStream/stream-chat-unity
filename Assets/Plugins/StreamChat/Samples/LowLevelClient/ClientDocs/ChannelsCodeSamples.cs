using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StreamChat.Core.LowLevelClient;
using StreamChat.Core.LowLevelClient.Events;
using StreamChat.Core.LowLevelClient.Models;
using StreamChat.Core.LowLevelClient.Requests;
using StreamChat.Libs.Auth;
using UnityEngine;

namespace StreamChat.Samples.LowLevelClient.ClientDocs
{
    /// <summary>
    /// Code samples for Channels sections: https://getstream.io/chat/docs/unity/creating_channels/?language=unity
    /// </summary>
    public class ChannelsCodeSamples
    {
        private async Task CreatingChannelUsingChannelId()
        {
            var channelState = await _lowLevelClient.ChannelApi.GetOrCreateChannelAsync(
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

            var channelState = await _lowLevelClient.ChannelApi.GetOrCreateChannelAsync(channelType: "messaging", requestBody);
        }

        private async Task WatchingChannel()
        {
            await _lowLevelClient.ChannelApi.GetOrCreateChannelAsync(channelType: "messaging", channelId: "channel-id-1",
                new ChannelGetOrCreateRequest
                {
                    Watch = true,
                });
        }

        private async Task GetReadStatesForChannel()
        {
            var channelState = await _lowLevelClient.ChannelApi.GetOrCreateChannelAsync(
                channelType: "messaging", channelId: "channel-id-1", new ChannelGetOrCreateRequest());

            foreach (var read in channelState.Read)
            {
                Debug.Log(read.User.Id);
                Debug.Log(read.UnreadMessages); //Total unread messages
                Debug.Log(read.LastRead); //Date of the last read message
            }
        }

        private async Task PaginateMessagesMembersOrWatchers()
        {
            var messages = new List<Message>();
            var lastMessageId = messages[0].Id;

            //Pick the parts you need: Messages, Members or Watchers
            var paginateMessages = new ChannelGetOrCreateRequest
            {
                State = true,

                //Paginate through messages
                Messages = new MessagePaginationParamsRequest
                {
                    IdLt = lastMessageId, //Get messages with ID less than provided Message ID
                    Limit = 50,
                },

                //Paginate through members
                Members = new PaginationParamsRequest
                {
                    Limit = 30,
                    Offset = 0
                },

                //Paginate through watcher
                Watchers = new PaginationParamsRequest
                {
                    Limit = 30,
                    Offset = 0
                },
            };

            var channelState = await _lowLevelClient.ChannelApi.GetOrCreateChannelAsync(channelType: "messaging",
                channelId: "channel-id-1", paginateMessages);
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

            var channelState = await _lowLevelClient.ChannelApi.GetOrCreateChannelAsync(
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
                            {"$in", new[] {user_id}}
                        }
                    }
                },

                //Channels are watched by default when queried
                //Watch = true
            };

            var channelsResponse = await _lowLevelClient.ChannelApi.QueryChannelsAsync(request);
        }

        private async Task StopWatchingChannel()
        {
            var stopWatchingResponse = await _lowLevelClient.ChannelApi.StopWatchingChannelAsync(channelType: "messaging",
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
                            {"$in", new[] {user_id}}
                        }
                    }
                }
            };

            //WatcherCount is a property of ChannelState which you get either from channels query or from GetOrCreateChannel request
            var channelsResponse = await _lowLevelClient.ChannelApi.QueryChannelsAsync(request);

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
                await _lowLevelClient.ChannelApi.DeleteChannelAsync(channelType: "messaging", channelId: "channel-id-1",
                    isHardDelete: false);
        }

        public async Task DeletingManyChannels()
        {
            var deleteChannelsResponse = await _lowLevelClient.ChannelApi.DeleteChannelsAsync(new DeleteChannelsRequest
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
            var channelState = await _lowLevelClient.ChannelApi.GetOrCreateChannelAsync(
                channelType: "messaging", channelId: "channel-id-1", new ChannelGetOrCreateRequest());

            var updateChannelResponse = await _lowLevelClient.ChannelApi.UpdateChannelPartialAsync(channelType: "messaging",
                channelId: "channel-id-1", new UpdateChannelPartialRequest
                {
                    Set = new Dictionary<string, object>
                    {
                        {"owned_dogs", 5},
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
            var channelState = await _lowLevelClient.ChannelApi.GetOrCreateChannelAsync(
                channelType: "messaging", channelId: "channel-id-1", new ChannelGetOrCreateRequest());

            var updateChannelResponse = await _lowLevelClient.ChannelApi.UpdateChannelAsync(channelType: "messaging",
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
            var truncateChannelResponse = await _lowLevelClient.ChannelApi.TruncateChannelAsync(channelType: "messaging",
                channelId: "channel-id-1", new TruncateChannelRequest());

//or with parameters
            var truncateChannelResponse2 = await _lowLevelClient.ChannelApi.TruncateChannelAsync(channelType: "messaging",
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

        public void GetInitialLocalUserReadState()
        {
            AuthCredentials authCredentials = default;
            InitStreamChatClient();

            void InitStreamChatClient()
            {
                _lowLevelClient = StreamChatLowLevelClient.CreateDefaultClient(authCredentials);
                _lowLevelClient.Connected += ClientOnConnected;
                _lowLevelClient.Connect();
            }

            void ClientOnConnected(OwnUser ownUser)
            {
                Debug.Log(ownUser.UnreadChannels);
                Debug.Log(ownUser.TotalUnreadCount);
            }
        }

        public async Task MarkChannelReadState()
        {
            Message message = default;
            ChannelState channelState = default;

            var markReadResponse = await _lowLevelClient.ChannelApi.MarkReadAsync(channelState.Channel.Type,
                channelState.Channel.Id, new MarkReadRequest
                {
                    //Optional Message ID to mark user last read message, if no Message ID is passed the whole channel is marked as read
                    MessageId = message.Id
                });
        }

        public void ListenToMarkReadStateEvents()
        {
            AuthCredentials authCredentials = default;
            InitStreamChatClient();

            void InitStreamChatClient()
            {
                _lowLevelClient = StreamChatLowLevelClient.CreateDefaultClient(authCredentials);
                _lowLevelClient.Connect();

                //sent when message is read to users watching the channel
                _lowLevelClient.MessageRead += OnMessageRead;

                //sent when unread state changes to all channel members even if they're not watching the channel
                _lowLevelClient.NotificationMarkRead += OnNotificationMarkRead;
            }

            void OnMessageRead(EventMessageRead eventMessageRead)
            {
                Debug.Log(eventMessageRead.Cid); //Channel CID
                Debug.Log(eventMessageRead.User); //Which user
            }

            void OnNotificationMarkRead(EventNotificationMarkRead eventNotificationMarkRead)
            {
                Debug.Log(eventNotificationMarkRead.Cid); //Channel CID
                Debug.Log(eventNotificationMarkRead.User); //Which user
                Debug.Log(eventNotificationMarkRead.TotalUnreadCount); //How many unread messages
                Debug.Log(eventNotificationMarkRead.UnreadChannels); //How many channels with unread messages
            }
        }

        public async Task GetChannelReadState()
        {
            var channelState =
                await _lowLevelClient.ChannelApi.GetOrCreateChannelAsync("messaging", "channel-id",
                    new ChannelGetOrCreateRequest());

            foreach (var readState in channelState.Read)
            {
                Debug.Log(readState.User); //Which user
                Debug.Log(readState.LastRead); //Last read message date
                Debug.Log(readState.UnreadMessages); //Total unread messages
            }
        }

        public async Task GetChannelLocalUserReadState()
        {
            //Get desired channel
            var channelState =
                await _lowLevelClient.ChannelApi.GetOrCreateChannelAsync("messaging", "channel-id",
                    new ChannelGetOrCreateRequest());

            Read localUserReadState = default;

            //Loop through results
            foreach (var readState in channelState.Read)
            {
                if (readState.User.Id == _lowLevelClient.UserId)
                {
                    localUserReadState = readState;
                    break;
                }
            }

            //Or use LINQ
            localUserReadState = channelState.Read.First(read => read.User.Id == _lowLevelClient.UserId);

            //Access local user read state for desired channel
            Debug.Log(localUserReadState.LastRead); //Last read message date
            Debug.Log(localUserReadState.UnreadMessages); //Total unread messages
        }

        public async Task GetMultipleChannelReadState()
        {
            var channelsResponse = await _lowLevelClient.ChannelApi.QueryChannelsAsync(new QueryChannelsRequest
            {
                FilterConditions = new Dictionary<string, object>
                {
                    {
                        //Get channels to which local user has joined as a member
                        "members", new Dictionary<string, object>
                        {
                            {"$in", new[] {_lowLevelClient.UserId}}
                        }
                    }
                },
            });

            foreach (var channelState in channelsResponse.Channels)
            {
                foreach (var readState in channelState.Read)
                {
                    Debug.Log(readState.User); //Which user
                    Debug.Log(readState.LastRead); //Last read message date
                    Debug.Log(readState.UnreadMessages); //Total unread messages
                }
            }
        }

        public async Task MarkAllAsRead()
        {
            //if MarkReadRequest.MessageId is empty, the whole channel is marked as read
            var markReadRequest = new MarkReadRequest();
            var markReadResponse = await _lowLevelClient.ChannelApi.MarkReadAsync("messaging", "channel-id", markReadRequest);
        }

        public async Task SendTypingStartStopEvents()
        {
            ChannelState channelState = default;

            //Notify other users that user started typing
            await _lowLevelClient.ChannelApi.SendTypingStartEventAsync(channelState.Channel.Type, channelState.Channel.Id);

            //Notify other users that user stopped typing
            await _lowLevelClient.ChannelApi.SendTypingStopEventAsync(channelState.Channel.Type, channelState.Channel.Id);
        }

        public void ReceiveTypingStartStopEvents()
        {
            SubscribeToTypingEvents();

            void SubscribeToTypingEvents()
            {
                _lowLevelClient.TypingStarted += OnTypingStarted;
                _lowLevelClient.TypingStopped += OnTypingStopped;
            }

            void OnTypingStarted(EventTypingStart eventTypingStart)
            {
                Debug.Log(eventTypingStart.ChannelId); //Channel ID
                Debug.Log(eventTypingStart.ChannelType); //Channel Type
                Debug.Log(eventTypingStart.Cid); //Channel CID
                Debug.Log(eventTypingStart.User); //User that started typing
            }

            void OnTypingStopped(EventTypingStop eventTypingStop)
            {
                Debug.Log(eventTypingStop.ChannelId); //Channel ID
                Debug.Log(eventTypingStop.ChannelType); //Channel Type
                Debug.Log(eventTypingStop.Cid); //Channel CID
                Debug.Log(eventTypingStop.User); //User that stopped typing
            }
        }

        private async Task AddMember()
        {
            ChannelState channelState = default;
            User user = default;

            //Make API call
            var response = await _lowLevelClient.ChannelApi.UpdateChannelAsync(channelState.Channel.Type,
                channelState.Channel.Id,
                new UpdateChannelRequest
                {
                    AddMembers = new List<ChannelMemberRequest>
                    {
                        new ChannelMemberRequest
                        {
                            UserId = user.Id,
                        }
                    }
                });
        }

        private IStreamChatLowLevelClient _lowLevelClient;
    }
}