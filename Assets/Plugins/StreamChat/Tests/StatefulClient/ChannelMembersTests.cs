#if STREAM_TESTS_ENABLED
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        public IEnumerator
            When_add_user_to_channel_with_hide_history_and_message_expect_user_as_members_and_message_sent()
            => ConnectAndExecute(
                When_add_user_to_channel_with_hide_history_and_message_expect_user_as_members_and_message_sent_Async);

        private async Task
            When_add_user_to_channel_with_hide_history_and_message_expect_user_as_members_and_message_sent_Async()
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
            channel.MemberRemoved += (channel2, member) =>
            {
                receivedEvent = true;
                eventMember = member;
                eventChannel = channel2;
            };

            var receivedEvent2 = false;
            IStreamChannelMember eventMember2 = null;
            IStreamChannel eventChannel2 = null;
            OperationType? opType = default;
            channel.MembersChanged += (channel3, member, op) =>
            {
                receivedEvent2 = true;
                eventMember2 = member;
                eventChannel2 = channel3;
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

        [UnityTest]
        public IEnumerator When_user_added_to_not_watched_channel_expect_user_receive_added_to_channel_event()
            => ConnectAndExecute(
                When_user_added_to_not_watched_channel_expect_user_receive_added_to_channel_event_Async);

        private async Task When_user_added_to_not_watched_channel_expect_user_receive_added_to_channel_event_Async()
        {
            var channel = await CreateUniqueTempChannelAsync(watch: false);

            var receivedEvent = false;
            IStreamChannelMember eventMember = null;
            IStreamChannel eventChannel = null;
            Client.AddedToChannelAsMember += (channel2, member) =>
            {
                receivedEvent = true;
                eventMember = member;
                eventChannel = channel2;
            };

            await channel.AddMembersAsync(hideHistory: default, optionalMessage: default, Client.LocalUserData.User);
            await WaitWhileFalseAsync(() => receivedEvent);

            Assert.IsTrue(receivedEvent);
            Assert.IsNotNull(eventChannel);
            Assert.IsNotNull(eventMember);
            Assert.AreEqual(channel, eventChannel);
            Assert.AreEqual(Client.LocalUserData.User, eventMember.User);
        }
        
        [UnityTest]
        public IEnumerator When_user_added_to_not_watched_channel_expect_user_receive_added_to_channel_event_from_main_thread()
            => ConnectAndExecute(
                When_user_added_to_not_watched_channel_expect_user_receive_added_to_channel_event_from_main_thread_Async);

        private async Task When_user_added_to_not_watched_channel_expect_user_receive_added_to_channel_event_from_main_thread_Async()
        {
            var channel = await CreateUniqueTempChannelAsync(watch: false);

            var receivedEvent = false;
            IStreamChannelMember eventMember = null;
            IStreamChannel eventChannel = null;
            var receivedEventThreadId = 0;
            Client.AddedToChannelAsMember += (channel2, member) =>
            {
                receivedEvent = true;
                eventMember = member;
                eventChannel = channel2;
                receivedEventThreadId = GetCurrentThreadId();
            };

            await channel.AddMembersAsync(hideHistory: default, optionalMessage: default, Client.LocalUserData.User);
            await WaitWhileFalseAsync(() => receivedEvent);

            Assert.IsTrue(receivedEvent);
            Assert.IsNotNull(eventChannel);
            Assert.IsNotNull(eventMember);
            Assert.AreEqual(channel, eventChannel);
            Assert.AreEqual(Client.LocalUserData.User, eventMember.User);
            
            Assert.AreEqual(receivedEventThreadId, MainThreadId);
        }
        
        [UnityTest]
        public IEnumerator When_user_added_to_not_watched_channel_expect_received_channel_being_watched()
            => ConnectAndExecute(
                When_user_added_to_not_watched_channel_expect_received_channel_being_watched_Async);

        private async Task When_user_added_to_not_watched_channel_expect_received_channel_being_watched_Async()
        {
            var otherClient = await GetConnectedOtherClientAsync();
            var otherClientChannel = await CreateUniqueTempChannelAsync(watch: false, overrideClient: otherClient);

            var receivedEvent = false;
            IStreamChannelMember eventMember = null;
            IStreamChannel eventChannel = null;
            var eventThreadId = -1;
            Client.AddedToChannelAsMember += (channel2, member) =>
            {
                receivedEvent = true;
                eventMember = member;
                eventChannel = channel2;
                eventThreadId = GetCurrentThreadId();
            };

            await otherClientChannel.AddMembersAsync(hideHistory: default, optionalMessage: default, Client.LocalUserData.User);
            await WaitWhileFalseAsync(() => receivedEvent);

            Assert.IsTrue(receivedEvent);
            Assert.IsNotNull(eventChannel);
            Assert.IsNotNull(eventMember);
            Assert.AreEqual(otherClientChannel.Cid, eventChannel.Cid);
            Assert.AreEqual(Client.LocalUserData.User, eventMember.User);
            
            Assert.AreEqual(MainThreadId, eventThreadId);

            var receivedMessageEvent = false;
            var receivedMessage = string.Empty;
            IStreamChannel receivedMessageChannel = null;
            var messageEventThreadId = -1;
            otherClientChannel.MessageReceived += (messageChannel, message) =>
            {
                receivedMessageEvent = true;
                receivedMessage = message.Text;
                receivedMessageChannel = messageChannel;
                messageEventThreadId = GetCurrentThreadId();
            };
            
            await otherClientChannel.SendNewMessageAsync("Hello");
            await WaitWhileFalseAsync(() => receivedEvent);
            
            Assert.IsTrue(receivedMessageEvent);
            Assert.AreEqual(otherClientChannel.Cid, receivedMessageChannel.Cid);
            Assert.AreEqual(receivedMessage, "Hello");
            
            Assert.AreEqual(MainThreadId, messageEventThreadId);
        }

        [UnityTest]
        public IEnumerator When_user_removed_from_not_watched_channel_expect_user_removed_from_channel_event()
            => ConnectAndExecute(
                When_user_removed_from_not_watched_channel_expect_user_removed_from_channel_event_Async);

        private async Task When_user_removed_from_not_watched_channel_expect_user_removed_from_channel_event_Async()
        {
            var channel = await CreateUniqueTempChannelAsync(watch: false);

            var receivedAddedEvent = false;
            var receivedRemovedEvent = false;
            IStreamChannelMember eventMember = null;
            IStreamChannel eventChannel = null;

            Client.AddedToChannelAsMember += (channel2, member) => { receivedAddedEvent = true; };

            await channel.AddMembersAsync(hideHistory: default, optionalMessage: default, Client.LocalUserData.User);
            await WaitWhileFalseAsync(() => receivedAddedEvent);

            Client.RemovedFromChannelAsMember += (channel3, member2) =>
            {
                receivedRemovedEvent = true;
                eventMember = member2;
                eventChannel = channel3;
            };

            await channel.RemoveMembersAsync(new IStreamUser[] { Client.LocalUserData.User });
            await WaitWhileFalseAsync(() => receivedRemovedEvent);
            
            Assert.IsTrue(receivedRemovedEvent);
            Assert.IsNotNull(eventChannel);
            Assert.IsNotNull(eventMember);
            Assert.AreEqual(channel, eventChannel);
            Assert.AreEqual(Client.LocalUserData.User, eventMember.User);
        }
        
    }
}
#endif