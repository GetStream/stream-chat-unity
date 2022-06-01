#if STREAM_TESTS_ENABLED
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using StreamChat.Core.Models;
using StreamChat.Core.Requests;
using UnityEngine.TestTools;

namespace StreamChat.Tests.Integration
{
    /// <summary>
    /// Integration tests for Users
    /// </summary>
    public class ModerationApiIntegrationTests : BaseIntegrationTests
    {
        [UnityTest]
        public IEnumerator Mute_user()
        {
            yield return Client.WaitForClientToConnect();

            const int MuteTimeout = 5;

            var muteRequest = new MuteUserRequest
            {
                TargetIds = new List<string>
                {
                    TestUserId
                },
                Timeout = MuteTimeout
            };

            var muteUserTask = Client.ModerationApi.MuteUserAsync(muteRequest);

            yield return muteUserTask.RunAsIEnumerator(response =>
            {
                var muteInfo = response.Mute;
                Assert.AreEqual(muteInfo.Target.Id, TestUserId);

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
                    TestUserId
                },
            };

            var unmuteUserAsync = Client.ModerationApi.UnmuteUserAsync(unmuteUserRequest);

            yield return unmuteUserAsync.RunAsIEnumerator(response =>
            {

            });
        }

        [UnityTest]
        public IEnumerator Ban_user()
        {
            yield return Client.WaitForClientToConnect();

            var channelType = "messaging";

            ChannelState channelState = null;
            yield return CreateTempUniqueChannel(channelType, new ChannelGetOrCreateRequest(), state => channelState = state);

            const int BanTimeout = 10;

            var banRequest = new BanRequest
            {
                TargetUserId = TestUserId,
                Id = channelState.Channel.Id,
                Type = channelType,
                Timeout = BanTimeout
            };

            var banUserTask = Client.ModerationApi.BanUserAsync(banRequest);

            yield return banUserTask.RunAsIEnumerator(response =>
            {
            });

            var queryBannedUsersRequest = new QueryBannedUsersRequest
            {
                FilterConditions = new Dictionary<string, object>
                {
                    {"channel_cid", new Dictionary<string, object>()
                    {
                        { "$eq", channelState.Channel.Cid }
                    }}
                }
            };

            var queryBannedUsersTask = Client.ModerationApi.QueryBannedUsersAsync(queryBannedUsersRequest);

            yield return queryBannedUsersTask.RunAsIEnumerator(response =>
            {
                var userBanInfo = response.Bans.FirstOrDefault(_ => _.User.Id == TestUserId);
                Assert.IsNotNull(userBanInfo);

                var calcedTimeout = (userBanInfo.Expires - userBanInfo.CreatedAt).Value.TotalMinutes;

                Assert.LessOrEqual(Math.Abs(calcedTimeout - BanTimeout), 0.1f);
            });
        }

        [UnityTest]
        public IEnumerator Unban_user()
        {
            yield return Client.WaitForClientToConnect();

            var channelType = "messaging";

            ChannelState channelState = null;
            yield return CreateTempUniqueChannel(channelType, new ChannelGetOrCreateRequest(), state => channelState = state);

            const int BanTimeout = 10;

            var banRequest = new BanRequest
            {
                TargetUserId = TestUserId,
                Id = channelState.Channel.Id,
                Type = channelType,
                Timeout = BanTimeout
            };

            var banUserTask = Client.ModerationApi.BanUserAsync(banRequest);

            yield return banUserTask.RunAsIEnumerator(response =>
            {
            });

            var queryBannedUsersRequest = new QueryBannedUsersRequest
            {
                FilterConditions = new Dictionary<string, object>
                {
                    {"channel_cid", new Dictionary<string, object>()
                    {
                        { "$eq", channelState.Channel.Cid }
                    }}
                }
            };

            var queryBannedUsersTask = Client.ModerationApi.QueryBannedUsersAsync(queryBannedUsersRequest);

            yield return queryBannedUsersTask.RunAsIEnumerator(response =>
            {
                var userBanInfo = response.Bans.FirstOrDefault(_ => _.User.Id == TestUserId);
                Assert.IsNotNull(userBanInfo);

                var calcedTimeout = (userBanInfo.Expires - userBanInfo.CreatedAt).Value.TotalMinutes;

                Assert.LessOrEqual(Math.Abs(calcedTimeout - BanTimeout), 0.1f);
            });

            var unbanRequest = new UnbanRequest
            {
                TargetUserId = TestUserId,
                Id = channelState.Channel.Id,
                Type = channelType,
            };

            var unbanUserTask = Client.ModerationApi.UnbanUserAsync(unbanRequest);

            yield return banUserTask.RunAsIEnumerator(response =>
            {
            });

            var queryBannedUsersRequest2 = new QueryBannedUsersRequest
            {
                FilterConditions = new Dictionary<string, object>
                {
                    {"channel_cid", new Dictionary<string, object>()
                    {
                        { "$eq", channelState.Channel.Cid }
                    }}
                }
            };

            var queryBannedUsersTask2 = Client.ModerationApi.QueryBannedUsersAsync(queryBannedUsersRequest2);

            yield return queryBannedUsersTask2.RunAsIEnumerator(response =>
            {
                var userBanInfo = response.Bans.FirstOrDefault(_ => _.User.Id == TestUserId);
                Assert.IsNull(userBanInfo);
            });
        }
    }
}
#endif