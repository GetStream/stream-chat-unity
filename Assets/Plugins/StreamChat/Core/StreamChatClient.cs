using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StreamChat.Core.Configs;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.LowLevelClient;
using StreamChat.Core.State;
using StreamChat.Core.State.Caches;
using StreamChat.Core.Models;
using StreamChat.Core.QueryBuilders.Filters;
using StreamChat.Core.QueryBuilders.Sort;
using StreamChat.Core.Requests;
using StreamChat.Core.Responses;
using StreamChat.Core.StatefulModels;
using StreamChat.Libs;
using StreamChat.Libs.AppInfo;
using StreamChat.Libs.Auth;
using StreamChat.Libs.ChatInstanceRunner;
using StreamChat.Libs.Http;
using StreamChat.Libs.Logs;
using StreamChat.Libs.NetworkMonitors;
using StreamChat.Libs.Serialization;
using StreamChat.Libs.Time;
using StreamChat.Libs.Websockets;

namespace StreamChat.Core
{
    /// <summary>
    /// Connection has been established
    /// You can access local user data via <see cref="StreamChatClient.LocalUserData"/>
    /// </summary>
    public delegate void ConnectionMadeHandler(IStreamLocalUserData localUserData);

    /// <summary>
    /// Connection state change handler
    /// </summary>
    public delegate void ConnectionChangeHandler(ConnectionState previous, ConnectionState current);

    /// <summary>
    /// Channel deletion handler
    /// </summary>
    public delegate void ChannelDeleteHandler(string channelCid, string channelId, ChannelType channelType);

    //StreamTodo: Handle restoring state after lost connection

    public delegate void ChannelInviteHandler(IStreamChannel channel, IStreamUser invitee);

    public sealed class StreamChatClient : IStreamChatClient
    {
        public event ConnectionMadeHandler Connected;

        public event Action Disconnected;

        public event Action Disposed;

        public event ConnectionChangeHandler ConnectionStateChanged;

        public event ChannelDeleteHandler ChannelDeleted;

        public event ChannelInviteHandler ChannelInviteReceived;
        public event ChannelInviteHandler ChannelInviteAccepted;
        public event ChannelInviteHandler ChannelInviteRejected;

        public const int QueryUsersLimitMaxValue = 30;
        public const int QueryUsersOffsetMaxValue = 1000;

        public ConnectionState ConnectionState => InternalLowLevelClient.ConnectionState;

        public bool IsConnected => InternalLowLevelClient.ConnectionState == ConnectionState.Connected;
        public bool IsConnecting => InternalLowLevelClient.ConnectionState == ConnectionState.Connecting;

        public IStreamLocalUserData LocalUserData => _localUserData;

        private StreamLocalUserData _localUserData;

        public IReadOnlyList<IStreamChannel> WatchedChannels => _cache.Channels.AllItems;

        public double? NextReconnectTime => InternalLowLevelClient.NextReconnectTime;

        public IStreamChatLowLevelClient LowLevelClient => InternalLowLevelClient;

        /// <inheritdoc cref="StreamChatLowLevelClient.SDKVersion"/>
        public static Version SDKVersion => StreamChatLowLevelClient.SDKVersion;

        /// <summary>
        /// Recommended method to create an instance of <see cref="IStreamChatClient"/>
        /// If you wish to create an instance with non default dependencies you can use the <see cref="CreateClientWithCustomDependencies"/>
        /// </summary>
        /// <param name="config">[Optional] configuration</param>
        public static IStreamChatClient CreateDefaultClient(IStreamClientConfig config = default)
        {
            config ??= StreamClientConfig.Default;
            var logs = StreamDependenciesFactory.CreateLogger(config.LogLevel.ToLogLevel());
            var websocketClient
                = StreamDependenciesFactory.CreateWebsocketClient(logs, config.LogLevel.IsDebugEnabled());
            var httpClient = StreamDependenciesFactory.CreateHttpClient();
            var serializer = StreamDependenciesFactory.CreateSerializer();
            var timeService = StreamDependenciesFactory.CreateTimeService();
            var applicationInfo = StreamDependenciesFactory.CreateApplicationInfo();
            var gameObjectRunner = StreamDependenciesFactory.CreateChatClientRunner();
            var networkMonitor = StreamDependenciesFactory.CreateNetworkMonitor();

            var client = new StreamChatClient(websocketClient, httpClient, serializer, timeService, networkMonitor,
                applicationInfo, logs, config);

            gameObjectRunner?.RunChatInstance(client);
            return client;
        }

        /// <summary>
        /// Create instance of <see cref="ITokenProvider"/>
        /// </summary>
        /// <param name="urlFactory">Delegate that will return a valid url that return JWT auth token for a given user ID</param>
        /// <example>
        /// <code>
        /// StreamChatClient.CreateDefaultTokenProvider(userId => new Uri($"https:your-awesome-page.com/get_token?userId={userId}"));
        /// </code>
        /// </example>
        public static ITokenProvider CreateDefaultTokenProvider(TokenProvider.TokenUriHandler urlFactory)
            => StreamDependenciesFactory.CreateTokenProvider(urlFactory);

        /// <summary>
        /// Create a new instance of <see cref="IStreamChatLowLevelClient"/> with custom provided dependencies.
        /// If you want to create a default new instance then just use the <see cref="CreateDefaultClient"/>.
        /// Important! Custom created client require calling the <see cref="Update"/> and <see cref="Destroy"/> methods.
        /// </summary>
        public static IStreamChatClient CreateClientWithCustomDependencies(IWebsocketClient websocketClient,
            IHttpClient httpClient, ISerializer serializer, ITimeService timeService, INetworkMonitor networkMonitor,
            IApplicationInfo applicationInfo, ILogs logs, IStreamClientConfig config)
            => new StreamChatClient(websocketClient, httpClient, serializer, timeService, networkMonitor,
                applicationInfo, logs, config);

        /// <inheritdoc cref="StreamChatLowLevelClient.CreateDeveloperAuthToken"/>
        public static string CreateDeveloperAuthToken(string userId)
            => StreamChatLowLevelClient.CreateDeveloperAuthToken(userId);

        /// <inheritdoc cref="StreamChatLowLevelClient.SanitizeUserId"/>
        public static string SanitizeUserId(string userId) => StreamChatLowLevelClient.SanitizeUserId(userId);

        public Task<IStreamLocalUserData> ConnectUserAsync(AuthCredentials userAuthCredentials,
            CancellationToken cancellationToken = default)
        {
            InternalLowLevelClient.ConnectUser(userAuthCredentials);

            //StreamTodo: test calling this method multiple times in a row

            //StreamTodo: timeout, like 5 seconds?
            _connectUserCancellationToken = cancellationToken;

            _connectUserCancellationTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(_connectUserCancellationToken);
            _connectUserCancellationTokenSource.Token.Register(TryCancelWaitingForUserConnection);

            //StreamTodo: check if we can pass the cancellation token here
            _connectUserTaskSource = new TaskCompletionSource<IStreamLocalUserData>();
            return _connectUserTaskSource.Task;
        }

        public Task<IStreamLocalUserData> ConnectUserAsync(string apiKey, string userId, string userAuthToken,
            CancellationToken cancellationToken = default)
        {
            StreamAsserts.AssertNotNullOrEmpty(apiKey, nameof(apiKey));
            StreamAsserts.AssertNotNullOrEmpty(userId, nameof(userId));
            StreamAsserts.AssertNotNullOrEmpty(userAuthToken, nameof(userAuthToken));

            return ConnectUserAsync(new AuthCredentials(apiKey, userId, userAuthToken), cancellationToken);
        }

        public async Task<IStreamLocalUserData> ConnectUserAsync(string apiKey, string userId,
            ITokenProvider tokenProvider,
            CancellationToken cancellationToken = default)
        {
            StreamAsserts.AssertNotNullOrEmpty(apiKey, nameof(apiKey));
            StreamAsserts.AssertNotNullOrEmpty(userId, nameof(userId));
            StreamAsserts.AssertNotNull(tokenProvider, nameof(tokenProvider));

            var ownUserDto
                = await InternalLowLevelClient.ConnectUserAsync(apiKey, userId, tokenProvider, cancellationToken);
            return UpdateLocalUser(ownUserDto);
        }

        //StreamTodo: test scenario: ConnectUserAsync and immediately all DisconnectUserAsync
        public Task DisconnectUserAsync()
        {
            TryCancelWaitingForUserConnection();
            return InternalLowLevelClient.DisconnectAsync();
        }

        public bool IsLocalUser(IStreamUser user) => LocalUserData.User == user;

        public async Task<IStreamChannel> GetOrCreateChannelWithIdAsync(ChannelType channelType, string channelId,
            string name = null, IDictionary<string, object> optionalCustomData = null)
        {
            StreamAsserts.AssertChannelTypeIsValid(channelType);
            StreamAsserts.AssertChannelIdLength(channelId);

            var requestBodyDto = new ChannelGetOrCreateRequestInternalDTO
            {
                Presence = true,
                State = true,
                Watch = true,
                Data = new ChannelRequestInternalDTO
                {
                    Name = name,
                },
            };

            if (optionalCustomData != null && optionalCustomData.Any())
            {
                requestBodyDto.Data.AdditionalProperties = optionalCustomData?.ToDictionary(x => x.Key, x => x.Value);
            }

            var channelResponseDto = await InternalLowLevelClient.InternalChannelApi.GetOrCreateChannelAsync(
                channelType,
                channelId, requestBodyDto);
            return _cache.TryCreateOrUpdate(channelResponseDto);
        }

        public async Task<IStreamChannel> GetOrCreateChannelWithMembersAsync(ChannelType channelType,
            IEnumerable<IStreamUser> members, IDictionary<string, object> optionalCustomData = null)
        {
            StreamAsserts.AssertChannelTypeIsValid(channelType);
            StreamAsserts.AssertNotNullOrEmpty(members, nameof(members));

            var membersRequest = new List<ChannelMemberRequestInternalDTO>();
            foreach (var m in members)
            {
                membersRequest.Add(new ChannelMemberRequestInternalDTO
                {
                    UserId = m.Id
                });
            }

            var requestBodyDto = new ChannelGetOrCreateRequestInternalDTO
            {
                Presence = true,
                State = true,
                Watch = true,
                Data = new ChannelRequestInternalDTO
                {
                    Members = membersRequest,
                }
            };

            if (optionalCustomData != null && optionalCustomData.Any())
            {
                requestBodyDto.Data.AdditionalProperties = optionalCustomData?.ToDictionary(x => x.Key, x => x.Value);
            }

            var channelResponseDto =
                await InternalLowLevelClient.InternalChannelApi.GetOrCreateChannelAsync(channelType, requestBodyDto);
            return _cache.TryCreateOrUpdate(channelResponseDto);
        }

        public async Task<IEnumerable<IStreamChannel>> QueryChannelsAsync(IEnumerable<IFieldFilterRule> filters = null,
            ChannelSortObject sort = null, int limit = 30, int offset = 0)
        {
            StreamAsserts.AssertWithinRange(limit, 0, 30, nameof(limit));
            StreamAsserts.AssertGreaterThanOrEqualZero(offset, nameof(offset));

            //StreamTodo: Perhaps MessageLimit and MemberLimit should be configurable
            var requestBodyDto = new QueryChannelsRequestInternalDTO
            {
                FilterConditions = filters?.Select(_ => _.GenerateFilterEntry()).ToDictionary(x => x.Key, x => x.Value),
                Limit = limit,
                MemberLimit = null,
                MessageLimit = null,
                Offset = offset,
                Presence = true,

                /*
                 * StreamTodo: Allowing to sort query can potentially lead to mixed sorting in WatchedChannels
                 * But there seems no other choice because its too limiting to force only a global sorting for channels
                 * e.g. user may want to show channels in multiple ways with different sorting which would not work with global only sorting
                 */
                Sort = sort?.ToSortParamRequestList(),
                State = true,
                Watch = true,
            };

            var channelsResponseDto
                = await InternalLowLevelClient.InternalChannelApi.QueryChannelsAsync(requestBodyDto);
            if (channelsResponseDto.Channels == null || channelsResponseDto.Channels.Count == 0)
            {
                return Enumerable.Empty<StreamChannel>();
            }

            var result = new List<IStreamChannel>();
            foreach (var channelDto in channelsResponseDto.Channels)
            {
                result.Add(_cache.TryCreateOrUpdate(channelDto));
            }

            return result;
        }

        [Obsolete("This method will be removed in the future. Please use the other overload method that uses " +
                  nameof(IFieldFilterRule) + " type filters")]
        public async Task<IEnumerable<IStreamChannel>> QueryChannelsAsync(IDictionary<string, object> filters,
            ChannelSortObject sort = null, int limit = 30, int offset = 0)
        {
            StreamAsserts.AssertWithinRange(limit, 0, 30, nameof(limit));
            StreamAsserts.AssertGreaterThanOrEqualZero(offset, nameof(offset));

            //StreamTodo: Perhaps MessageLimit and MemberLimit should be configurable
            var requestBodyDto = new QueryChannelsRequestInternalDTO
            {
                FilterConditions = filters?.ToDictionary(x => x.Key, x => x.Value),
                Limit = limit,
                MemberLimit = null,
                MessageLimit = null,
                Offset = offset,
                Presence = true,

                /*
                 * StreamTodo: Allowing to sort query can potentially lead to mixed sorting in WatchedChannels
                 * But there seems no other choice because its too limiting to force only a global sorting for channels
                 * e.g. user may want to show channels in multiple ways with different sorting which would not work with global only sorting
                 */
                Sort = sort?.ToSortParamRequestList(),
                State = true,
                Watch = true,
            };

            var channelsResponseDto
                = await InternalLowLevelClient.InternalChannelApi.QueryChannelsAsync(requestBodyDto);
            if (channelsResponseDto.Channels == null || channelsResponseDto.Channels.Count == 0)
            {
                return Enumerable.Empty<StreamChannel>();
            }

            var result = new List<IStreamChannel>();
            foreach (var channelDto in channelsResponseDto.Channels)
            {
                result.Add(_cache.TryCreateOrUpdate(channelDto));
            }

            return result;
        }

        [Obsolete("This method will be removed in the future. Please use the other overload method that uses " +
                  nameof(IFieldFilterRule) + " type filters")]
        public async Task<IEnumerable<IStreamUser>> QueryUsersAsync(IDictionary<string, object> filters = null)
        {
            //StreamTodo: Missing filter, and stuff like IdGte etc
            var requestBodyDto = new QueryUsersRequestInternalDTO
            {
                FilterConditions = filters?.ToDictionary(x => x.Key, x => x.Value) ?? new Dictionary<string, object>(),
                IdGt = null,
                IdGte = null,
                IdLt = null,
                IdLte = null,
                Limit = null,
                Offset = null,
                Presence = true, //StreamTodo: research whether user should be allowed to control this
                Sort = null,
            };

            var response = await InternalLowLevelClient.InternalUserApi.QueryUsersAsync(requestBodyDto);
            if (response == null || response.Users == null || response.Users.Count == 0)
            {
                return Enumerable.Empty<IStreamUser>();
            }

            var result = new List<IStreamUser>();
            foreach (var userDto in response.Users)
            {
                result.Add(_cache.TryCreateOrUpdate(userDto));
            }

            return result;
        }

        public async Task<IEnumerable<IStreamUser>> QueryUsersAsync(IEnumerable<IFieldFilterRule> filters = null,
            UsersSortObject sort = null, int offset = 0, int limit = 30)
        {
            StreamAsserts.AssertWithinRange(limit, 0, QueryUsersLimitMaxValue, nameof(limit));
            StreamAsserts.AssertWithinRange(offset, 0, QueryUsersOffsetMaxValue, nameof(offset));

            //StreamTodo: Missing IdGt, IdLt, etc. We could wrap all pagination parameters in a single struct
            var requestBodyDto = new QueryUsersRequestInternalDTO
            {
                FilterConditions
                    = filters?.Select(_ => _.GenerateFilterEntry()).ToDictionary(x => x.Key, x => x.Value) ??
                      new Dictionary<string, object>(),
                IdGt = null,
                IdGte = null,
                IdLt = null,
                IdLte = null,
                Limit = limit,
                Offset = offset,
                Presence = true, //StreamTodo: research whether user should be allowed to control this
                Sort = sort?.ToSortParamInternalDTOs(),
            };

            var response = await InternalLowLevelClient.InternalUserApi.QueryUsersAsync(requestBodyDto);
            if (response == null || response.Users == null || response.Users.Count == 0)
            {
                return Enumerable.Empty<IStreamUser>();
            }

            var result = new List<IStreamUser>();
            foreach (var userDto in response.Users)
            {
                result.Add(_cache.TryCreateOrUpdate(userDto));
            }

            return result;
        }

        //StreamTodo: write tests
        public async Task<IEnumerable<StreamUserBanInfo>> QueryBannedUsersAsync(
            StreamQueryBannedUsersRequest streamQueryBannedUsersRequest)
        {
            StreamAsserts.AssertNotNull(streamQueryBannedUsersRequest, nameof(streamQueryBannedUsersRequest));

            var response =
                await InternalLowLevelClient.InternalModerationApi.QueryBannedUsersAsync(streamQueryBannedUsersRequest
                    .TrySaveToDto());
            if (response.Bans == null || response.Bans.Count == 0)
            {
                return Enumerable.Empty<StreamUserBanInfo>();
            }

            var result = new List<StreamUserBanInfo>();
            foreach (var userDto in response.Bans)
            {
                var banInfo = new StreamUserBanInfo().LoadFromDto(userDto, _cache);
                result.Add(banInfo);
            }

            return result;
        }

        public async Task<IEnumerable<IStreamUser>> UpsertUsers(IEnumerable<StreamUserUpsertRequest> userRequests)
        {
            StreamAsserts.AssertNotNullOrEmpty(userRequests, nameof(userRequests));

            //StreamTodo: items could be null
            var requestDtos = userRequests.Select(_ => _.TrySaveToDto()).ToDictionary(_ => _.Id, _ => _);

            var response = await InternalLowLevelClient.InternalUserApi.UpsertManyUsersAsync(
                new UpdateUsersRequestInternalDTO
                {
                    Users = requestDtos
                });

            var result = new List<IStreamUser>();
            foreach (var userDto in response.Users.Values)
            {
                result.Add(_cache.TryCreateOrUpdate(userDto));
            }

            return result;
        }

        public async Task MuteMultipleChannelsAsync(IEnumerable<IStreamChannel> channels, int? milliseconds = default)
        {
            StreamAsserts.AssertNotNullOrEmpty(channels, nameof(channels));

            var channelCids = channels.Select(_ => _.Cid).ToList();
            if (channelCids.Count == 0)
            {
                throw new ArgumentException($"{nameof(channels)} is empty");
            }

            var response = await InternalLowLevelClient.InternalChannelApi.MuteChannelAsync(
                new MuteChannelRequestInternalDTO
                {
                    ChannelCids = channelCids,
                    Expiration = milliseconds
                });

            UpdateLocalUser(response.OwnUser);
        }

        public async Task UnmuteMultipleChannelsAsync(IEnumerable<IStreamChannel> channels)
        {
            if (channels == null)
            {
                throw new ArgumentNullException(nameof(channels));
            }

            var channelCids = channels.Select(_ => _.Cid).ToList();
            if (channelCids.Count == 0)
            {
                throw new ArgumentException($"{nameof(channels)} is empty");
            }

            await InternalLowLevelClient.InternalChannelApi.UnmuteChannelAsync(new UnmuteChannelRequestInternalDTO
            {
                ChannelCids = channelCids,
                //StreamTodo: what is this Expiration here?
            });
        }

        public async Task<StreamDeleteChannelsResponse> DeleteMultipleChannelsAsync(
            IEnumerable<IStreamChannel> channels,
            bool isHardDelete = false)
        {
            StreamAsserts.AssertNotNullOrEmpty(channels, nameof(channels));

            var responseDto = await InternalLowLevelClient.InternalChannelApi.DeleteChannelsAsync(
                new DeleteChannelsRequestInternalDTO
                {
                    Cids = channels.Select(_ => _.Cid).ToList(),
                    HardDelete = isHardDelete
                });

            var response = new StreamDeleteChannelsResponse().UpdateFromDto(responseDto);
            return response;
        }

        public async Task MuteMultipleUsersAsync(IEnumerable<IStreamUser> users, int? timeoutMinutes = default)
        {
            StreamAsserts.AssertNotNullOrEmpty(users, nameof(users));

            var responseDto = await InternalLowLevelClient.InternalModerationApi.MuteUserAsync(
                new MuteUserRequestInternalDTO
                {
                    TargetIds = users.Select(_ => _.Id).ToList(),
                    Timeout = timeoutMinutes
                });

            UpdateLocalUser(responseDto.OwnUser);
        }

        private Task<IEnumerable<IStreamUser>> QueryBannedUsersAsync()
        {
            //StreamTodo: IMPLEMENT, should we allow for query
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            //StreamTodo: disconnect current user

            TryCancelWaitingForUserConnection();

            if (InternalLowLevelClient != null)
            {
                UnsubscribeFrom(InternalLowLevelClient);
                InternalLowLevelClient.Dispose();
            }

            _isDisposed = true;
            Disposed?.Invoke();
        }

        void IStreamChatClientEventsListener.Destroy()
        {
            //StreamTodo: we should probably check: if waiting for connection -> cancel, if connected -> disconnect, etc
            DisconnectUserAsync().ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    _logs.Exception(t.Exception);
                    return;
                }

                Dispose();
            });
        }

        void IStreamChatClientEventsListener.Update() => InternalLowLevelClient.Update(_timeService.DeltaTime);

        internal StreamChatLowLevelClient InternalLowLevelClient { get; }

        internal IStreamLocalUserData UpdateLocalUser(OwnUserInternalDTO ownUserInternalDto)
        {
            _localUserData = _cache.TryCreateOrUpdate(ownUserInternalDto);

            //StreamTodo: Can we not rely on whoever called TryCreateOrUpdate to update this but make it more reliable? Better to react to some event
            // This could be solved if ChannelMutes would be an observable collection
            foreach (var channel in _cache.Channels.AllItems)
            {
                var isMuted = LocalUserData.ChannelMutes.Any(_ => _.Channel == channel);
                channel.Muted = isMuted;
            }

            return _localUserData;
        }

        internal Task RefreshChannelState(string cid)
        {
            if (!_cache.Channels.TryGet(cid, out var channel))
            {
                _logs.Error($"Tried to refresh state of channel with {cid} but no such channel was found in the cache");
                return Task.CompletedTask;
            }

            return GetOrCreateChannelWithIdAsync(channel.Type, channel.Id);
        }

        private readonly ILogs _logs;
        private readonly ITimeService _timeService;
        private readonly ICache _cache;

        private TaskCompletionSource<IStreamLocalUserData> _connectUserTaskSource;
        private CancellationToken _connectUserCancellationToken;
        private CancellationTokenSource _connectUserCancellationTokenSource;
        private bool _isDisposed;

        /// <summary>
        /// Use the <see cref="CreateDefaultClient"/> to create the default client instance.
        /// <example>
        /// Default example::
        /// <code>
        /// var streamChatClient = StreamChatClient.CreateDefaultClient();
        /// </code>
        /// </example>
        /// <example>
        /// Example with custom config:
        /// <code>
        /// var streamChatClient = StreamChatClient.CreateDefaultClient(new StreamClientConfig
        /// {
        ///     LogLevel = StreamLogLevel.Debug
        /// });
        /// </code>
        /// </example>
        /// In case you want to inject custom dependencies into the chat client you can use the <see cref="CreateClientWithCustomDependencies"/>
        /// </summary>
        private StreamChatClient(IWebsocketClient websocketClient, IHttpClient httpClient, ISerializer serializer,
            ITimeService timeService, INetworkMonitor networkMonitor, IApplicationInfo applicationInfo, ILogs logs,
            IStreamClientConfig config)
        {
            _timeService = timeService ?? throw new ArgumentNullException(nameof(timeService));
            _logs = logs ?? throw new ArgumentNullException(nameof(logs));

            InternalLowLevelClient = new StreamChatLowLevelClient(authCredentials: default, websocketClient, httpClient,
                serializer, _timeService, networkMonitor, applicationInfo, logs, config);

            _cache = new Cache(this, serializer, _logs);

            SubscribeTo(InternalLowLevelClient);
        }

        private void InternalDeleteChannel(StreamChannel channel)
        {
            //StreamTodo: mark StreamChannel object as deleted + probably silent clear all internal data?
            _cache.Channels.Remove(channel);
            ChannelDeleted?.Invoke(channel.Cid, channel.Id, channel.Type);
        }

        private void TryCancelWaitingForUserConnection()
        {
            var isConnectTaskRunning = _connectUserTaskSource?.Task != null && !_connectUserTaskSource.Task.IsCompleted;
            var isCancellationRequested = _connectUserCancellationTokenSource?.IsCancellationRequested ?? false;

            if (isConnectTaskRunning && !isCancellationRequested)
            {
#if STREAM_DEBUG_ENABLED
                _logs.Info($"Try Cancel {_connectUserTaskSource}");
#endif
                _connectUserTaskSource.TrySetCanceled();
            }
        }

        #region Events

        private void OnConnected(HealthCheckEventInternalDTO dto)
        {
            try
            {
                var localUserDto = dto.Me;
                UpdateLocalUser(localUserDto);
                Connected?.Invoke(LocalUserData);
            }
            finally
            {
                // This will be null if the ConnectUserAsync with token provider was used
                if (_connectUserTaskSource != null)
                {
                    _connectUserTaskSource.SetResult(LocalUserData);
                    _connectUserTaskSource = null;
                }
            }
        }

        private void OnDisconnected() => Disconnected?.Invoke();

        private void OnConnectionStateChanged(ConnectionState previous, ConnectionState current)
            => ConnectionStateChanged?.Invoke(previous, current);

        private void OnMessageDeleted(MessageDeletedEventInternalDTO eventMessageDeleted)
        {
            if (_cache.Channels.TryGet(eventMessageDeleted.Cid, out var streamChannel))
            {
                streamChannel.HandleMessageDeletedEvent(eventMessageDeleted);
            }
        }

        private void OnMessageUpdated(MessageUpdatedEventInternalDTO eventMessageUpdated)
        {
            if (_cache.Channels.TryGet(eventMessageUpdated.Cid, out var streamChannel))
            {
                streamChannel.HandleMessageUpdatedEvent(eventMessageUpdated);
            }
        }

        private void OnMessageReceived(MessageNewEventInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.HandleMessageNewEvent(eventDto);
            }
        }

        private void OnChannelTruncated(ChannelTruncatedEventInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.HandleChannelTruncatedEvent(eventDto);
            }
        }

        private void OnChannelDeletedNotification(NotificationChannelDeletedEventInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                InternalDeleteChannel(streamChannel);
            }
        }

        private void OnChannelVisible(ChannelVisibleEventInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.Hidden = false;
            }
        }

        private void OnChannelHidden(ChannelHiddenEventInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.Hidden = true;
            }
        }

        private void OnChannelDeleted(ChannelDeletedEventInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                InternalDeleteChannel(streamChannel);
            }
        }

        private void OnChannelUpdated(ChannelUpdatedEventInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.HandleChannelUpdatedEvent(eventDto);
            }
        }

        private void OnChannelTruncatedNotification(
            NotificationChannelTruncatedEventInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.HandleChannelTruncatedEvent(eventDto);
            }
        }

        private void OnChannelMutesUpdatedNotification(NotificationChannelMutesUpdatedEventInternalDTO eventDto)
        {
            UpdateLocalUser(eventDto.Me);
        }

        private void OnMessageReceivedNotification(NotificationNewMessageEventInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.InternalHandleMessageNewNotification(eventDto);
            }
        }

        private void OnMutesUpdatedNotification(NotificationMutesUpdatedEventInternalDTO eventDto)
        {
            UpdateLocalUser(eventDto.Me);
        }

        private void OnMemberAdded(MemberAddedEventInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                var member = _cache.TryCreateOrUpdate(eventDto.Member);
                StreamAsserts.AssertNotNull(member, nameof(member));
                streamChannel.InternalAddMember(member);
            }
        }

        private void OnMemberUpdated(MemberUpdatedEventInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                var member = _cache.TryCreateOrUpdate(eventDto.Member);
                StreamAsserts.AssertNotNull(member, nameof(member));
                streamChannel.InternalUpdateMember(member);
            }
        }

        private void OnMemberRemoved(MemberRemovedEventInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                var member = _cache.TryCreateOrUpdate(eventDto.Member);
                StreamAsserts.AssertNotNull(member, nameof(member));
                streamChannel.InternalRemoveMember(member);
            }
        }

        private void OnMessageRead(MessageReadEventInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.InternalHandleMessageReadEvent(eventDto);
            }
        }

        private void OnMarkReadNotification(NotificationMarkReadEventInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.InternalHandleMessageReadNotification(eventDto);
            }

            _localUserData.InternalHandleMarkReadNotification(eventDto);
        }

        private void OnAddedToChannelNotification(NotificationAddedToChannelEventInternalDTO obj)
        {
            //StreamTodo: IMPLEMENT
        }

        private void OnRemovedFromChannelNotification(
            NotificationRemovedFromChannelEventInternalDTO obj)
        {
//StreamTodo: IMPLEMENT
        }

        private void OnInvitedNotification(NotificationInvitedEventInternalDTO eventDto)
        {
            var channel = _cache.TryCreateOrUpdate(eventDto.Channel);
            var user = _cache.TryCreateOrUpdate(eventDto.User);

            ChannelInviteReceived?.Invoke(channel, user);
        }

        private void OnInviteAcceptedNotification(NotificationInviteAcceptedEventInternalDTO eventDto)
        {
            var channel = _cache.TryCreateOrUpdate(eventDto.Channel);
            var user = _cache.TryCreateOrUpdate(eventDto.User);

            ChannelInviteAccepted?.Invoke(channel, user);
        }

        private void OnInviteRejectedNotification(NotificationInviteRejectedEventInternalDTO eventDto)
        {
            var channel = _cache.TryCreateOrUpdate(eventDto.Channel);
            var user = _cache.TryCreateOrUpdate(eventDto.User);

            ChannelInviteRejected?.Invoke(channel, user);
        }

        private void OnReactionReceived(ReactionNewEventInternalDTO eventDto)
        {
            if (!_cache.Channels.TryGet(eventDto.Cid, out var channel))
            {
                return;
            }

            if (_cache.Messages.TryGet(eventDto.Message.Id, out var message))
            {
                var reaction = new StreamReaction().TryLoadFromDto(eventDto.Reaction, _cache);
                message.HandleReactionNewEvent(eventDto, channel, reaction);
                channel.InternalNotifyReactionReceived(message, reaction);
            }
        }

        private void OnReactionUpdated(ReactionUpdatedEventInternalDTO eventDto)
        {
            if (!_cache.Channels.TryGet(eventDto.Cid, out var channel))
            {
                return;
            }

            if (_cache.Messages.TryGet(eventDto.Message.Id, out var message))
            {
                var reaction = new StreamReaction().TryLoadFromDto(eventDto.Reaction, _cache);
                message.HandleReactionUpdatedEvent(eventDto, channel, reaction);
                channel.InternalNotifyReactionUpdated(message, reaction);
            }
        }

        private void OnReactionDeleted(ReactionDeletedEventInternalDTO eventDto)
        {
            if (!_cache.Channels.TryGet(eventDto.Cid, out var channel))
            {
                return;
            }

            if (_cache.Messages.TryGet(eventDto.Message.Id, out var message))
            {
                var reaction = new StreamReaction().TryLoadFromDto(eventDto.Reaction, _cache);
                message.HandleReactionDeletedEvent(eventDto, channel, reaction);
                channel.InternalNotifyReactionDeleted(message, reaction);
            }
        }

        private void OnUserWatchingStop(UserWatchingStopEventInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.InternalHandleUserWatchingStop(eventDto);
            }
        }

        private void OnUserWatchingStart(UserWatchingStartEventInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.InternalHandleUserWatchingStartEvent(eventDto);
            }
        }

        private void OnLowLevelClientUserUnbanned(UserUnbannedEventInternalDTO obj)
        {
            //StreamTodo: IMPLEMENT
        }

        private void OnLowLevelClientUserBanned(UserBannedEventInternalDTO obj)
        {
            //StreamTodo: IMPLEMENT
        }

        private void OnLowLevelClientUserDeleted(UserDeletedEventInternalDTO obj)
        {
            //StreamTodo: IMPLEMENT
        }

        private void OnLowLevelUserUpdated(UserUpdatedEventInternalDTO eventDto)
        {
            if (_cache.Users.TryGet(eventDto.User.Id, out var streamUser))
            {
                _cache.TryCreateOrUpdate(eventDto.User);
            }
        }

        private void OnUserPresenceChanged(UserPresenceChangedEventInternalDTO eventDto)
        {
            if (_cache.Users.TryGet(eventDto.User.Id, out var streamUser))
            {
                streamUser.InternalHandlePresenceChanged(eventDto);
            }
        }

        private void OnTypingStopped(TypingStopEventInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.InternalHandleTypingStopped(eventDto);
            }
        }

        private void OnTypingStarted(TypingStartEventInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.InternalHandleTypingStarted(eventDto);
            }
        }

        private void SubscribeTo(StreamChatLowLevelClient lowLevelClient)
        {
            lowLevelClient.InternalConnected += OnConnected;
            lowLevelClient.Disconnected += OnDisconnected;
            lowLevelClient.ConnectionStateChanged += OnConnectionStateChanged;

            lowLevelClient.InternalMessageReceived += OnMessageReceived;
            lowLevelClient.InternalMessageUpdated += OnMessageUpdated;
            lowLevelClient.InternalMessageDeleted += OnMessageDeleted;
            lowLevelClient.InternalMessageRead += OnMessageRead;

            lowLevelClient.InternalChannelUpdated += OnChannelUpdated;
            lowLevelClient.InternalChannelDeleted += OnChannelDeleted;
            lowLevelClient.InternalChannelTruncated += OnChannelTruncated;
            lowLevelClient.InternalChannelVisible += OnChannelVisible;
            lowLevelClient.InternalChannelHidden += OnChannelHidden;

            lowLevelClient.InternalMemberAdded += OnMemberAdded;
            lowLevelClient.InternalMemberRemoved += OnMemberRemoved;
            lowLevelClient.InternalMemberUpdated += OnMemberUpdated;

            lowLevelClient.InternalUserPresenceChanged += OnUserPresenceChanged;
            lowLevelClient.InternalUserUpdated += OnLowLevelUserUpdated;
            lowLevelClient.InternalUserDeleted += OnLowLevelClientUserDeleted;
            lowLevelClient.InternalUserBanned += OnLowLevelClientUserBanned;
            lowLevelClient.InternalUserUnbanned += OnLowLevelClientUserUnbanned;

            lowLevelClient.InternalUserWatchingStart += OnUserWatchingStart;
            lowLevelClient.InternalUserWatchingStop += OnUserWatchingStop;

            lowLevelClient.InternalReactionReceived += OnReactionReceived;
            lowLevelClient.InternalReactionUpdated += OnReactionUpdated;
            lowLevelClient.InternalReactionDeleted += OnReactionDeleted;

            lowLevelClient.InternalTypingStarted += OnTypingStarted;
            lowLevelClient.InternalTypingStopped += OnTypingStopped;

            lowLevelClient.InternalNotificationChannelMutesUpdated += OnChannelMutesUpdatedNotification;

            lowLevelClient.InternalNotificationMutesUpdated += OnMutesUpdatedNotification;
            lowLevelClient.InternalNotificationMessageReceived += OnMessageReceivedNotification;
            lowLevelClient.InternalNotificationMarkRead += OnMarkReadNotification;

            lowLevelClient.InternalNotificationChannelDeleted += OnChannelDeletedNotification;
            lowLevelClient.InternalNotificationChannelTruncated += OnChannelTruncatedNotification;

            lowLevelClient.InternalNotificationAddedToChannel += OnAddedToChannelNotification;
            lowLevelClient.InternalNotificationRemovedFromChannel += OnRemovedFromChannelNotification;

            lowLevelClient.InternalNotificationInvited += OnInvitedNotification;
            lowLevelClient.InternalNotificationInviteAccepted += OnInviteAcceptedNotification;
            lowLevelClient.InternalNotificationInviteRejected += OnInviteRejectedNotification;
        }

        private void UnsubscribeFrom(StreamChatLowLevelClient lowLevelClient)
        {
            lowLevelClient.InternalConnected -= OnConnected;
            lowLevelClient.Disconnected -= OnDisconnected;
            lowLevelClient.ConnectionStateChanged -= OnConnectionStateChanged;

            lowLevelClient.InternalMessageReceived -= OnMessageReceived;
            lowLevelClient.InternalMessageUpdated -= OnMessageUpdated;
            lowLevelClient.InternalMessageDeleted -= OnMessageDeleted;
            lowLevelClient.InternalMessageRead -= OnMessageRead;

            lowLevelClient.InternalChannelUpdated -= OnChannelUpdated;
            lowLevelClient.InternalChannelDeleted -= OnChannelDeleted;
            lowLevelClient.InternalChannelTruncated -= OnChannelTruncated;
            lowLevelClient.InternalChannelVisible -= OnChannelVisible;
            lowLevelClient.InternalChannelHidden -= OnChannelHidden;

            lowLevelClient.InternalMemberAdded -= OnMemberAdded;
            lowLevelClient.InternalMemberRemoved -= OnMemberRemoved;
            lowLevelClient.InternalMemberUpdated -= OnMemberUpdated;

            lowLevelClient.InternalUserPresenceChanged -= OnUserPresenceChanged;
            lowLevelClient.InternalUserUpdated -= OnLowLevelUserUpdated;
            lowLevelClient.InternalUserDeleted -= OnLowLevelClientUserDeleted;
            lowLevelClient.InternalUserBanned -= OnLowLevelClientUserBanned;
            lowLevelClient.InternalUserUnbanned -= OnLowLevelClientUserUnbanned;

            lowLevelClient.InternalUserWatchingStart -= OnUserWatchingStart;
            lowLevelClient.InternalUserWatchingStop -= OnUserWatchingStop;

            lowLevelClient.InternalReactionReceived -= OnReactionReceived;
            lowLevelClient.InternalReactionUpdated -= OnReactionUpdated;
            lowLevelClient.InternalReactionDeleted -= OnReactionDeleted;

            lowLevelClient.InternalTypingStarted -= OnTypingStarted;
            lowLevelClient.InternalTypingStopped -= OnTypingStopped;

            lowLevelClient.InternalNotificationChannelMutesUpdated -= OnChannelMutesUpdatedNotification;

            lowLevelClient.InternalNotificationMutesUpdated -= OnMutesUpdatedNotification;
            lowLevelClient.InternalNotificationMessageReceived -= OnMessageReceivedNotification;
            lowLevelClient.InternalNotificationMarkRead -= OnMarkReadNotification;

            lowLevelClient.InternalNotificationChannelDeleted -= OnChannelDeletedNotification;
            lowLevelClient.InternalNotificationChannelTruncated -= OnChannelTruncatedNotification;

            lowLevelClient.InternalNotificationAddedToChannel -= OnAddedToChannelNotification;
            lowLevelClient.InternalNotificationRemovedFromChannel -= OnRemovedFromChannelNotification;

            lowLevelClient.InternalNotificationInvited -= OnInvitedNotification;
            lowLevelClient.InternalNotificationInviteAccepted -= OnInviteAcceptedNotification;
            lowLevelClient.InternalNotificationInviteRejected -= OnInviteRejectedNotification;
        }

        #endregion
    }
}