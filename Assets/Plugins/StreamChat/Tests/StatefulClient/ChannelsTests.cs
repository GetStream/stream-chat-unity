#if STREAM_TESTS_ENABLED
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

        [UnityTest]
        public IEnumerator When_mute_channel_expect_muted()
            => ConnectAndExecute(When_mute_channel_expect_muted_Async);

        private async Task When_mute_channel_expect_muted_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();

            const string MessageText = "fds*fhjfdks9";

            await channel.MuteChannelAsync();

            Assert.IsNotEmpty(StatefulClient.LocalUser.ChannelMutes);

            var channelMute = StatefulClient.LocalUser.ChannelMutes.FirstOrDefault(m => m.Channel == channel);
            Assert.IsNotNull(channelMute);

            //StreamTodo: resolve that User and LocalUser should be the same object
            Assert.AreEqual(channelMute.User.Id, StatefulClient.LocalUser.Id);

            //StreamTodo: resolve this, perhaps there is a channel updated event that covers this
            //Assert.AreEqual(true, channel.Muted);
        }

        [UnityTest]
        public IEnumerator When_unmute_muted_channel_expect_unmuted()
            => ConnectAndExecute(When_unmute_muted_channel_expect_unmuted_Async);

        private async Task When_unmute_muted_channel_expect_unmuted_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();

            const string MessageText = "fds*fhjfdks9";

            await channel.MuteChannelAsync();

            Assert.IsNotEmpty(StatefulClient.LocalUser.ChannelMutes);

            var channelMute = StatefulClient.LocalUser.ChannelMutes.FirstOrDefault(m => m.Channel == channel);
            Assert.IsNotNull(channelMute);

            //StreamTodo: resolve that User and LocalUser should be the same object
            Assert.AreEqual(channelMute.User.Id, StatefulClient.LocalUser.Id);

            await channel.UnmuteChannel();

            channelMute = StatefulClient.LocalUser.ChannelMutes.FirstOrDefault(m => m.Channel == channel);
            Assert.IsNull(channelMute);
        }
    }
}
#endif