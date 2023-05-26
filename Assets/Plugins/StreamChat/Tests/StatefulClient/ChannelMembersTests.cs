#if STREAM_TESTS_ENABLED
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using StreamChat.Core;
using StreamChat.Core.QueryBuilders.Filters;
using StreamChat.Core.QueryBuilders.Filters.Users;
using StreamChat.Core.Requests;
using StreamChat.Core.StatefulModels;
using UnityEngine.TestTools;

namespace StreamChat.Tests.StatefulClient
{
    /// <summary>
    /// Tests related to <see cref="StreamChannel"/> <see cref="IStreamChannelMember"/> mechanics
    /// </summary>
    internal class ChannelMembersTests : BaseStateIntegrationTests
    {
        [UnityTest]
        public IEnumerator When_add_user_by_reference_to_channel_expect_user_included_in_members()
            => ConnectAndExecute(When_add_user_by_reference_to_channel_expect_user_included_in_members_Async);

        private async Task When_add_user_by_reference_to_channel_expect_user_included_in_members_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();

            var otherUserId = OtherAdminUsersCredentials.First().UserId;

            var filters = new IFieldFilterRule[]
            {
                UserFilter.Id.EqualsTo(otherUserId)
            };

            var users = await Client.QueryUsersAsync(filters);
            var otherUser = users.First();

            await channel.AddMembersAsync(hideHistory: default, optionalMessage: default, otherUser);

            await WaitWhileTrueAsync(() => channel.Members.All(m => m.User != otherUser));
            Assert.NotNull(channel.Members.FirstOrDefault(member => member.User == otherUser));
        }

        [UnityTest]
        public IEnumerator When_add_user_by_id_to_channel_expect_user_included_in_members()
            => ConnectAndExecute(When_add_user_by_id_to_channel_expect_user_included_in_members_Async);

        private async Task When_add_user_by_id_to_channel_expect_user_included_in_members_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();

            var otherUserId = OtherAdminUsersCredentials.First().UserId;

            var filters = new IFieldFilterRule[]
            {
                UserFilter.Id.EqualsTo(otherUserId)
            };

            var users = await Client.QueryUsersAsync(filters);
            var otherUser = users.First();

            await channel.AddMembersAsync(hideHistory: default, optionalMessage: default, otherUser.Id);

            await WaitWhileTrueAsync(() => channel.Members.All(m => m.User != otherUser));
            Assert.NotNull(channel.Members.FirstOrDefault(member => member.User == otherUser));
        }
        
        [UnityTest]
        public IEnumerator When_add_user_to_channel_with_hide_history_and_message_expect_user_as_members_and_message_sent()
            => ConnectAndExecute(When_add_user_to_channel_with_hide_history_and_message_expect_user_as_members_and_message_sent_Async);

        private async Task When_add_user_to_channel_with_hide_history_and_message_expect_user_as_members_and_message_sent_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            var otherUserId = OtherAdminUsersCredentials.First().UserId;

            var memberAddedMsg = $"{otherUserId} was added to the channel";

            var tcs = new TaskCompletionSource<bool>();
            channel.MessageReceived += (streamChannel, message) =>
            {
                Assert.AreEqual(message.Text, memberAddedMsg);
                tcs.SetResult(true);
            };

            var filters = new IFieldFilterRule[]
            {
                UserFilter.Id.EqualsTo(otherUserId)
            };

            var users = await Client.QueryUsersAsync(filters);
            var otherUser = users.First();

            await channel.AddMembersAsync(hideHistory: true, optionalMessage: new StreamMessageRequest
            {
                Text = memberAddedMsg
            }, otherUser.Id);

            await WaitWhileTrueAsync(() => channel.Members.All(m => m.User != otherUser));
            Assert.NotNull(channel.Members.FirstOrDefault(member => member.User == otherUser));

            await WaitWithTimeoutAsync(tcs.Task, 5, $"Event {nameof(channel.MessageReceived)} was not received");
        }

        [UnityTest]
        public IEnumerator When_remove_member_by_reference_to_channel_expect_member_removed_from_channel_members()
            => ConnectAndExecute(
                When_remove_member_by_reference_to_channel_expect_member_removed_from_channel_members_Async);

        private async Task When_remove_member_by_reference_to_channel_expect_member_removed_from_channel_members_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();

            var otherUserId = OtherAdminUsersCredentials.First().UserId;

            var filters = new IFieldFilterRule[]
            {
                UserFilter.Id.EqualsTo(otherUserId)
            };

            var users = await Client.QueryUsersAsync(filters);
            var otherUser = users.First();

            await channel.AddMembersAsync(hideHistory: default, optionalMessage: default, otherUser);

            await WaitWhileTrueAsync(() => channel.Members.All(m => m.User != otherUser));

            var otherUserMember = channel.Members.FirstOrDefault(m => m.User == otherUser);

            await channel.RemoveMembersAsync(otherUserMember);
            await WaitWhileTrueAsync(() => channel.Members.Any(m => m.User == otherUser));
            Assert.IsNull(channel.Members.FirstOrDefault(member => member.User == otherUser));
        }

        [UnityTest]
        public IEnumerator When_remove_member_by_user_id_to_channel_expect_member_removed_from_channel_members()
            => ConnectAndExecute(
                When_remove_member_by_user_id_to_channel_expect_member_removed_from_channel_members_Async);

        private async Task When_remove_member_by_user_id_to_channel_expect_member_removed_from_channel_members_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();

            var otherUserId = OtherAdminUsersCredentials.First().UserId;

            var filters = new IFieldFilterRule[]
            {
                UserFilter.Id.EqualsTo(otherUserId)
            };

            var users = await Client.QueryUsersAsync(filters);
            var otherUser = users.First();

            await channel.AddMembersAsync(hideHistory: default, optionalMessage: default, otherUser.Id);

            await WaitWhileTrueAsync(() => channel.Members.All(m => m.User != otherUser));

            await channel.RemoveMembersAsync(otherUser.Id);
            await WaitWhileTrueAsync(() => channel.Members.Any(m => m.User == otherUser));
            Assert.IsNull(channel.Members.FirstOrDefault(member => member.User == otherUser));
        }

        [UnityTest]
        public IEnumerator When_query_members_expect_proper_members_returned()
            => ConnectAndExecute(When_query_members_expect_proper_members_returned_Async);

        private async Task When_query_members_expect_proper_members_returned_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();

            var otherUsers = OtherAdminUsersCredentials.Take(3).ToArray();
            var firstCredentials = otherUsers.First();
            var lastCredentials = otherUsers.Last();

            var filters = new IFieldFilterRule[]
            {
                UserFilter.Id.In(otherUsers.Select(u => u.UserId))
            };

            var users = await Client.QueryUsersAsync(filters);

            var firstUser = users.FirstOrDefault(u => u.Id == firstCredentials.UserId);
            var lastUser = users.FirstOrDefault(u => u.Id == lastCredentials.UserId);

            Assert.NotNull(firstUser);
            Assert.NotNull(lastUser);

            await channel.AddMembersAsync(users);

            var result = await channel.QueryMembersAsync(new Dictionary<string, object>()
            {
                {
                    "id", new Dictionary<string, object>
                    {
                        { "$in", new[] { firstCredentials.UserId, lastCredentials.UserId } }
                    }
                }
            });

            var firstMember = result.FirstOrDefault(m => m.User == firstUser);
            var lastMember = result.FirstOrDefault(m => m.User == lastUser);

            Assert.NotNull(firstMember);
            Assert.NotNull(lastMember);
        }

        //[UnityTest] //StreamTodo: debug, works when triggered manually but fails in GitHub Actions
        public IEnumerator When_add_members_expect_member_added_event_fired()
            => ConnectAndExecute(When_add_members_expect_member_added_event_fired_Async);

        private async Task When_add_members_expect_member_added_event_fired_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            var user = await CreateUniqueTempUserAsync("Micheal");

            var receivedEvent = false;
            IStreamChannelMember eventMember = null;
            IStreamChannel eventChannel = null;
            channel.MemberAdded += (chanel, member) =>
            {
                receivedEvent = true;
                eventMember = member;
                eventChannel = chanel;
            };

            var receivedEvent2 = false;
            IStreamChannelMember eventMember2 = null;
            IStreamChannel eventChannel2 = null;
            OperationType? opType = default;
            channel.MembersChanged += (chanel, member, op) =>
            {
                receivedEvent2 = true;
                eventMember2 = member;
                eventChannel2 = chanel;
                opType = op;
            };

            await channel.AddMembersAsync(hideHistory: default, optionalMessage: default, user);

            await WaitWhileFalseAsync(() => receivedEvent && receivedEvent2);

            Assert.IsTrue(receivedEvent);
            Assert.IsNotNull(eventChannel);
            Assert.IsNotNull(eventMember);
            Assert.AreEqual(channel, eventChannel);
            Assert.AreEqual(user, eventMember.User);

            Assert.IsTrue(receivedEvent2);
            Assert.IsNotNull(eventChannel2);
            Assert.IsNotNull(eventMember2);
            Assert.AreEqual(channel, eventChannel2);
            Assert.AreEqual(user, eventMember2.User);
            Assert.AreEqual(OperationType.Added, opType.Value);
        }

        //[UnityTest] //StreamTodo: debug, works when triggered manually but fails in GitHub Actions
        public IEnumerator When_remove_members_expect_member_added_event_fired()
            => ConnectAndExecute(When_remove_members_expect_member_added_event_fired_Async);

        private async Task When_remove_members_expect_member_added_event_fired_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            var user = await CreateUniqueTempUserAsync("Micheal");

            var receivedEvent = false;
            IStreamChannelMember eventMember = null;
            IStreamChannel eventChannel = null;
            channel.MemberRemoved += (chanel, member) =>
            {
                receivedEvent = true;
                eventMember = member;
                eventChannel = chanel;
            };

            var receivedEvent2 = false;
            IStreamChannelMember eventMember2 = null;
            IStreamChannel eventChannel2 = null;
            OperationType? opType = default;
            channel.MembersChanged += (chanel, member, op) =>
            {
                receivedEvent2 = true;
                eventMember2 = member;
                eventChannel2 = chanel;
                opType = op;
            };

            await channel.AddMembersAsync(hideHistory: default, optionalMessage: default, user);
            await channel.RemoveMembersAsync(user);

            await WaitWhileFalseAsync(() => receivedEvent && receivedEvent2);

            Assert.IsTrue(receivedEvent);
            Assert.IsNotNull(eventChannel);
            Assert.IsNotNull(eventMember);
            Assert.AreEqual(channel, eventChannel);
            Assert.AreEqual(user, eventMember.User);

            Assert.IsTrue(receivedEvent2);
            Assert.IsNotNull(eventChannel2);
            Assert.IsNotNull(eventMember2);
            Assert.AreEqual(channel, eventChannel2);
            Assert.AreEqual(user, eventMember2.User);
            Assert.AreEqual(OperationType.Removed, opType.Value);
        }
    }
}
#endif