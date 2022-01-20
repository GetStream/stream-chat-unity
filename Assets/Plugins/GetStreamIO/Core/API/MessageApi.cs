using System.Threading.Tasks;
using GetStreamIO.Core.DTO.Requests;
using GetStreamIO.Core.DTO.Responses;
using Plugins.GetStreamIO.Core.Requests;
using Plugins.GetStreamIO.Core.Responses;
using Plugins.GetStreamIO.Core.Web;
using Plugins.GetStreamIO.Libs.Http;
using Plugins.GetStreamIO.Libs.Logs;
using Plugins.GetStreamIO.Libs.Serialization;

namespace Plugins.GetStreamIO.Core.API
{
    public class MessageApi : ApiClientBase, IMessageApi
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
            var parameters = QueryParameters.Create().Append("hard", hard);

            return Delete<MessageResponse, MessageResponseDTO>(endpoint, parameters);
        }
    }
}