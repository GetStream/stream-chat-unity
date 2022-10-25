#if STREAM_TESTS_ENABLED
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using StreamChat.Core.Requests;
using StreamChat.Core.State;
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
        public IEnumerator When_creating_channel_with_id_expect_no_errors()
            => ConnectAndExecute(When_creating_channel_with_id_expect_no_errors_Async);

        private async Task When_creating_channel_with_id_expect_no_errors_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
        }

        [UnityTest]
        public IEnumerator When_creating_channel_for_users_expect_no_errors()
            => ConnectAndExecute(When_creating_channel_for_users_expect_no_errors_Async);

        private async Task When_creating_channel_for_users_expect_no_errors_Async()
        {
            var userSteveId = "integration-test-user-1-" + Guid.NewGuid();
            var userDaveId = "integration-test-user-2-" + Guid.NewGuid();

            var requestBody = new UpdateUsersRequest
            {
                Users = new Dictionary<string, UserObjectRequest>
                {
                    {
                        userSteveId, new UserObjectRequest
                        {
                            Id = userSteveId,
                            AdditionalProperties = new Dictionary<string, object>
                            {
                                {"name", "Steve"}
                            }
                        }
                    },
                    {
                        userDaveId, new UserObjectRequest
                        {
                            Id = userDaveId,
                            AdditionalProperties = new Dictionary<string, object>
                            {
                                {"name", "Dave"}
                            }
                        }
                    }
                }
            };

            var response = await StatefulClient.LowLevelClient.UserApi.UpsertManyUsersAsync(requestBody);
            var users = response.Users.Select(_ => _.Value).ToList();

            var filters = new Dictionary<string, object>()
            {
                {
                    "id", new Dictionary<string, object>
                    {
                        {
                            "$in", users.Select(_ => _.Id).ToList()
                        }
                    }
                }
            };

            var streamUsers = await StatefulClient.QueryUsersAsync(filters);
            Assert.NotNull(streamUsers);

            var steveUser = streamUsers.FirstOrDefault(_ => _.Id == userSteveId);
            var daveUser = streamUsers.FirstOrDefault(_ => _.Id == userDaveId);

            Assert.NotNull(steveUser);
            Assert.NotNull(daveUser);

            var channelForMembers = await StatefulClient.GetOrCreateChannelAsync(ChannelType.Messaging, streamUsers);
            Assert.NotNull(channelForMembers);
            Assert.NotNull(channelForMembers.Members);
            Assert.AreEqual(2, channelForMembers.Members.Count);

            var steveMember = channelForMembers.Members.FirstOrDefault(_ => _.UserId == userSteveId);
            var daveMember = channelForMembers.Members.FirstOrDefault(_ => _.UserId == userDaveId);

            Assert.NotNull(steveMember);
            Assert.NotNull(daveMember);

            //StreamTodo: would be best to remove the users but this is restricted from FE SDK, we would need a backend service with CRON
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
        public IEnumerator When_mute_channel_expect_muted() => ConnectAndExecute(When_mute_channel_expect_muted_Async);

        private async Task When_mute_channel_expect_muted_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();

            await channel.MuteChannelAsync();

            Assert.IsNotEmpty(StatefulClient.LocalUser.ChannelMutes);

            var channelMute = StatefulClient.LocalUser.ChannelMutes.FirstOrDefault(m => m.Channel == channel);
            Assert.IsNotNull(channelMute);

            //StreamTodo: resolve that User and LocalUser should be the same object
            Assert.AreEqual(channelMute.User.Id, StatefulClient.LocalUser.Id);

            Assert.AreEqual(true, channel.Muted);
        }

        [UnityTest]
        public IEnumerator When_unmute_muted_channel_expect_unmuted()
            => ConnectAndExecute(When_unmute_muted_channel_expect_unmuted_Async);

        private async Task When_unmute_muted_channel_expect_unmuted_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();

            await channel.MuteChannelAsync();

            Assert.IsNotEmpty(StatefulClient.LocalUser.ChannelMutes);

            var channelMute = StatefulClient.LocalUser.ChannelMutes.FirstOrDefault(m => m.Channel == channel);
            Assert.IsNotNull(channelMute);
            Assert.AreEqual(true, channel.Muted);

            //StreamTodo: resolve that User and LocalUser should be the same object
            Assert.AreEqual(channelMute.User.Id, StatefulClient.LocalUser.Id);

            await channel.UnmuteChannel();

            channelMute = StatefulClient.LocalUser.ChannelMutes.FirstOrDefault(m => m.Channel == channel);
            Assert.IsNull(channelMute);
            Assert.AreEqual(false, channel.Muted);
        }

        [UnityTest]
        public IEnumerator When_delete_multiple_channels_expect_no_channels_in_state_client()
            => ConnectAndExecute(When_delete_multiple_channels_expect_no_channels_in_state_client_Async);

        private async Task When_delete_multiple_channels_expect_no_channels_in_state_client_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            var channel2 = await CreateUniqueTempChannelAsync();

            Assert.IsTrue(StatefulClient.WatchedChannels.Contains(channel));
            Assert.IsTrue(StatefulClient.WatchedChannels.Contains(channel2));

            var response
                = await StatefulClient.DeleteMultipleChannelsAsync(new StreamChannel[] { channel, channel2 },
                    isHardDelete: true);

            Assert.That(response.Result, Contains.Key(channel.Cid));
            Assert.That(response.Result, Contains.Key(channel2.Cid));

            SkipThisTempChannelDeletionInTearDown(channel);
            SkipThisTempChannelDeletionInTearDown(channel2);

            Assert.IsEmpty(StatefulClient.WatchedChannels);
        }
    }
}
#endif