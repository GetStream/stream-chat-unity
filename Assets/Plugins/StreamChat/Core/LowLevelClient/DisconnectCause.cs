namespace StreamChat.Core.LowLevelClient
{
    /// <summary>
    /// Why the WebSocket was closed. Used by
    /// <see cref="IStreamChatLowLevelClient.DisconnectAsync(DisconnectCause)"/> to decide whether the
    /// reconnect scheduler stays armed. Logout stops auto-reconnect; every other cause leaves it running.
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
        /// <see cref="IStreamChatClient.DisconnectUserAsync"/>. Session ended; the scheduler is stopped
        /// until the next <see cref="IStreamChatClient.ConnectUserAsync"/>.
        /// </summary>
        UserLogout,

        /// <summary>
        /// <see cref="IStreamChatClient.PauseConnectionAsync"/>. User session is kept; reconnect with
        /// <see cref="IStreamChatClient.ResumeConnectionAsync"/> (the scheduler also stays armed).
        /// </summary>
        ConnectionReleased,

        /// <summary>
        /// The app was backgrounded. Session is kept; reconnects when the app returns to the foreground.
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
