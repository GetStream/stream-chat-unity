#if STREAM_TESTS_ENABLED
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using StreamChat.Core.QueryBuilders.Filters;
using StreamChat.Core.QueryBuilders.Filters.Users;
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
            var users = await Client.QueryUsersAsync(Enumerable.Empty<IFieldFilterRule>());
        }
        
        [UnityTest]
        public IEnumerator When_query_banned_users_expect_valid_results()
            => ConnectAndExecute(When_query_banned_users_expect_valid_results_Async);

        private async Task When_query_banned_users_expect_valid_results_Async()
        {
            var channel1 = await CreateUniqueTempChannelAsync();
            var userSteven = await CreateUniqueTempUserAsync("Steven");
            var userAlexy = await CreateUniqueTempUserAsync("Alexy");

            await channel1.JoinAsMemberAsync();
            await channel1.BanUserAsync(userSteven);

            var bannedUsers = (await Client.QueryBannedUsersAsync(new StreamQueryBannedUsersRequest()
            {
                FilterConditions = new Dictionary<string, object>
                {
                    {
                        "channel_cid", channel1.Cid
                    }
                },
                Sort = new List<StreamSortParam>()
                {
                    new StreamSortParam()
                    {
                        Field = "created_at",
                        Direction = -1
                    }
                }
            })).ToArray();

            Assert.IsNotEmpty(bannedUsers);
            Assert.IsTrue(bannedUsers.Any(b => b.User == userSteven));
        }

        [UnityTest]
        public IEnumerator When_query_users_with_id_autocomplete_expect_valid_results()
            => ConnectAndExecute(When_query_users_with_id_autocomplete_expect_valid_results_Async);

        private async Task When_query_users_with_id_autocomplete_expect_valid_results_Async()
        {
            var userAnna = await CreateUniqueTempUserAsync("Anna");
            var userMike = await CreateUniqueTempUserAsync("Mike");

            var filters = new IFieldFilterRule[]
            {
                UserFilter.Name.Autocomplete("Ann"),
                //StreamTodo: uncomment when created_at issue is resolved
                //UserFilter.CreatedAt.GreaterThanOrEquals(DateTime.Now.AddMinutes(-5))
            };

            var users = await Client.QueryUsersAsync(filters);

            var usersArr = users.ToArray();

            Assert.IsTrue(usersArr.All(u => u.Name.ToLower().StartsWith("ann")));
            Assert.IsNull(usersArr.FirstOrDefault(u => u.Name.StartsWith(userMike.Name)));
        }
    }
}
#endif