using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.Responses
{
    /// <summary>
    /// A single hit from <see cref="IStreamChatClient.SearchMessagesAsync"/>.
    ///
    /// <para>
    /// <see cref="Message"/> and <see cref="Channel"/> share identity with the rest of the SDK:
    /// if you already hold a reference to the same message or channel (e.g. from
    /// <see cref="IStreamChatClient.QueryChannelsAsync"/> or any channel's
    /// <see cref="IStreamChannel.Messages"/>), the search hit returns that same reference.
    /// Changes coming from the server are reflected on the single instance, so updates observed
    /// through the search hit are also visible to every other reference to it.
    /// </para>
    ///
    /// <para>
    /// By default (<see cref="Requests.StreamSearchMessagesRequest.WatchResultChannels"/> = <c>true</c>)
    /// the result <see cref="Channel"/> is automatically watched, so both <see cref="Message"/> and
    /// <see cref="Channel"/> receive realtime updates exactly like objects obtained from
    /// <see cref="IStreamChatClient.QueryChannelsAsync"/>. If you opted out of that, neither receives
    /// updates until you call <see cref="IStreamChannel.WatchAsync"/> on <see cref="Channel"/>; see
    /// <see cref="IStreamChannel.IsWatched"/> / <see cref="IStreamMessage.IsWatched"/>.
    /// </para>
    /// </summary>
    public sealed class StreamSearchMessageResult
    {
        /// <summary>
        /// The matching message. Receives realtime updates (reactions, edits, deletions, ...)
        /// whenever its parent <see cref="Channel"/> is watched. See
        /// <see cref="IStreamMessage.IsWatched"/>.
        /// </summary>
        public IStreamMessage Message { get; internal set; }

        /// <summary>
        /// The channel the message belongs to. By default it is watched and appears in
        /// <see cref="IStreamChatClient.WatchedChannels"/>, receiving realtime updates. When the
        /// request was issued with
        /// <see cref="Requests.StreamSearchMessagesRequest.WatchResultChannels"/> = <c>false</c>,
        /// this channel does not receive updates - call <see cref="IStreamChannel.WatchAsync"/>
        /// on it to start receiving them on this same instance.
        /// </summary>
        public IStreamChannel Channel { get; internal set; }
    }
}
