using System.Threading.Tasks;
using StreamChat.Core.DTO.Requests;
using StreamChat.Core.DTO.Responses;

namespace StreamChat.Core.API.Internal
{
    internal interface IInternalMessageApi
    {
        Task<MessageResponseDTO> SendNewMessageAsync(string channelType, string channelId,
            SendMessageRequestDTO sendMessageRequest);

        Task<MessageResponseDTO> UpdateMessageAsync(UpdateMessageRequestDTO updateMessageRequest);

        Task<MessageResponseDTO> DeleteMessageAsync(string messageId, bool hard);

        Task<ReactionResponseDTO> SendReactionAsync(string messageId, SendReactionRequestDTO sendReactionRequest);

        Task<ReactionRemovalResponseDTO> DeleteReactionAsync(string messageId, string reactionType);

        Task<FileUploadResponseDTO> UploadFileAsync(string channelType, string channelId,
            byte[] fileContent, string fileName);

        Task<FileDeleteResponseDTO> DeleteFileAsync(string channelType, string channelId, string fileUrl);

        Task<SearchResponseDTO> SearchMessagesAsync(SearchRequestDTO searchRequest);
    }
}