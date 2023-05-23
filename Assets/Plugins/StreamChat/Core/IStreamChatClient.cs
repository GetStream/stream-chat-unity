using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StreamChat.Core.LowLevelClient;
using StreamChat.Core.QueryBuilders.Filters;
using StreamChat.Core.QueryBuilders.Sort;
using StreamChat.Core.Requests;
using StreamChat.Core.Responses;
using StreamChat.Core.StatefulModels;
using StreamChat.Libs.Auth;
using StreamChat.Libs.ChatInstanceRunner;

namespace StreamChat.Core
{
    /// <summary>
    /// The official Stream Chat API Client. This client is stateful meaning that the state of:
    /// - <see cref="IStreamChannel"/> 
    /// - <see cref="IStreamChannelMember"/> 
    /// - <see cref="IStreamUser"/> 
    /// - <see cref="IStreamLocalUserData"/>
    /// is automatically updated by the client and they always represent the most updated state.
    /// </summary>
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
        /// Invite to a <see cref="IStreamChannel"/> was received
        /// </summary>
        event ChannelInviteHandler ChannelInviteReceived;
        
        /// <summary>
        /// Invite to a <see cref="IStreamChannel"/> was accepted
        /// </summary>
        event ChannelInviteHandler ChannelInviteAccepted;
        
        /// <summary>
        /// Invite to a <see cref="IStreamChannel"/> was rejected
        /// </summary>
        event ChannelInviteHandler ChannelInviteRejected;

        /// <summary>
        /// Current connection state
        /// </summary>
        ConnectionState ConnectionState { get; }

        /// <summary>
        /// Is client connected. Subscribe to <see cref="Connected"/> to get notified when connection is established
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// If true it means that client initiated connection and is waiting for the Stream server to confirm the connection. Subscribe to <see cref="Connected"/> to get notified when connection is established
        /// </summary>
        bool IsConnecting { get; }

        /// <summary>
        /// Data of the user that is connected to the Stream Chat using the local device. This property is set after the client connection is established.
        /// YSubscribe to <see cref="Connected"/> to know when the connection is established.
        /// Use <see cref="IStreamLocalUserData.User"/> to access the local <see cref="IStreamUser"/> object
        /// </summary>
        IStreamLocalUserData LocalUserData { get; }

        /// <summary>
        /// Watched channels receive updates on all users activity like new messages, reactions, etc.
        /// Use <see cref="GetOrCreateChannelWithIdAsync"/> and <see cref="QueryChannelsAsync"/> to watch channels
        /// </summary>
        IReadOnlyList<IStreamChannel> WatchedChannels { get; }

        /// <summary>
        /// Next time since startup the client will attempt to reconnect to the Stream Server. 
        /// </summary>
        double? NextReconnectTime { get; }

        /// <summary>
        /// Low level client. Use it if you want to bypass the stateful client and execute low level requests directly.
        /// </summary>
        IStreamChatLowLevelClient LowLevelClient { get; }

        /// <summary>
        /// Connect user to Stream Chat server.
        /// User authentication credentials:
        /// ApiKey - Your application API KEY. You can get it from https://dashboard.getstream.io/
        /// UserId - Create it in Stream Dashboard or with server-side SDK or with <see cref="UpsertUsers"/>
        /// UserToken - Read here https://getstream.io/chat/docs/unity/unity_client_overview/?language=unity#auth-credentials
        /// </summary>
        /// <param name="userAuthCredentials">User authentication credentials</param>
        /// <param name="cancellationToken">Cancellation token that will abort the login request when cancelled</param>
        /// <remarks>https://getstream.io/chat/docs/unity/unity_client_overview/?language=unity#auth-credentials</remarks> 
        Task<IStreamLocalUserData> ConnectUserAsync(AuthCredentials userAuthCredentials,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Connect user to Stream Chat server.
        /// </summary>
        /// <param name="apiKey">Your application API KEY. You can get it from https://dashboard.getstream.io/</param>
        /// <param name="userId">ID of a user that will be connected to the Stream Chat</param>
        /// <param name="userAuthToken">User authentication token.</param>
        /// <param name="cancellationToken">Cancellation token that will abort the login request when cancelled</param>
        /// <remarks>https://getstream.io/chat/docs/unity/unity_client_overview/?language=unity#auth-credentials</remarks>
        Task<IStreamLocalUserData> ConnectUserAsync(string apiKey, string userId, string userAuthToken,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiKey">Your application API KEY. You can get it from https://dashboard.getstream.io/</param>
        /// <param name="userId">ID of a user that will be connected to the Stream Chat</param>
        /// <param name="tokenProvider">Service that will provide authorization token for a given user id. Use <see cref="TokenProvider"/></param>
        /// <param name="cancellationToken">Cancellation token that will abort the login request when cancelled</param>
        /// <returns></returns>
        Task<IStreamLocalUserData> ConnectUserAsync(string apiKey, string userId, ITokenProvider tokenProvider,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Create or get a channel with a given type and id
        /// Use this to create general purpose channel for unspecified group of users
        /// If you want to create a channel for a dedicated group of users e.g. private conversation use the <see cref="GetOrCreateChannelWithIdAsync"/> overload
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/creating_channels/?language=unity#1.-creating-a-channel-using-a-channel-id</remarks>
        Task<IStreamChannel> GetOrCreateChannelWithIdAsync(ChannelType channelType, string channelId,
            string name = null,
            IDictionary<string, object> optionalCustomData = null);

        /// <summary>
        /// Create or get a channel for a given groups of users.
        /// Use this to create channel for direct messages. This will return the same channel per unique group of users regardless of their order.
        /// If you wish to create channels with ID for users to join use the <see cref="GetOrCreateChannelWithIdAsync"/>
        /// </summary>
        /// <param name="channelType">Type of channel determines permissions and default settings.
        ///     Use predefined ones:
        ///     <see cref="ChannelType.Messaging"/>,
        ///     <see cref="ChannelType.Livestream"/>,
        ///     <see cref="ChannelType.Team"/>,
        ///     <see cref="ChannelType.Commerce"/>,
        ///     <see cref="ChannelType.Gaming"/>,
        ///     or create a custom type in your dashboard and use <see cref="ChannelType.Custom"/></param>
        /// <param name="members">Users for which a channel will be created. If channel </param>
        /// <param name="optionalCustomData">[Optional] Custom data to attach to this channel</param>
        Task<IStreamChannel> GetOrCreateChannelWithMembersAsync(ChannelType channelType,
            IEnumerable<IStreamUser> members,
            IDictionary<string, object> optionalCustomData = null);

        /// <summary>
        /// Query <see cref="IStreamChannel"/> with optional: filters, sorting, and pagination
        /// </summary>
        /// <param name="filters">[Optional] Filters</param>
        /// <param name="sort">[Optional] Sort object. You can chain multiple sorting fields</param>
        /// <param name="limit">[Optional] How many records to return. Think about it as "records per page"</param>
        /// <param name="offset">[Optional] How many records to skip. Example: if Limit is 30, the offset for 2nd page is 30, for 3rd page is 60, etc.</param>
        [Obsolete("This method will be removed in the future. Please use the other overload method that uses " +
                  nameof(IFieldFilterRule) + " type filters")]
        Task<IEnumerable<IStreamChannel>> QueryChannelsAsync(IDictionary<string, object> filters,
            ChannelSortObject sort = null, int limit = 30, int offset = 0);
        
        /// <summary>
        /// Query <see cref="IStreamChannel"/> with optional: filters, sorting, and pagination
        /// </summary>
        /// <param name="filters">[Optional] Filters</param>
        /// <param name="sort">[Optional] Sort object. You can chain multiple sorting fields</param>
        /// <param name="limit">[Optional] How many records to return. Think about it as "records per page"</param>
        /// <param name="offset">[Optional] How many records to skip. Example: if Limit is 30, the offset for 2nd page is 30, for 3rd page is 60, etc.</param>
        Task<IEnumerable<IStreamChannel>> QueryChannelsAsync(IEnumerable<IFieldFilterRule> filters = null,
            ChannelSortObject sort = null, int limit = 30, int offset = 0);

        /// <summary>
        /// Query <see cref="IStreamUser"/>
        /// </summary>
        /// <param name="filters">[Optional] filter object</param>
        /// <returns></returns>
        [Obsolete("This method will be removed in the future. Please use the other overload method that uses " +
                  nameof(IFieldFilterRule) + " type filters")]
        Task<IEnumerable<IStreamUser>> QueryUsersAsync(IDictionary<string, object> filters = null);
        
        /// <summary>
        /// Query <see cref="IStreamUser"/>
        /// </summary>
        /// <param name="filters">[Optional] filter rules</param>
        /// <returns></returns>
        Task<IEnumerable<IStreamUser>> QueryUsersAsync(IEnumerable<IFieldFilterRule> filters = null, UsersSortObject sort = null, int offset = 0, int limit = 30);

        /// <summary>
        /// Query banned users based on provided parameters
        /// </summary>
        /// <param name="streamQueryBannedUsersRequest">Request parameters object</param>
        Task<IEnumerable<StreamUserBanInfo>> QueryBannedUsersAsync(
            StreamQueryBannedUsersRequest streamQueryBannedUsersRequest);

        /// <summary>
        /// Upsert users. Upsert means update this user or create if not found
        /// </summary>
        /// <param name="userRequests">Collection of user upsert requests</param>
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