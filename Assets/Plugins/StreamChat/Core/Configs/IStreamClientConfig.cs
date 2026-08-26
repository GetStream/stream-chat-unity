using StreamChat.Core.LowLevelClient;

namespace StreamChat.Core.Configs
{
    /// <summary>
    /// Configuration for <see cref="IStreamChatLowLevelClient"/>
    /// </summary>
    public interface IStreamClientConfig
    {
        /// <summary>
        /// What type of logs are being emitted. Available options:
        /// FailureOnly - only errors will be logged. This option is recommended for production
        /// All - all errors will be logged. This can be useful during development
        /// Debug - This included All logs + some additional that can be useful for debugging
        /// Disabled - no logs will be emitted. Not recommended in general - this could be only viable if you're capturing all of the thrown exceptions and handling the logging on your own.
        /// </summary>
        StreamLogLevel LogLevel { get; set; }

        /// <summary>
        /// Whether a message you send is optimistically inserted into the local channel state and
        /// raised via <see cref="StatefulModels.IStreamChannel.MessageReceived"/> immediately, before
        /// the server's `message.new` WebSocket echo arrives. Defaults to <c>true</c>.
        ///
        /// When <c>true</c> (default), the sender sees their own message right away and the later
        /// WebSocket echo is de-duplicated. When <c>false</c>, the locally sent message is not added
        /// to the channel until its server echo arrives, so every participant - including the sender -
        /// observes messages in the same server-defined order. Disable this when consistent cross-client
        /// ordering matters more than instant local feedback (e.g. a shared, broadcast-ordered feed).
        /// </summary>
        bool OptimisticMessageInsert { get; set; }

        /// <summary>
        /// Default local message cache limit for all channels. <c>null</c> (default) = unlimited.
        /// Use <see cref="MessageCacheWindow.Recommended"/> for livestream-style channels.
        /// Per-channel overrides: <see cref="StatefulModels.IStreamChannel.OverrideMessageCacheWindow"/>.
        /// Does not change server history. See <see cref="StatefulModels.IStreamChannel.MessageCacheWindow"/>.
        /// </summary>
        MessageCacheWindow DefaultMessageCacheWindow { get; set; }

        /// <summary>
        /// When the app goes to the background, close the WebSocket. Other users see this user
        /// as offline while disconnected. When the app returns to the foreground, the client
        /// reconnects with the existing credentials and recovers missed state.
        /// Defaults to <c>true</c>. Set to <c>false</c> to keep the WebSocket open while backgrounded.
        ///
        /// In the Unity Editor this has no effect — pausing play mode or unfocusing the Game view
        /// would otherwise disconnect constantly. A warning is logged once.
        ///
        /// Applies when you create the client with <see cref="StreamChatClient.CreateDefaultClient"/>.
        /// If you drive the client yourself (you call Update each frame), close and reopen with
        /// <see cref="IStreamChatClient.PauseConnectionAsync"/> /
        /// <see cref="IStreamChatClient.ResumeConnectionAsync"/> on background / foreground.
        /// </summary>
        bool DisconnectOnApplicationPause { get; set; }

        /// <summary>
        /// How the client restores local state after the websocket reconnects.
        /// Default is <see cref="Configs.StateRecoveryStrategy.ReplayEvents"/>.
        /// See <see cref="Configs.StateRecoveryStrategy"/> for the other options.
        /// </summary>
        StateRecoveryStrategy StateRecoveryStrategy { get; set; }
    }
}