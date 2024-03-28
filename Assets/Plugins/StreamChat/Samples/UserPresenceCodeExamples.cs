using System.Linq;
using StreamChat.Core;

namespace StreamChat.Samples
{
    internal class UserPresenceCodeExamples
    {
        public async void UserObject()
        {
            // Get a channel
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, "my-channel-id");

            // Members are users belonging to this channel
            var member = channel.Members.First();

            // Each member object contains a user object. A single user can be a member of many channels
            var user = member.User;

            var message = channel.Messages.First();

            // Each message contains the author user object
            var user2 = message.User;

            // Presence related fields on a user object
            var isOnline = user.Online;
            var lastActive = user.LastActive;
        }

        public async void Invisible()
        {
            // Get local user object
            var localUserData = await Client.ConnectUserAsync("api-key", "user-id", "user-token");

            // Or like this
            var localUserData2 = Client.LocalUserData;

            // Get local user object
            var localUser = localUserData.User;

            // Check local user invisibility status
            var isInvisible = localUser.Invisible;

            // Mark invisible
            await localUser.MarkInvisibleAsync();

            // Mark visible
            await localUser.MarkVisibleAsync();
        }

        public void SetVisibilityOnConnect()
        {
            //StreamTodo: implement https://getstream.io/chat/docs/unity/presence_format/?language=csharp#invisible (second example)
            
            // Will be implemented soon, please send a support ticket if you need this feature
        }

        public async void ListeningForPresenceEvents()
        {
            // Get a channel
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, "my-channel-id");

            // Members are users belonging to this channel
            var member = channel.Members.First();

            // Each member object contains a user object. A single user can be a member of many channels
            var user = member.User;

            // Each user object exposes the PresenceChange event that will trigger when Online status changes
            user.PresenceChanged += (userObj, isOnline, isActive) =>
            {

            };
        }
        
        
        private IStreamChatClient Client { get; } = StreamChatClient.CreateDefaultClient();
    }
}