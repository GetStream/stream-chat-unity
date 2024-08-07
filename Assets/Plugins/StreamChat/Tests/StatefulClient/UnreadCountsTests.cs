using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine.TestTools;

#if STREAM_TESTS_ENABLED
namespace StreamChat.Tests.StatefulClient
{
    internal class UnreadCountsTests : BaseStateIntegrationTests
    {
        [UnityTest]
        public IEnumerator When_messaged_added_when_disconnected_expect_channel_unread_record_during_offline_state()
            => ConnectAndExecute(
                When_messaged_added_when_disconnected_expect_channel_unread_record_during_offline_state_Async);

        private async Task
            When_messaged_added_when_disconnected_expect_channel_unread_record_during_offline_state_Async()
        {
            var otherClient = await GetConnectedOtherClientAsync();

            // Create channel
            var channel = await CreateUniqueTempChannelAsync();

            // Add other user to the channel
            await channel.AddMembersAsync(new[] { otherClient.LocalUserData.User });

            var otherClientChannel = await TryAsync(
                () => Task.FromResult(otherClient.WatchedChannels.SingleOrDefault(c => c.Cid == channel.Cid)),
                c => c != null);

            var message = await channel.SendNewMessageAsync("Hello 1");

            var otherClientMessage
                = await TryAsync(() => Task.FromResult(otherClientChannel.Messages.Single(m => m.Id == message.Id)),
                    m => m != null);
            await otherClientMessage.MarkMessageAsLastReadAsync();

            // Disconnect other client
            await otherClient.DisconnectUserAsync();
            Assert.IsFalse(otherClient.IsConnected);

            // Send message while other client is offline
            await channel.SendNewMessageAsync("Hello 2");

            // Get latest unread counts
            var unreadCounts = await otherClient.GetLatestUnreadCountsAsync();

            var unreadChannel = unreadCounts.UnreadChannels.Single(c => c.ChannelCid == channel.Cid);
            Assert.AreEqual(1, unreadChannel.UnreadCount);
        }
    }
}
#endif