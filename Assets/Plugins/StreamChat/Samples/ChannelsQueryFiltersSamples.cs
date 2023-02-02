using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Core.QueryBuilders.Filters;
using StreamChat.Core.QueryBuilders.Filters.Channels;
using StreamChat.Core.QueryBuilders.Sort;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Samples
{
    internal class ChannelsQueryFiltersSamples
    {
        public async Task ByCid()
        {
            var filters = new IFieldFilterRule[]
            {
                ChannelFilter.Cid.In("channel-cid", "channel-2-cid", "channel-3-cid")
            };
            var sort = ChannelSort.OrderByAscending(ChannelSortFieldName.CreatedAt);

            var channels = await Client.QueryChannelsAsync(filters, sort);
        }

        public async Task Operators()
        {
            IStreamChannel channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, "channel-id-1");

// Each operator usually supports multiple argument types to match your needs
            ChannelFilter.Cid.EqualsTo("channel-cid"); // string
            ChannelFilter.Cid.EqualsTo(channel); // IStreamChannel
            ChannelFilter.Cid.In("channel-cid", "channel-2-cid", "channel-3-cid"); // Comma separated strings

            var channelCids = new List<string> { "channel-1-cid", "channel-2-cid" };
            ChannelFilter.Cid.In(channelCids); // Any collection of string
        }

        public async Task LocalUserIsAMember()
        {
            var filters = new IFieldFilterRule[]
            {
                ChannelFilter.Members.In(Client.LocalUserData.User)
            };
            var sort = ChannelSort.OrderByAscending(ChannelSortFieldName.CreatedAt);

            var channels = await Client.QueryChannelsAsync(filters, sort);
        }

        public async Task CreatedByLocalUser()
        {
            var filters = new IFieldFilterRule[]
            {
                ChannelFilter.CreatedById.EqualsTo(Client.LocalUserData.User)
            };
            var sort = ChannelSort.OrderByAscending(ChannelSortFieldName.CreatedAt);

            var channels = await Client.QueryChannelsAsync(filters, sort);
        }

        public async Task MutedByLocalUser()
        {
            var filters = new IFieldFilterRule[]
            {
                ChannelFilter.Muted.EqualsTo(true)
            };
            var sort = ChannelSort.OrderByAscending(ChannelSortFieldName.CreatedAt);

            var channels = await Client.QueryChannelsAsync(filters, sort);
        }

        public async Task MembersCountMoreThan10()
        {
            var filters = new IFieldFilterRule[]
            {
                ChannelFilter.MembersCount.GreaterThan(10)
            };
            var sort = ChannelSort.OrderByAscending(ChannelSortFieldName.CreatedAt);

            var channels = await Client.QueryChannelsAsync(filters, sort);
        }

        public async Task CreatedWithinLastWeek()
        {
            var weekAgo = DateTime.Now.AddDays(-7).Date;
            var filters = new IFieldFilterRule[]
            {
                ChannelFilter.CreatedAt.GreaterThan(weekAgo)
            };
            var sort = ChannelSort.OrderByAscending(ChannelSortFieldName.CreatedAt);

            var channels = await Client.QueryChannelsAsync(filters, sort);
        }

        public async Task UpdatedLast24Hours()
        {
            var dayAgo = DateTime.Now.AddHours(-24);
            var filters = new IFieldFilterRule[]
            {
                ChannelFilter.UpdatedAt.GreaterThan(dayAgo)
            };
            var sort = ChannelSort.OrderByAscending(ChannelSortFieldName.CreatedAt);

            var channels = await Client.QueryChannelsAsync(filters, sort);
        }

        private IStreamChatClient Client { get; } = StreamChatClient.CreateDefaultClient();
    }
}