#if STREAM_TESTS_ENABLED
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using StreamChat.Core.LowLevelClient.Requests;
using StreamChat.Core;
using StreamChat.Core.Models;
using StreamChat.Core.QueryBuilders.Filters;
using StreamChat.Core.QueryBuilders.Filters.Channels;
using StreamChat.Core.QueryBuilders.Filters.Users;
using StreamChat.Core.QueryBuilders.Sort;
using StreamChat.Core.Requests;
using StreamChat.Core.StatefulModels;
using UnityEngine.TestTools;

namespace StreamChat.Tests.StatefulClient
{
    /// <summary>
    /// Tests operations executed on <see cref="StreamChannel"/>
    /// </summary>
    internal class ChannelsTests : BaseStateIntegrationTests
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
                                { "name", "Steve" }
                            }
                        }
                    },
                    {
                        userDaveId, new UserObjectRequest
                        {
                            Id = userDaveId,
                            AdditionalProperties = new Dictionary<string, object>
                            {
                                { "name", "Dave" }
                            }
                        }
                    }
                }
            };

            var response = await Client.InternalLowLevelClient.UserApi.UpsertManyUsersAsync(requestBody);
            var users = response.Users.Select(_ => _.Value).ToList();

            var filters = new IFieldFilterRule[]
            {
                UserFilter.Id.In(users.Select(_ => _.Id).ToList())
            };

            var streamUsers = await Client.QueryUsersAsync(filters);
            Assert.NotNull(streamUsers);

            var steveUser = streamUsers.FirstOrDefault(_ => _.Id == userSteveId);
            var daveUser = streamUsers.FirstOrDefault(_ => _.Id == userDaveId);

            Assert.NotNull(steveUser);
            Assert.NotNull(daveUser);

            var channelForMembers =
                await Client.GetOrCreateChannelWithMembersAsync(ChannelType.Messaging, streamUsers);
            Assert.NotNull(channelForMembers);
            Assert.NotNull(channelForMembers.Members);
            Assert.AreEqual(2, channelForMembers.Members.Count);

            var steveMember = channelForMembers.Members.FirstOrDefault(_ => _.User.Id == userSteveId);
            var daveMember = channelForMembers.Members.FirstOrDefault(_ => _.User.Id == userDaveId);

            Assert.NotNull(steveMember);
            Assert.NotNull(daveMember);

            //StreamTodo: would be best to remove the users but this is restricted from FE SDK, we would need a backend service with CRON
        }

        [UnityTest]
        public IEnumerator When_mute_channel_expect_muted() => ConnectAndExecute(When_mute_channel_expect_muted_Async);

        private async Task When_mute_channel_expect_muted_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();

            await channel.MuteChannelAsync();

            // Wait for data to propagate
            await WaitWhileTrueAsync(() => Client.LocalUserData.ChannelMutes.Count == 0);

            Assert.IsNotEmpty(Client.LocalUserData.ChannelMutes);

            var channelMute = Client.LocalUserData.ChannelMutes.FirstOrDefault(m => m.Channel == channel);
            Assert.IsNotNull(channelMute);

            Assert.AreEqual(channelMute.User, Client.LocalUserData.User);

            Assert.AreEqual(true, channel.Muted);
        }

        [UnityTest]
        public IEnumerator When_unmute_muted_channel_expect_unmuted()
            => ConnectAndExecute(When_unmute_muted_channel_expect_unmuted_Async);

        private async Task When_unmute_muted_channel_expect_unmuted_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();

            await channel.MuteChannelAsync();

            Assert.IsNotEmpty(Client.LocalUserData.ChannelMutes);

            var channelMute = Client.LocalUserData.ChannelMutes.FirstOrDefault(m => m.Channel == channel);
            Assert.IsNotNull(channelMute);
            Assert.AreEqual(true, channel.Muted);

            Assert.AreEqual(channelMute.User, Client.LocalUserData.User);

            await channel.UnmuteChannelAsync();

            await WaitWhileTrueAsync(() =>
            {
                channelMute = Client.LocalUserData.ChannelMutes.FirstOrDefault(m => m.Channel == channel);
                return channelMute != null;
            });

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

            Assert.IsTrue(Client.WatchedChannels.Contains(channel));
            Assert.IsTrue(Client.WatchedChannels.Contains(channel2));

            var response
                = await Client.DeleteMultipleChannelsAsync(new IStreamChannel[] { channel, channel2 },
                    isHardDelete: true);

            Assert.That(response.Result, Contains.Key(channel.Cid));
            Assert.That(response.Result, Contains.Key(channel2.Cid));

            SkipThisTempChannelDeletionInTearDown(channel);
            SkipThisTempChannelDeletionInTearDown(channel2);

            await WaitWhileTrueAsync(
                () => Client.WatchedChannels.Contains(channel) || Client.WatchedChannels.Contains(channel2));

            Assert.IsFalse(Client.WatchedChannels.Contains(channel));
            Assert.IsFalse(Client.WatchedChannels.Contains(channel2));
        }

        [UnityTest]
        public IEnumerator When_truncate_channel_with_past_created_at_expect_no_messages_cleared()
            => ConnectAndExecute(When_truncate_channel_expect_messages_cleared_Async);

        private async Task When_truncate_channel_expect_messages_cleared_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();

            await channel.SendNewMessageAsync("Hello");
            await channel.SendNewMessageAsync("Hello 2");
            await channel.SendNewMessageAsync("Hello 3");

            var cts = new TaskCompletionSource<bool>();
            channel.MessageReceived += (streamChannel, streamMessage) => cts.SetResult(true);

            Assert.AreEqual(3, channel.Messages.Count);

            var beforeDate = DateTimeOffset.UtcNow.AddHours(-1);

            await channel.TruncateAsync(beforeDate, "Truncated everything from an hour ago", isHardDelete: true);

            // Wait for message.received event
            await cts.Task;

            //expect no messages removed + system message added
            Assert.AreEqual(4, channel.Messages.Count);
        }

        [UnityTest]
        public IEnumerator When_truncate_channel_with_system_message_expect_only_system_message()
            => ConnectAndExecute(When_truncate_channel_with_system_message_expect_only_system_message_Async);

        private async Task When_truncate_channel_with_system_message_expect_only_system_message_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();

            await channel.SendNewMessageAsync("Hello");
            await channel.SendNewMessageAsync("Hello 2");
            await channel.SendNewMessageAsync("Hello 3");

            Assert.AreEqual(3, channel.Messages.Count);

            const string systemMessage = "Hi, sorry for deleting all";
            await channel.TruncateAsync(systemMessage: systemMessage);

            // Wait for truncated event to be received
            await WaitWhileTrueAsync(() => channel.Messages.Count != 1);

            Assert.AreEqual(1, channel.Messages.Count);
            Assert.AreEqual(systemMessage, channel.Messages[0].Text);
        }

        private class ClanData
        {
            public int MaxMembers;
            public string Name;
            public List<string> Tags;
        }

        [UnityTest]
        public IEnumerator When_set_channel_custom_data_expect_data_on_channel_object()
            => ConnectAndExecute(When_set_channel_custom_data_expect_data_set_on_channel_Async);

        private async Task When_set_channel_custom_data_expect_data_set_on_channel_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();

            var setClanInfo = new ClanData
            {
                MaxMembers = 50,
                Name = "Wild Boards",
                Tags = new List<string>
                {
                    "Competitive",
                    "Legends",
                }
            };

            await channel.UpdatePartialAsync(setFields: new Dictionary<string, object>()
            {
                { "owned_dogs", 5 },
                {
                    "breakfast", new string[]
                    {
                        "donuts"
                    }
                },
                {
                    "clan_info", setClanInfo
                }
            });

            await WaitWhileFalseAsync(
                () => new[] { "owned_dogs", "breakfast", "clan_info" }.All(channel.CustomData.ContainsKey));

            var ownedDogs = channel.CustomData.Get<int>("owned_dogs");
            var breakfast = channel.CustomData.Get<List<string>>("breakfast");
            var clanInfo = channel.CustomData.Get<ClanData>("clan_info");
            Assert.AreEqual(5, ownedDogs);

            Assert.Contains("donuts", breakfast);

            Assert.AreEqual(50, clanInfo.MaxMembers);
            Assert.AreEqual("Wild Boards", clanInfo.Name);
            Assert.Contains("Competitive", clanInfo.Tags);
        }

        [UnityTest]
        public IEnumerator When_unset_channel_custom_data_expect_no_data_on_channel_object()
            => ConnectAndExecute(When_unset_channel_custom_data_expect_no_data_on_channel_object_Async);

        private async Task When_unset_channel_custom_data_expect_no_data_on_channel_object_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            await channel.UpdatePartialAsync(setFields: new Dictionary<string, object>()
            {
                { "owned_dogs", 5 },
                {
                    "breakfast", new string[]
                    {
                        "donuts"
                    }
                }
            });

            await WaitWhileFalseAsync(
                () => new[] { "owned_dogs", "breakfast" }.All(channel.CustomData.ContainsKey));

            var ownedDogs = channel.CustomData.Get<int>("owned_dogs");
            var breakfast = channel.CustomData.Get<List<string>>("breakfast");

            Assert.AreEqual(5, ownedDogs);
            Assert.Contains("donuts", breakfast);

            await channel.UpdatePartialAsync(unsetFields: new string[] { "owned_dogs", "breakfast" });

            //StreamTodo: this can potentially be non deterministic because we rely on WS event being received before call ends

            Assert.IsFalse(channel.CustomData.ContainsKey("owned_dogs"));
            Assert.IsFalse(channel.CustomData.ContainsKey("breakfast"));
        }
        
        [UnityTest]
        public IEnumerator When_update_overwrite_channel_with_full_data_expect_no_change()
            => ConnectAndExecute(When_update_overwrite_channel_with_full_data_expect_no_change_Async);

        private async Task When_update_overwrite_channel_with_full_data_expect_no_change_Async()
        {
            const string channelName = "AwesomeNews";
            var channel = await CreateUniqueTempChannelAsync(channelName);

            var cts = new TaskCompletionSource<bool>();
            var channelUpdatedEventsCount = 0;
            channel.Updated += streamChannel =>
            {
                channelUpdatedEventsCount++;
                if (channelUpdatedEventsCount == 2)
                {
                    cts.SetResult(true);
                }
            };
            
            await channel.UpdatePartialAsync(setFields: new Dictionary<string, object>
            {
                { "owned_dogs", 5 },
                {
                    "breakfast", new string[]
                    {
                        "donuts"
                    }
                }
            });

            await channel.UpdateOverwriteAsync(new StreamUpdateOverwriteChannelRequest(channel));

            await WaitWithTimeoutAsync(cts.Task, 5, $"Channel {nameof(channel.Updated)} event was not received");
            
            Assert.AreEqual(channelName, channel.Name);
            Assert.AreEqual(2, channel.CustomData.Count);
            Assert.IsTrue(channel.CustomData.ContainsKey("owned_dogs"));
            Assert.IsTrue(channel.CustomData.ContainsKey("breakfast"));
            Assert.AreEqual(5, channel.CustomData.Get<int>("owned_dogs"));
            Assert.AreEqual("donuts", channel.CustomData.Get<string[]>("breakfast")[0]);
        }
        
        [UnityTest]
        public IEnumerator When_update_overwrite_channel_with_no_data_expect_data_cleared()
            => ConnectAndExecute(When_update_overwrite_channel_with_no_data_expect_data_cleared_Async);

        private async Task When_update_overwrite_channel_with_no_data_expect_data_cleared_Async()
        {
            const string channelName = "AwesomeNews";
            var channel = await CreateUniqueTempChannelAsync(channelName);

            var cts = new TaskCompletionSource<bool>();
            var channelUpdatedEventsCount = 0;
            channel.Updated += streamChannel =>
            {
                channelUpdatedEventsCount++;
                if (channelUpdatedEventsCount == 2)
                {
                    cts.SetResult(true);
                }
            };
            
            await channel.UpdatePartialAsync(setFields: new Dictionary<string, object>
            {
                { "owned_dogs", 5 },
                {
                    "breakfast", new string[]
                    {
                        "donuts"
                    }
                }
            });

            await channel.UpdateOverwriteAsync(new StreamUpdateOverwriteChannelRequest());

            await WaitWithTimeoutAsync(cts.Task, 5, $"Channel {nameof(channel.Updated)} event was not received");
            
            Assert.AreEqual(string.Empty, channel.Name);
            Assert.AreEqual(0, channel.CustomData.Count);
        }
        
        [UnityTest]
        public IEnumerator When_update_overwrite_channel_with_partial_data_expect_data_partially_cleared()
            => ConnectAndExecute(When_update_overwrite_channel_with_partial_data_expect_data_partially_cleared_Async);

        private async Task When_update_overwrite_channel_with_partial_data_expect_data_partially_cleared_Async()
        {
            const string channelName = "AwesomeNews";
            const string channelName2 = "DifferentName";
            var channel = await CreateUniqueTempChannelAsync(channelName);

            var cts = new TaskCompletionSource<bool>();
            var channelUpdatedEventsCount = 0;
            channel.Updated += streamChannel =>
            {
                channelUpdatedEventsCount++;
                if (channelUpdatedEventsCount == 2)
                {
                    cts.SetResult(true);
                }
            };
            
            await channel.UpdatePartialAsync(setFields: new Dictionary<string, object>
            {
                { "owned_dogs", 5 },
                {
                    "breakfast", new string[]
                    {
                        "donuts"
                    }
                }
            });

            await channel.UpdateOverwriteAsync(new StreamUpdateOverwriteChannelRequest
            {
                Name = channelName2,
                CustomData = new StreamCustomDataRequest
                {
                    {"owned_dogs", 7},
                    {"owned_cats", "twenty"}
                }
            });

            await WaitWithTimeoutAsync(cts.Task, 5, $"Channel {nameof(channel.Updated)} event was not received");
            
            Assert.AreEqual(channelName2, channel.Name);
            Assert.AreEqual(2, channel.CustomData.Count);
            Assert.IsTrue(channel.CustomData.ContainsKey("owned_dogs"));
            Assert.IsTrue(channel.CustomData.ContainsKey("owned_cats"));
            Assert.IsFalse(channel.CustomData.ContainsKey("breakfast"));
            Assert.AreEqual(7, channel.CustomData.Get<int>("owned_dogs"));
            Assert.AreEqual("twenty", channel.CustomData.Get<string>("owned_cats"));
        }

        [UnityTest]
        public IEnumerator When_query_channels_with_no_parameters_expect_no_errors()
            => ConnectAndExecute(When_query_channels_with_no_parameters_expect_no_errors_Async);

        private async Task When_query_channels_with_no_parameters_expect_no_errors_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            var channel2 = await CreateUniqueTempChannelAsync();

            var channels = await Client.QueryChannelsAsync();
            Assert.NotNull(channels);
            Assert.AreNotEqual(0, channels.Count());
        }

        [UnityTest]
        public IEnumerator When_query_channels_with_pagination_expect_paged_results()
            => ConnectAndExecute(When_query_channels_with_pagination_expect_paged_results_Async);

        private async Task When_query_channels_with_pagination_expect_paged_results_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            var channel2 = await CreateUniqueTempChannelAsync();
            var channel3 = await CreateUniqueTempChannelAsync();
            var channel4 = await CreateUniqueTempChannelAsync();

            var filters = new IFieldFilterRule[]
            {
                ChannelFilter.Id.In(channel, channel2, channel3, channel4)
            };

            var channelsFirstPage = (await Client.QueryChannelsAsync(filters,
                ChannelSort.OrderByDescending(ChannelSortFieldName.CreatedAt), offset: 0, limit: 2)).ToArray();

            Assert.NotNull(channelsFirstPage);
            Assert.Contains(channel4, channelsFirstPage);
            Assert.Contains(channel3, channelsFirstPage);

            var channelsSecondPage = (await Client.QueryChannelsAsync(filters,
                ChannelSort.OrderByDescending(ChannelSortFieldName.CreatedAt), offset: 2, limit: 2)).ToArray();

            Assert.NotNull(channelsSecondPage);
            Assert.Contains(channel, channelsSecondPage);
            Assert.Contains(channel2, channelsSecondPage);
        }

        [UnityTest]
        public IEnumerator When_inviting_a_member_expect_channel_updated_with_invited_user()
            => ConnectAndExecute(When_inviting_a_member_expect_channel_updated_with_invited_user_Async);

        private async Task When_inviting_a_member_expect_channel_updated_with_invited_user_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();

            var taskCompletionSource = new TaskCompletionSource<bool>();
            channel.Updated += streamChannel =>
            {
                var invitedMember = streamChannel.Members.FirstOrDefault(m => m.User.Id == OtherUserId);

                Assert.IsNotNull(invitedMember);
                Assert.IsTrue(invitedMember.Invited);

                taskCompletionSource.SetResult(true);
            };

            await channel.InviteMembersAsync(OtherUserId);

            await WaitWithTimeoutAsync(taskCompletionSource.Task, maxSeconds: 3,
                $"Event {nameof(channel.Updated)} was not received");
        }
        
        //StreamTODO: debug why having 2 clients connected simultaneously doesn't work
        // [UnityTest]
        // public IEnumerator When_inviting_a_member_expect_invited_user_to_receive_an_invite()
        //     => ConnectAndExecute(When_inviting_a_member_expect_invited_user_to_receive_an_invite_Async);
        //
        // private async Task When_inviting_a_member_expect_invited_user_to_receive_an_invite_Async()
        // {
        //     var otherClient = await GetConnectedOtherClient();
        //     var channel = await CreateUniqueTempChannelAsync();
        //
        //     var taskCompletionSource = new TaskCompletionSource<bool>();
        //     otherClient.ChannelInviteReceived += (streamChannel, invitee) =>
        //     {
        //         Assert.AreEqual(streamChannel.Cid, channel.Cid);
        //         Assert.AreEqual(invitee.Id, otherClient.LocalUserData.UserId);
        //         
        //         taskCompletionSource.SetResult(true);
        //     };
        //
        //     await channel.InviteMembersAsync(OtherUserId);
        //
        //     await WaitWithTimeout(taskCompletionSource.Task, maxSeconds: 3,
        //         $"Event {nameof(otherClient.ChannelInviteReceived)} was not received");
        // }
        
        [UnityTest]
        public IEnumerator When_freezing_a_channel_expect_channel_frozen()
            => ConnectAndExecute(When_freezing_a_channel_expect_channel_frozen_Async);

        private async Task When_freezing_a_channel_expect_channel_frozen_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            await channel.FreezeAsync();
            Assert.IsTrue(channel.Frozen);
        }
        
        [UnityTest]
        public IEnumerator When_sending_message_to_frozen_channel_expect_error_message_returned()
            => ConnectAndExecute(When_sending_message_to_frozen_channel_expect_error_message_returned_Async);

        private async Task When_sending_message_to_frozen_channel_expect_error_message_returned_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();

            await channel.SendNewMessageAsync("Test1");
            var lastNormalMessage = channel.Messages.LastOrDefault();
            Assert.IsNotNull(lastNormalMessage);
            Assert.AreNotEqual(StreamMessageType.Error, lastNormalMessage.Type);
            
            await channel.FreezeAsync();
            Assert.IsTrue(channel.Frozen);
            
            await channel.SendNewMessageAsync("MessageAfterFrozenChannel");

            var lastMessage = channel.Messages.LastOrDefault();
            
            Assert.IsNotNull(lastMessage);
            Assert.AreEqual(StreamMessageType.Error, lastMessage.Type);
        }
        
        [UnityTest]
        public IEnumerator When_unfreezing_a_frozen_channel_expect_channel_unfrozen()
            => ConnectAndExecute(When_unfreezing_a_frozen_channel_expect_channel_unfrozen_Async);

        private async Task When_unfreezing_a_frozen_channel_expect_channel_unfrozen_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
            await channel.FreezeAsync();
            Assert.IsTrue(channel.Frozen);
            await channel.UnfreezeAsync();
            Assert.IsFalse(channel.Frozen);
            
            await channel.SendNewMessageAsync("MessageAfterUnfrozenChannel");
            var lastMessage = channel.Messages.LastOrDefault();
            Assert.IsNotNull(lastMessage);
            Assert.AreEqual("MessageAfterUnfrozenChannel", lastMessage.Text);
            Assert.AreNotEqual(StreamMessageType.Error, lastMessage.Type);
        }
    }
}

#endif