﻿#if STREAM_TESTS_ENABLED
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using StreamChat.Core.State.TrackedObjects;
using UnityEngine.TestTools;

namespace StreamChat.Tests.StatefulClient
{
    /// <summary>
    /// Tests operations executed on <see cref="StreamChannel"/>
    /// </summary>
    public class ChannelsTests : BaseStateIntegrationTests
    {
        [UnityTest]
        public IEnumerator When_GetOrCreateChannel_expect_no_errors()
            => ConnectAndExecute(When_GetOrCreateChannel_expect_no_errors_Async);

        private async Task When_GetOrCreateChannel_expect_no_errors_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
        }

        [UnityTest]
        public IEnumerator When_channel_send_message_expect_no_errors()
            => ConnectAndExecute(When_channel_send_message_expect_no_errors_Async);

        private async Task When_channel_send_message_expect_no_errors_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();

            const string MessageText = "fds*fhjfdks9";

            var sentMessage = await channel.SendNewMessageAsync(MessageText);
            Assert.AreEqual(sentMessage.Text, MessageText);

            var messageInChannel = channel.Messages.FirstOrDefault(_ => _.Id == sentMessage.Id);
            Assert.NotNull(messageInChannel);
        }
    }
}
#endif