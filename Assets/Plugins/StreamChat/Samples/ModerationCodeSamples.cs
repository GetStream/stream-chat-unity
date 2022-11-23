using System.Linq;
using System.Threading.Tasks;
using StreamChat.Core;

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
        }
        
        /// <summary>
        /// https://getstream.io/chat/docs/unity/moderation/?language=unity#ban
        /// </summary>
        public async Task Ban()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");
            
            // Dummy example to get IStreamUser to ban
            var streamUser = channel.Messages.First().User;

            // Ban a user
            //StreamTOdo: IMPLEMENT ban user
        }
        
        /// <summary>
        /// https://getstream.io/chat/docs/unity/moderation/?language=unity#query-banned-users
        /// </summary>
        public async Task QueryBannedUsers()
        {
            
        }
        
        /// <summary>
        /// https://getstream.io/chat/docs/unity/moderation/?language=unity#query-bans-endpoint
        /// </summary>
        public async Task QueryBansEndpoint()
        {
            
        }
        
        /// <summary>
        /// https://getstream.io/chat/docs/unity/moderation/?language=unity#shadow-ban
        /// </summary>
        public async Task ShadowBan()
        {
            
        }
        
        private IStreamChatClient Client { get; } = StreamChatClient.CreateDefaultClient();
    }
}