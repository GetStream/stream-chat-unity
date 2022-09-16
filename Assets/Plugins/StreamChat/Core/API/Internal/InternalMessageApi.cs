using System.Net.Http;
using System.Threading.Tasks;
using StreamChat.Core.DTO.Requests;
using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Web;
using StreamChat.Libs.Http;
using StreamChat.Libs.Logs;
using StreamChat.Libs.Serialization;

namespace StreamChat.Core.API.Internal
{
    internal class InternalMessageApi : InternalApiClientBase, IInternalMessageApi
    {
        public InternalMessageApi(IHttpClient httpClient, ISerializer serializer, ILogs logs,
            IRequestUriFactory requestUriFactory)
            : base(httpClient, serializer, logs, requestUriFactory)
        {
        }

        public Task<MessageResponseDTO> SendNewMessageAsync(string channelType, string channelId,
            SendMessageRequestDTO sendMessageRequest)
        {
            var endpoint = MessageEndpoints.SendMessage(channelType, channelId);
            return Post<SendMessageRequestDTO, MessageResponseDTO>(endpoint, sendMessageRequest);
        }

        public Task<MessageResponseDTO> UpdateMessageAsync(UpdateMessageRequestDTO updateMessageRequest)
        {
            var endpoint = MessageEndpoints.UpdateMessage(updateMessageRequest.Message.Id);
            return Post<UpdateMessageRequestDTO, MessageResponseDTO>(endpoint, updateMessageRequest);
        }

        public Task<MessageResponseDTO> DeleteMessageAsync(string messageId, bool hard)
        {
            var endpoint = MessageEndpoints.DeleteMessage(messageId);
            var parameters = QueryParameters.Default.Append("hard", hard);

            return Delete<MessageResponseDTO>(endpoint, parameters);
        }

        public Task<ReactionResponseDTO> SendReactionAsync(string messageId, SendReactionRequestDTO sendReactionRequest)
        {
            var endpoint = MessageEndpoints.SendReaction(messageId);
            return Post<SendReactionRequestDTO, ReactionResponseDTO>(endpoint, sendReactionRequest);
        }

        public Task<ReactionRemovalResponseDTO> DeleteReactionAsync(string messageId, string reactionType)
        {
            var endpoint = MessageEndpoints.DeleteReaction(messageId, reactionType);
            return Delete<ReactionRemovalResponseDTO>(endpoint);
        }

        public Task<FileUploadResponseDTO> UploadFileAsync(string channelType, string channelId,
            byte[] fileContent, string fileName)
        {
            var endpoint = $"/channels/{channelType}/{channelId}/file";

            var body = new MultipartFormDataContent();
            body.Add(new ByteArrayContent(fileContent), "file", fileName);

            return Post<FileUploadResponseDTO>(endpoint, body);
        }

        public Task<FileDeleteResponseDTO> DeleteFileAsync(string channelType, string channelId, string fileUrl)
        {
            var endpoint = $"channels/{channelType}/{channelId}/file";
            var parameters = QueryParameters.Default.Append("url", fileUrl);

            return Delete<FileDeleteResponseDTO>(endpoint, parameters);
        }

        public Task<SearchResponseDTO> SearchMessagesAsync(SearchRequestDTO searchRequest)
            => Get<SearchRequestDTO, SearchResponseDTO>("/search", searchRequest);
    }
}