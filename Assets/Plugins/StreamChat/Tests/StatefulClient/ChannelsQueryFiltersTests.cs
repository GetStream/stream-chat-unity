#if STREAM_TESTS_ENABLED
using System;
using System.Collections;
using System.Collections.Generic;
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
    /// Filter tests for <see cref="IStreamChatClient.QueryChannelsAsync"/>
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

            var filters = new IFieldFilterRule[]
            {
                ChannelFilter.Cid.In(channel1, channel2, channel3)
            };

            var channels = (await Client.QueryChannelsAsync(filters, _sortByCreatedAtAscending)).ToArray();
            Assert.Contains(channel1, channels);
            Assert.Contains(channel2, channels);
            Assert.Contains(channel3, channels);
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

            var filters = new IFieldFilterRule[]
            {
                ChannelFilter.Cid.In(channel1, channel2, channel3),
                ChannelFilter.Hidden.EqualsTo(false),
            };

            var channels = (await Client.QueryChannelsAsync(filters, _sortByCreatedAtAscending)).ToArray();
            Assert.Contains(channel1, channels);
            Assert.IsNull(channels.FirstOrDefault(c => c == channel2));
            Assert.Contains(channel3, channels);
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

            var filters = new IFieldFilterRule[]
            {
                ChannelFilter.Cid.In(channel1, channel2, channel3),
                ChannelFilter.Hidden.EqualsTo(false),
                ChannelFilter.Frozen.EqualsTo(false),
            };

            var channels = (await Client.QueryChannelsAsync(filters, _sortByCreatedAtAscending)).ToArray();
            Assert.Contains(channel1, channels);
            Assert.IsNull(channels.FirstOrDefault(c => c == channel2));
            Assert.IsNull(channels.FirstOrDefault(c => c == channel3));
        }

        [UnityTest]
        public IEnumerator When_query_channel_with_created_by_id_filter_expect_valid_results()
            => ConnectAndExecute(When_query_channel_with_created_by_id_filter_expect_valid_results_Async);

        private async Task When_query_channel_with_created_by_id_filter_expect_valid_results_Async()
        {
            var channel1 = await CreateUniqueTempChannelAsync();
            var channel2 = await CreateUniqueTempChannelAsync();
            var channel3 = await CreateUniqueTempChannelAsync();

            var filters = new IFieldFilterRule[]
            {
                ChannelFilter.CreatedById.EqualsTo(Client.LocalUserData.User),
            };

            var channels = (await Client.QueryChannelsAsync(filters, _sortByCreatedAtAscending)).ToArray();
            Assert.Contains(channel1, channels);
            Assert.Contains(channel2, channels);
            Assert.Contains(channel3, channels);
        }

        [UnityTest]
        public IEnumerator When_query_channel_with_muted_filter_expect_valid_results()
            => ConnectAndExecute(When_query_channel_with_muted_filter_expect_valid_results_Async);

        private async Task When_query_channel_with_muted_filter_expect_valid_results_Async()
        {
            var channel1 = await CreateUniqueTempChannelAsync();
            var channel2 = await CreateUniqueTempChannelAsync();
            var channel3 = await CreateUniqueTempChannelAsync();

            await channel2.MuteChannelAsync();

            var filters = new IFieldFilterRule[]
            {
                ChannelFilter.Muted.EqualsTo(true),
            };

            var channels = (await Client.QueryChannelsAsync(filters, _sortByCreatedAtAscending)).ToArray();
            Assert.IsNull(channels.FirstOrDefault(c => c == channel1));
            Assert.Contains(channel2, channels);
            Assert.IsNull(channels.FirstOrDefault(c => c == channel3));
        }

        [UnityTest]
        public IEnumerator When_query_channel_with_member_name_autocomplete_filter_expect_valid_results()
            => ConnectAndExecute(When_query_channel_with_member_name_autocomplete_filter_expect_valid_results_Async);

        private async Task When_query_channel_with_member_name_autocomplete_filter_expect_valid_results_Async()
        {
            var channel1 = await CreateUniqueTempChannelAsync();
            var channel2 = await CreateUniqueTempChannelAsync();
            var channel3 = await CreateUniqueTempChannelAsync();

            var userAnna = await CreateUniqueTempUserAsync("Anna");
            var userDaniel = await CreateUniqueTempUserAsync("Daniel");
            var userJonathan = await CreateUniqueTempUserAsync("Jonathan");

            await channel1.AddMembersAsync(hideHistory: default, optionalMessage: default, userAnna);
            await channel2.AddMembersAsync(hideHistory: default, optionalMessage: default, userDaniel);
            await channel3.AddMembersAsync(hideHistory: default, optionalMessage: default, userJonathan);
            
            // The search filter for MemberUserName relies on the `read-channel-members` permissions being enabled.
            // For channel type `Messaging` you can only view other members if you're a member yourself
            await channel2.AddMembersAsync(hideHistory: default, optionalMessage: default, Client.LocalUserData.User);

            var filters2 = new IFieldFilterRule[]
            {
                ChannelFilter.MemberUserName.Autocomplete("Dani"),
            };

            var channels2 = (await Client.QueryChannelsAsync(filters2, _sortByCreatedAtAscending)).ToArray();
            Assert.Contains(channel2, channels2);
        }
        
        [UnityTest]
        public IEnumerator When_query_channel_with_members_count_filter_expect_valid_results()
            => ConnectAndExecute(When_query_channel_with_members_count_filter_expect_valid_results_Async);

        private async Task When_query_channel_with_members_count_filter_expect_valid_results_Async()
        {
            var channel1 = await CreateUniqueTempChannelAsync();
            var channel2 = await CreateUniqueTempChannelAsync();
            var channel3 = await CreateUniqueTempChannelAsync();
            
            var userAnna = await CreateUniqueTempUserAsync("Anna");
            var userDaniel = await CreateUniqueTempUserAsync("Daniel");
            var userJonathan = await CreateUniqueTempUserAsync("Jonathan");

            await channel2.AddMembersAsync(hideHistory: default, optionalMessage: default, userAnna);
            await channel2.AddMembersAsync(hideHistory: default, optionalMessage: default, userDaniel);
            await channel2.AddMembersAsync(hideHistory: default, optionalMessage: default, userJonathan);

            var filters = new IFieldFilterRule[]
            {
                ChannelFilter.MembersCount.EqualsTo(3),
            };

            var channels = (await Client.QueryChannelsAsync(filters, _sortByCreatedAtAscending)).ToArray();
            Assert.IsNull(channels.FirstOrDefault(c => c == channel1));
            Assert.Contains(channel2, channels);
            Assert.IsNull(channels.FirstOrDefault(c => c == channel3));
            
            Assert.IsTrue(channels.All(c => c.MemberCount == 3));
        }
        
        [UnityTest]
        public IEnumerator When_query_channel_by_created_at_filter_expect_valid_results()
            => ConnectAndExecute(When_query_channel_by_created_at_filter_expect_valid_results_Async);

        private async Task When_query_channel_by_created_at_filter_expect_valid_results_Async()
        {
            var channel1 = await CreateUniqueTempChannelAsync();
            var channel2 = await CreateUniqueTempChannelAsync();
            var channel3 = await CreateUniqueTempChannelAsync();

            var filters = new IFieldFilterRule[]
            {
                ChannelFilter.CreatedAt.GreaterThanOrEquals(DateTime.Now.AddMinutes(-5)),
            };

            var channels = (await Client.QueryChannelsAsync(filters, _sortByCreatedAtAscending)).ToArray();
            Assert.Contains(channel1, channels);
            Assert.Contains(channel2, channels); 
            Assert.Contains(channel3, channels);
        }

        private readonly ChannelSortObject _sortByCreatedAtAscending
            = ChannelSort.OrderByDescending(ChannelSortFieldName.CreatedAt);
    }
}
#endif