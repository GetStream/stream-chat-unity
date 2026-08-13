#if STREAM_TESTS_ENABLED
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using StreamChat.Core;
using StreamChat.Core.Configs;
using StreamChat.Core.Requests;
using StreamChat.Core.StatefulModels;
using UnityEngine.TestTools;

namespace StreamChat.Tests.StatefulClient
{
    internal class MessageCacheWindowTests : BaseStateIntegrationTests
    {
        private static readonly MessageCacheWindow SmallWindow = new MessageCacheWindow(6, 3);

        [UnityTest]
        public IEnumerator When_no_message_cache_window_configured_expect_no_removal()
            => ConnectAndExecute(When_no_message_cache_window_configured_expect_no_removal_Async);

        private async Task When_no_message_cache_window_configured_expect_no_removal_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            var removedCount = 0;
            channel.MessagesRemovedFromCache += (_, __) => removedCount++;

            await SendMessagesAsync(channel, 10);

            Assert.AreEqual(10, channel.Messages.Count);
            Assert.AreEqual(0, removedCount);
        }

        [UnityTest]
        public IEnumerator When_message_count_exceeds_max_expect_trim_to_max_minus_discard()
            => ConnectAndExecute(When_message_count_exceeds_max_expect_trim_to_max_minus_discard_Async);

        private async Task When_message_count_exceeds_max_expect_trim_to_max_minus_discard_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            channel.OverrideMessageCacheWindow(SmallWindow);

            await SendMessagesAsync(channel, 7);

            Assert.AreEqual(3, channel.Messages.Count);
        }

        [UnityTest]
        public IEnumerator When_message_count_equals_max_expect_no_trim()
            => ConnectAndExecute(When_message_count_equals_max_expect_no_trim_Async);

        private async Task When_message_count_equals_max_expect_no_trim_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            channel.OverrideMessageCacheWindow(SmallWindow);
            var removedCount = 0;
            channel.MessagesRemovedFromCache += (_, __) => removedCount++;

            await SendMessagesAsync(channel, 6);

            Assert.AreEqual(6, channel.Messages.Count);
            Assert.AreEqual(0, removedCount);
        }

        [UnityTest]
        public IEnumerator When_trimmed_expect_oldest_removed_and_newest_retained()
            => ConnectAndExecute(When_trimmed_expect_oldest_removed_and_newest_retained_Async);

        private async Task When_trimmed_expect_oldest_removed_and_newest_retained_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            channel.OverrideMessageCacheWindow(SmallWindow);

            var sent = await SendMessagesAsync(channel, 7);
            var expectedIds = sent.Skip(4).Select(m => m.Id).ToList();
            var actualIds = channel.Messages.Select(m => m.Id).ToList();

            CollectionAssert.AreEqual(expectedIds, actualIds);
        }

        [UnityTest]
        public IEnumerator When_trimmed_expect_removed_messages_untracked_from_cache()
            => ConnectAndExecute(When_trimmed_expect_removed_messages_untracked_from_cache_Async);

        private async Task When_trimmed_expect_removed_messages_untracked_from_cache_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            channel.OverrideMessageCacheWindow(SmallWindow);

            var sent = await SendMessagesAsync(channel, 7);
            var removedIds = sent.Take(4).Select(m => m.Id).ToList();

            foreach (var id in removedIds)
            {
                Assert.IsFalse(Client.InternalCache.Messages.TryGet(id, out _),
                    $"Removed message {id} should no longer be tracked in the cache.");
            }
        }

        [UnityTest]
        public IEnumerator When_trimmed_expect_single_batched_event_with_all_removed_messages()
            => ConnectAndExecute(When_trimmed_expect_single_batched_event_with_all_removed_messages_Async);

        private async Task When_trimmed_expect_single_batched_event_with_all_removed_messages_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            channel.OverrideMessageCacheWindow(SmallWindow);

            var sent = await SendMessagesAsync(channel, 6);
            var eventInvocations = 0;
            IReadOnlyList<IStreamMessage> removedBatch = null;

            channel.MessagesRemovedFromCache += (_, messages) =>
            {
                eventInvocations++;
                removedBatch = messages;
            };

            await channel.SendNewMessageAsync($"msg-trigger-{Guid.NewGuid()}");

            Assert.AreEqual(1, eventInvocations);
            Assert.NotNull(removedBatch);
            Assert.AreEqual(4, removedBatch.Count);
            CollectionAssert.AreEqual(sent.Select(m => m.Id).ToList(), removedBatch.Select(m => m.Id).ToList());
        }

        [UnityTest]
        public IEnumerator When_message_triggers_trim_expect_MessageReceived_raised_before_removal()
            => ConnectAndExecute(When_message_triggers_trim_expect_MessageReceived_raised_before_removal_Async);

        private async Task When_message_triggers_trim_expect_MessageReceived_raised_before_removal_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            channel.OverrideMessageCacheWindow(SmallWindow);

            await SendMessagesAsync(channel, 6);

            var eventLog = new List<string>();
            channel.MessageReceived += (_, msg) => eventLog.Add($"received:{msg.Id}");
            channel.MessagesRemovedFromCache += (_, messages) =>
                eventLog.Add($"removed:{string.Join(",", messages.Select(m => m.Id))}");

            var trigger = await channel.SendNewMessageAsync($"msg-trigger-{Guid.NewGuid()}");

            var receivedIndex = eventLog.FindIndex(e => e == $"received:{trigger.Id}");
            var removedIndex = eventLog.FindIndex(e => e.StartsWith("removed:"));

            Assert.Greater(receivedIndex, -1);
            Assert.Greater(removedIndex, -1);
            Assert.Less(receivedIndex, removedIndex);
        }

        [UnityTest]
        public IEnumerator When_removed_message_is_pinned_expect_it_stays_in_cache()
            => ConnectAndExecute(When_removed_message_is_pinned_expect_it_stays_in_cache_Async);

        private async Task When_removed_message_is_pinned_expect_it_stays_in_cache_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            channel.OverrideMessageCacheWindow(SmallWindow);

            var first = await channel.SendNewMessageAsync($"msg-0-{Guid.NewGuid()}");
            await first.PinAsync();
            await WaitWhileFalseAsync(() => channel.PinnedMessages.Any(m => m.Id == first.Id),
                description: "pinned message to appear in PinnedMessages");

            await SendMessagesAsync(channel, 6);

            Assert.IsFalse(channel.Messages.Any(m => m.Id == first.Id));
            Assert.IsTrue(Client.InternalCache.Messages.TryGet(first.Id, out _));
        }

        [UnityTest]
        public IEnumerator When_pinned_message_removed_from_cache_expect_updates_still_applied()
            => ConnectAndExecute(When_pinned_message_removed_from_cache_expect_updates_still_applied_Async);

        private async Task When_pinned_message_removed_from_cache_expect_updates_still_applied_Async()
        {
            var otherClient = await GetConnectedOtherClientAsync();
            var channel = await CreateUniqueTempChannelAsync();
            var otherChannel = await otherClient.GetOrCreateChannelWithIdAsync(channel.Type, channel.Id);
            channel.OverrideMessageCacheWindow(SmallWindow);

            var first = await channel.SendNewMessageAsync($"msg-0-{Guid.NewGuid()}");
            await first.PinAsync();
            await WaitWhileFalseAsync(() => channel.PinnedMessages.Any(m => m.Id == first.Id),
                description: "pinned message to appear in PinnedMessages");

            await SendMessagesAsync(channel, 6);

            var pinned = channel.PinnedMessages.Single(m => m.Id == first.Id);
            const string UpdatedText = "pinned-after-cache-removal";

            var messageOnOther = otherChannel.Messages.Single(m => m.Id == first.Id);
            await messageOnOther.UpdateAsync(new StreamUpdateMessageRequest { Text = UpdatedText });

            await WaitWhileFalseAsync(() => pinned.Text == UpdatedText,
                description: "pinned message instance to receive message.updated after cache removal");
        }

        [UnityTest]
        public IEnumerator When_removed_message_is_thread_parent_expect_it_stays_in_cache()
            => ConnectAndExecute(When_removed_message_is_thread_parent_expect_it_stays_in_cache_Async);

        private async Task When_removed_message_is_thread_parent_expect_it_stays_in_cache_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            channel.OverrideMessageCacheWindow(SmallWindow);

            var parent = await channel.SendNewMessageAsync($"thread-parent-{Guid.NewGuid()}");
            await channel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                ParentId = parent.Id,
                ShowInChannel = false,
                Text = "thread reply",
            });

            await Client.GetThreadAsync(parent.Id, replyLimit: 5, participantLimit: 5);
            await SendMessagesAsync(channel, 6);

            Assert.IsFalse(channel.Messages.Any(m => m.Id == parent.Id));
            Assert.IsTrue(Client.InternalCache.Messages.TryGet(parent.Id, out _));
        }

        [UnityTest]
        public IEnumerator When_LoadOlderMessagesAsync_called_expect_trimming_paused()
            => ConnectAndExecute(When_LoadOlderMessagesAsync_called_expect_trimming_paused_Async);

        private async Task When_LoadOlderMessagesAsync_called_expect_trimming_paused_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            channel.OverrideMessageCacheWindow(SmallWindow);
            await SendMessagesAsync(channel, 3);

            await channel.LoadOlderMessagesAsync();

            Assert.IsTrue(channel.IsMessageCacheTrimmingPaused);
        }

        [UnityTest]
        public IEnumerator When_trimming_paused_expect_no_removal_on_new_messages()
            => ConnectAndExecute(When_trimming_paused_expect_no_removal_on_new_messages_Async);

        private async Task When_trimming_paused_expect_no_removal_on_new_messages_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            channel.OverrideMessageCacheWindow(SmallWindow);
            channel.PauseMessageCacheTrimming();

            var removedCount = 0;
            channel.MessagesRemovedFromCache += (_, __) => removedCount++;

            await SendMessagesAsync(channel, 10);

            Assert.AreEqual(10, channel.Messages.Count);
            Assert.AreEqual(0, removedCount);
        }

        [UnityTest]
        public IEnumerator When_ResumeMessageCacheTrimming_called_expect_immediate_trim()
            => ConnectAndExecute(When_ResumeMessageCacheTrimming_called_expect_immediate_trim_Async);

        private async Task When_ResumeMessageCacheTrimming_called_expect_immediate_trim_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            channel.OverrideMessageCacheWindow(SmallWindow);
            channel.PauseMessageCacheTrimming();

            await SendMessagesAsync(channel, 10);

            var removedCount = 0;
            channel.MessagesRemovedFromCache += (_, __) => removedCount++;

            channel.ResumeMessageCacheTrimming();

            Assert.AreEqual(3, channel.Messages.Count);
            Assert.AreEqual(1, removedCount);
        }

        [UnityTest]
        public IEnumerator When_OverrideMessageCacheWindow_called_expect_immediate_trim()
            => ConnectAndExecute(When_OverrideMessageCacheWindow_called_expect_immediate_trim_Async);

        private async Task When_OverrideMessageCacheWindow_called_expect_immediate_trim_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            await SendMessagesAsync(channel, 10);

            channel.OverrideMessageCacheWindow(SmallWindow);

            Assert.AreEqual(3, channel.Messages.Count);
        }

        [UnityTest]
        public IEnumerator When_channel_override_is_null_expect_unlimited_despite_client_default()
            => ConnectAndExecute(When_channel_override_is_null_expect_unlimited_despite_client_default_Async);

        private async Task When_channel_override_is_null_expect_unlimited_despite_client_default_Async()
        {
            var config = Client.LowLevelClient.Config;
            var previousDefault = config.DefaultMessageCacheWindow;
            try
            {
                config.DefaultMessageCacheWindow = SmallWindow;
                var channel = await CreateUniqueTempChannelAsync();
                channel.OverrideMessageCacheWindow(null);

                var removedCount = 0;
                channel.MessagesRemovedFromCache += (_, __) => removedCount++;

                await SendMessagesAsync(channel, 7);

                Assert.IsTrue(channel.HasMessageCacheWindowOverride);
                Assert.AreEqual(7, channel.Messages.Count);
                Assert.AreEqual(0, removedCount);
            }
            finally
            {
                config.DefaultMessageCacheWindow = previousDefault;
            }
        }

        [UnityTest]
        public IEnumerator When_ClearMessageCacheWindowOverride_called_expect_client_default_reapplied()
            => ConnectAndExecute(When_ClearMessageCacheWindowOverride_called_expect_client_default_reapplied_Async);

        private async Task When_ClearMessageCacheWindowOverride_called_expect_client_default_reapplied_Async()
        {
            var config = Client.LowLevelClient.Config;
            var previousDefault = config.DefaultMessageCacheWindow;
            try
            {
                config.DefaultMessageCacheWindow = SmallWindow;
                var channel = await CreateUniqueTempChannelAsync();
                channel.OverrideMessageCacheWindow(null);
                await SendMessagesAsync(channel, 7);

                channel.ClearMessageCacheWindowOverride();

                Assert.IsFalse(channel.HasMessageCacheWindowOverride);
                Assert.AreEqual(3, channel.Messages.Count);
            }
            finally
            {
                config.DefaultMessageCacheWindow = previousDefault;
            }
        }

        [UnityTest]
        public IEnumerator When_messages_removed_expect_LoadOlderMessagesAsync_refetches_them()
            => ConnectAndExecute(When_messages_removed_expect_LoadOlderMessagesAsync_refetches_them_Async);

        private async Task When_messages_removed_expect_LoadOlderMessagesAsync_refetches_them_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            channel.OverrideMessageCacheWindow(SmallWindow);

            var sent = await SendMessagesAsync(channel, 7);
            var removedIds = sent.Take(4).Select(m => m.Id).ToHashSet();
            var survivingIds = sent.Skip(4).Select(m => m.Id).ToList();

            channel.ResumeMessageCacheTrimming();
            await channel.LoadOlderMessagesAsync();

            var messageIds = channel.Messages.Select(m => m.Id).ToList();
            foreach (var id in removedIds)
            {
                Assert.Contains(id, messageIds);
            }

            CollectionAssert.IsOrdered(channel.Messages.Select(m => m.CreatedAt).ToList());
            Assert.AreEqual(survivingIds.Last(), messageIds.Last());
        }

        [UnityTest]
        public IEnumerator When_channel_query_returns_more_than_max_expect_no_removal_until_next_live_message()
            => ConnectAndExecute(When_channel_query_returns_more_than_max_expect_no_removal_until_next_live_message_Async);

        private async Task When_channel_query_returns_more_than_max_expect_no_removal_until_next_live_message_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            channel.OverrideMessageCacheWindow(SmallWindow);
            channel.PauseMessageCacheTrimming();

            await SendMessagesAsync(channel, 10);

            var removedCount = 0;
            channel.MessagesRemovedFromCache += (_, __) => removedCount++;

            await channel.LoadOlderMessagesAsync();

            Assert.Greater(channel.Messages.Count, SmallWindow.MaxMessages);
            Assert.AreEqual(0, removedCount);

            channel.ResumeMessageCacheTrimming();
            Assert.AreEqual(3, channel.Messages.Count);
            Assert.AreEqual(1, removedCount);

            removedCount = 0;
            await SendMessagesAsync(channel, 4);

            Assert.AreEqual(3, channel.Messages.Count);
            Assert.AreEqual(1, removedCount);
        }

        [Test]
        public void MessageCacheWindow_rejects_invalid_arguments()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new MessageCacheWindow(0, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new MessageCacheWindow(-1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new MessageCacheWindow(10, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new MessageCacheWindow(10, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new MessageCacheWindow(10, 10));
            Assert.Throws<ArgumentOutOfRangeException>(() => new MessageCacheWindow(10, 11));
        }

        [Test]
        public void MessageCacheWindow_Recommended_is_500_100()
        {
            Assert.AreEqual(500, MessageCacheWindow.Recommended.MaxMessages);
            Assert.AreEqual(100, MessageCacheWindow.Recommended.DiscardBatchSize);
        }

        private static async Task<List<IStreamMessage>> SendMessagesAsync(IStreamChannel channel, int count)
        {
            var messages = new List<IStreamMessage>(count);
            for (var i = 0; i < count; i++)
            {
                messages.Add(await channel.SendNewMessageAsync($"msg-{i}-{Guid.NewGuid()}"));
            }

            return messages;
        }
    }
}
#endif
