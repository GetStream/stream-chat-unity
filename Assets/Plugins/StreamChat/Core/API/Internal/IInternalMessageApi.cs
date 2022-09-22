using System.Threading.Tasks;
using StreamChat.Core.DTO.Requests;
using StreamChat.Core.DTO.Responses;

namespace StreamChat.Core.API.Internal
{
    internal interface IInternalMessageApi
    {
        Task<MessageResponseInternalDTO> SendNewMessageAsync(string channelType, string channelId,
            SendMessageRequestInternalInternalDTO sendMessageRequest);

        Task<MessageResponseInternalDTO> UpdateMessageAsync(UpdateMessageRequestInternalInternalDTO updateMessageRequest);

        Task<MessageResponseInternalDTO> DeleteMessageAsync(string messageId, bool hard);

        Task<ReactionResponseInternalInternalDTO> SendReactionAsync(string messageId, SendReactionRequestInternalDTO sendReactionRequest);

        Task<ReactionRemovalResponseInternalDTO> DeleteReactionAsync(string messageId, string reactionType);

        Task<FileUploadResponseInternalDTO> UploadFileAsync(string channelType, string channelId,
            byte[] fileContent, string fileName);

        Task<FileDeleteResponseInternalDTO> DeleteFileAsync(string channelType, string channelId, string fileUrl);

        Task<SearchResponseInternalDTO> SearchMessagesAsync(SearchRequestInternalDTO searchRequest);
    }
}