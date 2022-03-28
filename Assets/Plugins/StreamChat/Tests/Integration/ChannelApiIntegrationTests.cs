using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using StreamChat.Core;
using StreamChat.Core.Auth;
using StreamChat.Core.Exceptions;
using StreamChat.Core.Requests;
using UnityEngine;
using UnityEngine.TestTools;

namespace StreamChat.Tests.Integration
{
    /// <summary>
    /// Integration tests that operate on API and make real API requests
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class ChannelApiIntegrationTests
    {
        [SetUp]
        public void Up()
        {
            const string ApiKey = "";

            var guestAuthCredentials = new AuthCredentials(
                apiKey: ApiKey,
                userId: TestGuestId,
                userToken: "");

            var userAuthCredentials = new AuthCredentials(
                apiKey: ApiKey,
                userId: TestUserId,
                userToken: "");

            var adminAuthCredentials = new AuthCredentials(
                apiKey: ApiKey,
                userId: TestAdminId,
                userToken: "");

            _client = StreamChatClient.CreateDefaultClient(adminAuthCredentials);
            _client.Connect();
        }

        private const string TestUserId = "integration-tests-role-user";
        private const string TestAdminId = "integration-tests-role-admin";
        private const string TestGuestId = "integration-tests-role-guest";

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _client = null;
        }

        //[UnityTest]
        public IEnumerator Get_or_create_channel()
        {
            yield return _client.WaitForClientToConnect();

            var request = new ChannelGetOrCreateRequest();

            var channelType = "messaging";
            var channelId = "new-channel-id-1";

            var task = _client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId, request);

            yield return task.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(channelId, response.Channel.Id);
                Assert.AreEqual(channelType, response.Channel.Type);
            });
        }

        //[UnityTest]
        public IEnumerator Get_or_create_channel_for_list_of_members()
        {
            yield return _client.WaitForClientToConnect();

            var request = new ChannelGetOrCreateRequest
            {
                Data = new ChannelRequest
                {
                    Members = new List<ChannelMemberRequest>
                    {
                        new ChannelMemberRequest
                        {
                            UserId = TestUserId
                        },
                        new ChannelMemberRequest
                        {
                            UserId = TestAdminId
                        },
                    }
                }
            };

            string channelType = "messaging";

            var task = _client.ChannelApi.GetOrCreateChannelAsync(channelType, request);

            yield return task.RunAsIEnumerator(response => { Assert.AreEqual(channelType, response.Channel.Type); });
        }

        //[UnityTest]
        public IEnumerator Delete_channel()
        {
            yield return _client.WaitForClientToConnect();

            var channelType = "messaging";
            var channelId = "new-channel-id-1";

            var createChannelTask =
                _client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId, new ChannelGetOrCreateRequest());

            yield return createChannelTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(channelId, response.Channel.Id);
                Assert.AreEqual(channelType, response.Channel.Type);
            });

            var deleteChannelTask = _client.ChannelApi.DeleteChannelAsync(channelType, channelId);

            yield return deleteChannelTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(channelId, response.Channel.Id);
                Assert.AreEqual(channelType, response.Channel.Type);
            });
        }

        //[UnityTest]
        public IEnumerator Query_channels()
        {
            yield return _client.WaitForClientToConnect();

            var channelType = "messaging";
            var channelId = "new-channel-id-1";
            var channelId2 = "new-channel-id-2";

            //Create 2 channels with admin being member of one of them

            var createChannelTask =
                _client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId, new ChannelGetOrCreateRequest());

            yield return createChannelTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(channelId, response.Channel.Id);
                Assert.AreEqual(channelType, response.Channel.Type);
            });

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

            var createChannelTask2 =
                _client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId2, getOrCreateRequest2);

            yield return createChannelTask2.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(channelId2, response.Channel.Id);
                Assert.AreEqual(channelType, response.Channel.Type);
            });

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

            var queryChannelsTask = _client.ChannelApi.QueryChannelsAsync(queryChannelsRequest);

            yield return queryChannelsTask.RunAsIEnumerator(response =>
            {
                var allChannelsContainAdminUser = response.Channels.All(channelState =>
                    channelState.Members.Any(member => member.UserId == TestAdminId));

                Assert.IsTrue(allChannelsContainAdminUser);
            });
        }

        //[UnityTest]
        public IEnumerator Watch_channel()
        {
            yield return _client.WaitForClientToConnect();

            Assert.AreEqual(_client.UserId, TestAdminId);

            var channelType = "messaging";
            var channelId = "new-channel-id-1";

            var createChannelTask =
                _client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId, new ChannelGetOrCreateRequest());

            yield return createChannelTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(channelId, response.Channel.Id);
                Assert.AreEqual(channelType, response.Channel.Type);
            });

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

            var updateChannelTask = _client.ChannelApi.UpdateChannelAsync(channelType, channelId, updateRequestBody);

            yield return updateChannelTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(channelId, response.Channel.Id);
                Assert.AreEqual(channelType, response.Channel.Type);
                Assert.IsTrue(response.Members.Any(_ => _.UserId == TestAdminId));
            });

            var watchChannelTask = _client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId,
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

            var watchChannelTask2 = _client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId,
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

                Assert.IsTrue(response.Watchers.Any(_ => _.Id == TestAdminId));
            });
        }

        //[UnityTest]
        public IEnumerator Watcher_count()
        {
            yield return _client.WaitForClientToConnect();

            Assert.AreEqual(_client.UserId, TestAdminId);

            var channelType = "messaging";
            var channelId = "new-channel-id-1";

            //Delete channel to remove any previous information
            var deleteChannelTask = _client.ChannelApi.DeleteChannelAsync(channelType, channelId);

            yield return deleteChannelTask.RunAsIEnumerator(onFaulted: exception => {
                //ignore if deletion failed
            });

            var watchChannelTask = _client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId,
                new ChannelGetOrCreateRequest
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

                //Assert.IsTrue(response.Watchers.Any(_ => _.Id == TestAdminId));
                //It seems that if users starts watching a channel he will not be returned in watchers collection until the next request
                Assert.IsNull(response.Watchers);
            });

            var watchChannelTask2 = _client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId,
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
                Assert.IsTrue(response.Watchers.Any(_ => _.Id == TestAdminId));
            });
        }

        //[UnityTest]
        public IEnumerator When_deleting_existing_channel_expect_success()
        {
            yield return _client.WaitForClientToConnect();

            var channelType = "messaging";
            var channelId = "new-channel-id-1";

            var createChannelTask = _client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId, new ChannelGetOrCreateRequest());

            yield return createChannelTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(response.Channel.Id, channelId);
            });

            var deleteChannelTask = _client.ChannelApi.DeleteChannelAsync(channelType, channelId);
            yield return deleteChannelTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(response.Channel.Id, channelId);
            });
        }

        //[UnityTest]
        public IEnumerator When_deleting_non_existing_channel_expect_fail()
        {
            yield return _client.WaitForClientToConnect();

            var channelType = "messaging";
            var channelId = "new-channel-id-1";

            var createChannelTask = _client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId, new ChannelGetOrCreateRequest());

            yield return createChannelTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(response.Channel.Id, channelId);
            });

            var deleteChannelTask = _client.ChannelApi.DeleteChannelAsync(channelType, channelId);
            yield return deleteChannelTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(response.Channel.Id, channelId);
            });

            var deleteChannelTask2 = _client.ChannelApi.DeleteChannelAsync(channelType, channelId);

            deleteChannelTask2.RunAsIEnumerator(onFaulted: exception =>
            {
                Assert.AreEqual(((StreamApiException)exception).StatusCode, 404);
            });
        }

        //[UnityTest]
        public IEnumerator Delete_multiple_channels()
        {
            yield return _client.WaitForClientToConnect();

            var channelType = "messaging";
            var channelId = "new-channel-id-1";
            var channelId2 = "new-channel-id-2";
            var channelId3 = "new-channel-id-3";

            var channelsIdsToDelete = new List<string>();

            var createChannelTask = _client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId, new ChannelGetOrCreateRequest());

            yield return createChannelTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(response.Channel.Id, channelId);
                channelsIdsToDelete.Add(response.Channel.Cid);
            });

            var createChannelTask2 = _client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId2, new ChannelGetOrCreateRequest());

            yield return createChannelTask2.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(response.Channel.Id, channelId2);
                channelsIdsToDelete.Add(response.Channel.Cid);
            });

            var createChannelTask3 = _client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId3, new ChannelGetOrCreateRequest());

            yield return createChannelTask3.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(response.Channel.Id, channelId3);
                channelsIdsToDelete.Add(response.Channel.Cid);
            });

            Assert.AreEqual(channelsIdsToDelete.Count, 3);

            var deleteChannelsTask = _client.ChannelApi.DeleteChannelsAsync(new DeleteChannelsRequest()
            {
                Cids = channelsIdsToDelete,
                HardDelete = true
            });

            yield return deleteChannelsTask.RunAsIEnumerator(response =>
            {
                Assert.IsTrue(response.Result.Any(_ => _.Key == $"{channelType}:{channelId}"));
                Assert.IsTrue(response.Result.Any(_ => _.Key == $"{channelType}:{channelId2}"));
                Assert.IsTrue(response.Result.Any(_ => _.Key == $"{channelType}:{channelId3}"));
            });
        }

        private IStreamChatClient _client;
    }
}