#if STREAM_TESTS_ENABLED
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using StreamChat.Core.Exceptions;
using StreamChat.Core.LowLevelClient.Models;
using StreamChat.Core.LowLevelClient.Requests;
using StreamChat.Core.LowLevelClient.Responses;
using UnityEngine;
using UnityEngine.TestTools;

namespace StreamChat.Tests.LowLevelClient.Integration
{
    /// <summary>
    /// Integration tests for Channels
    /// </summary>
    internal class ChannelApiIntegrationTests : BaseIntegrationTests
    {
        [UnityTest]
        public IEnumerator Get_or_create_channel()
        {
            yield return LowLevelClient.WaitForClientToConnect();

            var request = new ChannelGetOrCreateRequest();

            const string channelType = "messaging";
            var channelId = "random-channel-" + Guid.NewGuid();

            var task = LowLevelClient.ChannelApi.GetOrCreateChannelAsync(channelType, channelId, request);

            yield return task.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(channelId, response.Channel.Id);
                Assert.AreEqual(channelType, response.Channel.Type);
            });

            yield return LowLevelClient.ChannelApi.DeleteChannelAsync(channelType, channelId, isHardDelete: false)
                .RunAsIEnumerator();
        }

        [UnityTest]
        public IEnumerator Create_channel_with_custom_data()
        {
            yield return LowLevelClient.WaitForClientToConnect();

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

            Assert.IsTrue(channelState.Channel.AdditionalProperties.ContainsKey("MyNumber"));
            Assert.AreEqual(3, channelState.Channel.AdditionalProperties["MyNumber"]);
            Assert.IsTrue(channelState.Channel.AdditionalProperties.ContainsKey("MyString"));
            Assert.AreEqual("Hey Joe!", channelState.Channel.AdditionalProperties["MyString"]);
            Assert.IsTrue(channelState.Channel.AdditionalProperties.ContainsKey("MyIntArray"));
            
            //StreamTodo: fix returned array here to not be JArray https://stream-io.atlassian.net/browse/PBE-4851
            //Assert.AreEqual(new int[] { 5, 8, 9 }, (int[])channelState.Channel.AdditionalProperties["MyIntArray"]);
        }

        [UnityTest]
        public IEnumerator Get_or_create_channel_for_list_of_members()
        {
            yield return LowLevelClient.WaitForClientToConnect();

            var request = new ChannelGetOrCreateRequest
            {
                Data = new ChannelRequest
                {
                    Members = new List<ChannelMemberRequest>
                    {
                        new ChannelMemberRequest
                        {
                            UserId = LowLevelClient.UserId
                        },
                        new ChannelMemberRequest
                        {
                            UserId = SecondUserId
                        },
                    }
                }
            };

            string channelType = "messaging";

            var task = LowLevelClient.ChannelApi.GetOrCreateChannelAsync(channelType, request);

            yield return task.RunAsIEnumerator(response => { Assert.AreEqual(channelType, response.Channel.Type); });
        }

        [UnityTest]
        public IEnumerator Delete_channel()
        {
            yield return LowLevelClient.WaitForClientToConnect();

            const string channelType = "messaging";
            var channelId = "new-channel-id-" + Guid.NewGuid();

            var createChannelTask =
                LowLevelClient.ChannelApi.GetOrCreateChannelAsync(channelType, channelId,
                    new ChannelGetOrCreateRequest());

            yield return createChannelTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(channelId, response.Channel.Id);
                Assert.AreEqual(channelType, response.Channel.Type);
            });

            var deleteChannelTask
                = LowLevelClient.ChannelApi.DeleteChannelAsync(channelType, channelId, isHardDelete: false);

            yield return deleteChannelTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(channelId, response.Channel.Id);
                Assert.AreEqual(channelType, response.Channel.Type);
                RemoveTempChannelFromDeleteList(response.Channel.Cid);
            });
        }

        [UnityTest]
        public IEnumerator Query_channels()
        {
            yield return LowLevelClient.WaitForClientToConnect();

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
                            UserId = LowLevelClient.UserId,
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
                            { "$in", new string[] { LowLevelClient.UserId } }
                        }
                    }
                }
            };

            var queryChannelsTask = LowLevelClient.ChannelApi.QueryChannelsAsync(queryChannelsRequest);

            yield return queryChannelsTask.RunAsIEnumerator(response =>
            {
                var allChannelsContainAdminUser = response.Channels.All(channelState =>
                    channelState.Members.Any(member => member.UserId == LowLevelClient.UserId));

                Assert.IsTrue(allChannelsContainAdminUser);
            });
        }

        //[UnityTest] StreamTodo: re-enable this test once Watchers issue is resolved
        public IEnumerator Watch_channel()
        {
            yield return LowLevelClient.WaitForClientToConnect();

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
                        UserId = LowLevelClient.UserId
                    }
                }
            };

            var updateChannelTask
                = LowLevelClient.ChannelApi.UpdateChannelAsync(channelType, channelId, updateRequestBody);

            yield return updateChannelTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(channelId, response.Channel.Id);
                Assert.AreEqual(channelType, response.Channel.Type);
                Assert.IsTrue(response.Members.Any(_ => _.UserId == LowLevelClient.UserId));
            });

            var watchChannelTask = LowLevelClient.ChannelApi.GetOrCreateChannelAsync(channelType, channelId,
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

            var watchChannelTask2 = LowLevelClient.ChannelApi.GetOrCreateChannelAsync(channelType, channelId,
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

                Assert.IsTrue(response.Watchers.Any(_ => _.Id == LowLevelClient.UserId));
            });
        }

        //[UnityTest] StreamTodo: re-enable this test once Watchers issue is resolved
        public IEnumerator When_start_watching_a_channel_expect_user_included_in_watchers()
        {
            yield return ConnectAndExecute(When_start_watching_a_channel_expect_user_included_in_watchers_Async);
        }

        private async Task When_start_watching_a_channel_expect_user_included_in_watchers_Async()
        {
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

            var channelState = await CreateTempUniqueChannelAsync(channelType, getOrCreateRequest);

            //Assert.IsTrue(response.Watchers.Any(_ => _.Id == TestAdminId));
            //It seems that if user starts watching a channel he will not be returned in watchers collection until the next request
            //This might be due to data propagation which may take few ms
            Assert.IsNull(channelState.Watchers);

            var channelId = channelState.Channel.Id;

            var response = await Try(() => LowLevelClient.ChannelApi.GetOrCreateChannelAsync(channelType, channelId,
                new ChannelGetOrCreateRequest
                {
                    Watch = true,
                    State = true,
                    Watchers = new PaginationParamsRequest
                    {
                        Limit = 10
                    }
                }), state => state.Watchers?.Any(_ => _.Id == LowLevelClient.UserId) ?? false);

            Assert.IsNotNull(response.Watchers);
            Assert.AreEqual(channelId, response.Channel.Id);
            Assert.AreEqual(channelType, response.Channel.Type);
            Assert.IsTrue(response.Watchers.Any(_ => _.Id == LowLevelClient.UserId));
        }

        [UnityTest]
        public IEnumerator When_deleting_existing_channel_expect_success() 
            => ConnectAndExecute(When_deleting_existing_channel_expect_success_Async);

        private async Task When_deleting_existing_channel_expect_success_Async()
        {
            const string channelType = "messaging";

            var channelState = await CreateTempUniqueChannelAsync(channelType, new ChannelGetOrCreateRequest());
            var channelId = channelState.Channel.Id;

            var response = await LowLevelClient.ChannelApi.DeleteChannelAsync(channelType, channelId, isHardDelete: false);
            Assert.AreEqual(response.Channel.Id, channelId);
        }

        [UnityTest]
        public IEnumerator When_deleting_non_existing_channel_expect_fail()
        {
            yield return LowLevelClient.WaitForClientToConnect();

            const string channelType = "messaging";

            var createChannelTask =
                CreateTempUniqueChannelAsync(channelType, new ChannelGetOrCreateRequest());

            yield return createChannelTask.RunAsIEnumerator();

            var channelId = createChannelTask.Result.Channel.Id;

            var deleteChannelTask
                = LowLevelClient.ChannelApi.DeleteChannelAsync(channelType, channelId, isHardDelete: false);
            yield return deleteChannelTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(response.Channel.Id, channelId);
            });

            var deleteChannelTask2
                = LowLevelClient.ChannelApi.DeleteChannelAsync(channelType, channelId, isHardDelete: false);

            yield return deleteChannelTask2.RunAsIEnumerator(onFaulted: exception =>
            {
                Assert.AreEqual(((StreamApiException)exception).StatusCode, 404);
            });
        }

        [UnityTest]
        public IEnumerator Delete_multiple_channels()
        {
            yield return LowLevelClient.WaitForClientToConnect();

            var channelType = "messaging";

            var channelsCIdsToDelete = new List<string>();
            yield return CreateTempUniqueChannel(channelType, new ChannelGetOrCreateRequest(),
                state => channelsCIdsToDelete.Add(state.Channel.Cid));
            yield return CreateTempUniqueChannel(channelType, new ChannelGetOrCreateRequest(),
                state => channelsCIdsToDelete.Add(state.Channel.Cid));
            yield return CreateTempUniqueChannel(channelType, new ChannelGetOrCreateRequest(),
                state => channelsCIdsToDelete.Add(state.Channel.Cid));

            Assert.AreEqual(channelsCIdsToDelete.Count, 3);

            var deleteChannelsTask = LowLevelClient.ChannelApi.DeleteChannelsAsync(new DeleteChannelsRequest()
            {
                Cids = channelsCIdsToDelete,
                HardDelete = true
            });

            yield return deleteChannelsTask.RunAsIEnumerator(response =>
            {
                foreach (var cidToDelete in channelsCIdsToDelete)
                {
                    Assert.IsTrue(response.Result.Any(_ => _.Key == cidToDelete));
                    RemoveTempChannelFromDeleteList(cidToDelete);
                }
            });
        }

        [UnityTest]
        public IEnumerator Partial_update()
        {
            yield return LowLevelClient.WaitForClientToConnect();

            var channelType = "messaging";

            ChannelState channelState = null;
            yield return CreateTempUniqueChannel(channelType, new ChannelGetOrCreateRequest(),
                state => channelState = state);
            var channelId = channelState.Channel.Id;

            var updateChannelPartialTask = LowLevelClient.ChannelApi.UpdateChannelPartialAsync(channelType, channelId,
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
                LowLevelClient.ChannelApi.GetOrCreateChannelAsync(channelType, channelId,
                    new ChannelGetOrCreateRequest());

            yield return getOrCreateTask2.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(2, response.Channel.AdditionalProperties["owned_dogs"]);
                Assert.AreEqual(4, response.Channel.AdditionalProperties["owned_hamsters"]);

                //StreamTodo: add support - this array is Newtonsoft.Json.Linq.JArray
                // Assert.That(response.Channel.AdditionalProperties["breakfast"], Contains.Item("oatmeal"));
                // Assert.That(response.Channel.AdditionalProperties["breakfast"], Contains.Item("juice"));
            });

            var updateChannelPartialTask2 = LowLevelClient.ChannelApi.UpdateChannelPartialAsync(channelType, channelId,
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
                LowLevelClient.ChannelApi.GetOrCreateChannelAsync(channelType, channelId,
                    new ChannelGetOrCreateRequest());

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
            yield return LowLevelClient.WaitForClientToConnect();

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
                        UserId = SecondUserId
                    },
                    new ChannelMemberRequest
                    {
                        UserId = LowLevelClient.UserId
                    }
                }
            };

            var updateChannelTask =
                LowLevelClient.ChannelApi.UpdateChannelAsync(channelType, channelState1.Channel.Id, updateRequestBody);
            yield return updateChannelTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(channelType, response.Channel.Type);
                Assert.IsTrue(response.Members.Any(_ => _.UserId == LowLevelClient.UserId));
            });

            var markReadTask = LowLevelClient.ChannelApi.MarkReadAsync(channelState1.Channel.Type,
                channelState1.Channel.Id,
                new MarkReadRequest
                {
                    MessageId = channelState1SecondMessageId
                });

            yield return markReadTask.RunAsIEnumerator(response => { });

            //Query channels to confirm the read state

            var queryChannelsTask = LowLevelClient.ChannelApi.QueryChannelsAsync(new QueryChannelsRequest
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
                    channelState1.Read.FirstOrDefault(_ => _.User.Id == LowLevelClient.UserId);

                Assert.NotNull(localUserChannelState1ReadState);

                //Assert channel unread counts
                Assert.AreEqual(localUserChannelState1ReadState.UnreadMessages, 1);
            });
        }

        [UnityTest]
        public IEnumerator Mark_single_read_without_message_id()
        {
            yield return LowLevelClient.WaitForClientToConnect();

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
                        UserId = SecondUserId
                    },
                    new ChannelMemberRequest
                    {
                        UserId = LowLevelClient.UserId
                    }
                }
            };

            var updateChannelTask =
                LowLevelClient.ChannelApi.UpdateChannelAsync(channelType, channelState1.Channel.Id, updateRequestBody);
            yield return updateChannelTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(channelType, response.Channel.Type);
                Assert.IsTrue(response.Members.Any(_ => _.UserId == LowLevelClient.UserId));
            });

            var markReadTask = LowLevelClient.ChannelApi.MarkReadAsync(channelState1.Channel.Type,
                channelState1.Channel.Id,
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

            var queryChannelsTask = LowLevelClient.ChannelApi.QueryChannelsAsync(queryChannelsRequest);

            // Expect 1 message unread

            yield return queryChannelsTask.RunAsIEnumerator(channelsResponse =>
            {
                Assert.AreEqual(channelsResponse.Channels.Count, 1);

                channelState1 =
                    channelsResponse.Channels.FirstOrDefault(_ => _.Channel.Cid == channelState1.Channel.Cid);

                Assert.NotNull(channelState1);

                var localUserChannelState1ReadState =
                    channelState1.Read.FirstOrDefault(_ => _.User.Id == LowLevelClient.UserId);

                Assert.NotNull(localUserChannelState1ReadState);

                //Assert channel unread counts
                Assert.AreEqual(localUserChannelState1ReadState.UnreadMessages, 1);
            });

            // Update again with message ID not specified - expect whole channel marked as read == 0 unread

            var markReadTask2 = LowLevelClient.ChannelApi.MarkReadAsync(channelState1.Channel.Type,
                channelState1.Channel.Id,
                new MarkReadRequest());

            yield return markReadTask2.RunAsIEnumerator(response => { });

            // Query channels again - expect 0 unread

            var queryChannelsTask2 = LowLevelClient.ChannelApi.QueryChannelsAsync(queryChannelsRequest);

            yield return queryChannelsTask2.RunAsIEnumerator(channelsResponse =>
            {
                Assert.AreEqual(channelsResponse.Channels.Count, 1);

                channelState1 =
                    channelsResponse.Channels.FirstOrDefault(_ => _.Channel.Cid == channelState1.Channel.Cid);

                Assert.NotNull(channelState1);

                var localUserChannelState1ReadState =
                    channelState1.Read.FirstOrDefault(_ => _.User.Id == LowLevelClient.UserId);

                Assert.NotNull(localUserChannelState1ReadState);

                //Assert channel unread counts
                Assert.AreEqual(localUserChannelState1ReadState.UnreadMessages, 0);
            });
        }


        private async Task Mark_many_read_with_specified_message_id_Async()
        {
            const string channelType = "messaging";
            
            await LowLevelClient.WaitForClientToConnectAsync();

            // Create channels
            var channel1 = await CreateTempUniqueChannelAsync(channelType, new ChannelGetOrCreateRequest());
            var channel2 = await CreateTempUniqueChannelAsync(channelType, new ChannelGetOrCreateRequest());
            
            // Send messages

            Task<MessageResponse> SendTestMessageAsync(string type, string id, string msg)
            {
                return LowLevelClient.MessageApi.SendNewMessageAsync(type,
                    id, new SendMessageRequest
                    {
                        Message = new MessageRequest
                        {
                            Text = msg
                        }
                    });
                ;
            }
            
            var channel1MessageResponse1 = await SendTestMessageAsync(channel1.Channel.Type, channel1.Channel.Id, "#1");
            var channel1MessageResponse2 = await SendTestMessageAsync(channel1.Channel.Type, channel1.Channel.Id, "#2");
            var channel2MessageResponse1 = await SendTestMessageAsync(channel2.Channel.Type, channel2.Channel.Id, "#3");
            var channel2MessageResponse2 = await SendTestMessageAsync(channel2.Channel.Type, channel2.Channel.Id, "#4");
            
            // Join as members
            var updateRequestBody = new UpdateChannelRequest
            {
                AddMembers = new List<ChannelMemberRequest>
                {
                    new ChannelMemberRequest
                    {
                        UserId = SecondUserId
                    },
                    new ChannelMemberRequest
                    {
                        UserId = LowLevelClient.UserId
                    }
                }
            };

            var updateChannelResponse = await
                LowLevelClient.ChannelApi.UpdateChannelAsync(channelType, channel1.Channel.Id, updateRequestBody);
            
            Assert.AreEqual(channelType, updateChannelResponse.Channel.Type);
            Assert.IsTrue(updateChannelResponse.Members.Any(_ => _.UserId == LowLevelClient.UserId));
            
            var updateChannel2Response =
                await LowLevelClient.ChannelApi.UpdateChannelAsync(channelType, channel2.Channel.Id, updateRequestBody);
            Assert.AreEqual(channelType, updateChannel2Response.Channel.Type);
            Assert.IsTrue(updateChannel2Response.Members.Any(_ => _.UserId == LowLevelClient.UserId));
            
            // Send mark read state

            var markReadResponse = await LowLevelClient.ChannelApi.MarkManyReadAsync(new MarkChannelsReadRequest
            {
                ReadByChannel = new Dictionary<string, string>
                {
                    { channel1.Channel.Cid, channel1MessageResponse1.Message.Id },
                    { channel2.Channel.Cid, channel2MessageResponse1.Message.Id },
                }
            });
            
            // Query channels to get the latest state

            await Task.Delay(15 * 1000);
            
            var queryChannelsResponse = await LowLevelClient.ChannelApi.QueryChannelsAsync(new QueryChannelsRequest
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
                                    channel1.Channel.Cid, channel2.Channel.Cid
                                }
                            }
                        }
                    }
                },
            });
            
            Assert.AreEqual(queryChannelsResponse.Channels.Count, 2);

            channel1 =
                queryChannelsResponse.Channels.FirstOrDefault(_ => _.Channel.Cid == channel1.Channel.Cid);
            channel2 =
                queryChannelsResponse.Channels.FirstOrDefault(_ => _.Channel.Cid == channel2.Channel.Cid);

            Assert.NotNull(channel1);
            Assert.NotNull(channel2);

            var localUserChannelState1ReadState =
                channel1.Read.FirstOrDefault(_ => _.User.Id == LowLevelClient.UserId);
            var localUserChannelState2ReadState =
                channel2.Read.FirstOrDefault(_ => _.User.Id == LowLevelClient.UserId);

            Assert.NotNull(localUserChannelState1ReadState);
            Assert.NotNull(localUserChannelState2ReadState);

            //Assert channel unread counts
            Assert.AreEqual(channel2MessageResponse1.Message.Id, localUserChannelState2ReadState.LastReadMessageId);
            
            Assert.AreEqual(2, localUserChannelState1ReadState.UnreadMessages);
            Assert.AreEqual(1, localUserChannelState2ReadState.UnreadMessages);
            
            await ReconnectClientAsync();

            //Should use Assert.AreEqual but there seems to be some delay with updating the values
            Assert.IsNotNull(LowLevelClientOwnUser);
            Assert.GreaterOrEqual(LowLevelClientOwnUser.UnreadChannels, 2);
            Assert.GreaterOrEqual(LowLevelClientOwnUser.TotalUnreadCount, 3);
        }
        
        /// <summary>
        /// 1. Create 2 channels with 2 messages each
        /// 2. Mark first, and second as read for each channel respectively
        /// 3. query channels and validate 2, 1 unread messages respectively
        /// </summary>
        [UnityTest]
        public IEnumerator Mark_many_read_with_specified_message_id()
        {
            yield return Mark_many_read_with_specified_message_id_Async().RunAsIEnumerator();
        }

        [UnityTest]
        public IEnumerator When_sending_typing_start_stop_events_expect_no_errors()
        {
            yield return ConnectAndExecute(When_sending_typing_start_stop_events_expect_no_exceptions_Async);
        }

        private async Task When_sending_typing_start_stop_events_expect_no_exceptions_Async()
        {
            const string channelType = "messaging";
            var tempChannel = await CreateTempUniqueChannelAsync(channelType, new ChannelGetOrCreateRequest());

            await LowLevelClient.ChannelApi.SendTypingStartEventAsync(channelType, tempChannel.Channel.Id);
            await LowLevelClient.ChannelApi.SendTypingStopEventAsync(channelType, tempChannel.Channel.Id);
        }
    }
}
#endif