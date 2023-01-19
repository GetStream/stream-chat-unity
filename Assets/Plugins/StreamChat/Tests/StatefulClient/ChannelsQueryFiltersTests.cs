using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using StreamChat.Core.QueryBuilders.Filters;
using StreamChat.Core.QueryBuilders.Filters.Channels;
using StreamChat.Core.QueryBuilders.Sort;
using StreamChat.Core.StatefulModels;
using UnityEngine.TestTools;

namespace StreamChat.Tests.StatefulClient
{
    /// <summary>
    /// Tests operations executed on <see cref="StreamChannel"/>
    /// </summary>
    internal class ChannelsQueryFiltersTests : BaseStateIntegrationTests
    {
        [UnityTest]
        public IEnumerator When_query_channel_with_id_in_filter_expect_valid_results()
            => ConnectAndExecute(When_query_channel_with_id_in_filter_expect_valid_results_Async);

        private async Task When_query_channel_with_id_in_filter_expect_valid_results_Async()
        {
            var channel1 = await CreateUniqueTempChannelAsync();
            var channel2 = await CreateUniqueTempChannelAsync();
            var channel3 = await CreateUniqueTempChannelAsync();

            var filters = new Dictionary<string, object>
            {
                {
                    "cid", new Dictionary<string, object>
                    {
                        {
                            "$in", new[] { channel1.Cid, channel2.Cid, channel3.Cid }
                        }
                    }
                }
            };

            var channels = (await Client.QueryChannelsAsync(filters)).ToArray();
            Assert.Contains(channel1, channels);
            Assert.Contains(channel2, channels);
            Assert.Contains(channel3, channels);
            
            // Query builder

            var filters2 = new IFieldFilterRule[]
            {
                ChannelFilter.Cid.In(channel1, channel2, channel3)
            };

            var channels2 = (await Client.QueryChannelsAsync(filters2, ChannelSort.OrderByAscending(ChannelSortFieldName.CreatedAt))).ToArray();
            Assert.Contains(channel1, channels2);
            Assert.Contains(channel2, channels2);
            Assert.Contains(channel3, channels2);
        }

        [UnityTest]
        public IEnumerator When_query_channel_with_id_in_and_hidden_filter_expect_valid_results()
            => ConnectAndExecute(When_query_channel_with_id_in_and_hidden_filter_expect_valid_results_Async);

        private async Task When_query_channel_with_id_in_and_hidden_filter_expect_valid_results_Async()
        {
            var channel1 = await CreateUniqueTempChannelAsync();
            var channel2 = await CreateUniqueTempChannelAsync();
            var channel3 = await CreateUniqueTempChannelAsync();

            await channel2.JoinAsMemberAsync();

            await channel2.HideAsync();

            var filters = new Dictionary<string, object>
            {
                {
                    "cid", new Dictionary<string, object>
                    {
                        {
                            "$in", new[] { channel1.Cid, channel2.Cid, channel3.Cid }
                        }
                    }
                },
                {
                    "hidden", new Dictionary<string, object>
                    {
                        {
                            "$eq", false
                        }
                    }
                }
            };

            var channels = (await Client.QueryChannelsAsync(filters)).ToArray();
            Assert.Contains(channel1, channels);
            Assert.IsNull(channels.FirstOrDefault(c => c == channel2));
            Assert.Contains(channel3, channels);

            // Query builder

            var filters2 = new IFieldFilterRule[]
            {
                ChannelFilter.Cid.In(channel1, channel2, channel3),
                ChannelFilter.Hidden.EqualsTo(false),
            };

            var channels2 = (await Client.QueryChannelsAsync(filters2, ChannelSort.OrderByAscending(ChannelSortFieldName.CreatedAt))).ToArray();
            Assert.Contains(channel1, channels2);
            Assert.IsNull(channels2.FirstOrDefault(c => c == channel2));
            Assert.Contains(channel3, channels2);
        }

        [UnityTest]
        public IEnumerator When_query_channel_with_id_in_and_hidden_and_frozen_filter_expect_valid_results()
            => ConnectAndExecute(When_query_channel_with_id_in_and_hidden_and_frozen_filter_expect_valid_results_Async);

        private async Task When_query_channel_with_id_in_and_hidden_and_frozen_filter_expect_valid_results_Async()
        {
            var channel1 = await CreateUniqueTempChannelAsync();
            var channel2 = await CreateUniqueTempChannelAsync();
            var channel3 = await CreateUniqueTempChannelAsync();

            await channel2.JoinAsMemberAsync();

            await channel2.HideAsync();

            await channel3.UpdatePartialAsync(setFields: new Dictionary<string, object>()
            {
                { "frozen", true }
            });

            Assert.AreEqual(true, channel3.Frozen);

            var filters = new Dictionary<string, object>
            {
                {
                    "cid", new Dictionary<string, object>
                    {
                        {
                            "$in", new[] { channel1.Cid, channel2.Cid, channel3.Cid }
                        }
                    }
                },
                {
                    "hidden", new Dictionary<string, object>
                    {
                        {
                            "$eq", false
                        }
                    }
                },
                {
                    "frozen", new Dictionary<string, object>
                    {
                        {
                            "$eq", false
                        }
                    }
                }
            };

            var channels = (await Client.QueryChannelsAsync(filters)).ToArray();
            Assert.Contains(channel1, channels);
            Assert.IsNull(channels.FirstOrDefault(c => c == channel2));
            Assert.IsNull(channels.FirstOrDefault(c => c == channel3));
            
            // Query builder

            var filters2 = new IFieldFilterRule[]
            {
                ChannelFilter.Cid.In(channel1, channel2, channel3),
                ChannelFilter.Hidden.EqualsTo(false),
                ChannelFilter.Frozen.EqualsTo(false),
            };

            var channels2 = (await Client.QueryChannelsAsync(filters2, ChannelSort.OrderByAscending(ChannelSortFieldName.CreatedAt))).ToArray();
            Assert.Contains(channel1, channels2);
            Assert.IsNull(channels2.FirstOrDefault(c => c == channel2));
            Assert.IsNull(channels2.FirstOrDefault(c => c == channel3));
        }
    }
}