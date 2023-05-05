using System;
using System.Threading.Tasks;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.LowLevelClient.API.Internal;
using StreamChat.Core.LowLevelClient.Requests;
using StreamChat.Core.LowLevelClient.Responses;

namespace StreamChat.Core.LowLevelClient.API
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
            return dto.ToDomain<MessageResponseInternalDTO, MessageResponse>();
        }

        public async Task<MessageResponse> UpdateMessageAsync(UpdateMessageRequest updateMessageRequest)
        {
            var dto = await _internalMessageApi.UpdateMessageAsync(updateMessageRequest.TrySaveToDto());
            return dto.ToDomain<MessageResponseInternalDTO, MessageResponse>();
        }

        public async Task<MessageResponse> UpdateMessagePartialAsync(string messageId, UpdateMessagePartialRequest updateMessagePartialRequest)
        {
            var dto = await _internalMessageApi.UpdateMessagePartialAsync(messageId, updateMessagePartialRequest.TrySaveToDto());
            return dto.ToDomain<MessageResponseInternalDTO, MessageResponse>();
        }

        public async Task<MessageResponse> DeleteMessageAsync(string messageId, bool hard)
        {
            var dto = await _internalMessageApi.DeleteMessageAsync(messageId, hard);
            return dto.ToDomain<MessageResponseInternalDTO, MessageResponse>();
        }

        public async Task<ReactionResponse> SendReactionAsync(string messageId, SendReactionRequest sendReactionRequest)
        {
            var dto = await _internalMessageApi.SendReactionAsync(messageId, sendReactionRequest.TrySaveToDto());
            return dto.ToDomain<ReactionResponseInternalDTO, ReactionResponse>();
        }

        public async Task<ReactionRemovalResponse> DeleteReactionAsync(string messageId, string reactionType)
        {
            var dto = await _internalMessageApi.DeleteReactionAsync(messageId, reactionType);
            return dto.ToDomain<ReactionRemovalResponseInternalDTO, ReactionRemovalResponse>();
        }

        public async Task<FileUploadResponse> UploadFileAsync(string channelType, string channelId,
            byte[] fileContent, string fileName)
        {
            var dto = await _internalMessageApi.UploadFileAsync(channelType, channelId, fileContent, fileName);
            return dto.ToDomain<FileUploadResponseInternalDTO, FileUploadResponse>();
        }

        public async Task<FileDeleteResponse> DeleteFileAsync(string channelType, string channelId, string fileUrl)
        {
            var dto = await _internalMessageApi.DeleteFileAsync(channelType, channelId, fileUrl);
            return dto.ToDomain<FileDeleteResponseInternalDTO, FileDeleteResponse>();
        }

        public async Task<ImageUploadResponse> UploadImageAsync(string channelType, string channelId,
            byte[] fileContent, string fileName)
        {
            var dto = await _internalMessageApi.UploadImageAsync(channelType, channelId, fileContent, fileName);
            return dto.ToDomain<ImageUploadResponseInternalDTO, ImageUploadResponse>();
        }

        public async Task<SearchResponse> SearchMessagesAsync(SearchRequest searchRequest)
        {
            var dto = await _internalMessageApi.SearchMessagesAsync(searchRequest.TrySaveToDto());
            return dto.ToDomain<SearchResponseInternalDTO, SearchResponse>();
        }

        private readonly IInternalMessageApi _internalMessageApi;
    }
}