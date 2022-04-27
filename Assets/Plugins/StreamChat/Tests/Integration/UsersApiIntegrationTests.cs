using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using StreamChat.Core.Models;
using StreamChat.Core.Requests;
using UnityEngine;
using UnityEngine.TestTools;

namespace StreamChat.Tests.Integration
{
    /// <summary>
    /// Integration tests for Users
    /// </summary>
    public class UsersApiIntegrationTests : BaseIntegrationTests
    {
        //[UnityTest]
        public IEnumerator Upsert_users()
        {
            yield return Client.WaitForClientToConnect();

            var upsertUsersTask = Client.UserApi.UpsertManyUsersAsync(new UpdateUsersRequest()
            {
                Users = new Dictionary<string, UserObjectRequest>
                {
                    {
                        "new-sample-user-1", new UserObjectRequest
                        {
                            Id = "new-sample-user-1",
                            Role = "user",
                            AdditionalProperties = new Dictionary<string, object>()
                            {
                                { "cat_name", "Fluffy" }
                            }
                        }
                    },
                    {
                        "new-sample-user-2", new UserObjectRequest
                        {
                            Id = "new-sample-user-2",
                            Role = "guest",
                            AdditionalProperties = new Dictionary<string, object>()
                            {
                                { "cat_name", "Pushen" }
                            }
                        }
                    }
                }
            });

            yield return upsertUsersTask.RunAsIEnumerator(response =>
            {
                Assert.That(response.Users, Contains.Key("new-sample-user-1"));
                Assert.That(response.Users, Contains.Key("new-sample-user-2"));

                var user1 = response.Users["new-sample-user-1"];
                var user2 = response.Users["new-sample-user-2"];

                Assert.AreEqual(user1.Role, "user");
                Assert.AreEqual(user1.AdditionalProperties["cat_name"], "Fluffy");
                Assert.AreEqual(user2.Role, "guest");
                Assert.AreEqual(user2.AdditionalProperties["cat_name"], "Pushen");
            });
        }

        //[UnityTest]
        public IEnumerator Query_users()
        {
            yield return Client.WaitForClientToConnect();

            var updateUsersTask = Client.UserApi.UpsertManyUsersAsync(new UpdateUsersRequest()
            {
                Users = new Dictionary<string, UserObjectRequest>()
                {
                    {
                        "new-user-1", new UserObjectRequest
                        {
                            Id = "new-user-1"
                        }
                    },
                    {
                        "new-user-2", new UserObjectRequest
                        {
                            Id = "new-user-2"
                        }
                    },
                }
            });

            yield return updateUsersTask.RunAsIEnumerator(response => { });

            var updateUsersTask2 = Client.UserApi.UpsertManyUsersAsync(new UpdateUsersRequest()
            {
                Users = new Dictionary<string, UserObjectRequest>()
                {
                    {
                        "new-user-3", new UserObjectRequest
                        {
                            Id = "new-user-3"
                        }
                    },
                    {
                        "new-user-4", new UserObjectRequest
                        {
                            Id = "new-user-4"
                        }
                    },
                }
            });

            yield return updateUsersTask2.RunAsIEnumerator(response => { });

            var queryUsersTask = Client.UserApi.QueryUsersAsync(new QueryUsersRequest
            {
                FilterConditions = new Dictionary<string, object>
                {
                    {
                        "id", new Dictionary<string, object>
                        {
                            {
                                "$in", new List<string>
                                {
                                    "new-user-1", "new-user-2", "new-user-3", "new-user-4"
                                }
                            }
                        }
                    }
                },
                Limit = 30,
                Offset = 0,

                Sort = new List<SortParam>
                {
                    new SortParam
                    {
                        Field = "id",
                        Direction = -1,
                    }
                }
            });

            yield return queryUsersTask.RunAsIEnumerator(response => { });
        }

        //[UnityTest]
        public IEnumerator Query_banned_users()
        {
            yield return Client.WaitForClientToConnect();

            var updateUsersTask = Client.UserApi.UpsertManyUsersAsync(new UpdateUsersRequest()
            {
                Users = new Dictionary<string, UserObjectRequest>()
                {
                    {
                        "new-user-1", new UserObjectRequest
                        {
                            Id = "new-user-1"
                        }
                    },
                    {
                        "new-user-2", new UserObjectRequest
                        {
                            Id = "new-user-2"
                        }
                    },
                }
            });

            yield return updateUsersTask.RunAsIEnumerator(response => { });

            var updateUsersTask2 = Client.UserApi.UpsertManyUsersAsync(new UpdateUsersRequest()
            {
                Users = new Dictionary<string, UserObjectRequest>()
                {
                    {
                        "new-user-3", new UserObjectRequest
                        {
                            Id = "new-user-3"
                        }
                    },
                    {
                        "new-user-4", new UserObjectRequest
                        {
                            Id = "new-user-4"
                        }
                    },
                }
            });

            yield return updateUsersTask2.RunAsIEnumerator(response => { });

            //TOdo implement Ban User

            var queryUsersTask = Client.UserApi.QueryUsersAsync(new QueryUsersRequest
            {
                FilterConditions = new Dictionary<string, object>
                {
                    {
                        "banned", new Dictionary<string, object>
                        {
                            {
                                "$eq", true
                            }
                        }
                    }
                },
                Limit = 30,
                Offset = 0,

                Sort = new List<SortParam>
                {
                    new SortParam
                    {
                        Field = "id",
                        Direction = -1,
                    }
                }
            });

            yield return queryUsersTask.RunAsIEnumerator(response => { });
        }
    }
}