using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using StreamChat.Core.Exceptions;
using StreamChat.Core.Requests;
using StreamChat.Libs.Utils;
using UnityEngine;
using UnityEngine.TestTools;

namespace StreamChat.Tests.Integration
{
    /// <summary>
    /// Integration tests for Channels
    /// </summary>
    public class ChannelApiIntegrationTests : BaseIntegrationTests
    {
        //[UnityTest]
        public IEnumerator Get_or_create_channel()
        {
            yield return Client.WaitForClientToConnect();

            var request = new ChannelGetOrCreateRequest();

            var channelType = "messaging";
            var channelId = "new-channel-id-1";

            var task = Client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId, request);

            yield return task.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(channelId, response.Channel.Id);
                Assert.AreEqual(channelType, response.Channel.Type);
            });
        }

        //[UnityTest]
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

            var task = Client.ChannelApi.GetOrCreateChannelAsync(channelType, request);

            yield return task.RunAsIEnumerator(response => { Assert.AreEqual(channelType, response.Channel.Type); });
        }

        //[UnityTest]
        public IEnumerator Delete_channel()
        {
            yield return Client.WaitForClientToConnect();

            var channelType = "messaging";
            var channelId = "new-channel-id-1";

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

        //[UnityTest]
        public IEnumerator Query_channels()
        {
            yield return Client.WaitForClientToConnect();

            var channelType = "messaging";
            var channelId = "new-channel-id-1";
            var channelId2 = "new-channel-id-2";

            //Create 2 channels with admin being member of one of them

            var createChannelTask =
                Client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId, new ChannelGetOrCreateRequest());

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
                Client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId2, getOrCreateRequest2);

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

            var queryChannelsTask = Client.ChannelApi.QueryChannelsAsync(queryChannelsRequest);

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
            yield return Client.WaitForClientToConnect();

            Assert.AreEqual(Client.UserId, TestAdminId);

            var channelType = "messaging";
            var channelId = "new-channel-id-1";

            var createChannelTask =
                Client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId, new ChannelGetOrCreateRequest());

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

                Assert.IsTrue(response.Watchers.Any(_ => _.Id == TestAdminId));
            });
        }

        //[UnityTest]
        public IEnumerator Watcher_count()
        {
            yield return Client.WaitForClientToConnect();

            Assert.AreEqual(Client.UserId, TestAdminId);

            var channelType = "messaging";
            var channelId = "new-channel-id-1";

            //Delete channel to remove any previous information
            var deleteChannelTask = Client.ChannelApi.DeleteChannelAsync(channelType, channelId);

            yield return deleteChannelTask.RunAsIEnumerator(onFaulted: exception => {
                //ignore if deletion failed
            });

            var watchChannelTask = Client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId,
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
                Assert.IsTrue(response.Watchers.Any(_ => _.Id == TestAdminId));
            });
        }

        //[UnityTest]
        public IEnumerator When_deleting_existing_channel_expect_success()
        {
            yield return Client.WaitForClientToConnect();

            var channelType = "messaging";
            var channelId = "new-channel-id-1";

            var createChannelTask = Client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId, new ChannelGetOrCreateRequest());

            yield return createChannelTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(response.Channel.Id, channelId);
            });

            var deleteChannelTask = Client.ChannelApi.DeleteChannelAsync(channelType, channelId);
            yield return deleteChannelTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(response.Channel.Id, channelId);
            });
        }

        //[UnityTest]
        public IEnumerator When_deleting_non_existing_channel_expect_fail()
        {
            yield return Client.WaitForClientToConnect();

            var channelType = "messaging";
            var channelId = "new-channel-id-1";

            var createChannelTask = Client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId, new ChannelGetOrCreateRequest());

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

        //[UnityTest]
        public IEnumerator Delete_multiple_channels()
        {
            yield return Client.WaitForClientToConnect();

            var channelType = "messaging";
            var channelId = "new-channel-id-1";
            var channelId2 = "new-channel-id-2";
            var channelId3 = "new-channel-id-3";

            var channelsIdsToDelete = new List<string>();

            var createChannelTask = Client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId, new ChannelGetOrCreateRequest());

            yield return createChannelTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(response.Channel.Id, channelId);
                channelsIdsToDelete.Add(response.Channel.Cid);
            });

            var createChannelTask2 = Client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId2, new ChannelGetOrCreateRequest());

            yield return createChannelTask2.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(response.Channel.Id, channelId2);
                channelsIdsToDelete.Add(response.Channel.Cid);
            });

            var createChannelTask3 = Client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId3, new ChannelGetOrCreateRequest());

            yield return createChannelTask3.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(response.Channel.Id, channelId3);
                channelsIdsToDelete.Add(response.Channel.Cid);
            });

            Assert.AreEqual(channelsIdsToDelete.Count, 3);

            var deleteChannelsTask = Client.ChannelApi.DeleteChannelsAsync(new DeleteChannelsRequest()
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

        //[UnityTest]
        public IEnumerator Partial_update()
        {
            yield return Client.WaitForClientToConnect();

            var channelType = "messaging";
            var channelId = "new-channel-id-1";

            //Clear all previous data
            var deleteChannelTask = Client.ChannelApi.DeleteChannelsAsync(new DeleteChannelsRequest
            {
                Cids = new List<string>()
                {
                    $"{channelType}:{channelId}"
                },
                HardDelete = true
            });

            yield return deleteChannelTask.RunAsIEnumerator(onSuccess: response =>
            {
                if (!response.TaskId.IsNullOrEmpty())
                {
                    Debug.LogError("TASK ID: " + response.TaskId);
                }
            },onFaulted: exception => {
                //ignore if deletion failed
            });

            var getOrCreateTask =
                Client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId, new ChannelGetOrCreateRequest());

            yield return getOrCreateTask.RunAsIEnumerator();

            var updateChannelPartialTask = Client.ChannelApi.UpdateChannelPartialAsync(channelType, channelId, new UpdateChannelPartialRequest
            {
                Set = new Dictionary<string, object>
                {
                    {"owned_dogs", 2},
                    {"owned_hamsters", 4},
                    {"breakfast", new string[]
                    {
                        "oatmeal", "juice"
                    }}
                }
            });

            yield return updateChannelPartialTask.RunAsIEnumerator(response =>
            {

            });

            var getOrCreateTask2 =
                Client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId, new ChannelGetOrCreateRequest());

            yield return getOrCreateTask2.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(2, response.Channel.AdditionalProperties["owned_dogs"]);
                Assert.AreEqual(4, response.Channel.AdditionalProperties["owned_hamsters"]);

                //Todo: add support - this array is Newtonsoft.Json.Linq.JArray
                // Assert.That(response.Channel.AdditionalProperties["breakfast"], Contains.Item("oatmeal"));
                // Assert.That(response.Channel.AdditionalProperties["breakfast"], Contains.Item("juice"));
            });

            var updateChannelPartialTask2 = Client.ChannelApi.UpdateChannelPartialAsync(channelType, channelId, new UpdateChannelPartialRequest
            {
                Set = new Dictionary<string, object>
                {
                    {"owned_dogs", 5},
                    {"breakfast", new string[]
                    {
                        "donuts"
                    }}
                },
                Unset = new List<string>
                {
                    "owned_hamsters"
                }
            });

            yield return updateChannelPartialTask2.RunAsIEnumerator(response =>
            {

            });

            var getOrCreateTask3 =
                Client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId, new ChannelGetOrCreateRequest());

            yield return getOrCreateTask3.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(5, response.Channel.AdditionalProperties["owned_dogs"]);
                Assert.That(response.Channel.AdditionalProperties, Does.Not.Contain("owned_hamsters"));

                //Todo: add support - this array is Newtonsoft.Json.Linq.JArray
                //Assert.That(response.Channel.AdditionalProperties["breakfast"], Contains.Item("donuts"));
                //Assert.That(response.Channel.AdditionalProperties["breakfast"], Has.Exactly(1).Count);
            });
        }
    }
}