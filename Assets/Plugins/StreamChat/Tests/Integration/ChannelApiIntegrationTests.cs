using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using StreamChat.Core;
using StreamChat.Core.Auth;
using StreamChat.Core.Requests;
using UnityEngine;
using UnityEngine.TestTools;

namespace StreamChat.Tests.Integration
{
    /// <summary>
    /// Integration tests that operate on API and make real API requests
    /// </summary>
    /// <remarks>
    /// You need to provide valid credentials and uncomment [UnityTest] attribute in order to launch these tests
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

            yield return task.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(channelType, response.Channel.Type);
            });
        }

        //[UnityTest]
        public IEnumerator Delete_channel()
        {
            var channelType = "messaging";
            var channelId = "new-channel-id-1";

            var createChannelTask = _client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId, new ChannelGetOrCreateRequest());

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
            var channelType = "messaging";
            var channelId = "new-channel-id-1";
            var channelId2 = "new-channel-id-2";

            //Create 2 channels with admin being member of one of them

            var createChannelTask = _client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId, new ChannelGetOrCreateRequest());

            yield return createChannelTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(channelId, response.Channel.Id);
                Assert.AreEqual(channelType, response.Channel.Type);
            });

            _client.Update(Time.deltaTime);

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

            var createChannelTask2 = _client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId2, getOrCreateRequest2);

            yield return createChannelTask2.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(channelId2, response.Channel.Id);
                Assert.AreEqual(channelType, response.Channel.Type);
            });

            _client.Update(Time.deltaTime);

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

            //Create channel

            var createChannelTask = _client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId, new ChannelGetOrCreateRequest());

            yield return createChannelTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(channelId, response.Channel.Id);
                Assert.AreEqual(channelType, response.Channel.Type);
            });

            _client.Update(Time.deltaTime);

            //Add channel member

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

            _client.Update(Time.deltaTime);

            var updateChannelTask = _client.ChannelApi.UpdateChannelAsync(channelType, channelId, updateRequestBody);

            yield return updateChannelTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(channelId, response.Channel.Id);
                Assert.AreEqual(channelType, response.Channel.Type);
                Assert.IsTrue(response.Members.Any(_ => _.UserId == TestAdminId));
            });

            _client.Update(Time.deltaTime);

            var watchChannelTask = _client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId, new ChannelGetOrCreateRequest()
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
                Assert.IsTrue(response.Watchers.Any(_ => _.Id == TestAdminId));
            });
        }

        private IStreamChatClient _client;
    }
}