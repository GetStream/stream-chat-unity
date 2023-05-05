#if STREAM_TESTS_ENABLED
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using StreamChat.Core.LowLevelClient.Models;
using StreamChat.Core.LowLevelClient.Requests;
using StreamChat.Core.LowLevelClient.Responses;
using UnityEngine.TestTools;

namespace StreamChat.Tests.LowLevelClient.Integration
{
    /// <summary>
    /// Integration tests for Users
    /// </summary>
    internal class ModerationApiIntegrationTests : BaseIntegrationTests
    {
        [UnityTest]
        public IEnumerator Mute_user()
        {
            yield return LowLevelClient.WaitForClientToConnect();

            const int MuteTimeout = 5;

            var muteRequest = new MuteUserRequest
            {
                TargetIds = new List<string>
                {
                    OtherUserId
                },
                Timeout = MuteTimeout
            };

            var muteUserTask = LowLevelClient.ModerationApi.MuteUserAsync(muteRequest);

            yield return muteUserTask.RunAsIEnumerator(response =>
            {
                var muteInfo = response.Mute;
                Assert.AreEqual(muteInfo.Target.Id, OtherUserId);

                var calcedTimeout = (muteInfo.Expires - muteInfo.UpdatedAt).Value.TotalMinutes;

                Assert.LessOrEqual(Math.Abs(calcedTimeout - MuteTimeout), 0.1f);
            });
        }

        [UnityTest]
        public IEnumerator Unmute_user()
        {
            yield return Mute_user();

            var unmuteUserRequest = new UnmuteUserRequest
            {
                TargetIds = new List<string>
                {
                    OtherUserId
                },
            };

            var unmuteUserAsync = LowLevelClient.ModerationApi.UnmuteUserAsync(unmuteUserRequest);

            yield return unmuteUserAsync.RunAsIEnumerator(response => { });
        }

        [UnityTest]
        public IEnumerator Ban_user()
        {
            yield return LowLevelClient.WaitForClientToConnect();

            const string channelType = "messaging";

            ChannelState channelState = null;
            yield return CreateTempUniqueChannel(channelType, new ChannelGetOrCreateRequest(),
                state => channelState = state);

            const int BanTimeout = 10;

            var banRequest = new BanRequest
            {
                TargetUserId = OtherUserId,
                Id = channelState.Channel.Id,
                Type = channelType,
                Timeout = BanTimeout
            };

            var banUserTask = LowLevelClient.ModerationApi.BanUserAsync(banRequest);

            yield return banUserTask.RunAsIEnumerator(response => { });

            var queryBannedUsersRequest = new QueryBannedUsersRequest
            {
                FilterConditions = new Dictionary<string, object>
                {
                    {
                        "channel_cid", new Dictionary<string, object>()
                        {
                            { "$eq", channelState.Channel.Cid }
                        }
                    }
                }
            };

            var queryBannedUsersTask = LowLevelClient.ModerationApi.QueryBannedUsersAsync(queryBannedUsersRequest);

            yield return queryBannedUsersTask.RunAsIEnumerator(response =>
            {
                var userBanInfo = response.Bans.FirstOrDefault(_ => _.User.Id == OtherUserId);
                Assert.IsNotNull(userBanInfo);

                var calcedTimeout = (userBanInfo.Expires - userBanInfo.CreatedAt).Value.TotalMinutes;

                Assert.LessOrEqual(Math.Abs(calcedTimeout - BanTimeout), 0.1f);
            });
        }

        [UnityTest]
        public IEnumerator Unban_user()
        {
            yield return LowLevelClient.WaitForClientToConnect();

            const string channelType = "messaging";

            ChannelState channelState = null;
            yield return CreateTempUniqueChannel(channelType, new ChannelGetOrCreateRequest(),
                state => channelState = state);

            const int BanTimeout = 10;

            var banRequest = new BanRequest
            {
                TargetUserId = OtherUserId,
                Id = channelState.Channel.Id,
                Type = channelType,
                Timeout = BanTimeout
            };

            var banUserTask = LowLevelClient.ModerationApi.BanUserAsync(banRequest);
            yield return banUserTask.RunAsIEnumerator(response => { });

            var queryBannedUsersRequest = new QueryBannedUsersRequest
            {
                FilterConditions = new Dictionary<string, object>
                {
                    {
                        "channel_cid", new Dictionary<string, object>()
                        {
                            { "$eq", channelState.Channel.Cid }
                        }
                    }
                }
            };

            var queryBannedUsersTask = LowLevelClient.ModerationApi.QueryBannedUsersAsync(queryBannedUsersRequest);

            yield return queryBannedUsersTask.RunAsIEnumerator(response =>
            {
                var userBanInfo = response.Bans.FirstOrDefault(_ => _.User.Id == OtherUserId);
                Assert.IsNotNull(userBanInfo);

                var calcedTimeout = (userBanInfo.Expires - userBanInfo.CreatedAt).Value.TotalMinutes;

                Assert.LessOrEqual(Math.Abs(calcedTimeout - BanTimeout), 0.1f);
            });

            var unbanRequest = new UnbanRequest
            {
                TargetUserId = OtherUserId,
                Id = channelState.Channel.Id,
                Type = channelType,
            };

            var unbanUserTask = LowLevelClient.ModerationApi.UnbanUserAsync(unbanRequest);
            yield return unbanUserTask.RunAsIEnumerator(response => { });

            var queryBannedUsersRequest2 = new QueryBannedUsersRequest
            {
                FilterConditions = new Dictionary<string, object>
                {
                    {
                        "channel_cid", new Dictionary<string, object>()
                        {
                            { "$eq", channelState.Channel.Cid }
                        }
                    }
                }
            };

            var queryBannedUsersTask2 = LowLevelClient.ModerationApi.QueryBannedUsersAsync(queryBannedUsersRequest2);
            yield return queryBannedUsersTask2.RunAsIEnumerator(response =>
            {
                var userBanInfo = response.Bans.FirstOrDefault(_ => _.User.Id == OtherUserId);
                Assert.IsNull(userBanInfo);
            });
        }

        [UnityTest]
        public IEnumerator When_message_flagged_expect_response_target_message_id_match()
        {
            yield return LowLevelClient.WaitForClientToConnect();

            const string channelType = "messaging";

            ChannelState channelState = null;
            yield return CreateTempUniqueChannel(channelType, new ChannelGetOrCreateRequest(),
                state => channelState = state);

            var sendMessageRequest = new SendMessageRequest
            {
                Message = new MessageRequest
                {
                    Text = "message content"
                }
            };

            var messageResponseTask =
                LowLevelClient.MessageApi.SendNewMessageAsync(channelType, channelState.Channel.Id, sendMessageRequest);

            MessageResponse messageResponse = null;
            yield return messageResponseTask.RunAsIEnumerator(response => { messageResponse = response; });

            var flagMessageTask = LowLevelClient.ModerationApi.FlagMessageAsync(messageResponse.Message.Id);

            yield return flagMessageTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(messageResponse.Message.Id, response.Flag.TargetMessageId);
            });
        }

        [UnityTest]
        public IEnumerator When_messages_flagged_expect_query_flagged_messages_return_them()
        {
            yield return LowLevelClient.WaitForClientToConnect();
            yield return When_messages_flagged_expect_query_flagged_messages_return_them_Async().RunAsIEnumerator();
        }

        private async Task When_messages_flagged_expect_query_flagged_messages_return_them_Async()
        {
            const string channelType = "messaging";

            var channelState = await CreateTempUniqueChannelAsync(channelType, new ChannelGetOrCreateRequest());

            //Send messages

            var sendMessageRequest = new SendMessageRequest
            {
                Message = new MessageRequest
                {
                    Text = "message content"
                }
            };

            var messageResponse = await
                LowLevelClient.MessageApi.SendNewMessageAsync(channelType, channelState.Channel.Id, sendMessageRequest);

            var sendMessageRequest2 = new SendMessageRequest
            {
                Message = new MessageRequest
                {
                    Text = "message content 2"
                }
            };

            var messageResponse2 = await
                LowLevelClient.MessageApi.SendNewMessageAsync(channelType, channelState.Channel.Id, sendMessageRequest2);

            //Flag messages

            var flagResponse = await LowLevelClient.ModerationApi.FlagMessageAsync(messageResponse.Message.Id);

            Assert.AreEqual(messageResponse.Message.Id, flagResponse.Flag.TargetMessageId);

            var flagResponse2 = await LowLevelClient.ModerationApi.FlagMessageAsync(messageResponse2.Message.Id);

            Assert.AreEqual(messageResponse2.Message.Id, flagResponse2.Flag.TargetMessageId);

            //Query message flags
            var queryMessageFlagsRequest = new QueryMessageFlagsRequest
            {
                FilterConditions = new Dictionary<string, object>
                {
                    {
                        "channel_cid", new Dictionary<string, object>
                        {
                            { "$eq", channelState.Channel.Cid }
                        }
                    }
                }
            };

            var flagsQueryResponse = await LowLevelClient.ModerationApi.QueryMessageFlagsAsync(queryMessageFlagsRequest);

            var message1 = flagsQueryResponse.Flags.FirstOrDefault(_ => _.Message.Id == messageResponse.Message.Id);
            var message2 = flagsQueryResponse.Flags.FirstOrDefault(_ => _.Message.Id == messageResponse2.Message.Id);

            Assert.AreEqual(2, flagsQueryResponse.Flags.Count);
            Assert.NotNull(message1);
            Assert.NotNull(message2);
        }

        [UnityTest]
        public IEnumerator When_user_flagged_expect_response_target_user_id_match()
        {
            yield return LowLevelClient.WaitForClientToConnect();
            yield return When_user_flagged_expect_response_target_user_id_match_Async().RunAsIEnumerator();
        }

        private async Task When_user_flagged_expect_response_target_user_id_match_Async()
        {
            const string channelType = "messaging";
            var channelState = await CreateTempUniqueChannelAsync(channelType, new ChannelGetOrCreateRequest());
            
            var response = await LowLevelClient.ModerationApi.FlagUserAsync(OtherUserId);
            Assert.AreEqual(OtherUserId, response.Flag.TargetUser.Id);
        } 
    }
}
#endif