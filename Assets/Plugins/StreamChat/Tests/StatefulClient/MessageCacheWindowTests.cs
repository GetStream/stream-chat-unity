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

        // History limit low enough to be reached by sending a handful of messages while paused.
        private static readonly MessageCacheWindow SmallWindowWithLowHistoryLimit = new MessageCacheWindow(4, 2, 8);

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

            // 6 sent + 1 trigger = 7 > MaxMessages(6), trimmed down to MaxMessages - DiscardBatchSize = 3,
            // so the 4 oldest are removed in a single batch, oldest first.
            Assert.AreEqual(1, eventInvocations);
            Assert.NotNull(removedBatch);
            Assert.AreEqual(4, removedBatch.Count);
            CollectionAssert.AreEqual(sent.Take(4).Select(m => m.Id).ToList(),
                removedBatch.Select(m => m.Id).ToList());
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
            await messageOnOther.UpdateOverwriteAsync(new StreamUpdateMessageRequest { Text = UpdatedText });

            await WaitWhileFalseAsync(() => pinned.Text == UpdatedText,
                description: "pinned message instance to receive message.updated after cache removal");
        }

        [UnityTest]
        public IEnumerator When_pinned_message_removed_from_cache_expect_partial_updates_still_applied()
            => ConnectAndExecute(When_pinned_message_removed_from_cache_expect_partial_updates_still_applied_Async);

        private async Task When_pinned_message_removed_from_cache_expect_partial_updates_still_applied_Async()
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
            const string UpdatedText = "pinned-partial-after-cache-removal";

            var messageOnOther = otherChannel.Messages.Single(m => m.Id == first.Id);
            await messageOnOther.UpdatePartialAsync(setFields: new Dictionary<string, object>
            {
                { "text", UpdatedText },
            });

            await WaitWhileFalseAsync(() => pinned.Text == UpdatedText,
                description: "pinned message instance to receive message.updated after partial update");
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

        /// <summary>
        /// Trimming removes the OLDEST messages, which while paused is exactly the history the user
        /// scrolled back to. So pausing must remove nothing at all, even past MaxHistoryMessages -
        /// growth is bounded by refusing to page in more history, not by deleting what is on screen.
        /// </summary>
        [UnityTest]
        public IEnumerator When_trimming_paused_expect_no_removal_even_past_max_history_messages()
            => ConnectAndExecute(When_trimming_paused_expect_no_removal_even_past_max_history_messages_Async);

        private async Task When_trimming_paused_expect_no_removal_even_past_max_history_messages_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            channel.OverrideMessageCacheWindow(SmallWindowWithLowHistoryLimit);
            channel.PauseMessageCacheTrimming();

            var removedCount = 0;
            channel.MessagesRemovedFromCache += (_, __) => removedCount++;

            var sent = await SendMessagesAsync(channel, 10);

            Assert.Greater(10, SmallWindowWithLowHistoryLimit.MaxHistoryMessages,
                "the test must actually push the channel past MaxHistoryMessages");
            Assert.IsTrue(channel.IsMessageCacheTrimmingPaused);
            Assert.AreEqual(10, channel.Messages.Count);
            Assert.AreEqual(0, removedCount);
            CollectionAssert.AreEqual(sent.Select(m => m.Id).ToList(),
                channel.Messages.Select(m => m.Id).ToList());
        }

        /// <summary>
        /// The history limit stops history from being paged in rather than deleting what is already there,
        /// so the oldest message stays put and the pagination anchor never moves backwards.
        /// </summary>
        [UnityTest]
        public IEnumerator When_max_history_messages_reached_expect_LoadOlderMessagesAsync_loads_nothing()
            => ConnectAndExecute(When_max_history_messages_reached_expect_LoadOlderMessagesAsync_loads_nothing_Async);

        private async Task When_max_history_messages_reached_expect_LoadOlderMessagesAsync_loads_nothing_Async()
        {
            var window = SmallWindowWithLowHistoryLimit;
            var channel = await CreateUniqueTempChannelAsync();
            await SendMessagesAsync(channel, 12);

            channel.OverrideMessageCacheWindow(window);
            Assert.AreEqual(window.MaxMessages - window.DiscardBatchSize, channel.Messages.Count);
            Assert.IsFalse(channel.IsMessageCacheHistoryLimitReached);

            var removedCount = 0;
            channel.MessagesRemovedFromCache += (_, __) => removedCount++;

            // Page back until the paused limit is reached. The channel only holds 12 messages, so this
            // terminates regardless of the server's page size.
            for (var i = 0; i < 5 && channel.Messages.Count < window.MaxHistoryMessages; i++)
            {
                await channel.LoadOlderMessagesAsync();
            }

            Assert.IsTrue(channel.IsMessageCacheTrimmingPaused);
            Assert.GreaterOrEqual(channel.Messages.Count, window.MaxHistoryMessages);
            Assert.IsTrue(channel.IsMessageCacheHistoryLimitReached,
                "the app must be able to see that loading more history is pointless");
            Assert.AreEqual(0, removedCount, "nothing may be removed while trimming is paused");

            var oldestBefore = channel.Messages.First().Id;
            var countBefore = channel.Messages.Count;

            await channel.LoadOlderMessagesAsync();

            Assert.AreEqual(countBefore, channel.Messages.Count,
                "LoadOlderMessagesAsync must not load more history once MaxHistoryMessages is reached");
            Assert.AreEqual(oldestBefore, channel.Messages.First().Id);
            Assert.AreEqual(0, removedCount);

            // Resuming is what releases the paged-in history and re-enables loading.
            channel.ResumeMessageCacheTrimming();

            Assert.IsFalse(channel.IsMessageCacheTrimmingPaused);
            Assert.IsFalse(channel.IsMessageCacheHistoryLimitReached);
            Assert.AreEqual(window.MaxMessages - window.DiscardBatchSize, channel.Messages.Count);
            Assert.AreEqual(1, removedCount);
        }

        [UnityTest]
        public IEnumerator When_trimming_paused_expect_resume_restores_max_messages()
            => ConnectAndExecute(When_trimming_paused_expect_resume_restores_max_messages_Async);

        private async Task When_trimming_paused_expect_resume_restores_max_messages_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            channel.OverrideMessageCacheWindow(SmallWindowWithLowHistoryLimit);
            channel.PauseMessageCacheTrimming();

            await SendMessagesAsync(channel, 8);
            Assert.AreEqual(8, channel.Messages.Count);

            channel.ResumeMessageCacheTrimming();

            Assert.IsFalse(channel.IsMessageCacheTrimmingPaused);
            Assert.AreEqual(2, channel.Messages.Count);
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
            var config = Client.InternalLowLevelClient.Config;
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
            var config = Client.InternalLowLevelClient.Config;
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
            var removedIds = new HashSet<string>(sent.Take(4).Select(m => m.Id));
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
            Assert.Throws<ArgumentOutOfRangeException>(() => new MessageCacheWindow(10, 5, 9));
            Assert.DoesNotThrow(() => new MessageCacheWindow(10, 5, 10));
        }

        [Test]
        public void When_max_history_messages_not_specified_expect_default_of_four_times_max_messages()
        {
            Assert.AreEqual(40, new MessageCacheWindow(10, 5).MaxHistoryMessages);
            Assert.AreEqual(int.MaxValue, new MessageCacheWindow(int.MaxValue, 1).MaxHistoryMessages);
        }

        [Test]
        public void MessageCacheWindow_Recommended_is_500_100_2000()
        {
            Assert.AreEqual(500, MessageCacheWindow.Recommended.MaxMessages);
            Assert.AreEqual(100, MessageCacheWindow.Recommended.DiscardBatchSize);
            Assert.AreEqual(2000, MessageCacheWindow.Recommended.MaxHistoryMessages);
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
