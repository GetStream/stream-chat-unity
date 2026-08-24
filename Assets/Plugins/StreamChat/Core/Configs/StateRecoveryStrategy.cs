namespace StreamChat.Core.Configs
{
    /// <summary>
    /// How <see cref="IStreamChatClient"/> restores local state after the websocket reconnects.
    /// Set this on <see cref="IStreamClientConfig.StateRecoveryStrategy"/>.
    /// </summary>
    /// <remarks>
    /// After a reconnect, the server always drops the old watches.
    /// <see cref="ReplayEvents"/> and <see cref="BatchStateUpdate"/> start watching those channels again.
    /// <see cref="Disabled"/> does not - you must do that yourself.
    /// </remarks>
    public enum StateRecoveryStrategy
    {
        /// <summary>
        /// Default.
        ///
        /// Missed events are replayed as normal events
        /// (<see cref="StatefulModels.IStreamChannel.MessageReceived"/>,
        /// <see cref="StatefulModels.IStreamChannel.ReactionAdded"/>, and so on).
        /// The client also refreshes those channels and starts watching them again.
        ///
        /// Use this if your UI updates from per-event callbacks.
        /// After a long disconnect on a busy channel this can replay many events at once
        /// and cause a hitch.
        /// </summary>
        ReplayEvents = 0,

        /// <summary>
        /// Same recovery as <see cref="ReplayEvents"/>, but missed events update local state
        /// without raising per-event callbacks. Listen to
        /// <see cref="IStreamChatClient.StateRecovered"/> and rebuild your UI from channel state
        /// (<see cref="StatefulModels.IStreamChannel.Messages"/>, and similar).
        ///
        /// Some events are still raised because they are not stored in channel state:
        /// <see cref="StatefulModels.IStreamChannel.CustomEventReceived"/>,
        /// <see cref="IStreamChatClient.ChannelDeleted"/>, and membership or invite notifications.
        ///
        /// Use this if a long disconnect causes a hitch when the app resumes.
        /// </summary>
        BatchStateUpdate = 1,

        /// <summary>
        /// The SDK does not restore state after a reconnect.
        /// It does not refresh channels, does not start watching them again,
        /// and does not raise <see cref="IStreamChatClient.StateRecovered"/>.
        /// Local state and <see cref="IStreamChatClient.WatchedChannels"/> stay as they were
        /// before the disconnect.
        ///
        /// Use this only if you handle recovery yourself. When the connection is
        /// <see cref="LowLevelClient.ConnectionState.Connected"/> again, query the channels you need.
        /// Example: <c>QueryChannelsAsync(new[] { ChannelFilter.Cid.In(cids) }, limit: 30)</c>.
        /// That call refreshes state and starts watching.
        /// Do not use <see cref="StatefulModels.IStreamChannel.WatchAsync"/> here:
        /// <see cref="StatefulModels.IStreamChannel.IsWatched"/> is still true, so WatchAsync does nothing.
        /// </summary>
        Disabled = 2,
    }
}
