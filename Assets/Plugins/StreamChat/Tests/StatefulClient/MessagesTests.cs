﻿#if STREAM_TESTS_ENABLED
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.Requests;
using StreamChat.Core.State.Requests;
using StreamChat.Core.State.TrackedObjects;
using UnityEngine.TestTools;

namespace StreamChat.Tests.StatefulClient
{
    /// <summary>
    /// Tests operations executed on <see cref="StreamMessage"/>
    /// </summary>
    public class MessagesTests : BaseStateIntegrationTests
    {
        [UnityTest]
        public IEnumerator When_Update_message_expect_message_changed()
            => ConnectAndExecute(When_Update_message_expect_message_changed_Async);

        private async Task When_Update_message_expect_message_changed_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();

            const string MessageText = "fds*fhjfdks9";

            var sentMessage = await channel.SendNewMessageAsync(MessageText);
            Assert.AreEqual(MessageText, sentMessage.Text);

            var messageInChannel = channel.Messages.FirstOrDefault(_ => _.Id == sentMessage.Id);
            Assert.NotNull(messageInChannel);

            // StreamTodo: Test all properties like pinned, attachments, mentions, Html, Mml, etc.
            await messageInChannel.UpdateAsync(new StreamUpdateMessageRequest
            {
                Text = "New changed message",
            });

            messageInChannel = channel.Messages.FirstOrDefault(_ => _.Id == sentMessage.Id);
            Assert.NotNull(messageInChannel);

            Assert.AreEqual("New changed message", messageInChannel.Text);
        }

        [UnityTest]
        public IEnumerator When_message_soft_delete_message_expect_text_cleared()
            => ConnectAndExecute(When_message_soft_delete_message_expect_text_cleared_Async);

        public async Task When_message_soft_delete_message_expect_text_cleared_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();

            const string MessageText = "fds*fhjfdks9";

            var sentMessage = await channel.SendNewMessageAsync(MessageText);
            Assert.AreEqual(sentMessage.Text, MessageText);

            var messageInChannel = channel.Messages.FirstOrDefault(_ => _.Id == sentMessage.Id);
            Assert.NotNull(messageInChannel);

            await messageInChannel.SoftDeleteAsync();

            //StreamTodo: seems not deterministic, probably a race condition between WS event and Rest Call complete, we should wait max 500ms for the event

            messageInChannel = channel.Messages.FirstOrDefault(_ => _.Id == sentMessage.Id);
            Assert.NotNull(messageInChannel);
            Assert.IsEmpty(messageInChannel.Text);
        }

        [UnityTest]
        public IEnumerator When_message_hard_delete_message_expect_message_removed()
            => ConnectAndExecute(When_message_hard_delete_message_expect_message_removed_Async);

        public async Task When_message_hard_delete_message_expect_message_removed_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();

            const string MessageText = "fds*fhjfdks9";

            var sentMessage = await channel.SendNewMessageAsync(MessageText);
            Assert.AreEqual(sentMessage.Text, MessageText);

            var messageInChannel = channel.Messages.FirstOrDefault(_ => _.Id == sentMessage.Id);
            Assert.NotNull(messageInChannel);

            await messageInChannel.HardDeleteAsync();

            messageInChannel = channel.Messages.FirstOrDefault(_ => _.Id == sentMessage.Id);
            Assert.IsNull(messageInChannel);
        }

        [UnityTest]
        public IEnumerator When_message_pin_expected_message_pinned()
            => ConnectAndExecute(When_message_pin_expected_message_pinned_Async);

        public async Task When_message_pin_expected_message_pinned_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();

            const string MessageText = "fds*fhjfdks9";

            var sentMessage = await channel.SendNewMessageAsync(MessageText);
            Assert.AreEqual(sentMessage.Text, MessageText);

            await sentMessage.PinAsync();

            var messageInChannel = channel.Messages.FirstOrDefault(_ => _.Id == sentMessage.Id);
            Assert.AreEqual(true, messageInChannel.Pinned);
            Assert.IsNull(messageInChannel.PinExpires);

            Assert.AreEqual(StatefulClient.LocalUserData.User, messageInChannel.PinnedBy);
        }

        [UnityTest]
        public IEnumerator When_message_pin_with_expire_expected_message_pinned_with_expire_set()
            => ConnectAndExecute(When_message_pin_with_expire_expected_message_pinned_with_expire_set_Async);

        public async Task When_message_pin_with_expire_expected_message_pinned_with_expire_set_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();

            const string MessageText = "fds*fhjfdks9";

            var sentMessage = await channel.SendNewMessageAsync(MessageText);
            Assert.AreEqual(sentMessage.Text, MessageText);

            await sentMessage.PinAsync(DateTime.UtcNow.Add(new TimeSpan(0, 0, 10, 0)));

            var messageInChannel = channel.Messages.FirstOrDefault(_ => _.Id == sentMessage.Id);
            Assert.AreEqual(true, messageInChannel.Pinned);

            var expiresInMinutes = (messageInChannel.PinExpires.Value - DateTime.UtcNow).TotalMinutes;
            Assert.That(expiresInMinutes, Is.InRange(9, 11));
        }

        [UnityTest]
        public IEnumerator When_pinned_message_unpin_expected_message_unpinned()
            => ConnectAndExecute(When_pinned_message_unpin_expected_message_unpinned_Async);

        public async Task When_pinned_message_unpin_expected_message_unpinned_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();

            const string MessageText = "fds*fhjfdks9";

            var sentMessage = await channel.SendNewMessageAsync(MessageText);
            Assert.AreEqual(sentMessage.Text, MessageText);

            await sentMessage.PinAsync();

            var messageInChannel = channel.Messages.FirstOrDefault(_ => _.Id == sentMessage.Id);
            Assert.AreEqual(true, messageInChannel.Pinned);

            await messageInChannel.Unpin();
            messageInChannel = channel.Messages.FirstOrDefault(_ => _.Id == sentMessage.Id);
            Assert.AreEqual(false, messageInChannel.Pinned);
        }

        [UnityTest]
        public IEnumerator When_message_flag_requested_expected_message_flag()
            => ConnectAndExecute(When_message_flag_requested_expected_message_flag_Async);

        public async Task When_message_flag_requested_expected_message_flag_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();

            const string MessageText = "fds*fhjfdks9";

            var sentMessage = await channel.SendNewMessageAsync(MessageText);
            Assert.AreEqual(sentMessage.Text, MessageText);

            await sentMessage.FlagAsync();

            var response = await StatefulClient.LowLevelClient.InternalModerationApi.QueryMessageFlagsAsync(
                new QueryMessageFlagsRequestInternalDTO
                {
                    FilterConditions = new Dictionary<string, object>()
                    {
                        {"channel_cid", new Dictionary<string, string>()
                        {
                            {"$eq", channel.Cid}
                        }}
                    },
                    Limit = 30,
                    Offset = 0,
                });

            var messageFlag = response.Flags.FirstOrDefault(_ => _.Message.Id == sentMessage.Id);
            Assert.NotNull(messageFlag);
        }
    }
}
#endif