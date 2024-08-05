using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Samples
{
    internal sealed class TypingIndicatorsCodeSamples
    {
        public async Task SendStartStopTypingEvents()
        {
            IStreamChannel channel = null;

// Send typing started event
            await channel.SendTypingStartedEventAsync();

// Send typing stopped event
            await channel.SendTypingStoppedEventAsync();
        }

        public async Task ReceivingTypingEvents()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, "channel-id");
            channel.UserStartedTyping += OnUserStartedTyping;
            channel.UserStoppedTyping += OnUserStoppedTyping;
        }

        private void OnUserStartedTyping(IStreamChannel channel, IStreamUser user)
        {
        }

        private void OnUserStoppedTyping(IStreamChannel channel, IStreamUser user)
        {
        }

        private IStreamChatClient Client { get; } = StreamChatClient.CreateDefaultClient();
    }
}