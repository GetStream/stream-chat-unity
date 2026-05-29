using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.Responses
{
    /// <summary>
    /// A single result from <see cref="IStreamChatClient.SearchMessagesAsync"/>, holding the
    /// matching <see cref="Message"/> and the <see cref="Channel"/> it belongs to.
    ///
    /// <para>
    /// By default (<see cref="Requests.StreamSearchMessagesRequest.WatchResultChannels"/> = <c>true</c>)
    /// the result <see cref="Channel"/> is watched, so both <see cref="Message"/> and
    /// <see cref="Channel"/> receive realtime updates just like channels and messages obtained from
    /// <see cref="IStreamChatClient.QueryChannelsAsync"/>. If you opted out of that, neither receives
    /// updates until you call <see cref="IStreamChannel.WatchAsync"/> on <see cref="Channel"/>; see
    /// <see cref="IStreamChannel.IsWatched"/> / <see cref="IStreamMessage.IsWatched"/>.
    /// </para>
    /// </summary>
    public sealed class StreamSearchMessageResult
    {
        /// <summary>
        /// The matching message. Receives realtime updates (reactions, edits, deletions, etc.)
        /// while its parent <see cref="Channel"/> is watched. See <see cref="IStreamMessage.IsWatched"/>.
        /// </summary>
        public IStreamMessage Message { get; internal set; }

        /// <summary>
        /// The channel the message belongs to. By default it is watched, appears in
        /// <see cref="IStreamChatClient.WatchedChannels"/> and receives realtime updates. When the
        /// request was issued with
        /// <see cref="Requests.StreamSearchMessagesRequest.WatchResultChannels"/> = <c>false</c>,
        /// this channel does not receive updates - call <see cref="IStreamChannel.WatchAsync"/>
        /// on it to start receiving them.
        /// </summary>
        public IStreamChannel Channel { get; internal set; }
    }
}
