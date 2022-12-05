using System.Threading.Tasks;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.InternalDTO.Responses;

namespace StreamChat.Core.LowLevelClient.API.Internal
{
    internal interface IInternalMessageApi
    {
        Task<MessageResponseInternalDTO> SendNewMessageAsync(string channelType, string channelId,
            SendMessageRequestInternalInternalDTO sendMessageRequest);

        Task<MessageResponseInternalDTO> UpdateMessageAsync(UpdateMessageRequestInternalInternalDTO updateMessageRequest);

        Task<MessageResponseInternalDTO> UpdateMessagePartialAsync(string messageId,
            UpdateMessagePartialRequestInternalDTO updateMessagePartialRequest);

        Task<MessageResponseInternalDTO> DeleteMessageAsync(string messageId, bool hard);

        Task<ReactionResponseInternalInternalDTO> SendReactionAsync(string messageId, SendReactionRequestInternalDTO sendReactionRequest);

        Task<ReactionRemovalResponseInternalDTO> DeleteReactionAsync(string messageId, string reactionType);

        Task<FileUploadResponseInternalDTO> UploadFileAsync(string channelType, string channelId,
            byte[] fileContent, string fileName);

        Task<FileDeleteResponseInternalDTO> DeleteFileAsync(string channelType, string channelId, string fileUrl);

        Task<ImageUploadResponseInternalDTO> UploadImageAsync(string channelType, string channelId,
            byte[] fileContent, string fileName);

        Task<SearchResponseInternalDTO> SearchMessagesAsync(SearchRequestInternalDTO searchRequest);
    }
}