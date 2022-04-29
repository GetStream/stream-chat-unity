using System.Net.Http;
using System.Threading.Tasks;
using StreamChat.Core.DTO.Requests;
using StreamChat.Core.DTO.Responses;
using StreamChat.Libs.Http;
using StreamChat.Libs.Logs;
using StreamChat.Libs.Serialization;
using StreamChat.Core.Requests;
using StreamChat.Core.Responses;
using StreamChat.Core.Web;

namespace StreamChat.Core.API
{
    internal class MessageApi : ApiClientBase, IMessageApi
    {
        public MessageApi(IHttpClient httpClient, ISerializer serializer, ILogs logs,
            IRequestUriFactory requestUriFactory)
            : base(httpClient, serializer, logs, requestUriFactory)
        {
        }

        public Task<MessageResponse> SendNewMessageAsync(string channelType, string channelId,
            SendMessageRequest sendMessageRequest)
        {
            var endpoint = MessageEndpoints.SendMessage(channelType, channelId);

            return Post<SendMessageRequest, SendMessageRequestDTO, MessageResponse, MessageResponseDTO>(endpoint,
                sendMessageRequest);
        }

        public Task<MessageResponse> UpdateMessageAsync(UpdateMessageRequest updateMessageRequest)
        {
            var endpoint = MessageEndpoints.UpdateMessage(updateMessageRequest.Message.Id);

            return Post<UpdateMessageRequest, UpdateMessageRequestDTO, MessageResponse, MessageResponseDTO>(endpoint,
                updateMessageRequest);
        }

        public Task<MessageResponse> DeleteMessageAsync(string messageId, bool hard)
        {
            var endpoint = MessageEndpoints.DeleteMessage(messageId);
            var parameters = QueryParameters.Default.Append("hard", hard);

            return Delete<MessageResponse, MessageResponseDTO>(endpoint, parameters);
        }

        public Task<ReactionResponse> SendReactionAsync(string messageId, SendReactionRequest sendReactionRequest)
        {
            var endpoint = MessageEndpoints.SendReaction(messageId);

            return Post<SendReactionRequest, SendReactionRequestDTO, ReactionResponse, ReactionResponseDTO>(endpoint,
                sendReactionRequest);
        }

        public Task<ReactionRemovalResponse> DeleteReactionAsync(string messageId, string reactionType)
        {
            var endpoint = MessageEndpoints.DeleteReaction(messageId, reactionType);

            return Delete<ReactionRemovalResponse, ReactionRemovalResponseDTO>(endpoint);
        }

        public async Task<FileUploadResponse> UploadFileAsync(string channelType, string channelId,
            byte[] fileContent, string fileName)
        {
            var endpoint = $"/channels/{channelType}/{channelId}/file";

            var body = new MultipartFormDataContent();
            body.Add(new ByteArrayContent(fileContent), "file", fileName);

            return await Post<FileUploadResponse, FileUploadResponseDTO>(endpoint, body);
        }

        public Task<FileDeleteResponse> DeleteFileAsync(string channelType, string channelId, string fileUrl)
        {
            var endpoint = $"channels/{channelType}/{channelId}/file";
            var parameters = QueryParameters.Default.Append("url", fileUrl);

            return Delete<FileDeleteResponse, FileDeleteResponseDTO>(endpoint, parameters);
        }
    }
}