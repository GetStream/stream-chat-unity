using System.Threading.Tasks;
using StreamChat.Core.Requests;
using StreamChat.Core.Responses;

namespace StreamChat.Core.API
{
    public interface IMessageApi
    {
        Task<MessageResponse> SendNewMessageAsync(string channelType, string channelId,
            SendMessageRequest sendMessageRequest);

        Task<MessageResponse> UpdateMessageAsync(UpdateMessageRequest updateMessageRequest);

        Task<MessageResponse> DeleteMessageAsync(string messageId, bool hard);

        Task<ReactionResponse> SendReactionAsync(string messageId, SendReactionRequest sendReactionRequest);

        Task<ReactionRemovalResponse> DeleteReactionAsync(string messageId, string reactionType);
    }
}