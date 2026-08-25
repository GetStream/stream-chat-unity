#if STREAM_TESTS_ENABLED
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.LowLevelClient.Requests;
using StreamChat.Core.Requests;
using StreamChat.Core.StatefulModels;
using UnityEngine.TestTools;

namespace StreamChat.Tests.StatefulClient
{
    /// <summary>
    /// Tests operations executed on <see cref="StreamMessage"/>
    /// </summary>
    internal class MessagesTests : BaseStateIntegrationTests
    {
        [UnityTest]
        public IEnumerator When_send_message_expect_no_errors()
            => ConnectAndExecute(When_send_message_expect_no_errors_Async);

        private async Task When_send_message_expect_no_errors_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();

            const string MessageText = "fds*fhjfdks9";

            var sentMessage = await channel.SendNewMessageAsync(MessageText);
            Assert.AreEqual(sentMessage.Text, MessageText);

            var messageInChannel = channel.Messages.FirstOrDefault(_ => _.Id == sentMessage.Id);
            Assert.NotNull(messageInChannel);
        }

        [UnityTest]
        public IEnumerator When_send_message_with_custom_data_expect_custom_data_on_message()
            => ConnectAndExecute(When_send_message_with_custom_data_expect_custom_data_on_message_Async);

        private async Task When_send_message_with_custom_data_expect_custom_data_on_message_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();

            const string MessageText = "fds*fhjfdks9";

            var sentMessage = await channel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                Text = MessageText,
                CustomData = new StreamCustomDataRequest
                {
                    { "Age", 12 },
                    { "Sports", new string[] { "Yoga", "Climbing" } }
                }
            });

            var messageInChannel = channel.Messages.FirstOrDefault(_ => _.Id == sentMessage.Id);
            Assert.NotNull(messageInChannel);

            Assert.AreEqual(MessageText, messageInChannel.Text);
            Assert.AreEqual(12, messageInChannel.CustomData.Get<int>("Age"));
            Assert.AreEqual(new string[] { "Yoga", "Climbing" }, messageInChannel.CustomData.Get<string[]>("Sports"));
        }

        // Requires the auto-translation feature to be enabled for the app the tests run against
        [UnityTest]
        public IEnumerator When_message_translate_expect_translation_in_i18n()
            => ConnectAndExecute(When_message_translate_expect_translation_in_i18n_Async);

        private async Task When_message_translate_expect_translation_in_i18n_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();

            var sentMessage = await channel.SendNewMessageAsync("Hello, world!");

            await sentMessage.TranslateAsync("fr");
            await WaitWhileFalseAsync(() => sentMessage.I18n.ContainsKey("fr_text"));

            Assert.IsFalse(string.IsNullOrEmpty(sentMessage.I18n["fr_text"]));
        }

        [UnityTest]
        public IEnumerator When_Update_message_expect_message_changed()
            => ConnectAndExecute(When_Update_message_expect_message_changed_Async);

        private async Task When_Update_message_expect_message_changed_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();

            const string MessageText = "fds*fhjfdks9";

            var sentMessage = await channel.SendNewMessageAsync(MessageText);
            Assert.AreEqual(MessageText, sentMessage.Text);

            var messageInChannel = channel.Messages.FirstOrDefault(_ => _.Id == sentMessage.Id);
            Assert.NotNull(messageInChannel);

            // StreamTodo: Test all properties like pinned, attachments, mentions, Html, Mml, etc.
            await messageInChannel.UpdateAsync(new StreamUpdateMessageRequest
            {
                Text = "New changed message",
            });

            messageInChannel = channel.Messages.FirstOrDefault(_ => _.Id == sentMessage.Id);
            Assert.NotNull(messageInChannel);

            Assert.AreEqual("New changed message", messageInChannel.Text);
        }

        [UnityTest]
        public IEnumerator When_Update_message_custom_data_expect_message_custom_data_changed()
            => ConnectAndExecute(When_Update_message_custom_data_expect_message_custom_data_changed_Async);

        private async Task When_Update_message_custom_data_expect_message_custom_data_changed_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();

            const string MessageText = "fds*fhjfdks9";

            var sentMessage = await channel.SendNewMessageAsync(MessageText);

            var messageInChannel = channel.Messages.FirstOrDefault(_ => _.Id == sentMessage.Id);
            Assert.NotNull(messageInChannel);

            await messageInChannel.UpdateAsync(new StreamUpdateMessageRequest
            {
                Text = "New changed message",
                CustomData = new StreamCustomDataRequest
                {
                    { "CategoryId", 12 },
                    { "Awards", new string[] { "Funny", "Inspirational" } }
                }
            });

            messageInChannel = channel.Messages.FirstOrDefault(_ => _.Id == sentMessage.Id);
            Assert.NotNull(messageInChannel);
            Assert.AreEqual(12, messageInChannel.CustomData.Get<int>("CategoryId"));
            Assert.AreEqual(new string[] { "Funny", "Inspirational" },
                messageInChannel.CustomData.Get<string[]>("Awards"));
        }

        [UnityTest]
        public IEnumerator When_message_soft_delete_message_expect_text_cleared()
            => ConnectAndExecute(When_message_soft_delete_message_expect_text_cleared_Async);

        public async Task When_message_soft_delete_message_expect_text_cleared_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();

            const string MessageText = "fds*fhjfdks9";

            var sentMessage = await channel.SendNewMessageAsync(MessageText);
            Assert.AreEqual(sentMessage.Text, MessageText);

            var messageInChannel = channel.Messages.FirstOrDefault(_ => _.Id == sentMessage.Id);
            Assert.NotNull(messageInChannel);

            // SoftDeleteAsync applies the REST response to the cache before returning,
            // so DeletedAt and the cleared text are visible immediately on the same
            // instance the customer is holding. No need to wait for the WS event here -
            // see When_other_client_soft_deletes_message_expect_message_deleted_event_observed
            // for the watcher-side WS verification.
            await messageInChannel.SoftDeleteAsync();

            Assert.NotNull(messageInChannel);
            Assert.IsNotNull(messageInChannel.DeletedAt);
            Assert.IsEmpty(messageInChannel.Text);
            Assert.IsTrue(messageInChannel.IsDeleted);
        }

        /// <summary>
        /// Companion to <see cref="When_message_soft_delete_message_expect_text_cleared"/>:
        /// the deleter applies the REST response to its own cache synchronously, so the
        /// only path we still need WS coverage for is *another* watching client. This
        /// test asserts that a watcher sees the soft-delete via the `message.deleted`
        /// WS event - both as a `MessageDeleted` channel event and as state on the
        /// cached <see cref="IStreamMessage"/>.
        /// </summary>
        [UnityTest]
        public IEnumerator When_other_client_soft_deletes_message_expect_message_deleted_event_observed()
            => ConnectAndExecute(When_other_client_soft_deletes_message_expect_message_deleted_event_observed_Async);

        private async Task When_other_client_soft_deletes_message_expect_message_deleted_event_observed_Async()
        {
            var otherClient = await GetConnectedOtherClientAsync();

            var channel = await CreateUniqueTempChannelAsync();

            // Watch the same channel from `otherClient` so it receives WS events for it.
            var otherClientChannel = await otherClient.GetOrCreateChannelWithIdAsync(channel.Type, channel.Id);
            Assert.AreEqual(channel.Cid, otherClientChannel.Cid);

            const string MessageText = "to-be-soft-deleted";
            var sentMessage = await channel.SendNewMessageAsync(MessageText);

            // Wait until the watcher has observed the new message via WS so we have
            // a stateful instance to assert on once it's deleted. Without this, a
            // very fast soft-delete could race the `message.new` delivery and we'd
            // miss the message entirely on `otherClient`.
            await WaitWhileFalseAsync(
                () => otherClientChannel.Messages.Any(m => m.Id == sentMessage.Id),
                description: "watcher to receive message.new for the message about to be deleted");

            var observedOnOther = otherClientChannel.Messages.Single(m => m.Id == sentMessage.Id);

            var deletedEventCount = 0;
            IStreamMessage eventMessage = null;
            bool? eventIsHardDelete = null;

            void OnDeleted(IStreamChannel ch, IStreamMessage msg, bool isHardDelete)
            {
                if (msg.Id != sentMessage.Id)
                {
                    return;
                }

                deletedEventCount++;
                eventMessage = msg;
                eventIsHardDelete = isHardDelete;
            }

            otherClientChannel.MessageDeleted += OnDeleted;
            try
            {
                await sentMessage.SoftDeleteAsync();

                await WaitWhileFalseAsync(
                    () => deletedEventCount > 0,
                    description: "watcher to receive message.deleted WS event after soft-delete");

                Assert.AreEqual(1, deletedEventCount, "MessageDeleted should fire exactly once on the watcher.");
                Assert.IsFalse(eventIsHardDelete.GetValueOrDefault(true),
                    "WS event must report this as a soft-delete (isHardDelete=false).");
                Assert.AreSame(observedOnOther, eventMessage,
                    "Event must surface the same cached message instance the watcher already holds.");
                Assert.IsTrue(observedOnOther.IsDeleted,
                    "Watcher's cached message should be flagged as deleted after WS event.");
                Assert.IsTrue(observedOnOther.DeletedAt.HasValue,
                    "Watcher's cached message should have DeletedAt populated after WS event.");
                Assert.IsEmpty(observedOnOther.Text,
                    "Watcher's cached message text should be cleared after soft-delete WS event.");
            }
            finally
            {
                otherClientChannel.MessageDeleted -= OnDeleted;
            }
        }

        [UnityTest]
        public IEnumerator When_message_hard_delete_message_expect_message_removed()
            => ConnectAndExecute(When_message_hard_delete_message_expect_message_removed_Async);

        public async Task When_message_hard_delete_message_expect_message_removed_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();

            const string MessageText = "fds*fhjfdks9";

            var sentMessage = await channel.SendNewMessageAsync(MessageText);
            Assert.AreEqual(sentMessage.Text, MessageText);

            var messageInChannel = channel.Messages.FirstOrDefault(_ => _.Id == sentMessage.Id);
            Assert.NotNull(messageInChannel);

            await messageInChannel.HardDeleteAsync();

            await WaitWhileTrueAsync(() => !messageInChannel.DeletedAt.HasValue);

            messageInChannel = channel.Messages.FirstOrDefault(_ => _.Id == sentMessage.Id);
            Assert.IsNull(messageInChannel);
        }

        [UnityTest]
        public IEnumerator When_message_pin_expected_message_pinned()
            => ConnectAndExecute(When_message_pin_expected_message_pinned_Async);

        public async Task When_message_pin_expected_message_pinned_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();

            const string MessageText = "fds*fhjfdks9";

            var sentMessage = await channel.SendNewMessageAsync(MessageText);
            Assert.AreEqual(sentMessage.Text, MessageText);

            await sentMessage.PinAsync();

            var messageInChannel = channel.Messages.FirstOrDefault(_ => _.Id == sentMessage.Id);
            Assert.AreEqual(true, messageInChannel.Pinned);
            Assert.IsNull(messageInChannel.PinExpires);

            Assert.AreEqual(Client.LocalUserData.User, messageInChannel.PinnedBy);
        }

        [UnityTest]
        public IEnumerator When_message_pin_with_expire_expected_message_pinned_with_expire_set()
            => ConnectAndExecute(When_message_pin_with_expire_expected_message_pinned_with_expire_set_Async);

        public async Task When_message_pin_with_expire_expected_message_pinned_with_expire_set_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();

            const string MessageText = "fds*fhjfdks9";

            var sentMessage = await channel.SendNewMessageAsync(MessageText);
            Assert.AreEqual(sentMessage.Text, MessageText);

            // Use hours because due to rate limit the response can potentially take some time
            var targetExpiryDate = DateTime.UtcNow.Add(new TimeSpan(0, 5, 0, 0));
            await sentMessage.PinAsync(targetExpiryDate);
            await WaitWhileFalseAsync(() => sentMessage.Pinned && sentMessage.PinExpires.HasValue);

            Assert.AreEqual(true, sentMessage.Pinned);

            Assert.That(sentMessage.PinExpires.Value, Is.EqualTo(new DateTimeOffset(targetExpiryDate)).Within(3).Minutes);
        }

        [UnityTest]
        public IEnumerator When_pinned_message_unpin_expected_message_unpinned()
            => ConnectAndExecute(When_pinned_message_unpin_expected_message_unpinned_Async);

        public async Task When_pinned_message_unpin_expected_message_unpinned_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();

            const string MessageText = "fds*fhjfdks9";

            var sentMessage = await channel.SendNewMessageAsync(MessageText);
            Assert.AreEqual(sentMessage.Text, MessageText);

            await sentMessage.PinAsync();

            var messageInChannel = channel.Messages.FirstOrDefault(_ => _.Id == sentMessage.Id);
            Assert.AreEqual(true, messageInChannel.Pinned);

            await messageInChannel.UnpinAsync();
            messageInChannel = channel.Messages.FirstOrDefault(_ => _.Id == sentMessage.Id);
            Assert.AreEqual(false, messageInChannel.Pinned);
        }

        [UnityTest]
        public IEnumerator When_message_pin_expected_message_appear_in_channel_pinned_messages()
            => ConnectAndExecute(When_message_pin_expected_message_appear_in_channel_pinned_messages_Async);

        public async Task When_message_pin_expected_message_appear_in_channel_pinned_messages_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();

            const string MessageText = "fds*fhjfdks9";

            var sentMessage = await channel.SendNewMessageAsync(MessageText);
            Assert.AreEqual(sentMessage.Text, MessageText);

            await sentMessage.PinAsync();

            var pinnedMessage = channel.PinnedMessages.FirstOrDefault(m => m == sentMessage);
            Assert.NotNull(pinnedMessage);
        }

        [UnityTest]
        public IEnumerator When_pinned_message_unpinned_expected_message_removed_from_channel_pinned_messages()
            => ConnectAndExecute(
                When_pinned_message_unpinned_expected_message_removed_from_channel_pinned_messages_Async);

        public async Task When_pinned_message_unpinned_expected_message_removed_from_channel_pinned_messages_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();

            const string MessageText = "fds*fhjfdks9";

            var sentMessage = await channel.SendNewMessageAsync(MessageText);
            Assert.AreEqual(sentMessage.Text, MessageText);

            await sentMessage.PinAsync();

            await sentMessage.UnpinAsync();

            var pinnedMessage = channel.PinnedMessages.FirstOrDefault(m => m == sentMessage);
            Assert.IsNull(pinnedMessage);
        }

        [UnityTest]
        public IEnumerator When_message_flag_requested_expected_message_flag()
            => ConnectAndExecute(When_message_flag_requested_expected_message_flag_Async);

        public async Task When_message_flag_requested_expected_message_flag_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();

            const string MessageText = "fds*fhjfdks9";

            var sentMessage = await channel.SendNewMessageAsync(MessageText);
            Assert.AreEqual(sentMessage.Text, MessageText);

            await sentMessage.FlagAsync();

            var response = await Client.InternalLowLevelClient.InternalModerationApi.QueryMessageFlagsAsync(
                new QueryMessageFlagsRequestInternalDTO
                {
                    FilterConditions = new Dictionary<string, object>()
                    {
                        {
                            "channel_cid", new Dictionary<string, string>()
                            {
                                { "$eq", channel.Cid }
                            }
                        }
                    },
                    Limit = 30,
                    Offset = 0,
                });

            var messageFlag = response.Flags.FirstOrDefault(_ => _.Message.Id == sentMessage.Id);
            Assert.NotNull(messageFlag);
        }

        [UnityTest]
        public IEnumerator when_search_for_mentioned_messages_expect_valid_results()
            => ConnectAndExecute(when_search_for_mentioned_messages_expect_valid_results_Async);

        private async Task when_search_for_mentioned_messages_expect_valid_results_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            var userToMention = await CreateUniqueTempUserAsync("Michael");

            await channel.SendNewMessageAsync("Hello");
            await channel.SendNewMessageAsync("How");
            await channel.SendNewMessageAsync("Are");
            var messageWithMention = await channel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                Text = "You doing? ",
                MentionedUsers = new List<IStreamUser>()
                {
                    userToMention
                }
            });

            //StreamTodo: implement with stateful client
            var searchResult = await TryAsync(() => Client.LowLevelClient.MessageApi.SearchMessagesAsync(new SearchRequest
            {
                FilterConditions = new Dictionary<string, object>
                {
                    {
                        "cid", new Dictionary<string, object>
                        {
                            { "$eq", channel.Cid }
                        }
                    }
                },
                MessageFilterConditions = new Dictionary<string, object>
                {
                    {
                        "mentioned_users.id", new Dictionary<string, object>
                        {
                            { "$contains", userToMention.Id }
                        }
                    }
                }
            }), results => results?.Results?.Count > 0);

            Assert.IsNotEmpty(searchResult.Results);
            Assert.IsNotNull(searchResult.Results.FirstOrDefault(s => s.Message.Id == messageWithMention.Id));
        }
    }
}
#endif