using System.Collections.Generic;
using System.Threading.Tasks;
using StreamChat.Core.LowLevelClient;
using StreamChat.Core.LowLevelClient.Requests;

namespace StreamChat.Samples.LowLevelClient.ClientDocs
{
    public class ModerationApiCodeSamples
    {
        public async Task MuteUser()
        {
            var muteUserRequest = new MuteUserRequest
            {
                TargetIds = new List<string> { "user-id-1" }
            };

            var muteUserResponse = await _lowLevelClient.ModerationApi.MuteUserAsync(muteUserRequest);
        }

        public async Task UnmuteUser()
        {
            var unmuteUserRequest = new UnmuteUserRequest
            {
                TargetIds = new List<string> { "user-id-1" }
            };

            var unmuteResponse = await _lowLevelClient.ModerationApi.UnmuteUserAsync(unmuteUserRequest);
        }

        public async Task BanUserForOneHour()
        {
            var channelState =
                await _lowLevelClient.ChannelApi.GetOrCreateChannelAsync("channel-type", "channel-id",
                    new ChannelGetOrCreateRequest());
            var channel = channelState.Channel;

            //Ban user with id: `user-to-ban-id` for 1 hour
            var banRequest = new BanRequest
            {
                TargetUserId = "user-to-ban-id",
                Id = channel.Id,
                Type = channel.Type,
                Timeout = 60,
                Reason = "Toxic behaviour towards other users"
            };

            var banResponse = await _lowLevelClient.ModerationApi.BanUserAsync(banRequest);
        }

        public async Task BanUserPermanently()
        {
            var channelState =
                await _lowLevelClient.ChannelApi.GetOrCreateChannelAsync("channel-type", "channel-id",
                    new ChannelGetOrCreateRequest());
            var channel = channelState.Channel;

            //Ban user with id: `user-to-ban-id` permanently
            var banRequest = new BanRequest
            {
                TargetUserId = "user-to-ban-id",
                Id = channel.Id,
                Type = channel.Type,
                Reason = "Toxic behaviour towards other users",
            };

            var banResponse = await _lowLevelClient.ModerationApi.BanUserAsync(banRequest);
        }

        public async Task BanUserAndIPAddress()
        {
            var channelState =
                await _lowLevelClient.ChannelApi.GetOrCreateChannelAsync("channel-type", "channel-id",
                    new ChannelGetOrCreateRequest());
            var channel = channelState.Channel;

            //Ban user with id: `user-to-ban-id` along with his last know IP address for 24 hours
            var banRequest = new BanRequest
            {
                TargetUserId = "user-to-ban-id",
                Id = channel.Id,
                Type = channel.Type,
                Timeout = 24 * 60,
                Reason = "Toxic behaviour towards other users",
                IpBan = true,
            };

            var banResponse = await _lowLevelClient.ModerationApi.BanUserAsync(banRequest);
        }

        public async Task UnbanUser()
        {
            var channelState =
                await _lowLevelClient.ChannelApi.GetOrCreateChannelAsync("channel-type", "channel-id",
                    new ChannelGetOrCreateRequest());
            var channel = channelState.Channel;

            var unbanRequest = new UnbanRequest
            {
                TargetUserId = "banned-user-id",
                Id = channel.Id,
                Type = channel.Type
            };

            var unbanResponse = await _lowLevelClient.ModerationApi.UnbanUserAsync(unbanRequest);
        }

        public async Task ShadowBanUserFor24Hour()
        {
            var channelState =
                await _lowLevelClient.ChannelApi.GetOrCreateChannelAsync("channel-type", "channel-id",
                    new ChannelGetOrCreateRequest());
            var channel = channelState.Channel;

            //Shadow Ban user with id: `user-to-ban-id` for 24 hours
            var banRequest = new ShadowBanRequest
            {
                TargetUserId = "user-to-ban-id",
                Id = channel.Id,
                Type = channel.Type,
                Timeout = 60 * 24,
                Reason = "Toxic behaviour towards other users"
            };

            var shadowBanResponse = await _lowLevelClient.ModerationApi.ShadowBanUserAsync(banRequest);
        }

        public async Task RemoveUserShadowBan()
        {
            var channelState =
                await _lowLevelClient.ChannelApi.GetOrCreateChannelAsync("channel-type", "channel-id",
                    new ChannelGetOrCreateRequest());
            var channel = channelState.Channel;

            var unbanRequest = new UnbanRequest
            {
                TargetUserId = "banned-user-id",
                Id = channel.Id,
                Type = channel.Type
            };

            var unbanResponse = await _lowLevelClient.ModerationApi.RemoveUserShadowBanAsync(unbanRequest);
        }

        public async Task ShadowBanUser()
        {
            var channelState =
                await _lowLevelClient.ChannelApi.GetOrCreateChannelAsync("channel-type", "channel-id",
                    new ChannelGetOrCreateRequest());
            var channel = channelState.Channel;

            //Ban user with id: `user-to-ban-id` along with his last know IP address for 24 hours
            var shadowBanRequest = new ShadowBanRequest
            {
                TargetUserId = "user-to-ban-id",
                Id = channel.Id,
                Type = channel.Type,
                Timeout = 24 * 60,
                Reason = "Toxic behaviour towards other users",
            };

            var banResponse = await _lowLevelClient.ModerationApi.ShadowBanUserAsync(shadowBanRequest);
        }

        public async Task QueryBannedUsers()
        {
            var channelState =
                await _lowLevelClient.ChannelApi.GetOrCreateChannelAsync("channel-type", "channel-id",
                    new ChannelGetOrCreateRequest());
            var channel = channelState.Channel;

            var queryBannedUsersRequest = new QueryBannedUsersRequest
            {
                FilterConditions = new Dictionary<string, object>
                {
                    {
                        "channel_cid", new Dictionary<string, object>()
                        {
                            { "$eq", channel.Cid }
                        }
                    }
                },
                Sort = new List<SortParamRequest>
                {
                    new SortParamRequest
                    {
                        Field = "created_at",
                        Direction = -1
                    }
                }
            };

            var queryBannedUsersResponse = await _lowLevelClient.ModerationApi.QueryBannedUsersAsync(queryBannedUsersRequest);
        }

        public async Task FlagMessage()
        {
            var flagMessageResponse = await _lowLevelClient.ModerationApi.FlagMessageAsync("message-id-1");
        }

        public async Task FlagUser()
        {
            var flagUserResponse = await _lowLevelClient.ModerationApi.FlagUserAsync("user-id-1");
        }

        public async Task QueryFlaggedMessages()
        {
            var channelState =
                await _lowLevelClient.ChannelApi.GetOrCreateChannelAsync("channel-type", "channel-id",
                    new ChannelGetOrCreateRequest());
            var channel = channelState.Channel;

            var queryMessageFlagsRequest = new QueryMessageFlagsRequest
            {
                FilterConditions = new Dictionary<string, object>
                {
                    {
                        "channel_cid", new Dictionary<string, object>
                        {
                            { "$eq", channel.Cid } //Note we use CID here instead of ID
                        }
                    }
                }
            };

            var queryMessageFlagsResponse = await _lowLevelClient.ModerationApi.QueryMessageFlagsAsync(queryMessageFlagsRequest);
        }

        private IStreamChatLowLevelClient _lowLevelClient;
    }
}