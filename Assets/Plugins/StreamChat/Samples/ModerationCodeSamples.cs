using System;
using System.Linq;
using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Core.Requests;

namespace StreamChat.Samples
{
    internal sealed class ModerationCodeSamples
    {
        /// <summary>
        /// https://getstream.io/chat/docs/unity/moderation/?language=unity#flag
        /// </summary>
        public async Task Flag()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");
            var message = channel.Messages.First();
            var channelMember = channel.Members.First();

// Flag a message
            await message.FlagAsync();

// Flag a user
            await channelMember.User.FlagAsync();
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/moderation/?language=unity#query-message-flags
        /// </summary>
        public async Task QueryMessageFlags()
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/moderation/?language=unity#mutes
        /// </summary>
        public async Task Mutes()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");
            var channelMember = channel.Members.First();

// Mute a user
            await channelMember.User.MuteAsync();

// Unmute previously muted user
            await channelMember.User.UnmuteAsync();

// Mute a channel
            await channel.MuteChannelAsync();

// Mute previously muted channel
            await channel.UnmuteChannelAsync();
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/moderation/?language=unity#ban
        /// </summary>
        public async Task Ban()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");

// Dummy example to get IStreamUser to ban
            var user = channel.Messages.First().User;

// Dummy example to get IStreamUser to ban
            var channelMember = channel.Members.First();

// Ban a user permanently from this channel permanently
            await channel.BanUserAsync(user);

// Use any combination of the optional parameters: reason, timeoutMinutes, isIpBan

// Ban a user from this channel for 2 hours with a reason
            await channel.BanUserAsync(user, "You got banned for 2 hours for toxic behaviour.", 120);

// Ban a user IP from this channel for 2 hours without a reason
            await channel.BanUserAsync(user, timeoutMinutes: 120, isIpBan: true);

// Ban a member from this channel permanently
            await channel.BanMemberAsync(channelMember);
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/moderation/?language=unity#query-banned-users
        /// </summary>
        public async Task QueryBannedUsers()
        {
// Get users banned in the last 24 hours
            var request = new StreamQueryBannedUsersRequest
            {
                CreatedAtAfterOrEqual = new DateTimeOffset().AddHours(-24),
                Limit = 30,
                Offset = 0,
            };

            var bannedUsersInfo = await Client.QueryBannedUsersAsync(request);
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/moderation/?language=unity#query-bans-endpoint
        /// </summary>
        public async Task QueryBansEndpoint()
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/moderation/?language=unity#shadow-ban
        /// </summary>
        public async Task ShadowBan()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");

// Dummy example to get IStreamUser to ban
            var user = channel.Messages.First().User;

// Dummy example to get IStreamUser to ban
            var channelMember = channel.Members.First();

// Shadow Ban a user from this channel permanently
            await channel.ShadowBanUserAsync(user);

// Shadow Ban a member from this channel
            await channel.ShadowBanMemberAsync(channelMember);

// Use any combination of optional parameters: reason, timeoutMinutes, isIpBan

// Shadow Ban a member from this channel permanently
            await channel.ShadowBanMemberAsync(channelMember);

// Shadow Ban a member from this channel for 2 hours with a reason
            await channel.ShadowBanMemberAsync(channelMember, "Banned for 2 hours for toxic behaviour.", 120);

// Shadow Ban a member IP from this channel for 2 hours without a reason
            await channel.ShadowBanMemberAsync(channelMember, timeoutMinutes: 120, isIpBan: true);
        }

        private IStreamChatClient Client { get; } = StreamChatClient.CreateDefaultClient();
    }
}