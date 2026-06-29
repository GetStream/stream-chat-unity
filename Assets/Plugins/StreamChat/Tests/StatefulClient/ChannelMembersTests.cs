#if STREAM_TESTS_ENABLED
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using StreamChat.Core;
using StreamChat.Core.Models;
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

            var otherUserId = AdminSecondaryCredentials.UserId;

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

            var otherUserId = AdminSecondaryCredentials.UserId;

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
            var otherUserId = AdminSecondaryCredentials.UserId;

            var memberAddedMsg = $"{otherUserId} was added to the channel";

            var tcs = new TaskCompletionSource<bool>();

            void OnMessageReceived(IStreamChannel channel2, IStreamMessage message)
            {
                if (channel2.Cid != channel.Cid)
                {
                    return;
                }

                Assert.AreEqual(message.Text, memberAddedMsg);
                tcs.SetResult(true);
            }

            channel.MessageReceived += OnMessageReceived;

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

            await WaitWithTimeoutAsync(tcs.Task, $"Event {nameof(channel.MessageReceived)} was not received");

            channel.MessageReceived -= OnMessageReceived;
        }

        [UnityTest]
        public IEnumerator When_remove_member_by_reference_to_channel_expect_member_removed_from_channel_members()
            => ConnectAndExecute(
                When_remove_member_by_reference_to_channel_expect_member_removed_from_channel_members_Async);

        private async Task When_remove_member_by_reference_to_channel_expect_member_removed_from_channel_members_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();

            var otherUserId = AdminSecondaryCredentials.UserId;

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

            var otherUserId = AdminSecondaryCredentials.UserId;

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

            var otherUsers = new[] { AdminSecondaryCredentials, UserPrimaryCredentials };
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

            void OnMemberAdded(IStreamChannel channel2, IStreamChannelMember member)
            {
                if (channel2.Cid != channel.Cid)
                {
                    return;
                }

                receivedEvent = true;
                eventMember = member;
                eventChannel = channel2;
            }

            channel.MemberAdded += OnMemberAdded;

            var receivedEvent2 = false;
            IStreamChannelMember eventMember2 = null;
            IStreamChannel eventChannel2 = null;
            OperationType? opType = default;

            void OnMembersChanged(IStreamChannel channel2, IStreamChannelMember member, OperationType op)
            {
                if (channel2.Cid != channel.Cid)
                {
                    return;
                }

                receivedEvent2 = true;
                eventMember2 = member;
                eventChannel2 = channel2;
                opType = op;
            }

            channel.MembersChanged += OnMembersChanged;

            await channel.AddMembersAsync(hideHistory: default, optionalMessage: default, user);

            await WaitWhileFalseAsync(() => receivedEvent && receivedEvent2);

            channel.MemberAdded -= OnMemberAdded;
            channel.MembersChanged -= OnMembersChanged;

            Assert.IsTrue(receivedEvent);
            Assert.IsNotNull(eventChannel);
            Assert.IsNotNull(eventMember);
            Assert.AreEqual(user, eventMember.User);

            Assert.IsTrue(receivedEvent2);
            Assert.IsNotNull(eventChannel2);
            Assert.IsNotNull(eventMember2);
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

            void OnMemberRemoved(IStreamChannel channel2, IStreamChannelMember member)
            {
                if (channel2.Cid != channel.Cid)
                {
                    return;
                }

                receivedEvent = true;
                eventMember = member;
                eventChannel = channel2;
            }

            channel.MemberRemoved += OnMemberRemoved;

            var receivedEvent2 = false;
            IStreamChannelMember eventMember2 = null;
            IStreamChannel eventChannel2 = null;
            OperationType? opType = default;

            void OnMembersChanged(IStreamChannel channel3, IStreamChannelMember member, OperationType op)
            {
                if (channel3.Cid != channel.Cid)
                {
                    return;
                }

                receivedEvent2 = true;
                eventMember2 = member;
                eventChannel2 = channel3;
                opType = op;
            }

            channel.MembersChanged += OnMembersChanged;

            await channel.AddMembersAsync(hideHistory: default, optionalMessage: default, user);
            await channel.RemoveMembersAsync(user);

            await WaitWhileFalseAsync(() => receivedEvent && receivedEvent2);

            channel.MemberRemoved -= OnMemberRemoved;
            channel.MembersChanged -= OnMembersChanged;

            Assert.IsTrue(receivedEvent);
            Assert.IsNotNull(eventChannel);
            Assert.IsNotNull(eventMember);
            Assert.AreEqual(user, eventMember.User);

            Assert.IsTrue(receivedEvent2);
            Assert.IsNotNull(eventChannel2);
            Assert.IsNotNull(eventMember2);
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

            void OnAddedToChannelAsMember(IStreamChannel channel2, IStreamChannelMember member)
            {
                if (channel2.Cid != channel.Cid)
                {
                    return;
                }

                receivedEvent = true;
                eventMember = member;
                eventChannel = channel2;
            }

            Client.AddedToChannelAsMember += OnAddedToChannelAsMember;

            await channel.AddMembersAsync(hideHistory: default, optionalMessage: default, Client.LocalUserData.User);
            await WaitWhileFalseAsync(() => receivedEvent);

            Client.AddedToChannelAsMember -= OnAddedToChannelAsMember;

            Assert.IsTrue(receivedEvent);
            Assert.IsNotNull(eventChannel);
            Assert.IsNotNull(eventMember);
            Assert.AreEqual(Client.LocalUserData.User, eventMember.User);
        }

        [UnityTest]
        public IEnumerator
            When_user_added_to_not_watched_channel_expect_user_receive_added_to_channel_event_from_main_thread()
            => ConnectAndExecute(
                When_user_added_to_not_watched_channel_expect_user_receive_added_to_channel_event_from_main_thread_Async);

        private async Task
            When_user_added_to_not_watched_channel_expect_user_receive_added_to_channel_event_from_main_thread_Async()
        {
            var channel = await CreateUniqueTempChannelAsync(watch: false);

            var receivedEvent = false;
            IStreamChannelMember eventMember = null;
            IStreamChannel eventChannel = null;
            var receivedEventThreadId = 0;

            void OnAddedToChannelAsMember(IStreamChannel channel2, IStreamChannelMember member)
            {
                if (channel2.Cid != channel.Cid)
                {
                    return;
                }

                receivedEvent = true;
                eventMember = member;
                eventChannel = channel2;
                receivedEventThreadId = GetCurrentThreadId();
            }

            Client.AddedToChannelAsMember += OnAddedToChannelAsMember;

            await channel.AddMembersAsync(hideHistory: default, optionalMessage: default, Client.LocalUserData.User);
            await WaitWhileFalseAsync(() => receivedEvent);

            Client.AddedToChannelAsMember -= OnAddedToChannelAsMember;

            Assert.IsTrue(receivedEvent);
            Assert.IsNotNull(eventChannel);
            Assert.IsNotNull(eventMember);
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

            void OnAddedToChannelAsMember(IStreamChannel channel2, IStreamChannelMember member)
            {
                if (channel2.Cid != otherClientChannel.Cid)
                {
                    return;
                }

                receivedEvent = true;
                eventMember = member;
                eventChannel = channel2;
                eventThreadId = GetCurrentThreadId();
            }

            Client.AddedToChannelAsMember += OnAddedToChannelAsMember;

            await otherClientChannel.AddMembersAsync(hideHistory: default, optionalMessage: default,
                Client.LocalUserData.User);
            await WaitWhileFalseAsync(() => receivedEvent);

            Client.AddedToChannelAsMember -= OnAddedToChannelAsMember;

            Assert.IsTrue(receivedEvent);
            Assert.IsNotNull(eventChannel);
            Assert.IsNotNull(eventMember);
            Assert.AreEqual(Client.LocalUserData.User, eventMember.User);
            Assert.AreEqual(MainThreadId, eventThreadId);

            var receivedMessageEvent = false;
            var receivedMessage = string.Empty;
            IStreamChannel receivedMessageChannel = null;
            var messageEventThreadId = -1;

            void OnMessageReceived(IStreamChannel messageChannel, IStreamMessage message)
            {
                if (messageChannel.Cid != otherClientChannel.Cid)
                {
                    return;
                }

                receivedMessageEvent = true;
                receivedMessage = message.Text;
                receivedMessageChannel = messageChannel;
                messageEventThreadId = GetCurrentThreadId();
            }

            otherClientChannel.MessageReceived += OnMessageReceived;

            await otherClientChannel.SendNewMessageAsync("Hello");
            await WaitWhileFalseAsync(() => receivedMessageEvent);

            otherClientChannel.MessageReceived -= OnMessageReceived;

            Assert.IsTrue(receivedMessageEvent);
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

            void OnAddedToChannelAsMember(IStreamChannel channel2, IStreamChannelMember member)
            {
                if (channel2.Cid != channel.Cid)
                {
                    return;
                }

                receivedAddedEvent = true;
            }

            Client.AddedToChannelAsMember += OnAddedToChannelAsMember;

            await channel.AddMembersAsync(hideHistory: default, optionalMessage: default, Client.LocalUserData.User);
            await WaitWhileFalseAsync(() => receivedAddedEvent);

            void OnRemovedFromChannelAsMember(IStreamChannel channel3, IStreamChannelMember member2)
            {
                if (channel3.Cid != channel.Cid)
                {
                    return;
                }

                receivedRemovedEvent = true;
                eventMember = member2;
                eventChannel = channel3;
            }

            Client.RemovedFromChannelAsMember += OnRemovedFromChannelAsMember;

            await channel.RemoveMembersAsync(new IStreamUser[] { Client.LocalUserData.User });
            await WaitWhileFalseAsync(() => receivedRemovedEvent);

            Client.AddedToChannelAsMember -= OnAddedToChannelAsMember;
            Client.RemovedFromChannelAsMember -= OnRemovedFromChannelAsMember;

            Assert.IsTrue(receivedRemovedEvent);
            Assert.IsNotNull(eventChannel);
            Assert.IsNotNull(eventMember);
            Assert.AreEqual(Client.LocalUserData.User, eventMember.User);
        }

        [UnityTest]
        public IEnumerator
            When_local_user_removed_from_unwatched_channel_expect_removed_from_channel_as_member_only()
            => ConnectAndExecute(
                When_local_user_removed_from_unwatched_channel_expect_removed_from_channel_as_member_only_Async);

        /// <summary>
        /// notification.removed_from_channel is delivered to the removed user when they are not
        /// watching the channel. The stateful client surfaces that as
        /// <see cref="IStreamChatClient.RemovedFromChannelAsMember"/>, not
        /// <see cref="IStreamChannel.MemberRemoved"/>.
        /// </summary>
        private async Task When_local_user_removed_from_unwatched_channel_expect_removed_from_channel_as_member_only_Async()
        {
            var channel = await CreateUniqueTempChannelAsync(watch: false);

            await channel.AddMembersAsync(hideHistory: default, optionalMessage: default, Client.LocalUserData.User);
            await WaitWhileFalseAsync(
                () => channel.Members.Any(m => m.User?.Id == Client.LocalUserData.UserId),
                description: "local user to appear in unwatched channel members after add");

            Assert.IsFalse(channel.IsWatched,
                "Precondition: channel must not be watched so the server sends notification.removed_from_channel");

            var removedFromChannelAsMemberFired = false;
            var memberRemovedFired = false;
            IStreamChannelMember removedMember = null;

            void OnRemovedFromChannelAsMember(IStreamChannel channel2, IStreamChannelMember member)
            {
                if (channel2.Cid != channel.Cid)
                {
                    return;
                }

                removedFromChannelAsMemberFired = true;
                removedMember = member;
            }

            void OnMemberRemoved(IStreamChannel channel2, IStreamChannelMember member)
            {
                if (channel2.Cid != channel.Cid)
                {
                    return;
                }

                memberRemovedFired = true;
            }

            Client.RemovedFromChannelAsMember += OnRemovedFromChannelAsMember;
            channel.MemberRemoved += OnMemberRemoved;

            await channel.RemoveMembersAsync(Client.LocalUserData.User);
            await WaitWhileFalseAsync(() => removedFromChannelAsMemberFired,
                description: "Client.RemovedFromChannelAsMember after removing local user from unwatched channel");

            Client.RemovedFromChannelAsMember -= OnRemovedFromChannelAsMember;
            channel.MemberRemoved -= OnMemberRemoved;

            Assert.IsTrue(removedFromChannelAsMemberFired);
            Assert.IsFalse(memberRemovedFired,
                "IStreamChannel.MemberRemoved must not fire when the local user is removed from an unwatched channel");
            Assert.IsNotNull(removedMember);
            Assert.AreEqual(Client.LocalUserData.UserId, removedMember.User?.Id);
        }

        [UnityTest]
        public IEnumerator When_local_user_removed_from_watched_channel_expect_member_removed_only()
            => ConnectAndExecute(When_local_user_removed_from_watched_channel_expect_member_removed_only_Async);

        /// <summary>
        /// member.removed is delivered to clients watching the channel. When the local user is
        /// removed while watching, the stateful client surfaces
        /// <see cref="IStreamChannel.MemberRemoved"/> instead of
        /// <see cref="IStreamChatClient.RemovedFromChannelAsMember"/>.
        /// The server may still send notification.removed_from_channel alongside member.removed;
        /// the SDK suppresses the stateful notification event when the channel is watched locally.
        /// </summary>
        private async Task When_local_user_removed_from_watched_channel_expect_member_removed_only_Async()
        {
            var otherClient = await GetConnectedOtherClientAsync();
            var otherClientChannel = await CreateUniqueTempChannelAsync(watch: true, overrideClient: otherClient);

            await otherClientChannel.AddMembersAsync(hideHistory: default, optionalMessage: default,
                Client.LocalUserData.User);

            var channel = await Client.GetOrCreateChannelWithIdAsync(otherClientChannel.Type, otherClientChannel.Id);

            await WaitWhileFalseAsync(
                () => channel.IsWatched && channel.Members.Any(m => m.User?.Id == Client.LocalUserData.UserId),
                description: "local client to watch channel and have local user membership before removal");

            Assert.IsTrue(channel.IsWatched,
                "Precondition: channel must be watched so the server sends member.removed");

            var removedFromChannelAsMemberFired = false;
            var memberRemovedFired = false;
            IStreamChannelMember removedMember = null;

            void OnRemovedFromChannelAsMember(IStreamChannel channel2, IStreamChannelMember member)
            {
                if (channel2.Cid != channel.Cid)
                {
                    return;
                }

                removedFromChannelAsMemberFired = true;
            }

            void OnMemberRemoved(IStreamChannel channel2, IStreamChannelMember member)
            {
                if (channel2.Cid != channel.Cid)
                {
                    return;
                }

                memberRemovedFired = true;
                removedMember = member;
            }

            Client.RemovedFromChannelAsMember += OnRemovedFromChannelAsMember;
            channel.MemberRemoved += OnMemberRemoved;

            await otherClientChannel.RemoveMembersAsync(Client.LocalUserData.User);
            await WaitWhileFalseAsync(() => memberRemovedFired,
                description: "channel.MemberRemoved after other client removes local user from watched channel");

            Client.RemovedFromChannelAsMember -= OnRemovedFromChannelAsMember;
            channel.MemberRemoved -= OnMemberRemoved;

            Assert.IsTrue(memberRemovedFired);
            Assert.IsFalse(removedFromChannelAsMemberFired,
                "Client.RemovedFromChannelAsMember must not fire when the local user is removed from a watched channel");
            Assert.IsNotNull(removedMember);
            Assert.AreEqual(Client.LocalUserData.UserId, removedMember.User?.Id);
        }
    }
}
#endif