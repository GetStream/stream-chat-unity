using System.Collections.Generic;
using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Core.Models;
using StreamChat.Core.Requests;

namespace Plugins.StreamChat.Samples.ClientDocs
{
    public class ModerationApiCodeSamples
    {
        public async Task MuteUser()
        {
            var muteUserRequest = new MuteUserRequest
            {
                TargetIds = new List<string> { "user-id-1" }
            };

            var muteUserResponse = await Client.ModerationApi.MuteUserAsync(muteUserRequest);
        }

        public async Task UnmuteUser()
        {
            var unmuteUserRequest = new UnmuteUserRequest
            {
                TargetIds = new List<string> { "user-id-1" }
            };

            var unmuteResponse = await Client.ModerationApi.UnmuteUserAsync(unmuteUserRequest);
        }

        public async Task BanUserForOneHour()
        {
            var channelState = await Client.ChannelApi.GetOrCreateChannelAsync("channel-type", "channel-id", new ChannelGetOrCreateRequest());
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

            var banResponse = await Client.ModerationApi.BanUserAsync(banRequest);
        }

        public async Task BanUserPermanently()
        {
            var channelState = await Client.ChannelApi.GetOrCreateChannelAsync("channel-type", "channel-id", new ChannelGetOrCreateRequest());
            var channel = channelState.Channel;

            //Ban user with id: `user-to-ban-id` permanently
            var banRequest = new BanRequest
            {
                TargetUserId = "user-to-ban-id",
                Id = channel.Id,
                Type = channel.Type,
                Reason = "Toxic behaviour towards other users",
            };

            var banResponse = await Client.ModerationApi.BanUserAsync(banRequest);
        }

        public async Task BanUserAndIPAddress()
        {
            var channelState = await Client.ChannelApi.GetOrCreateChannelAsync("channel-type", "channel-id", new ChannelGetOrCreateRequest());
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

            var banResponse = await Client.ModerationApi.BanUserAsync(banRequest);
        }

        public async Task UnbanUser()
        {
            var channelState = await Client.ChannelApi.GetOrCreateChannelAsync("channel-type", "channel-id", new ChannelGetOrCreateRequest());
            var channel = channelState.Channel;

            var unbanRequest = new UnbanRequest
            {
                TargetUserId = "banned-user-id",
                Id = channel.Id,
                Type = channel.Type
            };

            var banResponse = await Client.ModerationApi.UnbanUserAsync(unbanRequest);
        }

        public async Task ShadowBanUser()
        {
            var channelState = await Client.ChannelApi.GetOrCreateChannelAsync("channel-type", "channel-id", new ChannelGetOrCreateRequest());
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

            var banResponse = await Client.ModerationApi.ShadowBanUserAsync(shadowBanRequest);
        }

        public async Task QueryBannedUsers()
        {
            var channelState = await Client.ChannelApi.GetOrCreateChannelAsync("channel-type", "channel-id", new ChannelGetOrCreateRequest());
            var channel = channelState.Channel;

            var queryBannedUsersRequest = new QueryBannedUsersRequest
            {
                FilterConditions = new Dictionary<string, object>
                {
                    {"channel_cid", new Dictionary<string, object>()
                    {
                        { "$eq", channel.Cid }
                    }}
                },
                Sort = new List<SortParam>
                {
                    new SortParam
                    {
                        Field = "created_at",
                        Direction = -1
                    }
                }
            };

            var queryBannedUsersResponse = await Client.ModerationApi.QueryBannedUsersAsync(queryBannedUsersRequest);


        }

        private IStreamChatClient Client;
    }
}