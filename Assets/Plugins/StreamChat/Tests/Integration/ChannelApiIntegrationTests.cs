#if STREAM_TESTS_ENABLED
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using StreamChat.Core.Exceptions;
using StreamChat.Core.Models;
using StreamChat.Core.Requests;
using UnityEngine.TestTools;

namespace StreamChat.Tests.Integration
{
    /// <summary>
    /// Integration tests for Channels
    /// </summary>
    public class ChannelApiIntegrationTests : BaseIntegrationTests
    {
        [UnityTest]
        public IEnumerator Get_or_create_channel()
        {
            yield return Client.WaitForClientToConnect();

            var request = new ChannelGetOrCreateRequest();

            const string channelType = "messaging";
            var channelId = "random-channel-" + Guid.NewGuid();

            var task = Client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId, request);

            yield return task.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(channelId, response.Channel.Id);
                Assert.AreEqual(channelType, response.Channel.Type);
            });

            yield return Client.ChannelApi.DeleteChannelAsync(channelType, channelId).RunAsIEnumerator();
        }

        [UnityTest]
        public IEnumerator Create_channel_with_custom_data()
        {
            yield return Client.WaitForClientToConnect();

            var requestBody = new ChannelGetOrCreateRequest
            {
                Data = new ChannelRequest
                {
                    AdditionalProperties = new Dictionary<string, object>
                    {
                        { "MyNumber", 3 },
                        { "MyString", "Hey Joe!" },
                        { "MyIntArray", new int[] { 5, 8, 9 } }
                    }
                },
            };
            ChannelState channelState = null;
            yield return CreateTempUniqueChannel("messaging", requestBody, state => channelState = state);

            Assert.AreEqual(3, channelState.Channel.AdditionalProperties.Count);
            Assert.AreEqual(3, channelState.Channel.AdditionalProperties.Count);
        }

        [UnityTest]
        public IEnumerator Get_or_create_channel_for_list_of_members()
        {
            yield return Client.WaitForClientToConnect();

            var request = new ChannelGetOrCreateRequest
            {
                Data = new ChannelRequest
                {
                    Members = new List<ChannelMemberRequest>
                    {
                        new ChannelMemberRequest
                        {
                            UserId = Client.UserId
                        },
                        new ChannelMemberRequest
                        {
                            UserId = OtherUserId
                        },
                    }
                }
            };

            string channelType = "messaging";

            var task = Client.ChannelApi.GetOrCreateChannelAsync(channelType, request);

            yield return task.RunAsIEnumerator(response => { Assert.AreEqual(channelType, response.Channel.Type); });
        }

        [UnityTest]
        public IEnumerator Delete_channel()
        {
            yield return Client.WaitForClientToConnect();

            const string channelType = "messaging";
            var channelId = "new-channel-id-" + Guid.NewGuid();

            var createChannelTask =
                Client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId, new ChannelGetOrCreateRequest());

            yield return createChannelTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(channelId, response.Channel.Id);
                Assert.AreEqual(channelType, response.Channel.Type);
            });

            var deleteChannelTask = Client.ChannelApi.DeleteChannelAsync(channelType, channelId);

            yield return deleteChannelTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(channelId, response.Channel.Id);
                Assert.AreEqual(channelType, response.Channel.Type);
            });
        }

        [UnityTest]
        public IEnumerator Query_channels()
        {
            yield return Client.WaitForClientToConnect();

            const string channelType = "messaging";

            //Create 2 channels with admin being member of one of them

            yield return CreateTempUniqueChannel(channelType, new ChannelGetOrCreateRequest());

            var getOrCreateRequest2 = new ChannelGetOrCreateRequest()
            {
                Data = new ChannelRequest
                {
                    Members = new List<ChannelMemberRequest>()
                    {
                        new ChannelMemberRequest
                        {
                            UserId = TestAdminId,
                        }
                    }
                }
            };

            yield return CreateTempUniqueChannel(channelType, getOrCreateRequest2);

            //Query channels where admin is a member

            var queryChannelsRequest = new QueryChannelsRequest
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
                            { "$in", new string[] { TestAdminId } }
                        }
                    }
                }
            };

            var queryChannelsTask = Client.ChannelApi.QueryChannelsAsync(queryChannelsRequest);

            yield return queryChannelsTask.RunAsIEnumerator(response =>
            {
                var allChannelsContainAdminUser = response.Channels.All(channelState =>
                    channelState.Members.Any(member => member.UserId == TestAdminId));

                Assert.IsTrue(allChannelsContainAdminUser);
            });
        }

        [UnityTest]
        public IEnumerator Watch_channel()
        {
            yield return Client.WaitForClientToConnect();

            const string channelType = "messaging";

            ChannelState channelState = null;
            yield return CreateTempUniqueChannel(channelType, new ChannelGetOrCreateRequest(),
                state => channelState = state);
            var channelId = channelState.Channel.Id;

            var updateRequestBody = new UpdateChannelRequest
            {
                AddMembers = new List<ChannelMemberRequest>()
                {
                    new ChannelMemberRequest
                    {
                        UserId = TestAdminId
                    }
                }
            };

            var updateChannelTask = Client.ChannelApi.UpdateChannelAsync(channelType, channelId, updateRequestBody);

            yield return updateChannelTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(channelId, response.Channel.Id);
                Assert.AreEqual(channelType, response.Channel.Type);
                Assert.IsTrue(response.Members.Any(_ => _.UserId == TestAdminId));
            });

            var watchChannelTask = Client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId,
                new ChannelGetOrCreateRequest()
                {
                    Watch = true,
                    State = true,
                    Watchers = new PaginationParamsRequest
                    {
                        Limit = 10
                    }
                });

            yield return watchChannelTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(channelId, response.Channel.Id);
                Assert.AreEqual(channelType, response.Channel.Type);

                //Not valid behaviour
                Assert.IsNull(response.Watchers);

                //should be
                //Assert.IsTrue(response.Watchers.Any(_ => _.Id == TestAdminId));
            });

            var watchChannelTask2 = Client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId,
                new ChannelGetOrCreateRequest()
                {
                    Watch = true,
                    State = true,
                    Watchers = new PaginationParamsRequest
                    {
                        Limit = 10
                    }
                });

            yield return watchChannelTask2.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(channelId, response.Channel.Id);
                Assert.AreEqual(channelType, response.Channel.Type);

                Assert.IsTrue(response.Watchers.Any(_ => _.Id == Client.UserId));
            });
        }

        [UnityTest]
        public IEnumerator Watcher_count()
        {
            yield return Client.WaitForClientToConnect();

            const string channelType = "messaging";

            var getOrCreateRequest = new ChannelGetOrCreateRequest
            {
                Watch = true,
                State = true,
                Watchers = new PaginationParamsRequest
                {
                    Limit = 10
                }
            };

            ChannelState channelState = null;
            yield return CreateTempUniqueChannel(channelType, getOrCreateRequest, state =>
            {
                channelState = state;

                //Assert.IsTrue(response.Watchers.Any(_ => _.Id == TestAdminId));
                //It seems that if users starts watching a channel he will not be returned in watchers collection until the next request
                Assert.IsNull(state.Watchers);
            });
            var channelId = channelState.Channel.Id;

            var watchChannelTask2 = Client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId,
                new ChannelGetOrCreateRequest
                {
                    Watch = true,
                    State = true,
                    Watchers = new PaginationParamsRequest
                    {
                        Limit = 10
                    }
                });

            yield return watchChannelTask2.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(channelId, response.Channel.Id);
                Assert.AreEqual(channelType, response.Channel.Type);
                Assert.IsTrue(response.Watchers.Any(_ => _.Id == Client.UserId));
            });
        }

        [UnityTest]
        public IEnumerator When_deleting_existing_channel_expect_success()
        {
            yield return Client.WaitForClientToConnect();

            var channelType = "messaging";

            ChannelState channelState = null;
            yield return CreateTempUniqueChannel(channelType, new ChannelGetOrCreateRequest(),
                state => channelState = state);
            var channelId = channelState.Channel.Id;

            var deleteChannelTask = Client.ChannelApi.DeleteChannelAsync(channelType, channelId);
            yield return deleteChannelTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(response.Channel.Id, channelId);
            });
        }

        [UnityTest]
        public IEnumerator When_deleting_non_existing_channel_expect_fail()
        {
            yield return Client.WaitForClientToConnect();

            var channelType = "messaging";
            var channelId = "new-channel-id-1";

            var createChannelTask =
                Client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId, new ChannelGetOrCreateRequest());

            yield return createChannelTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(response.Channel.Id, channelId);
            });

            var deleteChannelTask = Client.ChannelApi.DeleteChannelAsync(channelType, channelId);
            yield return deleteChannelTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(response.Channel.Id, channelId);
            });

            var deleteChannelTask2 = Client.ChannelApi.DeleteChannelAsync(channelType, channelId);

            deleteChannelTask2.RunAsIEnumerator(onFaulted: exception =>
            {
                Assert.AreEqual(((StreamApiException)exception).StatusCode, 404);
            });
        }

        [UnityTest]
        public IEnumerator Delete_multiple_channels()
        {
            yield return Client.WaitForClientToConnect();

            var channelType = "messaging";

            var channelsCIdsToDelete = new List<string>();

            yield return CreateTempUniqueChannel(channelType, new ChannelGetOrCreateRequest(),
                state => channelsCIdsToDelete.Add(state.Channel.Cid));
            yield return CreateTempUniqueChannel(channelType, new ChannelGetOrCreateRequest(),
                state => channelsCIdsToDelete.Add(state.Channel.Cid));
            yield return CreateTempUniqueChannel(channelType, new ChannelGetOrCreateRequest(),
                state => channelsCIdsToDelete.Add(state.Channel.Cid));

            Assert.AreEqual(channelsCIdsToDelete.Count, 3);

            var deleteChannelsTask = Client.ChannelApi.DeleteChannelsAsync(new DeleteChannelsRequest()
            {
                Cids = channelsCIdsToDelete,
                HardDelete = true
            });

            yield return deleteChannelsTask.RunAsIEnumerator(response =>
            {
                foreach (var cidToDelete in channelsCIdsToDelete)
                {
                    Assert.IsTrue(response.Result.Any(_ => _.Key == cidToDelete));
                }
            });
        }

        [UnityTest]
        public IEnumerator Partial_update()
        {
            yield return Client.WaitForClientToConnect();

            var channelType = "messaging";

            ChannelState channelState = null;
            yield return CreateTempUniqueChannel(channelType, new ChannelGetOrCreateRequest(),
                state => channelState = state);
            var channelId = channelState.Channel.Id;

            var updateChannelPartialTask = Client.ChannelApi.UpdateChannelPartialAsync(channelType, channelId,
                new UpdateChannelPartialRequest
                {
                    Set = new Dictionary<string, object>
                    {
                        { "owned_dogs", 2 },
                        { "owned_hamsters", 4 },
                        {
                            "breakfast", new string[]
                            {
                                "oatmeal", "juice"
                            }
                        }
                    }
                });

            yield return updateChannelPartialTask.RunAsIEnumerator(response => { });

            var getOrCreateTask2 =
                Client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId, new ChannelGetOrCreateRequest());

            yield return getOrCreateTask2.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(2, response.Channel.AdditionalProperties["owned_dogs"]);
                Assert.AreEqual(4, response.Channel.AdditionalProperties["owned_hamsters"]);

                //StreamTodo: add support - this array is Newtonsoft.Json.Linq.JArray
                // Assert.That(response.Channel.AdditionalProperties["breakfast"], Contains.Item("oatmeal"));
                // Assert.That(response.Channel.AdditionalProperties["breakfast"], Contains.Item("juice"));
            });

            var updateChannelPartialTask2 = Client.ChannelApi.UpdateChannelPartialAsync(channelType, channelId,
                new UpdateChannelPartialRequest
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
                        "owned_hamsters"
                    }
                });

            yield return updateChannelPartialTask2.RunAsIEnumerator(response => { });

            var getOrCreateTask3 =
                Client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId, new ChannelGetOrCreateRequest());

            yield return getOrCreateTask3.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(5, response.Channel.AdditionalProperties["owned_dogs"]);
                Assert.That(response.Channel.AdditionalProperties, Does.Not.Contain("owned_hamsters"));

                //StreamTodo: add support - this array is Newtonsoft.Json.Linq.JArray
                //Assert.That(response.Channel.AdditionalProperties["breakfast"], Contains.Item("donuts"));
                //Assert.That(response.Channel.AdditionalProperties["breakfast"], Has.Exactly(1).Count);
            });
        }

        [UnityTest]
        public IEnumerator Mark_single_read_with_specified_message_id()
        {
            yield return Client.WaitForClientToConnect();

            var channelType = "messaging";

            ChannelState channelState1 = null;
            yield return CreateTempUniqueChannel(channelType, new ChannelGetOrCreateRequest(),
                state => channelState1 = state);

            //second is read -> 1 unread
            var channelState1SecondMessageId = string.Empty;
            yield return SendTestMessages(channelState1, count: 3, response =>
            {
                if (response.Index == 1)
                {
                    channelState1SecondMessageId = response.MessageResponse.Message.Id;
                }
            });

            //Join channel as members

            var updateRequestBody = new UpdateChannelRequest
            {
                AddMembers = new List<ChannelMemberRequest>
                {
                    new ChannelMemberRequest
                    {
                        UserId = OtherUserId
                    },
                    new ChannelMemberRequest
                    {
                        UserId = Client.UserId
                    }
                }
            };

            var updateChannelTask =
                Client.ChannelApi.UpdateChannelAsync(channelType, channelState1.Channel.Id, updateRequestBody);
            yield return updateChannelTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(channelType, response.Channel.Type);
                Assert.IsTrue(response.Members.Any(_ => _.UserId == TestAdminId));
            });

            var markReadTask = Client.ChannelApi.MarkReadAsync(channelState1.Channel.Type, channelState1.Channel.Id,
                new MarkReadRequest
                {
                    MessageId = channelState1SecondMessageId
                });

            yield return markReadTask.RunAsIEnumerator(response => { });

            //Query channels to confirm the read state

            var queryChannelsTask = Client.ChannelApi.QueryChannelsAsync(new QueryChannelsRequest
            {
                FilterConditions = new Dictionary<string, object>
                {
                    {
                        "cid", new Dictionary<string, object>
                        {
                            {
                                "$in",
                                new[]
                                {
                                    channelState1.Channel.Cid
                                }
                            }
                        }
                    }
                },
            });

            yield return queryChannelsTask.RunAsIEnumerator(channelsResponse =>
            {
                Assert.AreEqual(channelsResponse.Channels.Count, 1);

                channelState1 =
                    channelsResponse.Channels.FirstOrDefault(_ => _.Channel.Cid == channelState1.Channel.Cid);

                Assert.NotNull(channelState1);

                var localUserChannelState1ReadState =
                    channelState1.Read.FirstOrDefault(_ => _.User.Id == Client.UserId);

                Assert.NotNull(localUserChannelState1ReadState);

                //Assert channel unread counts
                Assert.AreEqual(localUserChannelState1ReadState.UnreadMessages, 1);
            });
        }

        [UnityTest]
        public IEnumerator Mark_single_read_without_message_id()
        {
            yield return Client.WaitForClientToConnect();

            var channelType = "messaging";

            ChannelState channelState1 = null;
            yield return CreateTempUniqueChannel(channelType, new ChannelGetOrCreateRequest(),
                state => channelState1 = state);

            //second is read -> 1 unread
            var channelState1SecondMessageId = string.Empty;
            yield return SendTestMessages(channelState1, count: 3, response =>
            {
                if (response.Index == 1)
                {
                    channelState1SecondMessageId = response.MessageResponse.Message.Id;
                }
            });

            //Join channel as members

            var updateRequestBody = new UpdateChannelRequest
            {
                AddMembers = new List<ChannelMemberRequest>
                {
                    new ChannelMemberRequest
                    {
                        UserId = OtherUserId
                    },
                    new ChannelMemberRequest
                    {
                        UserId = Client.UserId
                    }
                }
            };

            var updateChannelTask =
                Client.ChannelApi.UpdateChannelAsync(channelType, channelState1.Channel.Id, updateRequestBody);
            yield return updateChannelTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(channelType, response.Channel.Type);
                Assert.IsTrue(response.Members.Any(_ => _.UserId == TestAdminId));
            });

            var markReadTask = Client.ChannelApi.MarkReadAsync(channelState1.Channel.Type, channelState1.Channel.Id,
                new MarkReadRequest
                {
                    MessageId = channelState1SecondMessageId
                });

            yield return markReadTask.RunAsIEnumerator(response => { });

            //Query channels to confirm the read state

            var queryChannelsRequest = new QueryChannelsRequest
            {
                FilterConditions = new Dictionary<string, object>
                {
                    {
                        "cid", new Dictionary<string, object>
                        {
                            {
                                "$in",
                                new[]
                                {
                                    channelState1.Channel.Cid
                                }
                            }
                        }
                    }
                },
            };

            var queryChannelsTask = Client.ChannelApi.QueryChannelsAsync(queryChannelsRequest);

            // Expect 1 message unread

            yield return queryChannelsTask.RunAsIEnumerator(channelsResponse =>
            {
                Assert.AreEqual(channelsResponse.Channels.Count, 1);

                channelState1 =
                    channelsResponse.Channels.FirstOrDefault(_ => _.Channel.Cid == channelState1.Channel.Cid);

                Assert.NotNull(channelState1);

                var localUserChannelState1ReadState =
                    channelState1.Read.FirstOrDefault(_ => _.User.Id == Client.UserId);

                Assert.NotNull(localUserChannelState1ReadState);

                //Assert channel unread counts
                Assert.AreEqual(localUserChannelState1ReadState.UnreadMessages, 1);
            });

            // Update again with message ID not specified - expect whole channel marked as read == 0 unread

            var markReadTask2 = Client.ChannelApi.MarkReadAsync(channelState1.Channel.Type, channelState1.Channel.Id,
                new MarkReadRequest());

            yield return markReadTask2.RunAsIEnumerator(response => { });

            // Query channels again - expect 0 unread

            var queryChannelsTask2 = Client.ChannelApi.QueryChannelsAsync(queryChannelsRequest);

            yield return queryChannelsTask2.RunAsIEnumerator(channelsResponse =>
            {
                Assert.AreEqual(channelsResponse.Channels.Count, 1);

                channelState1 =
                    channelsResponse.Channels.FirstOrDefault(_ => _.Channel.Cid == channelState1.Channel.Cid);

                Assert.NotNull(channelState1);

                var localUserChannelState1ReadState =
                    channelState1.Read.FirstOrDefault(_ => _.User.Id == Client.UserId);

                Assert.NotNull(localUserChannelState1ReadState);

                //Assert channel unread counts
                Assert.AreEqual(localUserChannelState1ReadState.UnreadMessages, 0);
            });
        }

        /// <summary>
        /// 1. Create 3 channels with 3 messages
        /// 2. Mark first, second and third message as read for each channel respectively
        /// 3. query channels and validate 2, 1, 0 unread messages respectively
        /// </summary>
        [UnityTest]
        public IEnumerator Mark_many_read_with_specified_message_id()
        {
            yield return Client.WaitForClientToConnect();

            var channelType = "messaging";

            // Create channels

            ChannelState channelState1 = null;
            ChannelState channelState2 = null;
            yield return CreateTempUniqueChannel(channelType, new ChannelGetOrCreateRequest(),
                state => channelState1 = state);
            yield return CreateTempUniqueChannel(channelType, new ChannelGetOrCreateRequest(),
                state => channelState2 = state);

            //Send messages & pick the last read

            //first is read -> 2 unread
            var channelState1FirstMessageId = string.Empty;
            yield return SendTestMessages(channelState1, count: 3, response =>
            {
                if (response.Index == 0)
                {
                    channelState1FirstMessageId = response.MessageResponse.Message.Id;
                }
            });
            //second is read -> 1 unread
            var channelState2SecondMessageId = string.Empty;
            yield return SendTestMessages(channelState2, count: 3, response =>
            {
                if (response.Index == 1)
                {
                    channelState2SecondMessageId = response.MessageResponse.Message.Id;
                }
            });

            //Join channels as members

            var updateRequestBody = new UpdateChannelRequest
            {
                AddMembers = new List<ChannelMemberRequest>
                {
                    new ChannelMemberRequest
                    {
                        UserId = OtherUserId
                    },
                    new ChannelMemberRequest
                    {
                        UserId = Client.UserId
                    }
                }
            };

            var updateChannelTask =
                Client.ChannelApi.UpdateChannelAsync(channelType, channelState1.Channel.Id, updateRequestBody);
            yield return updateChannelTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(channelType, response.Channel.Type);
                Assert.IsTrue(response.Members.Any(_ => _.UserId == TestAdminId));
            });

            var updateChannelTask2 =
                Client.ChannelApi.UpdateChannelAsync(channelType, channelState2.Channel.Id, updateRequestBody);
            yield return updateChannelTask2.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(channelType, response.Channel.Type);
                Assert.IsTrue(response.Members.Any(_ => _.UserId == TestAdminId));
            });

            //Send mark read state

            var markReadRequestTask = Client.ChannelApi.MarkManyReadAsync(new MarkChannelsReadRequest
            {
                ReadByChannel = new Dictionary<string, string>
                {
                    { channelState1.Channel.Cid, channelState1FirstMessageId },
                    { channelState2.Channel.Cid, channelState2SecondMessageId },
                }
            });

            yield return markReadRequestTask.RunAsIEnumerator(markReadResponse => { });

            //Query channels to confirm the read state

            var queryChannelsTask = Client.ChannelApi.QueryChannelsAsync(new QueryChannelsRequest
            {
                FilterConditions = new Dictionary<string, object>
                {
                    {
                        "cid", new Dictionary<string, object>
                        {
                            {
                                "$in",
                                new[]
                                {
                                    channelState1.Channel.Cid, channelState2.Channel.Cid
                                }
                            }
                        }
                    }
                },
            });

            // Expect specific unread counts

            yield return queryChannelsTask.RunAsIEnumerator(channelsResponse =>
            {
                Assert.AreEqual(channelsResponse.Channels.Count, 2);

                channelState1 =
                    channelsResponse.Channels.FirstOrDefault(_ => _.Channel.Cid == channelState1.Channel.Cid);
                channelState2 =
                    channelsResponse.Channels.FirstOrDefault(_ => _.Channel.Cid == channelState2.Channel.Cid);

                Assert.NotNull(channelState1);
                Assert.NotNull(channelState2);

                var localUserChannelState1ReadState =
                    channelState1.Read.FirstOrDefault(_ => _.User.Id == Client.UserId);
                var localUserChannelState2ReadState =
                    channelState2.Read.FirstOrDefault(_ => _.User.Id == Client.UserId);

                Assert.NotNull(localUserChannelState1ReadState);
                Assert.NotNull(localUserChannelState2ReadState);

                //Assert channel unread counts
                Assert.AreEqual(localUserChannelState1ReadState.UnreadMessages, 2);
                Assert.AreEqual(localUserChannelState2ReadState.UnreadMessages, 1);
            });

            yield return ReconnectClient();

            //Should use Assert.AreEqual but there seems to be some delay with updating the values
            Assert.IsNotNull(InitialLocalUser);
            Assert.GreaterOrEqual(InitialLocalUser.UnreadChannels, 2);
            Assert.GreaterOrEqual(InitialLocalUser.TotalUnreadCount, 3);
        }

        [UnityTest]
        public IEnumerator When_sending_typing_start_stop_events_expect_no_errors()
        {
            yield return Client.WaitForClientToConnect();
            yield return When_sending_typing_start_stop_events_expect_no_exceptions_Async().RunAsIEnumerator();
        }

        private async Task When_sending_typing_start_stop_events_expect_no_exceptions_Async()
        {
            const string channelType = "messaging";
            var tempChannel = await CreateTempUniqueChannelAsync(channelType, new ChannelGetOrCreateRequest());

            await Client.ChannelApi.SendTypingStartEventAsync(channelType, tempChannel.Channel.Id);
            await Client.ChannelApi.SendTypingStopEventAsync(channelType, tempChannel.Channel.Id);
        }
    }
}
#endif