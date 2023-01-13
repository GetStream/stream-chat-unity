#if STREAM_TESTS_ENABLED
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using StreamChat.Core.Requests;
using StreamChat.Core.StatefulModels;
using UnityEngine.TestTools;

namespace StreamChat.Tests.StatefulClient
{
    /// <summary>
    /// Tests operations executed on <see cref="IStreamUser"/>
    /// </summary>
    internal class UsersTests : BaseStateIntegrationTests
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
                CustomData = new StreamCustomDataRequest
                {
                    { "Age", 24 },
                    { "Passions", new string[] { "Tennis", "Football", "Basketball" } }
                }
            };

            var response = await Client.UpsertUsers(new[] { createNewUserRequest });
            var davidUser = response.FirstOrDefault();

            Assert.NotNull(davidUser);
            Assert.AreEqual(userId, davidUser.Id);
            Assert.AreEqual(24, davidUser.CustomData.Get<int>("Age"));

            var davidPassions = davidUser.CustomData.Get<string[]>("Passions");
            var sequenceEqual = davidPassions.SequenceEqual(new string[] { "Tennis", "Football", "Basketball" });
            Assert.IsTrue(sequenceEqual);
        }
        
        [UnityTest]
        public IEnumerator When_query_users_without_parameters_expect_no_exception()
            => ConnectAndExecute(When_query_users_without_parameters_expect_no_exception_Async);

        private async Task When_query_users_without_parameters_expect_no_exception_Async()
        {
            var users = await Client.QueryUsersAsync();
        }
    }
}
#endif