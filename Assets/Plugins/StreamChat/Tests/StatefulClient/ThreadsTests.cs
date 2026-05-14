#if STREAM_TESTS_ENABLED
using System.Collections;
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

            var thread = await Client.Threads.GetThreadAsync(parent.Id, replyLimit: 5, participantLimit: 5);

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

            var response = await Client.Threads.QueryThreadsAsync(new StreamQueryThreadsRequest
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
            var thread = await Client.Threads.GetThreadAsync(parent.Id);
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

            var thread = await Client.Threads.GetThreadAsync(parent.Id);

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
    }
}
#endif
