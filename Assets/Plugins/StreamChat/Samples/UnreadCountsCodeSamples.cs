using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Core.StatefulModels;
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

        private IStreamChatClient Client { get; } = StreamChatClient.CreateDefaultClient();
    }

    internal sealed class TypingIndicatorsCodeSamples
    {
        public async Task SendStartStopTypingEvents()
        {
            IStreamChannel channel = null;

// Send typing started event
            await channel.SendTypingStartedEventAsync();

// Send typing stopped event
            await channel.SendTypingStoppedEventAsync();
        }

        public async Task ReceivingTypingEvents()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, "channel-id");
            channel.UserStartedTyping += OnUserStartedTyping;
            channel.UserStoppedTyping += OnUserStoppedTyping;
        }

        private void OnUserStartedTyping(IStreamChannel channel, IStreamUser user)
        {
        }

        private void OnUserStoppedTyping(IStreamChannel channel, IStreamUser user)
        {
        }

        private IStreamChatClient Client { get; } = StreamChatClient.CreateDefaultClient();
    }
}