using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using StreamChat.Core.State.Requests;
using StreamChat.Core.State.TrackedObjects;
using UnityEngine.TestTools;

namespace StreamChat.Tests.StatefulClient
{
    /// <summary>
    /// Tests operations executed on <see cref="StreamUser"/>
    /// </summary>
    public class UsersTests : BaseStateIntegrationTests
    {
        [UnityTest]
        public IEnumerator When_upsert_user_expect_valid_user_returned()
            => ConnectAndExecute(When_upsert_user_expect_valid_user_returned_Async);

        private async Task When_upsert_user_expect_valid_user_returned_Async()
        {
            var userId = "temp-test-user-" + Guid.NewGuid();
            var createNewUserRequest = new StreamUserUpsertRequest
            {
                Id = userId,
                Role = "user",
                Name = "David",
                CustomData = new Dictionary<string, object>
                {
                    { "Age", 24 },
                    { "Passions", new string[] { "Tennis", "Football", "Basketball" } }
                }
            };

            var response = await StatefulClient.UpsertUsers(new[] { createNewUserRequest });
            var davidUser = response.FirstOrDefault();

            Assert.NotNull(davidUser);
            Assert.AreEqual(userId, davidUser.Id);
            Assert.AreEqual(24, davidUser.GetCustomData("Age"));

            //StreamTodo: solve this, because array is JArray we can't do much
            var davidPassions = davidUser.GetCustomData("Passions");
            //var sequenceEqual = davidPassions.SequenceEqual(new string[] { "Tennis", "Football", "Basketball" });
            //Assert.IsTrue(sequenceEqual);
        }
    }
}