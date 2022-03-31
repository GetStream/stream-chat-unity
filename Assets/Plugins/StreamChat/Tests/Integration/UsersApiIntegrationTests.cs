using System.Collections;
using System.Collections.Generic;
using System.ServiceModel.Discovery;
using NUnit.Framework;
using StreamChat.Core.Requests;
using UnityEngine.TestTools;

namespace StreamChat.Tests.Integration
{
    /// <summary>
    /// Integration tests for Users
    /// </summary>
    public class UsersApiIntegrationTests : BaseIntegrationTests
    {
        [UnityTest]
        public IEnumerator Upsert_users()
        {
            yield return Client.WaitForClientToConnect();

            var upsertUsersTask = Client.UserApi.UpdateUsersAsync(new UpdateUsersRequest()
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
    }
}