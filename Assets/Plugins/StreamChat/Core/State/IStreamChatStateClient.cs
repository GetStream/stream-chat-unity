using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StreamChat.Core.State.Responses;
using StreamChat.Core.State.TrackedObjects;
using StreamChat.Libs.Auth;

namespace StreamChat.Core.State
{
    public interface IStreamChatStateClient : IDisposable
    {
        /// <summary>
        /// Triggered when connection with Stream Chat server is established
        /// </summary>
        event ConnectionMadeHandler Connected;

        /// <summary>
        /// Triggered when connection with Stream Chat server is lost
        /// </summary>
        event Action Disconnected;

        /// <summary>
        /// Triggered when connection state with Stream Chat server has changed
        /// </summary>
        event ConnectionChangeHandler ConnectionStateChanged;

        /// <summary>
        /// Current connection state
        /// </summary>
        ConnectionState ConnectionState { get; }

        /// <summary>
        /// Local user that is connected to the Stream Chat. This property is set after the client connection is established.
        /// You can subscribe to <see cref="Connected"/> or <see cref="ConnectionStateChanged"/> events to know when the connection is established.
        /// You can access the local <see cref="StreamUser"/> via <see cref="LocalUserData"/> <see cref="StreamLocalUserData.User"/> property
        /// </summary>
        StreamLocalUserData LocalUserData { get; }

        /// <summary>
        /// Watched channels receive updates on all users activity like new messages, reactions, etc.
        /// Use <see cref="GetOrCreateChannelAsync"/> and <see cref="QueryChannelsAsync"/> to watch channels
        /// </summary>
        IReadOnlyList<StreamChannel> WatchedChannels { get; }

        double? NextReconnectTime { get; }

        /// <summary>
        /// Update needs to be called every frame 
        /// </summary>
        void Update();

        Task<StreamLocalUserData> ConnectUserAsync(AuthCredentials userAuthCredentials,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Create or get a channel with a given type and id
        /// Use this to create general purpose channel for unspecified group of users
        /// If you want to create a channel for a dedicated group of users e.g. private conversation use the <see cref="IStreamChatStateClient.GetOrCreateChannelAsync(StreamChat.Core.State.ChannelType,System.Collections.Generic.IEnumerable{StreamChat.Core.State.TrackedObjects.StreamUser},IStreamChannelCustomData)"/> overload
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/creating_channels/?language=unity#1.-creating-a-channel-using-a-channel-id</remarks>
        Task<StreamChannel> GetOrCreateChannelAsync(ChannelType channelType, string channelId, string name = null,
            Dictionary<string, object> optionalCustomData = null);

        /// <summary>
        /// Create or get a channel with a given type for a given groups of members.
        /// Use this to create channel for group messages
        ///
        /// </summary>
        /// <param name="channelType">Type of channel determines its permissions and default settings. Use predefined ones: <see cref="ChannelType.Messaging"/>, <see cref="ChannelType.Livestream"/>, <see cref="ChannelType.Team"/>, <see cref="ChannelType.Commerce"/>, <see cref="ChannelType.Gaming"/> or create a custom type in your dashboard and use <see cref="ChannelType.Custom"/></param>
        /// <param name="members">Users for which a channel will be created. If channel </param>
        /// <param name="optionalCustomData"></param>
        /// <returns></returns>
        Task<StreamChannel> GetOrCreateChannelAsync(ChannelType channelType, IEnumerable<StreamUser> members,
            Dictionary<string, object> optionalCustomData = null);

        Task<IEnumerable<StreamChannel>> QueryChannelsAsync(IDictionary<string, object> filters);

        /// <summary>
        /// Mute channels with optional duration in milliseconds
        /// </summary>
        /// <param name="channels">Channels to mute</param>
        /// <param name="milliseconds">[Optional] Duration in milliseconds</param>
        Task MuteMultipleChannelsAsync(IEnumerable<StreamChannel> channels, int? milliseconds = default);

        Task UnmuteMultipleChannelsAsync(IEnumerable<StreamChannel> channels);

        /// <summary>
        /// Delete multiple channels. This in an asynchronous server operation meaning it may still be executing when this method Task is completed.
        /// Response contains <see cref="StreamDeleteChannelsResponse.TaskId"/> which can be used to check the status of the delete operation
        /// </summary>
        /// <param name="channels">Collection of <see cref="StreamChannel"/> to delete</param>
        /// <param name="isHardDelete">Hard delete removes channels entirely whereas Soft Delete deletes them from client but still allows to access them from the server-side SDK</param>
        /// <remarks>https://getstream.io/chat/docs/unity/channel_delete/?language=unity</remarks>
        Task<StreamDeleteChannelsResponse> DeleteMultipleChannelsAsync(IEnumerable<StreamChannel> channels,
            bool isHardDelete = false);

        Task DisconnectUserAsync();
        
        bool IsLocalUser(StreamUser messageUser);
    }
}