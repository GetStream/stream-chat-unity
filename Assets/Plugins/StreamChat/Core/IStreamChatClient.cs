using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StreamChat.Core.State.Requests;
using StreamChat.Core.State.Responses;
using StreamChat.Core.State.TrackedObjects;
using StreamChat.Libs.Auth;
using StreamChat.Libs.ChatInstanceRunner;

namespace StreamChat.Core
{
    public interface IStreamChatClient : IDisposable, IStreamChatClientEventsListener
    {
        /// <summary>
        /// Event fired when connection with Stream Chat server is established
        /// </summary>
        event ConnectionMadeHandler Connected;

        /// <summary>
        /// Event fired when connection with Stream Chat server is lost
        /// </summary>
        event Action Disconnected;

        /// <summary>
        /// Event fired when connection state with Stream Chat server has changed
        /// </summary>
        event ConnectionChangeHandler ConnectionStateChanged;
        
        /// <summary>
        /// Channel was deleted
        /// </summary>
        event ChannelDeleteHandler ChannelDeleted;

        /// <summary>
        /// Current connection state
        /// </summary>
        ConnectionState ConnectionState { get; }

        /// <summary>
        /// Data of the user that is connected to the Stream Chat using the local device. This property is set after the client connection is established.
        /// YSubscribe to <see cref="Connected"/> to know when the connection is established.
        /// Use <see cref="IStreamLocalUserData.User"/> to access the local <see cref="IStreamUser"/> object
        /// </summary>
        IStreamLocalUserData LocalUserData { get; }

        /// <summary>
        /// Watched channels receive updates on all users activity like new messages, reactions, etc.
        /// Use <see cref="GetOrCreateChannelAsync"/> and <see cref="QueryChannelsAsync"/> to watch channels
        /// </summary>
        IReadOnlyList<IStreamChannel> WatchedChannels { get; }

        double? NextReconnectTime { get; }

        Task<IStreamLocalUserData> ConnectUserAsync(AuthCredentials userAuthCredentials,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Create or get a channel with a given type and id
        /// Use this to create general purpose channel for unspecified group of users
        /// If you want to create a channel for a dedicated group of users e.g. private conversation use the <see cref="IStreamChatClient.GetOrCreateChannelAsync(StreamChat.Core.State.ChannelType,System.Collections.Generic.IEnumerable{StreamChat.Core.State.TrackedObjects.IStreamUser},IStreamChannelCustomData)"/> overload
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/creating_channels/?language=unity#1.-creating-a-channel-using-a-channel-id</remarks>
        Task<IStreamChannel> GetOrCreateChannelAsync(ChannelType channelType, string channelId, string name = null,
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
        Task<IStreamChannel> GetOrCreateChannelAsync(ChannelType channelType, IEnumerable<IStreamUser> members,
            Dictionary<string, object> optionalCustomData = null);

        //StreamTodo: add missing descriptions
        Task<IEnumerable<IStreamChannel>> QueryChannelsAsync(IDictionary<string, object> filters);

        Task<IEnumerable<IStreamUser>> QueryUsersAsync(IDictionary<string, object> filters);

        Task<IEnumerable<IStreamUser>> UpsertUsers(IEnumerable<StreamUserUpsertRequest> userRequests);
        
        /// <summary>
        /// Mute channels with optional duration in milliseconds
        /// </summary>
        /// <param name="channels">Channels to mute</param>
        /// <param name="milliseconds">[Optional] Duration in milliseconds</param>
        Task MuteMultipleChannelsAsync(IEnumerable<IStreamChannel> channels, int? milliseconds = default);

        Task UnmuteMultipleChannelsAsync(IEnumerable<IStreamChannel> channels);

        /// <summary>
        /// Delete multiple channels. This in an asynchronous server operation meaning it may still be executing when this method Task is completed.
        /// Response contains <see cref="StreamDeleteChannelsResponse.TaskId"/> which can be used to check the status of the delete operation
        /// </summary>
        /// <param name="channels">Collection of <see cref="IStreamChannel"/> to delete</param>
        /// <param name="isHardDelete">Hard delete removes channels entirely whereas Soft Delete deletes them from client but still allows to access them from the server-side SDK</param>
        /// <remarks>https://getstream.io/chat/docs/unity/channel_delete/?language=unity</remarks>
        Task<StreamDeleteChannelsResponse> DeleteMultipleChannelsAsync(IEnumerable<IStreamChannel> channels,
            bool isHardDelete = false);

        /// <summary>
        /// You mute single user by using <see cref="IStreamUser.MuteAsync"/>
        /// </summary>
        /// <param name="users">Users to mute</param>
        /// <param name="timeoutMinutes">Optional timeout. Without timeout users will stay muted indefinitely</param>
        Task MuteMultipleUsersAsync(IEnumerable<IStreamUser> users, int? timeoutMinutes = default);
        
        Task DisconnectUserAsync();
        
        bool IsLocalUser(IStreamUser messageUser);

    }
}