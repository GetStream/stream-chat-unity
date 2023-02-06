#if STREAM_TESTS_ENABLED
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using StreamChat.Core;
using StreamChat.Core.QueryBuilders.Filters;
using StreamChat.Core.QueryBuilders.Filters.Users;
using StreamChat.Core.QueryBuilders.Sort;
using UnityEngine.TestTools;

namespace StreamChat.Tests.StatefulClient
{
    /// <summary>
    /// Filter tests for <see cref="IStreamChatClient.QueryUsersAsync"/>
    /// </summary>
    internal class UsersQueryFiltersTests : BaseStateIntegrationTests
    {
        [UnityTest]
        public IEnumerator When_query_users_with_name_eq_filter_expect_valid_results()
            => ConnectAndExecute(When_query_users_with_name_eq_filter_expect_valid_results_Async);

        private async Task When_query_users_with_name_eq_filter_expect_valid_results_Async()
        {
            var userSteven = await CreateUniqueTempUserAsync("Steven");
            var userAlexy = await CreateUniqueTempUserAsync("Alexy");

            var filters = new IFieldFilterRule[]
            {
                UserFilter.Name.EqualsTo("Steven")
            };
            
            var users = (await Client.QueryUsersAsync(filters)).ToArray();
            Assert.IsTrue(users.All(u => u.Name == "Steven"));
        }

        [UnityTest]
        public IEnumerator When_query_users_with_id_in_filter_expect_valid_results()
            => ConnectAndExecute(When_query_users_with_id_in_filter_expect_valid_results_Async);

        private async Task When_query_users_with_id_in_filter_expect_valid_results_Async()
        {
            var userSteven = await CreateUniqueTempUserAsync("Steven");
            var userAlexy = await CreateUniqueTempUserAsync("Alexy");

            var filters = new IFieldFilterRule[]
            {
                UserFilter.Id.In(userSteven.Id, userAlexy.Id)
            };
            
            var users = (await Client.QueryUsersAsync(filters)).ToArray();
            Assert.IsNotEmpty(users);
            Assert.IsTrue(users.All(u => u == userSteven || u == userAlexy));
        }

        //[UnityTest] //StreamTodo: restore when created_at issue is resolved
        public IEnumerator When_query_users_with_created_at_in_filter_expect_valid_results()
            => ConnectAndExecute(When_query_users_with_created_at_in_filter_expect_valid_results_Async);

        private async Task When_query_users_with_created_at_in_filter_expect_valid_results_Async()
        {
            var userSteven = await CreateUniqueTempUserAsync("Steven");
            var userAlexy = await CreateUniqueTempUserAsync("Alexy");

            var filters = new IFieldFilterRule[]
            {
                UserFilter.CreatedAt.In(userSteven.CreatedAt, userAlexy.CreatedAt)
            };

            var users = (await Client.QueryUsersAsync(filters)).ToArray();
            Assert.IsNotEmpty(users);
            Assert.IsTrue(users.All(u => u == userSteven || u == userAlexy));
        }

        private readonly ChannelSortObject _sortByCreatedAtAscending
            = ChannelSort.OrderByDescending(ChannelSortFieldName.CreatedAt);
    }
}
#endif