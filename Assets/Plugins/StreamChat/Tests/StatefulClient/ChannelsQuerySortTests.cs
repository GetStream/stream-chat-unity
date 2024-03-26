#if STREAM_TESTS_ENABLED
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using StreamChat.Core;
using StreamChat.Core.QueryBuilders.Filters;
using StreamChat.Core.QueryBuilders.Filters.Channels;
using StreamChat.Core.QueryBuilders.Sort;
using UnityEngine.TestTools;

namespace StreamChat.Tests.StatefulClient
{
    /// <summary>
    /// Tests for <see cref="IStreamChatClient.QueryChannelsAsync"/>
    /// </summary>
    internal class ChannelsQuerySortTests : BaseStateIntegrationTests
    {
        [UnityTest]
        public IEnumerator When_order_channels_by_created_at_desc_expect_valid_order()
            => ConnectAndExecute(When_order_channels_by_created_at_desc_expect_valid_order_Async);

        private async Task When_order_channels_by_created_at_desc_expect_valid_order_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            var channel2 = await CreateUniqueTempChannelAsync();
            var channel3 = await CreateUniqueTempChannelAsync();
            var channel4 = await CreateUniqueTempChannelAsync();

            var filters = new IFieldFilterRule[]
            {
                ChannelFilter.Cid.In(channel.Cid, channel2.Cid, channel3.Cid, channel4.Cid)
            };

            var sort = ChannelSort.OrderByDescending(ChannelSortFieldName.CreatedAt);

            var channels = await Client.QueryChannelsAsync(filters, sort);
            Assert.NotNull(channels);
            Assert.AreEqual(4, channels.Count());
            Assert.IsTrue(channels.SequenceEqual(new[] { channel4, channel3, channel2, channel }));
        }

        [UnityTest]
        public IEnumerator When_order_channels_by_two_fields_expect_valid_order()
            => ConnectAndExecute(When_order_channels_by_two_fields_expect_valid_order_Async);

        private async Task When_order_channels_by_two_fields_expect_valid_order_Async()
        {
            /*
             * Scenario:
             *
             * member counts:
             * channel1 -> 2
             * channel3 -> 2
             * channel2 -> 1
             * channel4 -> 1
             *
             * order by member_count DESC then by created_at DESC should result in:
             *  channel3 -> channel1 -> channel4 -> channel2
             */
            var channel1 = await CreateUniqueTempChannelAsync();
            var channel2 = await CreateUniqueTempChannelAsync();
            var channel3 = await CreateUniqueTempChannelAsync();
            var channel4 = await CreateUniqueTempChannelAsync();

            await channel1.AddMembersAsync(hideHistory: default, optionalMessage: default, TestAdminId, TestUserId);
            await channel3.AddMembersAsync(hideHistory: default, optionalMessage: default, TestAdminId, TestUserId);

            await channel2.AddMembersAsync(hideHistory: default, optionalMessage: default, TestAdminId);
            await channel4.AddMembersAsync(hideHistory: default, optionalMessage: default, TestAdminId);

            var filters = new IFieldFilterRule[]
            {
                ChannelFilter.Cid.In(channel1.Cid, channel2.Cid, channel3.Cid, channel4.Cid)
            };

            var sort = ChannelSort.OrderByDescending(ChannelSortFieldName.MemberCount)
                .ThenByDescending(ChannelSortFieldName.CreatedAt);

            var channels = await Client.QueryChannelsAsync(filters, sort);
            Assert.NotNull(channels);
            Assert.AreEqual(4, channels.Count());
            Assert.IsTrue(channels.SequenceEqual(new[] { channel3, channel1, channel4, channel2 }));
        }
    }
}
#endif