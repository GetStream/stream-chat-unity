#if STREAM_TESTS_ENABLED
using NUnit.Framework;
using StreamChat.Core.StatefulModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.TestTools;

namespace StreamChat.Tests.StatefulClient
{
    /// <summary>
    /// Tests verifying state recovery after disconnections
    /// </summary>
    internal class ClientStateSyncTests : BaseStateIntegrationTests
    {
        [UnityTest]
        public IEnumerator When_client_reconnects_expect_receiving_missed_messages()
            => ConnectAndExecute(When_client_reconnects_expect_receiving_missed_messages_Async);

        private async Task When_client_reconnects_expect_receiving_missed_messages_Async()
        {
            // Create channel
            var channel = await CreateUniqueTempChannelAsync();

            var otherClient = await GetConnectedOtherClientAsync();

            // Fetch channel on other client to get it loaded into state layer
            var otherClientChannel = await otherClient.GetOrCreateChannelWithIdAsync(channel.Type, channel.Id);

            Assert.AreEqual(channel.Cid, otherClientChannel.Cid);

            var otherClientMessage = await otherClientChannel.SendNewMessageAsync("BEFORE DISCONNECT #1");
            var otherClientMessage2 = await otherClientChannel.SendNewMessageAsync("BEFORE DISCONNECT #2");

            await otherClientMessage.SendReactionAsync("like");

            // DISCONNECT
            await otherClient.DisconnectUserAsync();
            Assert.IsFalse(otherClient.IsConnected);

            // Send 2 messages on the first client
            var message = await channel.SendNewMessageAsync("RECONNECTED #1");
            var message2 = await channel.SendNewMessageAsync("RECONNECTED #2");

            // Messages should be presents on the connected client
            Assert.IsTrue(channel.Messages.Contains(message));
            Assert.IsTrue(channel.Messages.Contains(message2));

            // And should not be present on the disconnected client
            Assert.IsFalse(otherClientChannel.Messages.Any(m => m.Id == message.Id));
            Assert.IsFalse(otherClientChannel.Messages.Any(m => m.Id == message2.Id));

            // Reconnect other client
            await GetConnectedOtherClientAsync();

            // Wait for sync request to complete
            await WaitWhileTrueAsync(() => otherClientChannel.Messages.All(m => m.Id != message.Id));

            // Messages should now be present on the second client with no duplicates
            Assert.IsNotNull(otherClientChannel.Messages.Single(m => m.Id == message.Id));
            Assert.IsNotNull(otherClientChannel.Messages.Single(m => m.Id == message2.Id));

            // Check for duplicates
            var uniqueIds = new HashSet<string>();
            Assert.IsTrue(otherClientChannel.Messages.All(m => uniqueIds.Add(m.Id)));

            Assert.AreEqual(4, otherClientChannel.Messages.Count);
            Assert.AreEqual(1, otherClientChannel.Messages.Sum(m => m.ReactionCounts.Values.Sum()));
        }

        [UnityTest]
        public IEnumerator When_client_reconnects_expect_receiving_missed_messages2()
            => ConnectAndExecute(When_client_reconnects_expect_receiving_missed_messages2_Async);

        private async Task When_client_reconnects_expect_receiving_missed_messages2_Async()
        {
            // Create channel
            var channel = await CreateUniqueTempChannelAsync();

            var otherClient = await GetConnectedOtherClientAsync();

            // Fetch channel on other client to get it loaded into state layer
            var otherClientChannel = await otherClient.GetOrCreateChannelWithIdAsync(channel.Type, channel.Id);

            Assert.AreEqual(channel.Cid, otherClientChannel.Cid);

            var otherClientMessage = await otherClientChannel.SendNewMessageAsync("BEFORE DISCONNECT #1");

            // DISCONNECT
            await otherClient.DisconnectUserAsync();
            Assert.IsFalse(otherClient.IsConnected);

            // Send messages on the first client
            var message = await channel.SendNewMessageAsync("RECONNECTED #1");
            var message2 = await channel.SendNewMessageAsync("RECONNECTED #2");

            await message.SendReactionAsync("heart");
            await message.SendReactionAsync("like");

            await message2.SendReactionAsync("like");
            await message2.SendReactionAsync("nice");
            await message2.SendReactionAsync("smile");

            // Messages should be presents on the connected client
            Assert.IsTrue(channel.Messages.Contains(message));
            Assert.IsTrue(channel.Messages.Contains(message2));

            // And should not be present on the disconnected client
            Assert.IsFalse(otherClientChannel.Messages.Any(m => m.Id == message.Id));
            Assert.IsFalse(otherClientChannel.Messages.Any(m => m.Id == message2.Id));

            // Reconnect other client
            await GetConnectedOtherClientAsync();

            // Wait for sync request to complete
            await WaitWhileTrueAsync(() => otherClientChannel.Messages.All(m => m.Id != message.Id));

            // Messages should now be present on the second client with no duplicates
            var otherClientMessage1 = otherClientChannel.Messages.Single(m => m.Id == message.Id);
            var otherClientMessage2 = otherClientChannel.Messages.Single(m => m.Id == message2.Id);
            Assert.IsNotNull(otherClientMessage1);
            Assert.IsNotNull(otherClientMessage2);

            // Reactions should now be present on the second client
            Assert.AreEqual(2, otherClientMessage1.ReactionCounts.Values.Sum());
            Assert.AreEqual(3, otherClientMessage2.ReactionCounts.Values.Sum());
            Assert.IsTrue(new[] { "like", "heart" }.All(otherClientMessage1.ReactionCounts.Keys.Contains));
            Assert.IsTrue(new[] { "like", "nice", "smile" }.All(otherClientMessage2.ReactionCounts.Keys.Contains));

            // Check for duplicates
            var uniqueIds = new HashSet<string>();
            Assert.IsTrue(otherClientChannel.Messages.All(m => uniqueIds.Add(m.Id)));

            Assert.AreEqual(3, otherClientChannel.Messages.Count);
        }

        //StreamTodo: validate that appropriate events are being triggered on the StreamChatClient instance


        [UnityTest]
        public IEnumerator When_client_sends_message_right_after_reconnect_expect_received_older_messages_to_be_in_correct_order()
    => ConnectAndExecute(When_client_sends_message_right_after_reconnect_expect_received_older_messages_to_be_in_correct_order_Async);

        private async Task When_client_sends_message_right_after_reconnect_expect_received_older_messages_to_be_in_correct_order_Async()
        {
            // Create channel
            var channel = await CreateUniqueTempChannelAsync();

            var otherClient = await GetConnectedOtherClientAsync();

            // Fetch channel on other client to get it loaded into state layer
            var otherClientChannel = await otherClient.GetOrCreateChannelWithIdAsync(channel.Type, channel.Id);

            Assert.AreEqual(channel.Cid, otherClientChannel.Cid);

            var otherClientMessage = await otherClientChannel.SendNewMessageAsync("BEFORE DISCONNECT #1");

            // DISCONNECT
            await otherClient.DisconnectUserAsync();
            Assert.IsFalse(otherClient.IsConnected);

            // Send messages on the first client
            var message = await channel.SendNewMessageAsync("RECONNECTED #1");
            var message2 = await channel.SendNewMessageAsync("RECONNECTED #2");

            // Messages should be presents on the connected client
            Assert.IsTrue(channel.Messages.Contains(message));
            Assert.IsTrue(channel.Messages.Contains(message2));

            // And should not be present on the disconnected client
            Assert.IsFalse(otherClientChannel.Messages.Any(m => m.Id == message.Id));
            Assert.IsFalse(otherClientChannel.Messages.Any(m => m.Id == message2.Id));

            // Reconnect other client
            await GetConnectedOtherClientAsync();

            // We do not wait for the /sync to complete
            var otherClientMessageAfterReconnect = await otherClientChannel.SendNewMessageAsync("AFTER #1");
            var otherClientMessageAfterReconnect2 = await otherClientChannel.SendNewMessageAsync("AFTER #2");

            // Wait for sync request to complete
            await WaitWhileTrueAsync(() => otherClientChannel.Messages.All(m => m.Id != message2.Id));

            // Assert correct number of messages
            Assert.AreEqual(5, otherClientChannel.Messages.Count);

            // Assert no duplicates
            var uniqueIds = new HashSet<string>();
            Assert.IsTrue(otherClientChannel.Messages.All(m => uniqueIds.Add(m.Id)));

            var messages = otherClientChannel.Messages.ToArray();
            // Assert correct order
            Assert.AreEqual(0, Array.FindIndex(messages, m => m.Id == otherClientMessage.Id));
            Assert.AreEqual(1, Array.FindIndex(messages, m => m.Id == message.Id));
            Assert.AreEqual(2, Array.FindIndex(messages, m => m.Id == message2.Id));
            Assert.AreEqual(3, Array.FindIndex(messages, m => m.Id == otherClientMessageAfterReconnect.Id));
            Assert.AreEqual(4, Array.FindIndex(messages, m => m.Id == otherClientMessageAfterReconnect2.Id));


        }
    }
}
#endif