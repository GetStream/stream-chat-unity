using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Core.StatefulModels;
using StreamChat.Libs.Auth;
using UnityEngine;

namespace StreamChat.Samples
{
    internal sealed class UnreadCountsCodeSamples
    {
        /// <summary>
        /// https://getstream.io/chat/docs/unity/unread/?language=unity#reading-unread-counts
        /// </summary>
        public async Task ReadingUnreadCounts()
        {
            // Step 1: Get initial unread counts when connecting
            var localUserData = await Client.ConnectUserAsync("api_key", "user_id", "user_token");

            Debug.Log(localUserData.UnreadChannels);
            Debug.Log(localUserData.TotalUnreadCount);

            // You can also access unread counts via Client.LocalUserData after connection
            // Or subscribe to the Connected event for real-time updates
            Client.Connected += (IStreamLocalUserData userData) =>
            {
                Debug.Log(userData.UnreadChannels);
                Debug.Log(userData.TotalUnreadCount);
            };
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/unread/?language=unity#unread-counts---server-side
        /// </summary>
        public async Task GetLatestUnreadCounts()
        {
            var current = await Client.GetLatestUnreadCountsAsync();

            Debug.Log(current.TotalUnreadCount); // Total unread messages
            Debug.Log(current.TotalUnreadThreadsCount); // Total unread threads

            foreach (var unreadChannel in current.UnreadChannels)
            {
                Debug.Log(unreadChannel.ChannelCid); // CID of the channel with unread messages
                Debug.Log(unreadChannel.UnreadCount); // Count of unread messages
                Debug.Log(unreadChannel.LastRead); // Datetime of the last read message
            }

            foreach (var unreadChannelByType in current.UnreadChannelsByType)
            {
                Debug.Log(unreadChannelByType.ChannelType); // Channel type
                Debug.Log(unreadChannelByType.ChannelCount); // How many channels of this type have unread messages
                Debug.Log(unreadChannelByType.UnreadCount); // How many unread messages in all channels of this type
            }

            foreach (var unreadThread in current.UnreadThreads)
            {
                Debug.Log(unreadThread.ParentMessageId); // Message ID of the parent message for this thread
                Debug.Log(unreadThread.LastReadMessageId); // Last read message in this thread
                Debug.Log(unreadThread.UnreadCount); // Count of unread messages
                Debug.Log(unreadThread.LastRead); // Datetime of the last read message
            }
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/unread/?language=unity#mark-read
        /// </summary>
        public async Task MarkRead()
        {
            IStreamMessage message = null;

            await message.MarkMessageAsLastReadAsync();
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/unread/?language=unity#mark-read
        /// </summary>
        public async Task MarkAlreadyReadMessageAsUnread()
        {
            IStreamMessage message = null;
            IStreamChannel channel = null;
            var messageId = "message-id";

            // Mark the channel containing this message as unread starting from this message
            await message.MarkAsUnreadAsync();

            // Or mark a channel as unread starting from a specific message id
            await channel.MarkChannelAsUnreadAsync(messageId);
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/unread/?language=unity#mark-all-as-read
        /// </summary>
        public async Task MarkAllAsRead()
        {
            IStreamMessage message = null;
            IStreamChannel channel = null;

            // Mark this message as last read
            await message.MarkMessageAsLastReadAsync();

            // Mark whole channel as read
            await channel.MarkChannelReadAsync();
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/unread/?language=unity#read-state---showing-how-far-other-users-have-read
        /// </summary>
        public void ReadState()
        {
            IStreamChannel channel = null;

            // Every channel maintains a full list of read state for each channel member
            foreach (var read in channel.Read)
            {
                Debug.Log(read.User); // User
                Debug.Log(read.UnreadMessages); // How many unread messages
                Debug.Log(read.LastRead); // Last read date
            }
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/unread/?language=unity#unread-messages-per-channel
        /// </summary>
        public void UnreadMessagesPerChannel()
        {
            IStreamChannel channel = null;

            // Every channel maintains a full list of read state for each channel member
            foreach (var read in channel.Read)
            {
                Debug.Log(read.User); // User
                Debug.Log(read.UnreadMessages); // How many unread messages
                Debug.Log(read.LastRead); // Last read date
            }
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/unread/?language=unity#unread-mentions-per-channel
        /// </summary>
        public Task UnreadMentionsPerChannel()
        {
            // Will be implemented soon, raise a GitHub issue if you need this feature https://github.com/GetStream/stream-chat-unity/issues/
            return Task.CompletedTask;
        }

        /// <summary>
        /// Retrieve unread counts without first calling <see cref="IStreamChatClient.ConnectUserAsync"/>.
        /// This is useful for surfacing an unread badge in the background without a persistent connection.
        /// </summary>
        public async Task GetLatestUnreadCountsInOfflineMode()
        {
            // Set authorization credentials
            var authCredentials = new AuthCredentials("api_key", "user_id", "user_token");
            Client.SetAuthorizationCredentials(authCredentials);

            // Retrieve unread counts without connecting to the chat service via Client.ConnectUserAsync
            var unreadCounts = await Client.GetLatestUnreadCountsAsync();
        }

        private IStreamChatClient Client { get; } = StreamChatClient.CreateDefaultClient();
    }
}
