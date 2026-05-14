using System.Collections.Generic;
using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Core.QueryBuilders.Filters;
using StreamChat.Core.QueryBuilders.Filters.Threads;
using StreamChat.Core.QueryBuilders.Sort;
using StreamChat.Core.Requests;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Samples
{
    internal sealed class ThreadsCodeSamples
    {
        /// <summary>
        /// https://getstream.io/chat/docs/unity/threads/?language=unity
        /// </summary>
        public async Task SendReply()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, "my-channel-id");

            var parentMessage = await channel.SendNewMessageAsync("Let's start a thread!");

            // Reply in the thread (won't appear in main channel timeline)
            var reply = await channel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                ParentId = parentMessage.Id,
                ShowInChannel = false,
                Text = "Hello!",
            });
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/threads/?language=unity#load-replies
        /// </summary>
        public async Task LoadReplies()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, "my-channel-id");

            var parentMessage = await channel.SendNewMessageAsync("Let's start a thread!");

            // Load the most recent replies of a thread (oldest-first)
            var firstPage = await parentMessage.LoadRepliesAsync(limit: 25);

            // Load older replies using id_lt pagination
            if (firstPage.Count > 0)
            {
                var olderPage = await parentMessage.LoadRepliesAsync(limit: 25, idLessThan: firstPage[0].Id);
            }
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/threads/?language=unity#get-thread
        /// </summary>
        public async Task GetThread()
        {
            // Fetch a thread by its parent message id
            var thread = await Client.Threads.GetThreadAsync("parent-message-id", replyLimit: 25, participantLimit: 25);

            var participants = thread.ThreadParticipants;
            var replies = thread.LatestReplies;
            var replyCount = thread.ReplyCount;

            // Subscribe to thread updates
            thread.Updated += changedThread => { /* handle change */ };
            thread.ReplyReceived += (changedThread, reply) => { /* handle new reply */ };
            thread.ReadStateChanged += changedThread => { /* unread state changed */ };
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/threads/?language=unity#query-threads
        /// </summary>
        public async Task QueryThreads()
        {
            var request = new StreamQueryThreadsRequest
            {
                Limit = 20,
                ReplyLimit = 5,
                ParticipantLimit = 5,
                Watch = true,
                Filter = new IFieldFilterRule[]
                {
                    ThreadFilter.ChannelCid.EqualsTo("messaging:my-channel-id"),
                },
                Sort = ThreadSort.OrderByDescending(ThreadSortFieldName.LastMessageAt),
            };

            var response = await Client.Threads.QueryThreadsAsync(request);

            foreach (var thread in response.Threads)
            {
                // Use thread state. The thread is already cached and gets WS updates.
                var title = thread.Title;
                var replyCount = thread.ReplyCount;
            }

            // Pagination
            if (!string.IsNullOrEmpty(response.Next))
            {
                request.Next = response.Next;
                var nextPage = await Client.Threads.QueryThreadsAsync(request);
            }
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/threads/?language=unity#partial-update
        /// </summary>
        public async Task UpdateThread()
        {
            var thread = await Client.Threads.GetThreadAsync("parent-message-id");

            await thread.UpdatePartialAsync(
                setFields: new Dictionary<string, object>
                {
                    { "title", "Updated thread title" },
                    { "my_custom_field", 42 },
                },
                unsetFields: new[] { "obsolete_field" });
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/threads/?language=unity#mark-as-read
        /// </summary>
        public async Task MarkThreadReadAndUnread()
        {
            var thread = await Client.Threads.GetThreadAsync("parent-message-id");

            // Mark this thread as read
            await thread.MarkReadAsync();

            // Mark this thread as unread starting from the parent message
            await thread.MarkUnreadAsync();

            // From a message reference (the parent)
            IStreamMessage parentMessage = thread.ParentMessage;
            await parentMessage.MarkThreadAsReadAsync();
            await parentMessage.MarkThreadAsUnreadAsync();
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/threads/?language=unity#unread-count
        /// </summary>
        public void ObserveUnreadThreadsCount()
        {
            var localUserData = Client.LocalUserData;

            var unreadThreads = localUserData.UnreadThreads;
        }

        public IStreamChatClient Client { get; private set; }
    }
}
