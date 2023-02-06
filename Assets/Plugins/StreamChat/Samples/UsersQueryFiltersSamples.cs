using System;
using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Core.QueryBuilders.Filters;
using StreamChat.Core.QueryBuilders.Filters.Users;

namespace StreamChat.Samples
{
    /// <summary>
    /// Code Examples for <see cref="IStreamChatClient.QueryUsersAsync"/> filters
    /// </summary>
    internal class UsersQueryFiltersSamples
    {
        public async Task UserIdEquals()
        {
            var filters = new IFieldFilterRule[]
            {
                UserFilter.Id.EqualsTo("user-id")
            };

            var users = await Client.QueryUsersAsync(filters);
        }

        public async Task UserNameAutocomplete()
        {
            var filters = new IFieldFilterRule[]
            {
                UserFilter.Name.Autocomplete("Ann")
            };

            var users = await Client.QueryUsersAsync(filters);
        }

        public async Task UserCreatedAtLastWeek()
        {
            var lastWeek = DateTime.Now.AddDays(-7);
            var filters = new IFieldFilterRule[]
            {
                UserFilter.CreatedAt.GreaterThanOrEquals(lastWeek)
            };

            var users = await Client.QueryUsersAsync(filters);
        }

        public async Task UserCreatedAtOlderThanMonth()
        {
            var lastMonth = DateTime.Now.AddMonths(-1);
            var filters = new IFieldFilterRule[]
            {
                UserFilter.CreatedAt.LessThan(lastMonth)
            };

            var users = await Client.QueryUsersAsync(filters);
        }

        public async Task UserNotActiveLastWeek()
        {
            var lastWeek = DateTime.Now.AddDays(-7);
            var filters = new IFieldFilterRule[]
            {
                UserFilter.LastActive.LessThan(lastWeek)
            };

            var users = await Client.QueryUsersAsync(filters);
        }

        public async Task UserLastActive24Hours()
        {
            var lastDay = DateTime.Now.AddDays(-1);
            var filters = new IFieldFilterRule[]
            {
                UserFilter.LastActive.GreaterThanOrEquals(lastDay)
            };

            var users = await Client.QueryUsersAsync(filters);
        }

        public async Task UserWithAdminRole()
        {
            var filters = new IFieldFilterRule[]
            {
                UserFilter.Role.EqualsTo("admin")
            };

            var users = await Client.QueryUsersAsync(filters);
        }

        public async Task UserWithAdminOrModeratorRole()
        {
            var filters = new IFieldFilterRule[]
            {
                UserFilter.Role.In("admin", "moderator")
            };

            var users = await Client.QueryUsersAsync(filters);
        }

        public async Task UserGloballyBanned()
        {
            var filters = new IFieldFilterRule[]
            {
                UserFilter.Banned.EqualsTo(true)
            };

            var users = await Client.QueryUsersAsync(filters);
        }

        public async Task UserGloballyShadowBanned()
        {
            var filters = new IFieldFilterRule[]
            {
                UserFilter.ShadowBanned.EqualsTo(true)
            };

            var users = await Client.QueryUsersAsync(filters);
        }

        private IStreamChatClient Client { get; } = StreamChatClient.CreateDefaultClient();
    }
}