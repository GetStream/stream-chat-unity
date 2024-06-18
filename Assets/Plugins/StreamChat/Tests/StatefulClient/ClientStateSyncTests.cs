#if STREAM_TESTS_ENABLED
using NUnit.Framework;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TestTools;

namespace StreamChat.Tests.StatefulClient
{
    /// <summary>
    /// Tests verifying state recovery after disconnections
    /// </summary>
    internal class ClientStateSyncTests : BaseStateIntegrationTests
    {
        [UnityTest]
        public IEnumerator When_client_reconnects_expect_retrieving_missed_events()
            => ConnectAndExecute(When_client_reconnects_expect_retrieving_missed_events_Async);

        private async Task When_client_reconnects_expect_retrieving_missed_events_Async()
        {

            // Create channel
            var channel = await CreateUniqueTempChannelAsync();

            var otherClient = await GetConnectedOtherClientAsync();

            Debug.LogWarning($"AAAA CLIENT: {Client.LocalUserData.UserId}, OTHER CLIENT: {otherClient.LocalUserData.UserId}");

            // Fetch channel on other client to get it loaded into state layer
            var otherClientChannel = await otherClient.GetOrCreateChannelWithIdAsync(channel.Type, channel.Id);

            Assert.AreEqual(channel.Cid, otherClientChannel.Cid);

            var otherClientMessage = await otherClientChannel.SendNewMessageAsync("BEFORE DISCONNECT #1");
            var otherClientMessage2 = await otherClientChannel.SendNewMessageAsync("BEFORE DISCONNECT #2");

            await otherClientMessage.SendReactionAsync("like");

            // Naive way to simulate disconnect StreamTodo: simulate disconnect by closing WS 
            await otherClient.DisconnectUserAsync();

            Assert.IsFalse(otherClient.IsConnected);

            // Send messages on the first client

            var message = await channel.SendNewMessageAsync("RECONNECTED #1");
            var message2 = await channel.SendNewMessageAsync("RECONNECTED #2");

            Assert.IsTrue(channel.Messages.Contains(message));
            Assert.IsTrue(channel.Messages.Contains(message2));

            Assert.IsFalse(otherClientChannel.Messages.Any(m => m.Id == message.Id));
            Assert.IsFalse(otherClientChannel.Messages.Any(m => m.Id == message2.Id));

            // Reconnect other client
            await GetConnectedOtherClientAsync();


            await Task.Delay(1000);

            Assert.IsTrue(otherClientChannel.Messages.Any(m => m.Id == message.Id));
            Assert.IsTrue(otherClientChannel.Messages.Any(m => m.Id == message2.Id));

            //StreamTodo: make sure there are no duplicates

        }
    }
}
#endif