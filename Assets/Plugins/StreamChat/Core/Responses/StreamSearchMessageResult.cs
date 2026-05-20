using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.Responses
{
    /// <summary>
    /// A single hit from <see cref="IStreamChatClient.SearchMessagesAsync"/>.
    ///
    /// Both <see cref="Message"/> and <see cref="Channel"/> are stateful, cache-tracked instances:
    /// if the same message/channel is already in the cache (because the channel is watched, the
    /// message was loaded as a reply, etc.) the same object reference is returned here.
    /// </summary>
    public sealed class StreamSearchMessageResult
    {
        /// <summary>
        /// The matching message. Updated by realtime events the same way as any other
        /// stateful message returned by the SDK.
        /// </summary>
        public IStreamMessage Message { get; internal set; }

        /// <summary>
        /// The channel the message belongs to. May be the same instance as one in
        /// <see cref="IStreamChatClient.WatchedChannels"/> if the channel is already watched.
        /// </summary>
        /// <remarks>
        /// The channel object is cached but is not automatically watched (no WS subscription)
        /// unless <see cref="Requests.StreamSearchMessagesRequest.WatchResultChannels"/> is set
        /// to <c>true</c>.
        /// </remarks>
        public IStreamChannel Channel { get; internal set; }
    }
}
