using System;
using System.Threading.Tasks;
using StreamChat.Core.API.Internal;
using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Helpers;
using StreamChat.Core.Requests;
using StreamChat.Core.Responses;

namespace StreamChat.Core.API
{
    internal class MessageApi : IMessageApi
    {
        public MessageApi(IInternalMessageApi internalMessageApi)
        {
            _internalMessageApi = internalMessageApi ?? throw new ArgumentNullException(nameof(internalMessageApi));
        }

        public async Task<MessageResponse> SendNewMessageAsync(string channelType, string channelId,
            SendMessageRequest sendMessageRequest)
        {
            var dto = await _internalMessageApi.SendNewMessageAsync(channelType, channelId,
                sendMessageRequest.TrySaveToDto());
            return dto.ToDomain<MessageResponseDTO, MessageResponse>();
        }

        public async Task<MessageResponse> UpdateMessageAsync(UpdateMessageRequest updateMessageRequest)
        {
            var dto = await _internalMessageApi.UpdateMessageAsync(updateMessageRequest.TrySaveToDto());
            return dto.ToDomain<MessageResponseDTO, MessageResponse>();
        }

        public async Task<MessageResponse> DeleteMessageAsync(string messageId, bool hard)
        {
            var dto = await _internalMessageApi.DeleteMessageAsync(messageId, hard);
            return dto.ToDomain<MessageResponseDTO, MessageResponse>();
        }

        public async Task<ReactionResponse> SendReactionAsync(string messageId, SendReactionRequest sendReactionRequest)
        {
            var dto = await _internalMessageApi.SendReactionAsync(messageId, sendReactionRequest.TrySaveToDto());
            return dto.ToDomain<ReactionResponseDTO, ReactionResponse>();
        }

        public async Task<ReactionRemovalResponse> DeleteReactionAsync(string messageId, string reactionType)
        {
            var dto = await _internalMessageApi.DeleteReactionAsync(messageId, reactionType);
            return dto.ToDomain<ReactionRemovalResponseDTO, ReactionRemovalResponse>();
        }

        public async Task<FileUploadResponse> UploadFileAsync(string channelType, string channelId,
            byte[] fileContent, string fileName)
        {
            var dto = await _internalMessageApi.UploadFileAsync(channelType, channelId, fileContent, fileName);
            return dto.ToDomain<FileUploadResponseDTO, FileUploadResponse>();
        }

        public async Task<FileDeleteResponse> DeleteFileAsync(string channelType, string channelId, string fileUrl)
        {
            var dto = await _internalMessageApi.DeleteFileAsync(channelType, channelId, fileUrl);
            return dto.ToDomain<FileDeleteResponseDTO, FileDeleteResponse>();
        }

        public async Task<SearchResponse> SearchMessagesAsync(SearchRequest searchRequest)
        {
            var dto = await _internalMessageApi.SearchMessagesAsync(searchRequest.TrySaveToDto());
            return dto.ToDomain<SearchResponseDTO, SearchResponse>();
        }

        private readonly IInternalMessageApi _internalMessageApi;
    }
}