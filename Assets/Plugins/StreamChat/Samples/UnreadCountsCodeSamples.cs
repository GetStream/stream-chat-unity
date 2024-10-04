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
        /// https://getstream.io/chat/docs/unity/unread/?language=unity
        /// </summary>
        /// <returns></returns>
        public async Task GetUnreadCounts()
        {
            // Once user is connected you can access unread counts via IStreamLocalUserData
            var localUserData = Client.LocalUserData;

            Debug.Log(localUserData.UnreadChannels);
            Debug.Log(localUserData.TotalUnreadCount);

            // It's also returned by the ConnectUserAsync method
            var localUserData2 = await Client.ConnectUserAsync("api_key", "user_id", "user_token");

            // And also returned by the Connected event
            Client.Connected += ClientOnConnected;

            // All above examples returned the same IStreamLocalUserData object which represents the local user connected to the Stream Chat server
        }

        private void ClientOnConnected(IStreamLocalUserData localUserData)
        {
        }

        public async Task MarkRead()
        {
            IStreamMessage message = null;

            await message.MarkMessageAsLastReadAsync();
        }

        public async Task ObserveReadState()
        {
            await Task.CompletedTask;
        }

        public void ChannelsReadState()
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

        public async Task MarkRead2()
        {
            IStreamChannel channel = null;
            IStreamMessage message = null;

            // Mark this message as last read
            await message.MarkMessageAsLastReadAsync();

            // Mark whole channel as read
            await channel.MarkChannelReadAsync();
        }

        public async Task GetCurrentUnreadCounts()
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