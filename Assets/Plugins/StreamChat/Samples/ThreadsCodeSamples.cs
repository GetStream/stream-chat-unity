using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Core.QueryBuilders.Filters;
using StreamChat.Core.QueryBuilders.Filters.Threads;
using StreamChat.Core.QueryBuilders.Sort;
using StreamChat.Core.Requests;
using StreamChat.Core.StatefulModels;
using UnityEngine;

namespace StreamChat.Samples
{
    internal sealed class ThreadsCodeSamples
    {
        /// <summary>
        /// https://getstream.io/chat/docs/unity/threads/?language=unity#starting-a-thread
        /// </summary>
        public async Task SendReply()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, "my-channel-id");

            var parentMessage = await channel.SendNewMessageAsync("Starting a thread");

            var reply = await channel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                ParentId = parentMessage.Id,
                ShowInChannel = false,
                Text = "This is a reply in a thread",
            });
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/threads/?language=unity#paginating-thread-replies
        /// </summary>
        public async Task LoadReplies()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, "my-channel-id");
            var parentMessage = await channel.SendNewMessageAsync("Starting a thread");

            // Get the latest 20 replies (oldest-first)
            var replies = await parentMessage.LoadRepliesAsync(limit: 20);

            // Get older replies (before message with id "42")
            var olderReplies = await parentMessage.LoadRepliesAsync(limit: 20, idLessThan: "42");
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/threads/?language=unity#inline-replies
        /// </summary>
        public async Task InlineReply()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, "my-channel-id");
            var originalMessage = await channel.SendNewMessageAsync("Original message");

            var message = await channel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                QuotedMessage = originalMessage,
                Text = "I agree with this point",
            });
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/threads/?language=unity#querying-threads
        /// </summary>
        public async Task QueryThreads()
        {
            var response = await Client.QueryThreadsAsync(new StreamQueryThreadsRequest
            {
                Watch = true,
                Limit = 10,
            });

            foreach (var thread in response.Threads)
            {
                // Threads are cached, watched and kept in sync with realtime events
                Debug.Log(thread.ParentMessage.Text);
                Debug.Log(thread.LatestReplies);
                Debug.Log(thread.ThreadParticipants);
                Debug.Log(thread.Read);
            }
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/threads/?language=unity#filtering-and-sorting
        /// </summary>
        public async Task QueryThreadsWithFilterAndSort()
        {
            var since = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);

            var request = new StreamQueryThreadsRequest
            {
                Filter = new IFieldFilterRule[]
                {
                    ThreadFilter.CreatedByUserId.EqualsTo("user-1"),
                    ThreadFilter.UpdatedAt.GreaterThanOrEquals(since),
                },
                Sort = ThreadSort.OrderByDescending(ThreadSortFieldName.CreatedAt),
                Limit = 10,
            };

            var page1 = await Client.QueryThreadsAsync(request);

            // Get next page using the cursor returned by the previous response
            if (!string.IsNullOrEmpty(page1.Next))
            {
                request.Next = page1.Next;
                var page2 = await Client.QueryThreadsAsync(request);
            }
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/threads/?language=unity#getting-a-thread-by-id
        /// </summary>
        public async Task GetThread()
        {
            // The returned IStreamThread is auto-watched (watch defaults to true) and stays in
            // sync with realtime events via Updated / ReplyReceived / ReadStateChanged.
            var thread = await Client.GetThreadAsync("parent-message-id",
                replyLimit: 10, participantLimit: 25);

            var participants = thread.ThreadParticipants;
            var replies = thread.LatestReplies;
            var replyCount = thread.ReplyCount;

            thread.Updated += changedThread => { /* title or custom data changed */ };
            thread.ReplyReceived += (changedThread, reply) => { /* new reply arrived */ };
            thread.ReadStateChanged += changedThread => { /* unread state changed */ };
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/threads/?language=unity#updating-thread-title-and-custom-data
        /// </summary>
        public async Task UpdateThread()
        {
            var thread = await Client.GetThreadAsync("parent-message-id");

            // Set title and custom fields; unset a previously set field
            await thread.UpdatePartialAsync(
                setFields: new Dictionary<string, object>
                {
                    { "title", "Project Discussion" },
                    { "priority", "high" },
                },
                unsetFields: new[] { "priority" });
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/threads/?language=unity#total-unread-threads
        /// </summary>
        public async Task ObserveTotalUnreadThreads()
        {
            // Available immediately after connect on IStreamLocalUserData
            var unreadThreads = Client.LocalUserData.UnreadThreads;
            Debug.Log(unreadThreads);

            // The same total is also available on demand from the server
            var unreadCounts = await Client.GetLatestUnreadCountsAsync();
            Debug.Log(unreadCounts.TotalUnreadThreadsCount);
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/threads/?language=unity#marking-threads-as-read-or-unread
        /// </summary>
        public async Task MarkThreadReadAndUnread()
        {
            var thread = await Client.GetThreadAsync("parent-message-id");

            // Mark this thread as read for the local user
            await thread.MarkReadAsync();

            // Mark this thread as unread starting from the parent message
            await thread.MarkUnreadAsync();

            // Equivalent helpers from the parent message of the thread
            IStreamMessage parentMessage = thread.ParentMessage;
            await parentMessage.MarkThreadAsReadAsync();
            await parentMessage.MarkThreadAsUnreadAsync();

            // Or by parent message id when you already have the channel
            await thread.Channel.MarkThreadAsReadAsync(thread.ParentMessageId);
            await thread.Channel.MarkThreadAsUnreadAsync(thread.ParentMessageId);
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/threads/?language=unity#unread-count-per-thread
        /// </summary>
        public async Task UnreadCountPerThread()
        {
            var unreadCounts = await Client.GetLatestUnreadCountsAsync();

            Debug.Log(unreadCounts.TotalUnreadThreadsCount);

            foreach (var thread in unreadCounts.UnreadThreads)
            {
                Debug.Log(thread.ParentMessageId);
                Debug.Log(thread.UnreadCount);
                Debug.Log(thread.LastRead);
                Debug.Log(thread.LastReadMessageId);
            }
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/threads/?language=unity#thread-manager
        /// </summary>
        public async Task TrackThreadsAndStayInSync()
        {
            // Get notified when a thread starts or stops being tracked locally
            Client.ThreadTracked += thread => { /* a new IStreamThread is now tracked */ };
            Client.ThreadUntracked += thread => { /* an IStreamThread is no longer tracked */ };

            // Load threads. Watch defaults to true so realtime updates are delivered.
            var response = await Client.QueryThreadsAsync(new StreamQueryThreadsRequest
            {
                Watch = true,
                Limit = 10,
            });

            // Each returned IStreamThread is stateful: it is cached and kept in sync with
            // realtime events automatically. Subscribe to per-thread events to react to
            // changes (e.g. new replies, title / custom data updates, read state changes).
            foreach (var thread in response.Threads)
            {
                thread.Updated += changedThread => { /* title or custom data changed */ };
                thread.ReplyReceived += (changedThread, reply) => { /* new reply arrived */ };
                thread.ReadStateChanged += changedThread => { /* unread state changed */ };
            }
        }

        public IStreamChatClient Client { get; private set; }
    }
}
