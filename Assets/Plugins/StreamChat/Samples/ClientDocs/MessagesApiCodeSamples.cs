using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Core.Requests;

namespace Plugins.StreamChat.Samples.ClientDocs
{
    /// <summary>
    /// Code samples for Channels sections: https://getstream.io/chat/docs/unity/send_message/?language=unity
    /// </summary>
    public class MessagesApiCodeSamples
    {
        private IStreamChatClient Client;

        public async Task SendMessage()
        {
            var sendMessageRequest = new SendMessageRequest
            {
                Message = new MessageRequest
                {
                    Text = "message content"
                }
            };

            var messageResponse = await Client.MessageApi.SendNewMessageAsync(channelType: "messaging",
                channelId: "channel-id-1", sendMessageRequest);
        }

        public async Task UpdateMessage()
        {
            await Client.MessageApi.UpdateMessageAsync(new UpdateMessageRequest
            {
                Message = new MessageRequest
                {
                    Id = "message-id-1",
                    Text = "updated message content"
                }
            });
        }

        public async Task DeleteMessage()
        {
            var messageResponse = await Client.MessageApi.DeleteMessageAsync(messageId: "message-id-1", hard: false);
        }

        public async Task OpenGraphScraper()
        {
            var sendMessageRequest = new SendMessageRequest
            {
                Message = new MessageRequest
                {
                    Text = "Check this bear out https://imgur.com/r/bears/4zmGbMN"
                }
            };

            var messageResponse = await Client.MessageApi.SendNewMessageAsync(channelType: "messaging",
                channelId: "channel-id-1", sendMessageRequest);
        }

        public async Task SendReaction()
        {
            var reactionResponse = await Client.MessageApi.SendReactionAsync(messageId: "message-id-1",
                new SendReactionRequest
                {
                    Reaction = new ReactionRequest
                    {
                        Type = "like",
                    }
                });
        }

        public async Task RemoveReaction()
        {
            var reactionRemovalResponse =
                await Client.MessageApi.DeleteReactionAsync(messageId: "message-id-1", reactionType: "like");
        }

        public async Task SendSilentMessage()
        {
            var sendMessageRequest = new SendMessageRequest
            {
                Message = new MessageRequest
                {
                    Text = "message content",
                    Silent = true
                },
            };

            var messageResponse = await Client.MessageApi.SendNewMessageAsync(channelType: "messaging",
                channelId: "channel-id-1", sendMessageRequest);
        }
    }
}