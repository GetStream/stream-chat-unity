using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using StreamChat.Core.LowLevelClient.Events;
using UnityEngine.TestTools;

#if STREAM_TESTS_ENABLED
namespace StreamChat.Tests.StatefulClient
{
    internal class ChannelsModerationTests : BaseStateIntegrationTests
    {
        // This test requires active AI moderation policy - we rely on message being shadowed by the AI moderation.
        // Set "Threat" AI filter to "shadow block"
        // Note: The moderation won't work if you have the `Skip Message Moderation` permission set for this role. Admins probably have this by default.
        [UnityTest]
        public IEnumerator When_client_sends_message_shadowed_by_ai_moderation_expect_other_client_to_not_receive_it()
            => ConnectAndExecute(When_client_sends_message_shadowed_by_ai_moderation_expect_other_client_to_not_receive_it_Async);

        private async Task When_client_sends_message_shadowed_by_ai_moderation_expect_other_client_to_not_receive_it_Async()
        {
            // Create channel
            var channel = await CreateUniqueTempChannelAsync();

            var otherClient = await GetConnectedOtherClientAsync();
            
            // Fetch channel on other client to get it loaded into state layer
            var otherClientChannel = await otherClient.GetOrCreateChannelWithIdAsync(channel.Type, channel.Id);

            
            Assert.AreEqual(channel.Cid, otherClientChannel.Cid);
            
            var normalMessage = await channel.SendNewMessageAsync("normal message");
            
            // Appropriate message should be present for both clients
            Assert.IsTrue(channel.Messages.Contains(normalMessage));
            
            // Wait for other client to receive the message
            await WaitWhileFalseAsync(() => otherClientChannel.Messages.Any(m => m.Id == normalMessage.Id));
            
            Assert.IsNotNull(otherClientChannel.Messages.Single(m => m.Id == normalMessage.Id));
            
            // Setup waiting for the message on the other client
            var messagesReceivedOnOtherClient = new List<EventMessageNew>();
            otherClient.InternalLowLevelClient.MessageReceived += eventMessageNew =>
            {
                messagesReceivedOnOtherClient.Add(eventMessageNew);
            };
            
            var offensiveMessage = await channel.SendNewMessageAsync("I shall kidnap your hamster!");
            
            // Author should believe the message was sent
            Assert.IsTrue(channel.Messages.Contains(offensiveMessage));

            // Wait for other client to receive the offensive message
            await WaitWhileFalseAsync(() => messagesReceivedOnOtherClient.Any(m => m.Message.Id == offensiveMessage.Id));
            
            // Other client should have the offensive message shadowed
            Assert.IsFalse(otherClientChannel.Messages.Any(m => m.Id == offensiveMessage.Id));
        }
    }
}
#endif