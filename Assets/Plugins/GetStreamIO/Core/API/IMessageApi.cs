using System.Threading.Tasks;
using Plugins.GetStreamIO.Core.Requests;
using Plugins.GetStreamIO.Core.Responses;

namespace Plugins.GetStreamIO.Core.API
{
    public interface IMessageApi
    {
        Task<MessageResponse> SendNewMessageAsync(string channelType, string channelId,
            SendMessageRequest sendMessageRequest);

        Task<MessageResponse> UpdateMessageAsync(UpdateMessageRequest updateMessageRequest);

        Task<MessageResponse> DeleteMessageAsync(string messageId, bool hard);
    }
}