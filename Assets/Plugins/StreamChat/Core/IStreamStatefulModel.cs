using StreamChat.Core.StatefulModels;

namespace StreamChat.Core
{
    /// <summary>
    /// Model with its state being automatically updated by the <see cref="StreamChatClient"/>
    ///
    /// This means that this objects corresponds to an object on the Stream Chat server with the same ID
    /// its state will be automatically updated whenever new information is received from the server
    /// </summary>
    public interface IStreamStatefulModel
    {
        string UniqueId { get; }

        /// <summary>
        /// Custom data (max 5KB) that you can assign to:
        /// - <see cref="IStreamChannel"/>
        /// - <see cref="IStreamMessage"/>
        /// - <see cref="IStreamUser"/>
        /// - <see cref="StreamChannelMember"/>
        /// If you want to have images or files as custom data, upload them using <see cref="IStreamChannel.UploadFileAsync"/> and <see cref="IStreamChannel.UploadImageAsync"/> and put only file URL as a custom data
        /// You can set custom data by using <see cref="IStreamChannel.UpdatePartialAsync"/> or <see cref="IStreamChannel.UpdateOverwriteAsync"/>
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/creating_channels/?language=unity</remarks>
        IStreamCustomData CustomData { get; }
    }
}