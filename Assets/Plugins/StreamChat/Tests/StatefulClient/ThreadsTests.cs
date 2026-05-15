#if STREAM_TESTS_ENABLED
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using StreamChat.Core.QueryBuilders.Filters;
using StreamChat.Core.QueryBuilders.Filters.Threads;
using StreamChat.Core.QueryBuilders.Sort;
using StreamChat.Core.Requests;
using StreamChat.Core.StatefulModels;
using UnityEngine.TestTools;

namespace StreamChat.Tests.StatefulClient
{
    /// <summary>
    /// Tests operations for Threads API
    /// </summary>
    internal class ThreadsTests : BaseStateIntegrationTests
    {
        [UnityTest]
        public IEnumerator When_sending_reply_in_thread_expect_parent_reply_count_increases()
            => ConnectAndExecute(When_sending_reply_in_thread_expect_parent_reply_count_increases_Async);

        private async Task When_sending_reply_in_thread_expect_parent_reply_count_increases_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            var parent = await channel.SendNewMessageAsync("thread root");

            await channel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                ParentId = parent.Id,
                ShowInChannel = false,
                Text = "first reply",
            });

            await WaitWhileTrueAsync(() => (parent.ReplyCount ?? 0) < 1);

            Assert.GreaterOrEqual(parent.ReplyCount ?? 0, 1);
        }

        [UnityTest]
        public IEnumerator When_calling_get_thread_expect_thread_returned()
            => ConnectAndExecute(When_calling_get_thread_expect_thread_returned_Async);

        private async Task When_calling_get_thread_expect_thread_returned_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            var parent = await channel.SendNewMessageAsync("thread parent");

            // Need at least one reply for the thread to exist
            await channel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                ParentId = parent.Id,
                ShowInChannel = false,
                Text = "reply",
            });

            var thread = await Client.GetThreadAsync(parent.Id, replyLimit: 5, participantLimit: 5);

            Assert.NotNull(thread);
            Assert.AreEqual(parent.Id, thread.ParentMessageId);
            Assert.AreEqual(channel.Cid, thread.ChannelCid);
            Assert.GreaterOrEqual(thread.ReplyCount ?? 0, 1);
        }

        [UnityTest]
        public IEnumerator When_querying_threads_expect_thread_returned()
            => ConnectAndExecute(When_querying_threads_expect_thread_returned_Async);

        private async Task When_querying_threads_expect_thread_returned_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            var parent = await channel.SendNewMessageAsync("thread parent for query");

            await channel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                ParentId = parent.Id,
                ShowInChannel = false,
                Text = "reply",
            });

            var response = await Client.QueryThreadsAsync(new StreamQueryThreadsRequest
            {
                Limit = 20,
                ReplyLimit = 5,
                ParticipantLimit = 5,
                Watch = true,
                Filter = new IFieldFilterRule[]
                {
                    ThreadFilter.ChannelCid.EqualsTo(channel),
                },
                Sort = ThreadSort.OrderByDescending(ThreadSortFieldName.LastMessageAt),
            });

            Assert.NotNull(response);
            Assert.NotNull(response.Threads);

            var match = response.Threads.FirstOrDefault(t => t.ParentMessageId == parent.Id);
            Assert.NotNull(match, "Thread for the just-created parent message should be returned");
        }

        [UnityTest]
        public IEnumerator When_load_replies_called_expect_replies_returned_oldest_first()
            => ConnectAndExecute(When_load_replies_called_expect_replies_returned_oldest_first_Async);

        private async Task When_load_replies_called_expect_replies_returned_oldest_first_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            var parent = await channel.SendNewMessageAsync("thread parent for replies");

            for (int i = 0; i < 4; i++)
            {
                await channel.SendNewMessageAsync(new StreamSendMessageRequest
                {
                    ParentId = parent.Id,
                    ShowInChannel = false,
                    Text = $"reply {i}",
                });
            }

            var firstPage = await parent.LoadRepliesAsync(limit: 2);
            Assert.AreEqual(2, firstPage.Count);

            AssertOrderedAscendingByCreatedAt(firstPage);

            var olderPage = await parent.LoadRepliesAsync(limit: 2, idLessThan: firstPage[0].Id);
            Assert.GreaterOrEqual(olderPage.Count, 1);
            // Older page items must not contain ids from first page
            foreach (var older in olderPage)
            {
                Assert.IsFalse(firstPage.Any(m => m.Id == older.Id));
            }

            AssertOrderedAscendingByCreatedAt(olderPage);

            // Older messages should be ordered before the first page in the cached thread list
            var thread = await Client.GetThreadAsync(parent.Id);
            AssertOrderedAscendingByCreatedAt(thread.LatestReplies);
            foreach (var older in olderPage)
            {
                foreach (var newer in firstPage)
                {
                    Assert.Less(older.CreatedAt, newer.CreatedAt,
                        "Older page replies must have CreatedAt before first page replies");
                }
            }
        }

        private static void AssertOrderedAscendingByCreatedAt(System.Collections.Generic.IReadOnlyList<IStreamMessage> messages)
        {
            for (var i = 1; i < messages.Count; i++)
            {
                Assert.LessOrEqual(messages[i - 1].CreatedAt, messages[i].CreatedAt,
                    $"Messages must be ordered oldest-first by CreatedAt (index {i - 1} -> {i})");
            }
        }

        [UnityTest]
        public IEnumerator When_partial_update_thread_expect_title_set()
            => ConnectAndExecute(When_partial_update_thread_expect_title_set_Async);

        private async Task When_partial_update_thread_expect_title_set_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            var parent = await channel.SendNewMessageAsync("thread parent for update");
            await channel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                ParentId = parent.Id,
                ShowInChannel = false,
                Text = "reply",
            });

            var thread = await Client.GetThreadAsync(parent.Id);

            await thread.UpdatePartialAsync(setFields: new System.Collections.Generic.Dictionary<string, object>
            {
                { "title", "My Thread Title" },
            });

            Assert.AreEqual("My Thread Title", thread.Title);
        }

        [UnityTest]
        public IEnumerator When_marking_thread_read_expect_no_exception()
            => ConnectAndExecute(When_marking_thread_read_expect_no_exception_Async);

        private async Task When_marking_thread_read_expect_no_exception_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            var parent = await channel.SendNewMessageAsync("thread parent for mark read");
            await channel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                ParentId = parent.Id,
                ShowInChannel = false,
                Text = "reply",
            });

            await parent.MarkThreadAsReadAsync();
            await parent.MarkThreadAsUnreadAsync();
            await parent.MarkThreadAsReadAsync();
        }

        [UnityTest]
        public IEnumerator When_get_thread_called_via_message_helper_expect_thread_returned()
            => ConnectAndExecute(When_get_thread_called_via_message_helper_expect_thread_returned_Async);

        private async Task When_get_thread_called_via_message_helper_expect_thread_returned_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            var parent = await channel.SendNewMessageAsync("thread parent helper");
            await channel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                ParentId = parent.Id,
                ShowInChannel = false,
                Text = "reply",
            });

            var thread = await parent.GetThreadAsync(replyLimit: 5);
            Assert.NotNull(thread);
            Assert.AreEqual(parent.Id, thread.ParentMessageId);
        }

        /// <summary>
        /// Verifies that ParticipantCount / ActiveParticipantCount are not silently zeroed
        /// out when a thread is updated via partial-update.
        ///
        /// Why this matters: ThreadResponseInternalDTO declares these as non-nullable int,
        /// so any payload that omits them deserializes to 0; StreamThread.UpdateFromDto
        /// then writes 0 over a valid local value. The Android SDK already declared the
        /// equivalents Int? after observing that thread.updated payloads omit the fields.
        ///
        /// The single-client design is deliberate. UpdatePartialAsync triggers two
        /// UpdateFromDto invocations: one synchronously from the REST response, and one
        /// asynchronously from the thread.updated WS event echoed back to this client.
        /// We record participant counts on every Updated invocation. If any invocation
        /// reports a value different from what we set up (specifically smaller than the
        /// pre-update snapshot), the bug reproduces.
        /// </summary>
        [UnityTest]
        public IEnumerator When_thread_updated_event_received_expect_participant_counts_preserved()
            => ConnectAndExecute(When_thread_updated_event_received_expect_participant_counts_preserved_Async);

        private async Task When_thread_updated_event_received_expect_participant_counts_preserved_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            var parent = await channel.SendNewMessageAsync("thread parent for event count preservation");
            await channel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                ParentId = parent.Id,
                ShowInChannel = false,
                Text = "reply",
            });

            var thread = await Client.GetThreadAsync(parent.Id, replyLimit: 5, participantLimit: 5);

            await WaitWhileTrueAsync(() => (thread.ParticipantCount ?? 0) == 0);

            var participantsBefore = thread.ParticipantCount;
            var activeParticipantsBefore = thread.ActiveParticipantCount;

            Assert.Greater(participantsBefore ?? 0, 0,
                "Precondition: GetThreadAsync should return a non-zero participant_count");

            // Capture counts on every Updated invocation. We expect at least two:
            //   1) UpdatePartialAsync applies the REST response synchronously.
            //   2) The WS thread.updated event echoes back and applies again.
            // If the bug exists, the WS-driven invocation surfaces counts == 0.
            var observations = new List<(int? Participants, int? Active)>();
            var newTitle = "renamed-" + Guid.NewGuid().ToString("N").Substring(0, 6);
            var sawTitleUpdate = false;
            StreamThreadChangeHandler handler = t =>
            {
                observations.Add((t.ParticipantCount, t.ActiveParticipantCount));
                if (t.Title == newTitle)
                {
                    sawTitleUpdate = true;
                }
            };
            thread.Updated += handler;

            try
            {
                await thread.UpdatePartialAsync(setFields: new Dictionary<string, object>
                {
                    { "title", newTitle },
                });

                // Wait until we have seen the title-bearing invocation AND at least 2 Updated
                // callbacks (REST apply + WS echo). Title check guarantees the WS round-trip
                // happened (the REST response also carries the new title, but the WS event is
                // what gives the bug an opportunity to overwrite).
                await WaitWhileTrueAsync(() => !sawTitleUpdate || observations.Count < 2);
            }
            finally
            {
                thread.Updated -= handler;
            }

            // Every recorded snapshot must preserve at least the pre-update participant_count.
            // active_participant_count is 0 in this minimal one-author setup, so it cannot
            // discriminate, but participant_count is >= 1 and so any zeroing reveals the bug.
            for (var i = 0; i < observations.Count; i++)
            {
                var (participants, active) = observations[i];
                Assert.AreEqual(participantsBefore, participants,
                    $"participant_count regressed on Updated invocation #{i} (got {participants}, expected {participantsBefore}). " +
                    "This indicates UpdateFromDto applied a payload whose participant_count was missing/0.");
                Assert.AreEqual(activeParticipantsBefore, active,
                    $"active_participant_count regressed on Updated invocation #{i} (got {active}, expected {activeParticipantsBefore}).");
            }

            // Also assert the final committed state, since downstream consumers read it directly.
            Assert.AreEqual(participantsBefore, thread.ParticipantCount,
                "Final ParticipantCount on the thread must not be zeroed by event propagation");
            Assert.AreEqual(activeParticipantsBefore, thread.ActiveParticipantCount,
                "Final ActiveParticipantCount on the thread must not be zeroed by event propagation");
        }

        /// <summary>
        /// Same shape as the thread.updated test, but exercises the notification.mark_read code path
        /// (StreamChatClient.OnMarkReadNotification → UpdateFromDto on a ThreadResponseInternalDTO).
        ///
        /// The Android team explicitly named notification.mark_read as one of the two events whose
        /// embedded thread payload omits participant_count / active_participant_count. We force the
        /// event to fire by first marking the thread as unread (no-op for the bug since
        /// OnNotificationMarkUnread does not call UpdateFromDto) and then marking it as read.
        /// ReadStateChanged is used as the arrival signal since it is raised unconditionally in
        /// OnMarkReadNotification, even if the embedded Thread is null.
        /// </summary>
        [UnityTest]
        public IEnumerator When_notification_mark_read_event_received_expect_participant_counts_preserved()
            => ConnectAndExecute(When_notification_mark_read_event_received_expect_participant_counts_preserved_Async);

        private async Task When_notification_mark_read_event_received_expect_participant_counts_preserved_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            var parent = await channel.SendNewMessageAsync("thread parent for mark-read count preservation");
            await channel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                ParentId = parent.Id,
                ShowInChannel = false,
                Text = "reply",
            });

            var thread = await Client.GetThreadAsync(parent.Id, replyLimit: 5, participantLimit: 5);

            await WaitWhileTrueAsync(() => (thread.ParticipantCount ?? 0) == 0);

            var participantsBefore = thread.ParticipantCount;
            var activeParticipantsBefore = thread.ActiveParticipantCount;

            Assert.Greater(participantsBefore ?? 0, 0,
                "Precondition: GetThreadAsync should return a non-zero participant_count");

            // Put the thread into an unread state so the subsequent MarkReadAsync produces a
            // meaningful notification.mark_read event. The REST call is awaited, but we do NOT
            // wait for the corresponding notification.mark_unread WS event: empirically the
            // server does not echo notification.mark_* events back to the caller (the caller
            // already knows what they did). Waiting indefinitely here hangs the test.
            await thread.MarkUnreadAsync();

            // Capture counts on every Updated invocation. Updated only fires inside
            // OnMarkReadNotification when eventDto.Thread != null - which is precisely the code
            // path that would zero out the counts if the bug were present.
            var observations = new List<(int? Participants, int? Active)>();
            StreamThreadChangeHandler updatedHandler = t =>
            {
                observations.Add((t.ParticipantCount, t.ActiveParticipantCount));
            };

            var readSeen = false;
            StreamThreadReadHandler readHandler = _ => readSeen = true;

            thread.Updated += updatedHandler;
            thread.ReadStateChanged += readHandler;

            try
            {
                await thread.MarkReadAsync();

                // Best-effort wait for the notification.mark_read WS event echo. If the server
                // does not echo to the marking user (as appears to be the case for at least
                // notification.mark_unread), nothing arrives and the buggy code path is never
                // reached - in which case observations stay empty and the test vacuously passes.
                try
                {
                    await WaitWhileTrueAsync(() => !readSeen, maxSeconds: 5);
                }
                catch (TimeoutException)
                {
                    // Intentionally swallowed - see comment above.
                }
            }
            finally
            {
                thread.Updated -= updatedHandler;
                thread.ReadStateChanged -= readHandler;
            }

            // If the server omits the thread payload from notification.mark_read entirely,
            // observations will be empty - in that case the bug code path is simply not reached
            // and there is nothing to assert. Otherwise we require every recorded snapshot to
            // preserve the pre-event participant counts.
            for (var i = 0; i < observations.Count; i++)
            {
                var (participants, active) = observations[i];
                Assert.AreEqual(participantsBefore, participants,
                    $"participant_count regressed on Updated invocation #{i} after notification.mark_read " +
                    $"(got {participants}, expected {participantsBefore}). " +
                    "This indicates UpdateFromDto applied a payload whose participant_count was missing/0.");
                Assert.AreEqual(activeParticipantsBefore, active,
                    $"active_participant_count regressed on Updated invocation #{i} after notification.mark_read " +
                    $"(got {active}, expected {activeParticipantsBefore}).");
            }

            Assert.AreEqual(participantsBefore, thread.ParticipantCount,
                "Final ParticipantCount must not be zeroed by notification.mark_read propagation");
            Assert.AreEqual(activeParticipantsBefore, thread.ActiveParticipantCount,
                "Final ActiveParticipantCount must not be zeroed by notification.mark_read propagation");
        }

        /// <summary>
        /// notification.mark_read carries a narrowed ThreadResponse payload that omits the read
        /// array, so applying it via UpdateFromDto leaves the local user's StreamRead untouched.
        /// Customers listening to ReadStateChanged would observe a stale UnreadMessages > 0 even
        /// though the server has just marked the thread as read. Mirror Android's
        /// Thread.markAsReadByUser by zeroing UnreadMessages and bumping LastRead on the local
        /// user's entry before raising the event.
        ///
        /// Two-client setup is required: the server only emits a per-user read entry for the
        /// thread when somebody other than the local user posts a reply. A single-author thread
        /// returns an empty read array regardless of MarkUnread, so we use otherClient to send
        /// the reply that creates the unread state we are about to clear.
        /// </summary>
        [UnityTest]
        public IEnumerator When_notification_mark_read_event_received_expect_local_user_unread_count_cleared()
            => ConnectAndExecute(When_notification_mark_read_event_received_expect_local_user_unread_count_cleared_Async);

        private async Task When_notification_mark_read_event_received_expect_local_user_unread_count_cleared_Async()
        {
            var otherClient = await GetConnectedOtherClientAsync();

            var channel = await CreateUniqueTempChannelAsync();

            // Pre-watch the channel from otherClient so we hold a stateful IStreamChannel reference
            // we can post replies on below. (Historical note: this also worked around a bug where
            // the notification.added_to_channel handler crashed deserializing threads[].channel.config.commands
            // because the nested ChannelInternalDTO.Config pointed at ChannelConfigInternalDTO whose
            // Commands was List<string>. That has since been fixed by retyping the nested fields to
            // ChannelConfigWithInfoInternalDTO; the pre-watch is now kept only for test ergonomics.)
            var otherClientChannel = await otherClient.GetOrCreateChannelWithIdAsync(channel.Type, channel.Id);

            await channel.AddMembersAsync(new[] { otherClient.LocalUserData.User });

            var parent = await channel.SendNewMessageAsync("thread parent for unread clear");

            // Local must post the first reply so they become a thread participant. A Stream thread
            // is owned by the first replier, not the parent's author; without this the server
            // returns thread.read = [] for the local user and we can't observe unread state.
            await channel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                ParentId = parent.Id,
                ShowInChannel = false,
                Text = "local reply (makes local user a thread participant)",
            });

            var otherClientParent = await TryAsync(
                () => Task.FromResult(otherClientChannel.Messages.SingleOrDefault(m => m.Id == parent.Id)),
                m => m != null);

            await otherClientChannel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                ParentId = otherClientParent.Id,
                ShowInChannel = false,
                Text = "reply from other client",
            });

            var thread = await Client.GetThreadAsync(parent.Id, replyLimit: 5, participantLimit: 5);

            var localUserId = Client.LocalUserData.UserId;

            // The local Read entry materializes only once the server has propagated the other
            // client's reply through the thread's read aggregation. Refresh on a backoff matches
            // what a customer would do if they wanted up-to-date thread state.
            var localRead = await TryAsync(
                async () =>
                {
                    await thread.RefreshAsync();
                    return thread.Read.FirstOrDefault(r => r.User != null && r.User.Id == localUserId);
                },
                r => r != null && r.UnreadMessages > 0);

            Assert.Greater(localRead.UnreadMessages, 0,
                "Precondition: local user's unread count must be >0 after the other client posts a reply");

            var beforeLastRead = localRead.LastRead;

            var readSeen = false;
            StreamThreadReadHandler readHandler = _ => readSeen = true;
            thread.ReadStateChanged += readHandler;

            try
            {
                await thread.MarkReadAsync();

                // notification.mark_read may not be echoed back to the caller; if no event arrives
                // the buggy code path is never exercised and there is nothing to assert.
                try
                {
                    await WaitWhileTrueAsync(() => !readSeen, maxSeconds: 5);
                }
                catch (TimeoutException)
                {
                }
            }
            finally
            {
                thread.ReadStateChanged -= readHandler;
            }

            if (!readSeen)
            {
                return;
            }

            var afterRead = thread.Read.FirstOrDefault(r => r.User != null && r.User.Id == localUserId);
            Assert.NotNull(afterRead, "Local user's Read entry must still exist after mark-read");
            Assert.AreEqual(0, afterRead.UnreadMessages,
                "After notification.mark_read fires, the local user's UnreadMessages must be reset to 0");
            Assert.GreaterOrEqual(afterRead.LastRead, beforeLastRead,
                "After notification.mark_read fires, the local user's LastRead must advance");
        }

        /// <summary>
        /// New replies arriving via message.new / notification.thread_message_new must update the
        /// thread's local state beyond just LatestReplies and ReplyCount: every other StreamRead's
        /// UnreadMessages must grow by 1 and the sender's recency in ThreadParticipants must be
        /// re-bumped. Mirrors Android's Thread.upsertReply.
        ///
        /// Setup mirrors the mark-read test's timing exactly: the other client must post their
        /// first reply BEFORE the local user fetches the thread, otherwise the server response
        /// omits the `read` array entirely and the local Read entry never materializes on refresh.
        /// That seed reply also makes the other user a thread participant from the local cache's
        /// initial perspective, so this integration test covers the existing-participant branch
        /// (count unchanged, recency bumped) and the unread-increment branch via a second reply.
        /// The brand-new-participant branch is covered by code review only - exercising it via
        /// integration would require a third client and the server timing is fragile.
        /// </summary>
        [UnityTest]
        public IEnumerator When_other_client_replies_in_thread_expect_participants_and_unread_updated()
            => ConnectAndExecute(When_other_client_replies_in_thread_expect_participants_and_unread_updated_Async);

        private async Task When_other_client_replies_in_thread_expect_participants_and_unread_updated_Async()
        {
            var otherClient = await GetConnectedOtherClientAsync();

            var channel = await CreateUniqueTempChannelAsync();

            // Pre-watch from otherClient before AddMembers so we hold a stateful reference for
            // posting replies. (Same historical note as the mark-read test: this also used to
            // dodge the threads[].channel.config.commands deserialization crash, which has been
            // fixed by retyping the nested config fields to ChannelConfigWithInfoInternalDTO.)
            var otherClientChannel = await otherClient.GetOrCreateChannelWithIdAsync(channel.Type, channel.Id);

            await channel.AddMembersAsync(new[] { otherClient.LocalUserData.User });

            var parent = await channel.SendNewMessageAsync("thread parent for upsert reply");

            // Local must post the first reply so they become a thread participant.
            await channel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                ParentId = parent.Id,
                ShowInChannel = false,
                Text = "local reply (becomes thread participant)",
            });

            var otherClientParent = await TryAsync(
                () => Task.FromResult(otherClientChannel.Messages.SingleOrDefault(m => m.Id == parent.Id)),
                m => m != null);

            // Seed reply from other client BEFORE local fetches the thread. Without this, the
            // server's subsequent GET /threads/{id} responses omit the `read` array and the
            // local Read entry never materializes on refresh - the WS event would still fire
            // and our HandleNewReply would still run, but the test cannot observe its effect
            // on read state.
            await otherClientChannel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                ParentId = otherClientParent.Id,
                ShowInChannel = false,
                Text = "other-client seed reply",
            });

            var thread = await Client.GetThreadAsync(parent.Id, replyLimit: 10, participantLimit: 10);

            var localUserId = Client.LocalUserData.UserId;
            var otherUserId = otherClient.LocalUserData.UserId;

            // Refresh until the server has propagated the seed reply through the read aggregation.
            var localReadBefore = await TryAsync(
                async () =>
                {
                    await thread.RefreshAsync();
                    return thread.Read.FirstOrDefault(r => r.User != null && r.User.Id == localUserId);
                },
                r => r != null && r.UnreadMessages > 0);

            Assert.IsTrue(
                thread.ThreadParticipants.Any(p => (p.User?.Id ?? p.UserId) == otherUserId),
                "Precondition: other user must already be a thread participant from the seed reply");

            var unreadBefore = localReadBefore.UnreadMessages;
            var participantCountBefore = thread.ParticipantCount;

            // Drive the upsertReply code path via a second non-local reply over the wire.
            var replyReceived = false;
            StreamThreadReplyHandler replyHandler = (_, __) => replyReceived = true;
            thread.ReplyReceived += replyHandler;

            try
            {
                await otherClientChannel.SendNewMessageAsync(new StreamSendMessageRequest
                {
                    ParentId = otherClientParent.Id,
                    ShowInChannel = false,
                    Text = "other-client reply driving upsertReply",
                });

                await WaitWhileTrueAsync(() => !replyReceived);
            }
            finally
            {
                thread.ReplyReceived -= replyHandler;
            }

            Assert.AreEqual(participantCountBefore, thread.ParticipantCount,
                "ParticipantCount must NOT change when an existing participant replies again");

            var localReadAfter = thread.Read.First(r => r.User != null && r.User.Id == localUserId);
            Assert.AreEqual(unreadBefore + 1, localReadAfter.UnreadMessages,
                "Local user's UnreadMessages must increment by exactly 1 for the new non-local reply");

            var top = thread.ThreadParticipants[0];
            Assert.AreEqual(otherUserId, top.User?.Id ?? top.UserId,
                "Most recent replier must be sorted to index 0 of ThreadParticipants");
        }

        /// <summary>
        /// Regression for the message.new (channel watch) path: a watching client that is NOT a
        /// thread participant must still see the parent message's ReplyCount increment when another
        /// user posts a reply. Previously only OnNotificationThreadMessageNew bumped
        /// parent.ReplyCount, so watchers who never joined the thread would see a stale value
        /// until the next REST refresh. Mirrors Android's updateParentOrReply.
        ///
        /// Setup: otherClient authors the parent and the first reply (so otherClient is the only
        /// thread participant). The local client watches the channel via GetOrCreateChannelWithIdAsync
        /// without ever fetching the thread, so the local cache holds no IStreamThread for this
        /// parent and notification.thread_message_new will not be delivered. The only WS path that
        /// can update parent.ReplyCount on the local client is message.new.
        /// </summary>
        [UnityTest]
        public IEnumerator When_watcher_receives_message_new_for_thread_reply_expect_parent_reply_count_increments()
            => ConnectAndExecute(When_watcher_receives_message_new_for_thread_reply_expect_parent_reply_count_increments_Async);

        private async Task When_watcher_receives_message_new_for_thread_reply_expect_parent_reply_count_increments_Async()
        {
            var otherClient = await GetConnectedOtherClientAsync();

            var channel = await CreateUniqueTempChannelAsync();
            await channel.AddMembersAsync(new[] { otherClient.LocalUserData.User });

            // otherClient owns the thread end-to-end so the local user never becomes a participant
            // and therefore never receives notification.thread_message_new. The local user is
            // already watching `channel` from CreateUniqueTempChannelAsync, so every WS message.new
            // reaches OnMessageReceived - exactly the regression path under test.
            var otherClientChannel = await otherClient.GetOrCreateChannelWithIdAsync(channel.Type, channel.Id);
            var otherParent = await otherClientChannel.SendNewMessageAsync("thread parent for watcher reply count");

            // Wait until the parent itself is visible in the local watcher's channel.Messages
            // before posting any reply, so we can capture a deterministic baseline against the
            // exact StreamMessage instance that OnMessageReceived will mutate.
            var localParent = await TryAsync(
                () => Task.FromResult(channel.Messages.SingleOrDefault(m => m.Id == otherParent.Id)),
                m => m != null);

            var replyCountBefore = localParent.ReplyCount ?? 0;

            await otherClientChannel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                ParentId = otherParent.Id,
                ShowInChannel = false,
                Text = "thread reply driving message.new on the non-participant watcher",
            });

            await WaitWhileTrueAsync(() => (localParent.ReplyCount ?? 0) <= replyCountBefore);

            Assert.AreEqual(replyCountBefore + 1, localParent.ReplyCount ?? 0,
                "Watcher's parent.ReplyCount must increment by exactly 1 when message.new arrives " +
                "for a thread reply, even though the watcher is not a thread participant.");
        }

        /// <summary>
        /// Verifies that hard-deleting a thread reply removes it from the parent
        /// thread's <see cref="IStreamThread.LatestReplies"/> and decrements the
        /// reply count tracked by the local thread state.
        /// </summary>
        [UnityTest]
        public IEnumerator When_thread_reply_hard_deleted_expect_reply_removed_from_latest_replies()
            => ConnectAndExecute(When_thread_reply_hard_deleted_expect_reply_removed_from_latest_replies_Async);

        private async Task When_thread_reply_hard_deleted_expect_reply_removed_from_latest_replies_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            var parent = await channel.SendNewMessageAsync("thread parent for hard-delete cleanup");

            var reply = await channel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                ParentId = parent.Id,
                ShowInChannel = false,
                Text = "reply that will be hard-deleted",
            });

            var thread = await Client.GetThreadAsync(parent.Id, replyLimit: 10);

            await WaitWhileTrueAsync(() => thread.LatestReplies.All(m => m.Id != reply.Id));
            Assert.IsTrue(thread.LatestReplies.Any(m => m.Id == reply.Id),
                "Precondition: thread must contain the reply before delete");

            var latestRepliesCountBefore = thread.LatestReplies.Count;

            await reply.HardDeleteAsync();

            await WaitWhileTrueAsync(() => !reply.DeletedAt.HasValue);

            Assert.IsFalse(thread.LatestReplies.Any(m => m.Id == reply.Id),
                "Hard-deleted reply must be removed from Thread.LatestReplies");

            Assert.AreEqual(latestRepliesCountBefore - 1, thread.LatestReplies.Count,
                "LatestReplies.Count must shrink by exactly 1 after a hard delete");
        }

        /// <summary>
        /// Regression test for the nested-channel commands deserialization bug.
        ///
        /// ChannelInternalDTO is the response-side, nested channel DTO used by
        /// ThreadStateInternalDTO.Channel (and ThreadInternalDTO.Channel, PendingMessageInternalDTO.Channel,
        /// BanInternalDTO.Channel). Its Config field used to point at ChannelConfigInternalDTO whose
        /// Commands was List&lt;string&gt;, while the server consistently returns commands as command
        /// objects (e.g. {"name":"giphy","description":...}). The top-level
        /// ChannelResponseInternalDTO.Config already uses ChannelConfigWithInfoInternalDTO (correct),
        /// so simple channel queries on channels without threads worked. The bug only surfaced
        /// when the channel-state query response included threads[], because the nested
        /// threads[i].channel.config.commands traversal hit the wrong type.
        ///
        /// This test reproduces that exact path: a channel with at least one thread, then a
        /// second client fetching the same channel via query-channel (POST /channels/{type}/{id}/query).
        /// Before the fix it failed with:
        ///   StreamDeserializationException: Failed to deserialize string to type: `ChannelStateResponseInternalDTO`
        ///       ---> Newtonsoft.Json.JsonReaderException: Unexpected character encountered while parsing value: {.
        ///            Path 'threads[0].channel.config.commands', ...
        /// </summary>
        [UnityTest]
        public IEnumerator When_querying_channel_with_existing_thread_expect_no_deserialization_error()
            => ConnectAndExecute(When_querying_channel_with_existing_thread_expect_no_deserialization_error_Async);

        private async Task When_querying_channel_with_existing_thread_expect_no_deserialization_error_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            var parent = await channel.SendNewMessageAsync("parent");
            await channel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                ParentId = parent.Id,
                ShowInChannel = false,
                Text = "reply",
            });

            var otherClient = await GetConnectedOtherClientAsync();

            // Direct await rather than Assert.DoesNotThrowAsync: the latter is a synchronous
            // NUnit helper that blocks the awaiting thread, which deadlocks against the Unity
            // synchronization context our continuations need to resume on. If
            // GetOrCreateChannelWithIdAsync throws (which is exactly the bug we are guarding
            // against), the unhandled exception will fail the test on its own.
            var otherClientChannel = await otherClient.GetOrCreateChannelWithIdAsync(channel.Type, channel.Id);

            Assert.NotNull(otherClientChannel,
                "GetOrCreateChannelWithIdAsync should return the channel after fix " +
                "(regression for threads[0].channel.config.commands deserialization).");
            Assert.AreEqual(channel.Cid, otherClientChannel.Cid);
        }

        /// <summary>
        /// Regression for ChannelStateResponse.Threads being silently dropped.
        ///
        /// The channel watch response (ChannelStateResponseInternalDTO / ChannelStateResponseFieldsInternalDTO)
        /// carries the channel's threads, but both StreamChannel.UpdateFromDto overloads used to ignore the
        /// field. Two consequences:
        ///   1. IStreamChatClient.ThreadTracked never fired for those threads on the watcher, so customers
        ///      had no signal that threads were now reachable.
        ///   2. WS thread handlers (thread.updated, mark read/unread, notification.thread_message_new)
        ///      early-return on a Cache.Threads miss, so every subsequent thread mutation was silently
        ///      dropped on the floor for a watcher that never called QueryThreadsAsync explicitly.
        ///
        /// This test exercises both: clientB watches a channel that already has a thread without ever
        /// calling GetThread/QueryThreads, asserts ThreadTracked fires for the carried thread, then has
        /// clientA edit the thread title over the wire and verifies the same cached IStreamThread on
        /// clientB picks the change up through the thread.updated WS path.
        /// </summary>
        [UnityTest]
        public IEnumerator When_watching_channel_with_existing_thread_expect_thread_tracked_and_ws_updates_propagate()
            => ConnectAndExecute(When_watching_channel_with_existing_thread_expect_thread_tracked_and_ws_updates_propagate_Async);

        private async Task When_watching_channel_with_existing_thread_expect_thread_tracked_and_ws_updates_propagate_Async()
        {
            var otherClient = await GetConnectedOtherClientAsync();

            var channel = await CreateUniqueTempChannelAsync();
            await channel.AddMembersAsync(new[] { otherClient.LocalUserData.User });

            var parent = await channel.SendNewMessageAsync("thread parent for ThreadTracked test");
            await channel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                ParentId = parent.Id,
                ShowInChannel = false,
                Text = "reply (creates the thread)",
            });

            // Wait for the thread to materialize server-side before clientB watches it,
            // otherwise the watch response may legitimately arrive without it.
            await TryAsync(
                () => Client.QueryThreadsAsync(new StreamQueryThreadsRequest
                {
                    Limit = 5,
                    Filter = new IFieldFilterRule[] { ThreadFilter.ChannelCid.EqualsTo(channel) },
                }),
                r => r != null && r.Threads != null && r.Threads.Any(t => t.ParentMessageId == parent.Id));

            // Subscribe BEFORE the watch so we capture the Tracked emission for the thread carried
            // in the watch response - a customer would do the same in their session-init code path.
            IStreamThread tracked = null;
            void OnTracked(IStreamThread t)
            {
                if (t.ParentMessageId == parent.Id)
                {
                    tracked = t;
                }
            }
            otherClient.ThreadTracked += OnTracked;

            try
            {
                var otherClientChannel = await otherClient.GetOrCreateChannelWithIdAsync(channel.Type, channel.Id);

                await WaitWhileTrueAsync(() => tracked == null);

                Assert.NotNull(tracked,
                    "ThreadTracked must fire for a thread carried by the channel watch response " +
                    "(without any explicit GetThread/QueryThreads call on the watcher).");
                Assert.AreEqual(channel.Cid, tracked.ChannelCid);

                // Drive a real thread.updated WS event from clientA. The cached IStreamThread on
                // clientB is the same singleton in Cache.Threads, so OnThreadUpdated mutates it
                // instead of early-returning on an unknown id.
                var newTitle = "ThreadTracked test " + Guid.NewGuid();
                var updatedFired = false;
                StreamThreadChangeHandler updateHandler = _ => updatedFired = true;
                tracked.Updated += updateHandler;

                try
                {
                    var clientAThread = await Client.GetThreadAsync(parent.Id);
                    await clientAThread.UpdatePartialAsync(setFields: new Dictionary<string, object>
                    {
                        { "title", newTitle },
                    });

                    await WaitWhileTrueAsync(() => tracked.Title != newTitle);
                }
                finally
                {
                    tracked.Updated -= updateHandler;
                }

                Assert.AreEqual(newTitle, tracked.Title,
                    "Cached thread on the watcher must reflect the title from the WS thread.updated event " +
                    "(proves the thread entered Cache.Threads at watch time).");
                Assert.IsTrue(updatedFired,
                    "IStreamThread.Updated must fire on the watcher's cached thread when the WS event arrives.");
            }
            finally
            {
                otherClient.ThreadTracked -= OnTracked;
            }
        }

        /// <summary>
        /// Verifies the wasCreated semantics of <see cref="IStreamChatClient.ThreadTracked"/>:
        ///   1. Fires exactly once per thread, on the first cache insertion.
        ///   2. Does NOT re-fire when the same thread is fetched again (the second QueryThreadsAsync
        ///      call observes a cache hit and only updates the existing instance).
        ///   3. Supplies a fully-hydrated thread - the event fires AFTER the first UpdateFromDto
        ///      completes, never with a blank instance. This is the deferred-firing contract
        ///      documented on ICacheRepository.Tracked.
        /// </summary>
        [UnityTest]
        public IEnumerator When_query_threads_called_twice_expect_thread_tracked_fires_exactly_once_with_hydrated_state()
            => ConnectAndExecute(When_query_threads_called_twice_expect_thread_tracked_fires_exactly_once_with_hydrated_state_Async);

        private async Task When_query_threads_called_twice_expect_thread_tracked_fires_exactly_once_with_hydrated_state_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            var parent = await channel.SendNewMessageAsync("thread parent for ThreadTracked-once test");
            await channel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                ParentId = parent.Id,
                ShowInChannel = false,
                Text = "reply (creates the thread server-side)",
            });

            var emissionCount = 0;
            IStreamThread firstEmission = null;
            string firstEmissionParentMessageIdAtRaise = null;
            string firstEmissionChannelCidAtRaise = null;

            void OnTracked(IStreamThread t)
            {
                if (t.ParentMessageId != parent.Id)
                {
                    return;
                }

                emissionCount++;
                if (firstEmission == null)
                {
                    firstEmission = t;
                    // Snapshot the state AT the moment of the event to validate the deferred-firing
                    // contract (Tracked must fire after UpdateFromDto, never with a blank object).
                    firstEmissionParentMessageIdAtRaise = t.ParentMessageId;
                    firstEmissionChannelCidAtRaise = t.ChannelCid;
                }
            }
            Client.ThreadTracked += OnTracked;

            try
            {
                var firstQuery = await TryAsync(
                    () => Client.QueryThreadsAsync(new StreamQueryThreadsRequest
                    {
                        Limit = 5,
                        Filter = new IFieldFilterRule[] { ThreadFilter.ChannelCid.EqualsTo(channel) },
                    }),
                    r => r != null && r.Threads != null && r.Threads.Any(t => t.ParentMessageId == parent.Id));

                await WaitWhileTrueAsync(() => emissionCount == 0);

                Assert.AreEqual(1, emissionCount,
                    "ThreadTracked must fire exactly once when the thread enters the cache for the first time.");
                Assert.NotNull(firstEmission, "Captured emission must not be null.");
                Assert.AreEqual(parent.Id, firstEmissionParentMessageIdAtRaise,
                    "ThreadTracked must fire AFTER UpdateFromDto so ParentMessageId is already populated " +
                    "(deferred-firing contract on CacheRepository.Tracked).");
                Assert.AreEqual(channel.Cid, firstEmissionChannelCidAtRaise,
                    "ThreadTracked must fire with a hydrated thread, not a blank instance.");

                var queryResultThread = firstQuery.Threads.First(t => t.ParentMessageId == parent.Id);
                Assert.AreSame(queryResultThread, firstEmission,
                    "The thread instance from QueryThreadsAsync must be the same singleton emitted by ThreadTracked.");

                // Re-query: cache hit, must NOT fire Tracked again.
                var secondQuery = await Client.QueryThreadsAsync(new StreamQueryThreadsRequest
                {
                    Limit = 5,
                    Filter = new IFieldFilterRule[] { ThreadFilter.ChannelCid.EqualsTo(channel) },
                });

                Assert.AreEqual(1, emissionCount,
                    "ThreadTracked must NOT re-fire when the same thread is fetched again (cache hit).");

                var secondQueryThread = secondQuery.Threads.First(t => t.ParentMessageId == parent.Id);
                Assert.AreSame(firstEmission, secondQueryThread,
                    "Subsequent queries must return the same cached thread instance.");
            }
            finally
            {
                Client.ThreadTracked -= OnTracked;
            }
        }

        /// <summary>
        /// Verifies the parent-hard-delete teardown path emits <see cref="IStreamChatClient.ThreadUntracked"/>:
        /// hard-deleting the parent message of a thread destroys the thread server-side, the SDK removes
        /// it from Cache.Threads in OnMessageDeleted, and that removal must surface to customers.
        /// </summary>
        [UnityTest]
        public IEnumerator When_parent_message_hard_deleted_expect_thread_untracked_fires()
            => ConnectAndExecute(When_parent_message_hard_deleted_expect_thread_untracked_fires_Async);

        private async Task When_parent_message_hard_deleted_expect_thread_untracked_fires_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            var parent = await channel.SendNewMessageAsync("thread parent for ThreadUntracked test");
            await channel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                ParentId = parent.Id,
                ShowInChannel = false,
                Text = "reply (so the thread exists)",
            });

            // Pull the thread into local cache so OnMessageDeleted can find it and Remove() can fire.
            await TryAsync(
                () => Client.QueryThreadsAsync(new StreamQueryThreadsRequest
                {
                    Limit = 5,
                    Filter = new IFieldFilterRule[] { ThreadFilter.ChannelCid.EqualsTo(channel) },
                }),
                r => r != null && r.Threads != null && r.Threads.Any(t => t.ParentMessageId == parent.Id));

            IStreamThread untracked = null;
            void OnUntracked(IStreamThread t)
            {
                if (t.ParentMessageId == parent.Id)
                {
                    untracked = t;
                }
            }
            Client.ThreadUntracked += OnUntracked;

            try
            {
                await parent.HardDeleteAsync();

                await WaitWhileTrueAsync(() => untracked == null);

                Assert.NotNull(untracked,
                    "ThreadUntracked must fire when the thread's parent message is hard-deleted.");
                Assert.AreEqual(parent.Id, untracked.ParentMessageId,
                    "The emitted thread reference must match the parent message id of the destroyed thread.");

                // The thread should no longer be reachable from a fresh QueryThreadsAsync result.
                var afterDelete = await Client.QueryThreadsAsync(new StreamQueryThreadsRequest
                {
                    Limit = 20,
                    Filter = new IFieldFilterRule[] { ThreadFilter.ChannelCid.EqualsTo(channel) },
                });
                Assert.IsFalse(
                    afterDelete.Threads.Any(t => t.ParentMessageId == parent.Id),
                    "After parent hard-delete, the thread must no longer be returned by the server.");
            }
            finally
            {
                Client.ThreadUntracked -= OnUntracked;
            }
        }

        /// <summary>
        /// End-to-end backstop for the notification.added_to_channel path that previously
        /// silently swallowed the threads[].channel.config.commands deserialization failure
        /// in StreamChatClient.OnAddedToChannelNotification's InternalGetOrCreateChannelAsync
        /// ContinueWith and never added the channel to WatchedChannels on the other client.
        ///
        /// Flow:
        ///   1. clientA creates a channel and a thread on it.
        ///   2. clientB is connected fresh - no cache entry for this channel.
        ///   3. clientA adds clientB as a member.
        ///   4. clientB's notification.added_to_channel handler runs InternalGetOrCreateChannelAsync,
        ///      which previously crashed on the nested threads[].channel.config.commands.
        ///   5. After the fix, the watch succeeds and clientB raises AddedToChannelAsMember and
        ///      the channel appears in clientB.WatchedChannels.
        /// </summary>
        [UnityTest]
        public IEnumerator When_added_to_channel_with_existing_thread_expect_channel_watched_on_other_client()
            => ConnectAndExecute(When_added_to_channel_with_existing_thread_expect_channel_watched_on_other_client_Async);

        private async Task When_added_to_channel_with_existing_thread_expect_channel_watched_on_other_client_Async()
        {
            var otherClient = await GetConnectedOtherClientAsync();

            var channel = await CreateUniqueTempChannelAsync();
            var parent = await channel.SendNewMessageAsync("thread parent for added-to-channel watch");
            await channel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                ParentId = parent.Id,
                ShowInChannel = false,
                Text = "reply (creates the thread)",
            });

            // No pre-watch here - we want the notification.added_to_channel handler to take
            // the wasCreated == true branch and exercise the previously-crashing fetch.
            IStreamChannel addedChannel = null;
            void OnAddedToChannelAsMember(IStreamChannel ch, IStreamChannelMember _)
            {
                if (ch.Cid == channel.Cid)
                {
                    addedChannel = ch;
                }
            }
            otherClient.AddedToChannelAsMember += OnAddedToChannelAsMember;

            try
            {
                await channel.AddMembersAsync(new[] { otherClient.LocalUserData.User });

                await WaitWhileTrueAsync(() => addedChannel == null, maxSeconds: 30);
            }
            finally
            {
                otherClient.AddedToChannelAsMember -= OnAddedToChannelAsMember;
            }

            Assert.NotNull(addedChannel,
                "AddedToChannelAsMember should fire on the other client even though the channel " +
                "already has a thread (regression for the nested config.commands deserialization).");
            Assert.IsTrue(
                otherClient.WatchedChannels.Any(c => c.Cid == channel.Cid),
                "The newly-added channel must appear in the other client's WatchedChannels - " +
                "previously the buggy fetch faulted inside ContinueWith and silently dropped it.");
        }

        /// <summary>
        /// A thread reply with <c>ShowInChannel = false</c> (or unset) must live only in
        /// <see cref="IStreamThread.LatestReplies"/>. It must not pollute the channel timeline
        /// or fire <see cref="IStreamChannel.MessageReceived"/>. Mirrors Android's
        /// ChannelLogic filter `parentId != null AND !showInChannel`.
        /// </summary>
        [UnityTest]
        public IEnumerator When_thread_only_reply_received_expect_not_added_to_channel_messages()
            => ConnectAndExecute(When_thread_only_reply_received_expect_not_added_to_channel_messages_Async);

        private async Task When_thread_only_reply_received_expect_not_added_to_channel_messages_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            var parent = await channel.SendNewMessageAsync("thread parent");

            // Server creates the thread only after the first reply, so seed one before GetThreadAsync.
            await channel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                ParentId = parent.Id,
                ShowInChannel = false,
                Text = "first reply (creates thread)",
            });

            var thread = await Client.GetThreadAsync(parent.Id, replyLimit: 5, participantLimit: 5);

            IStreamMessage receivedInChannel = null;
            void OnMessageReceived(IStreamChannel ch, IStreamMessage msg) => receivedInChannel = msg;
            channel.MessageReceived += OnMessageReceived;

            try
            {
                var reply = await channel.SendNewMessageAsync(new StreamSendMessageRequest
                {
                    ParentId = parent.Id,
                    ShowInChannel = false,
                    Text = "thread-only reply",
                });

                await WaitWhileTrueAsync(() => !thread.LatestReplies.Any(r => r.Id == reply.Id), maxSeconds: 15);

                Assert.IsTrue(thread.LatestReplies.Any(r => r.Id == reply.Id),
                    "Thread-only reply must be added to thread.LatestReplies.");
                Assert.IsFalse(channel.Messages.Any(m => m.Id == reply.Id),
                    "Thread-only reply must NOT be added to channel.Messages.");
                Assert.IsNull(receivedInChannel,
                    "channel.MessageReceived must NOT fire for thread-only replies.");
            }
            finally
            {
                channel.MessageReceived -= OnMessageReceived;
            }
        }

        /// <summary>
        /// A thread reply with <c>ShowInChannel = true</c> must live in both
        /// <see cref="IStreamThread.LatestReplies"/> and the channel timeline.
        /// </summary>
        [UnityTest]
        public IEnumerator When_thread_reply_with_show_in_channel_received_expect_added_to_channel_messages()
            => ConnectAndExecute(When_thread_reply_with_show_in_channel_received_expect_added_to_channel_messages_Async);

        private async Task When_thread_reply_with_show_in_channel_received_expect_added_to_channel_messages_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            var parent = await channel.SendNewMessageAsync("thread parent");

            // Server creates the thread only after the first reply, so seed one before GetThreadAsync.
            await channel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                ParentId = parent.Id,
                ShowInChannel = false,
                Text = "first reply (creates thread)",
            });

            var thread = await Client.GetThreadAsync(parent.Id, replyLimit: 5, participantLimit: 5);

            var reply = await channel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                ParentId = parent.Id,
                ShowInChannel = true,
                Text = "thread reply also in channel",
            });

            await WaitWhileTrueAsync(
                () => !thread.LatestReplies.Any(r => r.Id == reply.Id)
                      || !channel.Messages.Any(m => m.Id == reply.Id),
                maxSeconds: 15);

            Assert.IsTrue(thread.LatestReplies.Any(r => r.Id == reply.Id),
                "Reply with ShowInChannel=true must appear in thread.LatestReplies.");
            Assert.IsTrue(channel.Messages.Any(m => m.Id == reply.Id),
                "Reply with ShowInChannel=true must also appear in channel.Messages.");
        }
    }
}
#endif
