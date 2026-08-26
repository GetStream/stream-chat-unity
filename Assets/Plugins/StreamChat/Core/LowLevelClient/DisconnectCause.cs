namespace StreamChat.Core.LowLevelClient
{
    /// <summary>
    /// Why the WebSocket was closed. Used by
    /// <see cref="IStreamChatLowLevelClient.DisconnectAsync(DisconnectCause)"/> to decide whether the
    /// reconnect scheduler stays armed. <see cref="UserLogout"/> stops auto-reconnect; other causes
    /// may leave it running depending on the high-level API that initiated the close.
    ///
    /// Stateful clients should call <see cref="IStreamChatClient.DisconnectUserAsync"/>,
    /// <see cref="IStreamChatClient.PauseConnectionAsync"/>, or
    /// <see cref="IStreamChatClient.ResumeConnectionAsync"/> instead of this enum.
    /// </summary>
    public enum DisconnectCause
    {
        /// <summary>
        /// No disconnect has been recorded yet, or the close was not classified.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// <see cref="IStreamChatClient.DisconnectUserAsync"/>. SDK login state is cleared;
        /// automatic reconnects stop until the next <see cref="IStreamChatClient.ConnectUserAsync"/>.
        /// </summary>
        UserLogout,

        /// <summary>
        /// Intentional WebSocket close via <see cref="IStreamChatClient.PauseConnectionAsync"/>.
        /// Local credentials and client state are kept; automatic reconnects stay disabled until
        /// <see cref="IStreamChatClient.ResumeConnectionAsync"/>.
        /// </summary>
        ConnectionReleased,

        /// <summary>
        /// The app was backgrounded. The WebSocket is closed and the user appears offline on the
        /// server; the client reconnects when the app returns to the foreground.
        /// </summary>
        ApplicationPause,

        /// <summary>
        /// Network became unavailable. Scheduler reconnects when the network is back.
        /// </summary>
        Network,

        /// <summary>
        /// Server health-check timed out. Scheduler reconnects.
        /// </summary>
        HealthTimeout,
    }
}
