#if STREAM_TESTS_ENABLED
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using StreamChat.Core.QueryBuilders.Filters;
using StreamChat.Core.QueryBuilders.Filters.Users;
using StreamChat.Core.QueryBuilders.Sort;
using UnityEngine.TestTools;

namespace StreamChat.Tests.StatefulClient
{
    /// <summary>
    /// Tests for <see cref="IStreamChatClient.QueryUsersAsync"/>
    /// </summary>
    internal class UsersQuerySortTests : BaseStateIntegrationTests
    {
        [UnityTest]
        public IEnumerator When_order_users_by_created_at_expect_valid_order()
            => ConnectAndExecute(When_order_users_by_created_at_expect_valid_order_Async);

        private async Task When_order_users_by_created_at_expect_valid_order_Async()
        {
            var user1 = await CreateUniqueTempUserAsync("Marco");
            var user2 = await CreateUniqueTempUserAsync("Mariano");

            var filters = new IFieldFilterRule[]
            {
                UserFilter.Id.In(user1, user2)
            };

            var sortAsc = UsersSort.OrderByAscending(UserSortField.CreatedAt);
            var sortDesc = UsersSort.OrderByDescending(UserSortField.CreatedAt);

            var usersAsc = await Client.QueryUsersAsync(filters, sortAsc);
            Assert.NotNull(usersAsc);
            Assert.AreEqual(2, usersAsc.Count());
            Assert.IsTrue(usersAsc.SequenceEqual(new[] { user1, user2 }));
            
            var usersDesc = await Client.QueryUsersAsync(filters, sortDesc);
            Assert.NotNull(usersDesc);
            Assert.AreEqual(2, usersDesc.Count());
            Assert.IsTrue(usersDesc.SequenceEqual(new[] { user2, user1 }));
        }
        
        [UnityTest]
        public IEnumerator When_order_users_by_name_expect_valid_order()
            => ConnectAndExecute(When_order_users_by_name_expect_valid_order_Async);

        private async Task When_order_users_by_name_expect_valid_order_Async()
        {
            var user1 = await CreateUniqueTempUserAsync("Xavier", "Xavier");
            var user2 = await CreateUniqueTempUserAsync("Mariano", "Mariano");
            var user3 = await CreateUniqueTempUserAsync("Albert", "Albert");

            var filters = new IFieldFilterRule[]
            {
                UserFilter.Id.In(user1, user2, user3)
            };

            var sortAsc = UsersSort.OrderByAscending(UserSortField.Name);
            var sortDesc = UsersSort.OrderByDescending(UserSortField.Name);

            var usersAsc = await Client.QueryUsersAsync(filters, sortAsc);
            Assert.NotNull(usersAsc);
            Assert.AreEqual(3, usersAsc.Count());
            Assert.IsTrue(usersAsc.SequenceEqual(new[] { user3, user2, user1 }));
            
            var usersDesc = await Client.QueryUsersAsync(filters, sortDesc);
            Assert.NotNull(usersDesc);
            Assert.AreEqual(3, usersDesc.Count());
            Assert.IsTrue(usersDesc.SequenceEqual(new[] { user1, user2, user3 }));
        }
        
        [UnityTest]
        public IEnumerator When_order_users_by_last_active_expect_valid_order()
            => ConnectAndExecute(When_order_users_by_last_active_expect_valid_order_Async);

        private async Task When_order_users_by_last_active_expect_valid_order_Async()
        {
            var sortAsc = UsersSort.OrderByAscending(UserSortField.LastActive);
            var sortDesc = UsersSort.OrderByDescending(UserSortField.LastActive);

            var usersAsc = await Client.QueryUsersAsync(null, sortAsc);
            
            //StreamTodo: remove when null last_active is properly handled
            usersAsc = usersAsc.Where(u => u.LastActive != null);
            
            if (usersAsc.Count() >= 2)
            {
                Assert.IsTrue(usersAsc.First().LastActive < usersAsc.Last().LastActive);
            }
            
            var usersDesc = await Client.QueryUsersAsync(null, sortDesc);
            
            //StreamTodo: remove when null last_active is properly handled
            usersDesc = usersDesc.Where(u => u.LastActive != null);
            
            if (usersDesc.Count() >= 2)
            {
                Assert.IsTrue(usersDesc.First().LastActive > usersDesc.Last().LastActive);
            }
        }
    }
}
#endif