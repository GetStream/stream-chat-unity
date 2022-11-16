using System.Collections.Generic;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core
{
    /// <summary>
    /// Object with its state being automatically tracked by the <see cref="StreamChatClient"/>
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
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/creating_channels/?language=unity</remarks>
        IDictionary<string, object> CustomData { get; }
        
        /// <summary>
        /// Set custom data with key. If this key already exists it will be overriden. Use <see cref="CustomData"/> to check if key already exists
        /// </summary>
        /// <param name="key">Key of custom data</param>
        /// <param name="value">Value of custom data. This object needs to be serializable to JSON</param>
        object SetCustomData(string key, object value);
        
        /// <summary>
        /// Get custom data by key. You can also use <see cref="CustomData"/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object GetCustomData(string key);
    }
}