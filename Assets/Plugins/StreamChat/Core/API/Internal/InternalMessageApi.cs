﻿using System.Net.Http;
using System.Threading.Tasks;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.InternalDTO.Responses;
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

        public Task<MessageResponseInternalDTO> SendNewMessageAsync(string channelType, string channelId,
            SendMessageRequestInternalInternalDTO sendMessageRequest)
        {
            var endpoint = MessageEndpoints.SendMessage(channelType, channelId);
            return Post<SendMessageRequestInternalInternalDTO, MessageResponseInternalDTO>(endpoint, sendMessageRequest);
        }

        public Task<MessageResponseInternalDTO> UpdateMessageAsync(UpdateMessageRequestInternalInternalDTO updateMessageRequest)
        {
            var endpoint = MessageEndpoints.UpdateMessage(updateMessageRequest.Message.Id);
            return Post<UpdateMessageRequestInternalInternalDTO, MessageResponseInternalDTO>(endpoint, updateMessageRequest);
        }

        public Task<MessageResponseInternalDTO> DeleteMessageAsync(string messageId, bool hard)
        {
            var endpoint = MessageEndpoints.DeleteMessage(messageId);
            var parameters = QueryParameters.Default.Append("hard", hard);

            return Delete<MessageResponseInternalDTO>(endpoint, parameters);
        }

        public Task<ReactionResponseInternalInternalDTO> SendReactionAsync(string messageId, SendReactionRequestInternalDTO sendReactionRequest)
        {
            var endpoint = MessageEndpoints.SendReaction(messageId);
            return Post<SendReactionRequestInternalDTO, ReactionResponseInternalInternalDTO>(endpoint, sendReactionRequest);
        }

        public Task<ReactionRemovalResponseInternalDTO> DeleteReactionAsync(string messageId, string reactionType)
        {
            var endpoint = MessageEndpoints.DeleteReaction(messageId, reactionType);
            return Delete<ReactionRemovalResponseInternalDTO>(endpoint);
        }

        public Task<FileUploadResponseInternalDTO> UploadFileAsync(string channelType, string channelId,
            byte[] fileContent, string fileName)
        {
            var endpoint = $"/channels/{channelType}/{channelId}/file";

            var body = new MultipartFormDataContent();
            body.Add(new ByteArrayContent(fileContent), "file", fileName);

            return Post<FileUploadResponseInternalDTO>(endpoint, body);
        }

        public Task<FileDeleteResponseInternalDTO> DeleteFileAsync(string channelType, string channelId, string fileUrl)
        {
            var endpoint = $"channels/{channelType}/{channelId}/file";
            var parameters = QueryParameters.Default.Append("url", fileUrl);

            return Delete<FileDeleteResponseInternalDTO>(endpoint, parameters);
        }

        public Task<ImageUploadResponseInternalDTO> UploadImageAsync(string channelType, string channelId,
            byte[] fileContent, string fileName)
        {
            var endpoint = $"/channels/{channelType}/{channelId}/image";

            var body = new MultipartFormDataContent();
            body.Add(new ByteArrayContent(fileContent), "file", fileName);

            return Post<ImageUploadResponseInternalDTO>(endpoint, body);
        }

        public Task<SearchResponseInternalDTO> SearchMessagesAsync(SearchRequestInternalDTO searchRequest)
            => Get<SearchRequestInternalDTO, SearchResponseInternalDTO>("/search", searchRequest);
    }
}