using System.Threading.Tasks;
using StreamChat.Core.Requests;
using StreamChat.Core.Responses;

namespace StreamChat.Core.API
{
    /// <summary>
    /// A client that can be used to retrieve, create and alter messages of a Stream Chat application.
    /// </summary>
    /// <remarks>https://getstream.io/chat/docs/unity/send_message/?language=unity</remarks>
    public interface IMessageApi
    {
        /// <summary>
        /// <para>Sends a message to a channel.</para>
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/send_message/?language=unity</remarks>
        Task<MessageResponse> SendNewMessageAsync(string channelType, string channelId,
            SendMessageRequest sendMessageRequest);

        /// <summary>
        /// <para>Fully overwrites a message.</para>
        /// For partial update, use <see cref="UpdateMessagePartialAsync"/> method.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/send_message/?language=unity#update-a-message</remarks>
        Task<MessageResponse> UpdateMessageAsync(UpdateMessageRequest updateMessageRequest);

        /// <summary>
        /// Deletes a message.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/send_message/?language=unity</remarks>
        Task<MessageResponse> DeleteMessageAsync(string messageId, bool hard);

        /// <summary>
        /// <para>Sends a new reaction to a given message.</para>
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/send_reaction/?language=unity</remarks>
        Task<ReactionResponse> SendReactionAsync(string messageId, SendReactionRequest sendReactionRequest);

        /// <summary>
        /// Deletes a reaction from a given message.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/send_reaction/?language=unity#removing-a-reaction</remarks>
        Task<ReactionRemovalResponse> DeleteReactionAsync(string messageId, string reactionType);
    }
}