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

        /// <summary>
        /// <para>Uploads a file.</para>
        /// This functionality defaults to using the Stream CDN. If you would like, you can
        /// easily change the logic to upload to your own CDN of choice.
        /// </summary>
        /// <returns>The URL of the uploaded file.</returns>
        /// <remarks>https://getstream.io/chat/docs/unity/file_uploads/?language=unity</remarks>
        Task<FileUploadResponse> UploadFileAsync(string channelType, string channelId,
            byte[] fileContent, string fileName);

        /// <summary>
        /// <para>Deletes a file.</para>
        /// This functionality defaults to using the Stream CDN. If you would like, you can
        /// easily change the logic to upload to your own CDN of choice.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/file_uploads/?language=unity</remarks>
        Task<FileDeleteResponse> DeleteFileAsync(string channelType, string channelId, string fileUrl);
    }
}