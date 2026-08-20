using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using StreamChat.Core.Auth;
using StreamChat.Core.Configs;
using StreamChat.Core.Exceptions;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.LowLevelClient.API;
using StreamChat.Core.LowLevelClient.API.Internal;
using StreamChat.Core.LowLevelClient.Events;
using StreamChat.Core.LowLevelClient.Models;
using StreamChat.Core.Web;
using StreamChat.Libs;
using StreamChat.Libs.AppInfo;
using StreamChat.Libs.Auth;
using StreamChat.Libs.Http;
using StreamChat.Libs.Logs;
using StreamChat.Libs.NetworkMonitors;
using StreamChat.Libs.Serialization;
using StreamChat.Libs.Time;
using StreamChat.Libs.Utils;
using StreamChat.Libs.Websockets;
using StreamChat.Core.LowLevelClient.Requests;
using System.Linq;
using StreamChat.Core.Helpers;
using Thread = System.Threading.Thread;

#if STREAM_TESTS_ENABLED || STREAM_RUNTIME_TESTS_ENABLED
using System.Runtime.CompilerServices;
#endif

#if STREAM_TESTS_ENABLED
[assembly: InternalsVisibleTo("StreamChat.Tests")]
#endif
#if STREAM_TESTS_ENABLED || STREAM_RUNTIME_TESTS_ENABLED
[assembly: InternalsVisibleTo("StreamChat.Tests.Runtime")]
#endif

namespace StreamChat.Core.LowLevelClient
{
    /// <inheritdoc cref="IStreamChatLowLevelClient"/>
    public class StreamChatLowLevelClient : IStreamChatLowLevelClient
    {
        public const string MenuPrefix = "Stream/";

        public static readonly Uri ServerBaseUrl = new Uri("wss://chat.stream-io-api.com");

        public event ConnectionHandler Connected;
        public event Action Reconnecting;
        public event Action Disconnected;
        public event ConnectionStateChangeHandler ConnectionStateChanged;

        public event Action<string> EventReceived;

        public event Action<EventMessageNew> MessageReceived;
        public event Action<EventMessageUpdated> MessageUpdated;
        public event Action<EventMessageDeleted> MessageDeleted;
        public event Action<EventMessageRead> MessageRead;

        public event Action<EventChannelUpdated> ChannelUpdated;
        public event Action<EventChannelDeleted> ChannelDeleted;
        public event Action<EventChannelTruncated> ChannelTruncated;
        public event Action<EventChannelVisible> ChannelVisible;
        public event Action<EventChannelHidden> ChannelHidden;

        public event Action<EventMemberAdded> MemberAdded;
        public event Action<EventMemberRemoved> MemberRemoved;
        public event Action<EventMemberUpdated> MemberUpdated;

        public event Action<EventUserPresenceChanged> UserPresenceChanged;
        public event Action<EventUserUpdated> UserUpdated;
        public event Action<EventUserDeleted> UserDeleted;
        public event Action<EventUserBanned> UserBanned;
        public event Action<EventUserUnbanned> UserUnbanned;

        public event Action<EventUserWatchingStart> UserWatchingStart;
        public event Action<EventUserWatchingStop> UserWatchingStop;

        public event Action<EventReactionNew> ReactionReceived;
        public event Action<EventReactionUpdated> ReactionUpdated;
        public event Action<EventReactionDeleted> ReactionDeleted;

        public event Action<EventTypingStart> TypingStarted;
        public event Action<EventTypingStop> TypingStopped;

        public event Action<EventCustom> CustomEventReceived;

        public event Action<EventNotificationChannelMutesUpdated> NotificationChannelMutesUpdated;
        public event Action<EventNotificationMutesUpdated> NotificationMutesUpdated;


        public event Action<EventNotificationMessageNew> NotificationMessageReceived;
        public event Action<EventNotificationMarkRead> NotificationMarkRead;

        public event Action<EventNotificationChannelDeleted> NotificationChannelDeleted;
        public event Action<EventNotificationChannelTruncated> NotificationChannelTruncated;

        public event Action<EventNotificationAddedToChannel> NotificationAddedToChannel;
        public event Action<EventNotificationRemovedFromChannel> NotificationRemovedFromChannel;

        public event Action<EventNotificationInvited> NotificationInvited;
        public event Action<EventNotificationInviteAccepted> NotificationInviteAccepted;
        public event Action<EventNotificationInviteRejected> NotificationInviteRejected;

        public event Action<EventPollClosed> PollClosed;
        public event Action<EventPollDeleted> PollDeleted;
        public event Action<EventPollUpdated> PollUpdated;
        public event Action<EventPollVoteCasted> PollVoteCasted;
        public event Action<EventPollVoteChanged> PollVoteChanged;
        public event Action<EventPollVoteRemoved> PollVoteRemoved;

        public event Action<EventThreadUpdated> ThreadUpdated;
        public event Action<EventNotificationThreadMessageNew> NotificationThreadMessageNew;
        public event Action<EventNotificationMarkUnread> NotificationMarkUnread;

        #region Internal Events

        internal event Action<HealthCheckEventInternalDTO> InternalConnected;

        internal event Action<MessageNewEventInternalDTO> InternalMessageReceived;
        internal event Action<MessageUpdatedEventInternalDTO> InternalMessageUpdated;
        internal event Action<MessageDeletedEventInternalDTO> InternalMessageDeleted;
        internal event Action<MessageReadEventInternalDTO> InternalMessageRead;

        internal event Action<ChannelUpdatedEventInternalDTO> InternalChannelUpdated;
        internal event Action<ChannelDeletedEventInternalDTO> InternalChannelDeleted;
        internal event Action<ChannelTruncatedEventInternalDTO> InternalChannelTruncated;
        internal event Action<ChannelVisibleEventInternalDTO> InternalChannelVisible;
        internal event Action<ChannelHiddenEventInternalDTO> InternalChannelHidden;

        internal event Action<MemberAddedEventInternalDTO> InternalMemberAdded;
        internal event Action<MemberRemovedEventInternalDTO> InternalMemberRemoved;
        internal event Action<MemberUpdatedEventInternalDTO> InternalMemberUpdated;

        internal event Action<UserPresenceChangedEventInternalDTO> InternalUserPresenceChanged;
        internal event Action<UserUpdatedEventInternalDTO> InternalUserUpdated;
        internal event Action<UserDeletedEventInternalDTO> InternalUserDeleted;
        internal event Action<UserBannedEventInternalDTO> InternalUserBanned;
        internal event Action<UserUnbannedEventInternalDTO> InternalUserUnbanned;

        internal event Action<UserWatchingStartEventInternalDTO> InternalUserWatchingStart;
        internal event Action<UserWatchingStopEventInternalDTO> InternalUserWatchingStop;

        internal event Action<ReactionNewEventInternalDTO> InternalReactionReceived;
        internal event Action<ReactionUpdatedEventInternalDTO> InternalReactionUpdated;
        internal event Action<ReactionDeletedEventInternalDTO> InternalReactionDeleted;

        internal event Action<TypingStartEventInternalDTO> InternalTypingStarted;
        internal event Action<TypingStopEventInternalDTO> InternalTypingStopped;

        internal event Action<CustomEventInternalDTO> InternalCustomEventReceived;

        internal event Action<NotificationChannelMutesUpdatedEventInternalDTO> InternalNotificationChannelMutesUpdated;
        internal event Action<NotificationMutesUpdatedEventInternalDTO> InternalNotificationMutesUpdated;

        internal event Action<NotificationNewMessageEventInternalDTO> InternalNotificationMessageReceived;
        internal event Action<NotificationMarkReadEventInternalDTO> InternalNotificationMarkRead;

        internal event Action<NotificationChannelDeletedEventInternalDTO> InternalNotificationChannelDeleted;
        internal event Action<NotificationChannelTruncatedEventInternalDTO> InternalNotificationChannelTruncated;

        internal event Action<NotificationAddedToChannelEventInternalDTO> InternalNotificationAddedToChannel;
        internal event Action<NotificationRemovedFromChannelEventInternalDTO> InternalNotificationRemovedFromChannel;

        internal event Action<NotificationInvitedEventInternalDTO> InternalNotificationInvited;
        internal event Action<NotificationInviteAcceptedEventInternalDTO> InternalNotificationInviteAccepted;
        internal event Action<NotificationInviteRejectedEventInternalDTO> InternalNotificationInviteRejected;

        internal event Action<PollClosedEventInternalDTO> InternalPollClosed;
        internal event Action<PollDeletedEventInternalDTO> InternalPollDeleted;
        internal event Action<PollUpdatedEventInternalDTO> InternalPollUpdated;
        internal event Action<PollVoteCastedEventInternalDTO> InternalPollVoteCasted;
        internal event Action<PollVoteChangedEventInternalDTO> InternalPollVoteChanged;
        internal event Action<PollVoteRemovedEventInternalDTO> InternalPollVoteRemoved;

        internal event Action<ThreadUpdatedEventInternalDTO> InternalThreadUpdated;
        internal event Action<NotificationThreadMessageNewEventInternalDTO> InternalNotificationThreadMessageNew;
        internal event Action<NotificationMarkUnreadEventInternalDTO> InternalNotificationMarkUnread;

        #endregion

        public IChannelApi ChannelApi { get; }
        public IMessageApi MessageApi { get; }
        public IModerationApi ModerationApi { get; }
        public IUserApi UserApi { get; }
        public IDeviceApi DeviceApi { get; }
        public IPollsApi PollsApi { get; }

        [Obsolete(
            "This property presents only initial state of the LocalUser when connection is made and is not ever updated. " +
            "Please use the OwnUser object returned from StreamChatClient.Connected event. This property will  be removed in the future.")]
        public OwnUser LocalUser { get; private set; }

        public ConnectionState ConnectionState
        {
            get => _connectionState;
            private set
            {
                if (_connectionState == value)
                {
                    return;
                }

                var previous = _connectionState;
                _connectionState = value;

#if STREAM_DEBUG_ENABLED
                _logs.Warning($"Connection state changed from: {previous} to: {value}");
#endif

                RaiseConnectionStateChanged(previous, _connectionState);

                if (value == ConnectionState.Disconnected)
                {
                    _disconnectionLastEventReceivedAt = _lastEventReceivedAt;
                    RaiseDisconnected();
                }
            }
        }

        //StreamTodo: wrap all params in a ReconnectPolicy object
        public ReconnectStrategy ReconnectStrategy => _reconnectScheduler.ReconnectStrategy;
        public float ReconnectConstantInterval => _reconnectScheduler.ReconnectConstantInterval;
        public float ReconnectExponentialMinInterval => _reconnectScheduler.ReconnectExponentialMinInterval;
        public float ReconnectExponentialMaxInterval => _reconnectScheduler.ReconnectExponentialMaxInterval;
        public int ReconnectMaxInstantTrials => _reconnectScheduler.ReconnectMaxInstantTrials;
        public double? NextReconnectTime => _reconnectScheduler.NextReconnectTime;

        /// <summary>
        /// SDK Version number
        /// </summary>
        public static readonly Version SDKVersion = new Version(5, 7, 0);

        /// <summary>
        /// Use this method to create the main client instance or use StreamChatClient constructor to create a client instance with custom dependencies
        /// </summary>
        /// <param name="authCredentials">Authorization data with ApiKey, UserToken and UserId</param>
        public static IStreamChatLowLevelClient CreateDefaultClient(AuthCredentials authCredentials,
            IStreamClientConfig config = default)
        {
            if (config == null)
            {
                config = StreamClientConfig.Default;
            }

            var logs = StreamDependenciesFactory.CreateLogger(config.LogLevel.ToLogLevel());
            var applicationInfo = StreamDependenciesFactory.CreateApplicationInfo();
            var websocketClient
                = StreamDependenciesFactory.CreateWebsocketClient(logs, isDebugMode: config.LogLevel.IsDebugEnabled());
            var httpClient = StreamDependenciesFactory.CreateHttpClient();
            var serializer = StreamDependenciesFactory.CreateSerializer();
            var timeService = StreamDependenciesFactory.CreateTimeService();
            var networkMonitor = StreamDependenciesFactory.CreateNetworkMonitor();

            return new StreamChatLowLevelClient(authCredentials, websocketClient, httpClient, serializer,
                timeService, networkMonitor, applicationInfo, logs, config);
        }

        /// <summary>
        /// Create Development Authorization Token. Dev tokens work only if you enable "Disable Auth Checks" in your project's Dashboard.
        /// Dev tokens bypasses authorization and should only be used during development and never in production!
        /// More info <see cref="https://getstream.io/chat/docs/unity/tokens_and_authentication/?language=unity#developer-tokens"/>
        /// </summary>
        public static string CreateDeveloperAuthToken(string userId)
        {
            if (!IsUserIdValid(userId))
            {
                throw new ArgumentException($"{nameof(userId)} can only contain: a-z, 0-9, @, _ and - ");
            }

            var header = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9"; //  header content = {"alg": "HS256", "typ": "JWT"}
            var devSignature = "devToken";

            var payloadBytes = Encoding.UTF8.GetBytes("{\"user_id\":\"" + userId + "\"}");
            var payload = Base64UrlEncode(payloadBytes);
            return $"{header}.{payload}.{devSignature}";
        }

        /// <summary>
        /// Strip invalid characters from a given Stream user id. The only allowed characters are: a-z, 0-9, @, _ and -
        /// </summary>
        public static string SanitizeUserId(string userId)
        {
            if (IsUserIdValid(userId))
            {
                return userId;
            }

            return Regex.Replace(userId, @"[^\w\.@_-]", "", RegexOptions.None, TimeSpan.FromSeconds(1));
        }

        public StreamChatLowLevelClient(AuthCredentials authCredentials, IWebsocketClient websocketClient,
            IHttpClient httpClient, ISerializer serializer, ITimeService timeService, INetworkMonitor networkMonitor,
            IApplicationInfo applicationInfo, ILogs logs, IStreamClientConfig config)
        {
            _mainThreadId = Thread.CurrentThread.ManagedThreadId;

            _authCredentials = authCredentials;
            _websocketClient = websocketClient ?? throw new ArgumentNullException(nameof(websocketClient));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _timeService = timeService ?? throw new ArgumentNullException(nameof(timeService));
            _networkMonitor = networkMonitor ?? throw new ArgumentNullException(nameof(networkMonitor));
            applicationInfo = applicationInfo ?? throw new ArgumentNullException(nameof(applicationInfo));
            _logs = logs ?? throw new ArgumentNullException(nameof(logs));
            _config = config ?? throw new ArgumentNullException(nameof(config));

            _logs.Prefix = "[Stream Chat] ";

            var streamClientHeader = BuildStreamClientHeader(applicationInfo);
            _requestUriFactory = new RequestUriFactory(authProvider: this, connectionProvider: this, _serializer,
                streamClientHeader);

            _httpClient.AddDefaultCustomHeader("stream-auth-type", DefaultStreamAuthType);
            _httpClient.AddDefaultCustomHeader("X-Stream-Client", streamClientHeader);

            _websocketClient.ConnectionFailed += OnWebsocketsConnectionFailed;
            _websocketClient.Disconnected += OnWebsocketDisconnected;

            InternalChannelApi
                = new InternalChannelApi(httpClient, serializer, logs, _requestUriFactory, lowLevelClient: this);
            InternalMessageApi
                = new InternalMessageApi(httpClient, serializer, logs, _requestUriFactory, lowLevelClient: this);
            InternalModerationApi
                = new InternalModerationApi(httpClient, serializer, logs, _requestUriFactory, lowLevelClient: this);
            InternalUserApi
                = new InternalUserApi(httpClient, serializer, logs, _requestUriFactory, lowLevelClient: this);
            InternalDeviceApi
                = new InternalDeviceApi(httpClient, serializer, logs, _requestUriFactory, lowLevelClient: this);
            InternalPollsApi
                = new InternalPollsApi(httpClient, serializer, logs, _requestUriFactory, lowLevelClient: this);
            InternalThreadsApi
                = new InternalThreadsApi(httpClient, serializer, logs, _requestUriFactory, lowLevelClient: this);

            ChannelApi = new ChannelApi(InternalChannelApi);
            MessageApi = new MessageApi(InternalMessageApi);
            ModerationApi = new ModerationApi(InternalModerationApi);
            UserApi = new UserApi(InternalUserApi);
            DeviceApi = new DeviceApi(InternalDeviceApi);
            PollsApi = new PollsApi(InternalPollsApi);

            _reconnectScheduler = new ReconnectScheduler(_timeService, this, _networkMonitor, _logs);
            _reconnectScheduler.ReconnectionScheduled += OnReconnectionScheduled;

            RegisterEventHandlers();

            LogErrorIfUpdateIsNotBeingCalled();
        }

        public void ConnectUser(AuthCredentials userAuthCredentials)
        {
            SeAuthorizationCredentials(userAuthCredentials);
            Connect();
        }

        public void Connect()
        {
            SeAuthorizationCredentials(_authCredentials);

            if (!ConnectionState.IsValidToConnect())
            {
                throw new InvalidOperationException("Attempted to connect, but client is in state: " + ConnectionState);
            }

            TryCancelWaitingForUserConnection();

            //StreamTodo: hidden dependency on SetUser being called
            var connectionUri = _requestUriFactory.CreateConnectionUri();

            _logs.Info($"Attempt to connect");

            ConnectionState = ConnectionState.Connecting;

            _websocketClient.ConnectAsync(connectionUri).LogIfFailed(_logs);
        }

        public void SeAuthorizationCredentials(AuthCredentials authCredentials)
        {
            if (authCredentials.IsAnyEmpty())
            {
                throw new StreamMissingAuthCredentialsException(
                    "Please provide valid credentials: `Api Key`, 'User id`, `User token`");
            }

            _authCredentials = authCredentials;
            _httpClient.SetDefaultAuthenticationHeader(authCredentials.UserToken);
        }

        public async Task DisconnectAsync(bool permanent = false)
        {
            TryCancelWaitingForUserConnection();
            //StreamTodo: remove this, this cannot be used when internal disconnect due to expired token. Perhaps we should allow user to Suspend() and Unsupend() the client reconnection

            if (permanent)
            {
                _reconnectScheduler.Stop();
            }

            await _websocketClient.DisconnectAsync(WebSocketCloseStatus.NormalClosure, "User called Disconnect");
        }

        public void Update(float deltaTime)
        {
            _networkMonitor?.Update();

#if !STREAM_TESTS_ENABLED
            _updateCallReceived = true;
#endif

            TryHandleWebsocketsConnectionFailed();
            TryHandleWebsocketDisconnected();
            TryToReconnect();

            UpdateHealthCheck();

            _websocketClient.Update();

            while (_websocketClient.TryDequeueMessage(out var msg))
            {
#if STREAM_DEBUG_ENABLED
                _logs.Info(_authCredentials.UserId + " WS message: " + msg);
#endif
                HandleNewWebsocketMessage(msg, isLiveEvent: true);
            }
        }

        public bool IsLocalUser(User user) => user.Id == _authCredentials.UserId;

        public bool IsLocalUser(ChannelMember channelMember) => channelMember.User.Id == _authCredentials.UserId;

        //StreamTodo: move this to injected config object
        public void SetReconnectStrategySettings(ReconnectStrategy reconnectStrategy, float? exponentialMinInterval,
            float? exponentialMaxInterval, float? constantInterval)
        {
            _reconnectScheduler.SetReconnectStrategySettings(reconnectStrategy, exponentialMinInterval,
                exponentialMaxInterval, constantInterval);
        }

        public async Task FetchAndProcessEventsSinceLastReceivedEvent(IEnumerable<string> channelCids)
        {
            var response = await TrySyncHistoryAsync(channelCids);
            ReplayHistoryEvents(response?.Events);
        }

        /// <summary>
        /// The <c>/sync</c> endpoint counts events summed across every requested cid against a
        /// server-side ceiling of roughly 1000 and refuses the whole request once it is exceeded, so
        /// asking about fewer channels makes a successful catch-up more likely. Swift and Android both
        /// cap the request at 100 cids; this matches them.
        /// </summary>
        internal const int MaxSyncChannelCids = 100;

        /// <summary>
        /// Best-effort <c>/sync</c> for up to <see cref="MaxSyncChannelCids"/> channels. Returns
        /// <c>null</c> when catch-up is skipped, which happens when there is no sync point to catch up
        /// from or when the sync point is older than the 30 days the server accepts.
        /// </summary>
        internal async Task<Responses.SyncResponse> TrySyncHistoryAsync(IEnumerable<string> channelCids)
        {
            if (channelCids == null || !_disconnectionLastEventReceivedAt.HasValue)
            {
                return null;
            }

            var lastEventReceivedAt = _disconnectionLastEventReceivedAt.Value;

            if ((_timeService.Now - lastEventReceivedAt).TotalDays > 30)
            {
                return null;
            }

            // Released only once the request has completed: the request body is serialized from this
            // list, and an auth retry re-serializes it after the first attempt has already awaited.
            using (new ListPoolScope<string>(out var cids))
            {
                foreach (var cid in channelCids)
                {
                    if (cids.Count == MaxSyncChannelCids)
                    {
                        break;
                    }

                    cids.Add(cid);
                }

                if (cids.Count == 0)
                {
                    return null;
                }

                return await ChannelApi.SyncAsync(new SyncRequest
                {
                    ChannelCids = cids,
                    LastSyncAt = lastEventReceivedAt,
                    Watch = true,

                    // Lets the caller tell "the server will never return this channel again" apart from
                    // "the query happened to omit it", so recovery can stop retrying channels that were
                    // deleted or that the local user lost access to while offline.
                    WithInaccessibleCids = true,
                });
            }
        }

        /// <summary>
        /// Replay history events through the live event pipeline, so every per-event public callback
        /// fires exactly as it would for a real-time event. This is
        /// <see cref="StateRecoveryStrategy.ReplayEvents"/> and the behaviour of
        /// <see cref="FetchAndProcessEventsSinceLastReceivedEvent"/>.
        /// </summary>
        internal void ReplayHistoryEvents(IEnumerable<object> events)
        {
            if (events == null)
            {
                return;
            }

            foreach (var e in events)
            {
                // Each event is isolated by the try/catch inside the registered handler, so one
                // malformed event cannot abandon the rest of the replay.
                HandleNewWebsocketMessage(SerializeHistoryEvent(e));
            }
        }

        /// <summary>
        /// Apply history events to local state without raising the per-event public callbacks whose
        /// effect is observable in model state afterwards. This is
        /// <see cref="StateRecoveryStrategy.BatchStateUpdate"/>. Mirrors Android's
        /// <c>isFromHistorySync</c> and Swift's <c>postNotifications: false</c>.
        /// </summary>
        internal HistorySyncApplyResult ApplyHistoryEvents(IEnumerable<object> events)
        {
            var result = new HistorySyncApplyResult();
            if (events == null)
            {
                return result;
            }

            _isApplyingHistoryEvents = true;
            _historyMaxAppliedCreatedAt = null;

            try
            {
                foreach (var e in events)
                {
                    try
                    {
                        HandleNewWebsocketMessage(SerializeHistoryEvent(e));
                    }
                    catch (Exception ex)
                    {
                        // Only count and log. Abandoning the batch would leave state half-applied,
                        // and the watermark below only ever advances to the newest event that was
                        // actually applied, so a partial batch is retried on the next reconnect.
                        result.FailedEventCount++;
                        _logs.Exception(ex);
                    }
                }
            }
            finally
            {
                result.MaxAppliedCreatedAt = _historyMaxAppliedCreatedAt;
                _historyMaxAppliedCreatedAt = null;
                _isApplyingHistoryEvents = false;

                if (result.MaxAppliedCreatedAt.HasValue)
                {
                    TryAdvanceLastEventReceivedAt(result.MaxAppliedCreatedAt.Value, HistorySyncWatermarkSource);
                }
            }

            return result;
        }

        /// <summary>
        /// True while <see cref="ApplyHistoryEvents"/> is running.
        /// </summary>
        internal bool IsApplyingHistoryEvents => _isApplyingHistoryEvents;

        private const string HistorySyncWatermarkSource = "history.sync";

        // The batch advances the watermark once, at the end, and only to the newest event it managed
        // to apply. Advancing per event would let a throwing event in the middle leave a watermark
        // claiming a catch-up that did not happen.
        private void RecordHistoryWatermark(DateTimeOffset createdAt)
        {
            if (createdAt == DateTimeOffset.MinValue)
            {
                return;
            }

            if (!_historyMaxAppliedCreatedAt.HasValue || createdAt > _historyMaxAppliedCreatedAt.Value)
            {
                _historyMaxAppliedCreatedAt = createdAt;
            }
        }

        private string SerializeHistoryEvent(object e)
        {
            if (e is string serialized)
            {
                return serialized;
            }

            return _serializer.Serialize(e);
        }

        public void Dispose()
        {
            ConnectionState = ConnectionState.Closing;

            _reconnectScheduler.Dispose();

            TryCancelWaitingForUserConnection();

            _websocketClient.ConnectionFailed -= OnWebsocketsConnectionFailed;
            _websocketClient.Disconnected -= OnWebsocketDisconnected;
            _websocketClient.Dispose();

            _updateMonitorCts.Cancel();
        }

        string IAuthProvider.ApiKey => _authCredentials.ApiKey;
        string IAuthProvider.UserToken => _authCredentials.UserToken;
        string IAuthProvider.UserId => _authCredentials.UserId;
        string IAuthProvider.StreamAuthType => DefaultStreamAuthType;
        string IConnectionProvider.ConnectionId => _connectionId;
        Uri IConnectionProvider.ServerUri => ServerBaseUrl;

        internal IInternalChannelApi InternalChannelApi { get; }
        internal IInternalMessageApi InternalMessageApi { get; }
        internal IInternalModerationApi InternalModerationApi { get; }
        internal InternalUserApi InternalUserApi { get; }
        internal IInternalDeviceApi InternalDeviceApi { get; }
        internal IInternalPollsApi InternalPollsApi { get; }

        internal IInternalThreadsApi InternalThreadsApi { get; }

        internal IStreamClientConfig Config => _config;

        internal async Task<OwnUserInternalDTO> ConnectUserAsync(string apiKey, string userId,
            ITokenProvider tokenProvider, CancellationToken cancellationToken = default)
        {
            if (!ConnectionState.IsValidToConnect())
            {
                throw new InvalidOperationException("Attempted to connect, but client is in state: " + ConnectionState);
            }

            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            SetPartialConnectionCredentials(apiKey, userId);

            TryCancelWaitingForUserConnection();

            ConnectionState = ConnectionState.Connecting;

            _connectUserCancellationToken = cancellationToken;

            _connectUserCancellationTokenSource
                = CancellationTokenSource.CreateLinkedTokenSource(_connectUserCancellationToken);
            _connectUserCancellationTokenSource.Token.Register(TryCancelWaitingForUserConnection);

            _connectUserTaskSource = new TaskCompletionSource<OwnUserInternalDTO>();

            try
            {
                await RefreshAuthTokenFromProvider();

                var connectionUri = _requestUriFactory.CreateConnectionUri();

                //StreamTodo: pass the cancellation token here cancellationToken
                await _websocketClient.ConnectAsync(connectionUri);

                var ownUserDto = await _connectUserTaskSource.Task;
                return ownUserDto;
            }
            catch (Exception e)
            {
                _logs.Exception(e);
                ConnectionState = ConnectionState.Disconnected;
                throw;
            }
        }

        private const string DefaultStreamAuthType = "jwt";
        private const int HealthCheckMaxWaitingTime = 30;

        // For WebGL there is a slight delay when sending therefore we send HC event a bit sooner just in case
        private const int HealthCheckSendInterval = HealthCheckMaxWaitingTime - 1;

        private readonly IWebsocketClient _websocketClient;
        private readonly ISerializer _serializer;
        private readonly ILogs _logs;
        private readonly ITimeService _timeService;
        private readonly INetworkMonitor _networkMonitor;
        private readonly IRequestUriFactory _requestUriFactory;
        private readonly IHttpClient _httpClient;
        private readonly StringBuilder _errorSb = new StringBuilder();
        private readonly StringBuilder _logSb = new StringBuilder();
        private readonly IStreamClientConfig _config;
        private readonly ReconnectScheduler _reconnectScheduler;

        private readonly Dictionary<string, Action<string>> _eventKeyToHandler =
            new Dictionary<string, Action<string>>();

        private readonly object _websocketConnectionFailedFlagLock = new object();
        private readonly object _websocketDisconnectedFlagLock = new object();

        /// <summary>
        /// Every <see cref="ConnectionState"/> write must happen on this thread
        /// </summary>
        private readonly int _mainThreadId;

        private TaskCompletionSource<OwnUserInternalDTO> _connectUserTaskSource;
        private CancellationToken _connectUserCancellationToken;
        private CancellationTokenSource _connectUserCancellationTokenSource;
        private CancellationTokenSource _updateMonitorCts;

        private AuthCredentials _authCredentials;

        private ConnectionState _connectionState;
        private string _connectionId;
        private float _lastHealthCheckReceivedTime;
        private float _lastHealthCheckSendTime;
        private bool _updateCallReceived;

        private bool _websocketConnectionFailed;
        private bool _websocketDisconnected;
        private ITokenProvider _tokenProvider;

        /// <summary>
        /// Date Time of the last received WebSocket event from the API. When calling /sync endpoint use <see cref="_disconnectionLastEventReceivedAt"/>
        /// </summary>
        private DateTimeOffset? _lastEventReceivedAt;

        /// <summary>
        /// The last value of <see cref="_lastEventReceivedAt"/> when the client disconnected. Use this value when calling /sync endpoint
        /// </summary>
        private DateTimeOffset? _disconnectionLastEventReceivedAt;

        private bool _isApplyingHistoryEvents;
        private DateTimeOffset? _historyMaxAppliedCreatedAt;

        private async Task RefreshAuthTokenFromProvider()
        {
#if STREAM_DEBUG_ENABLED
            _logs.Info($"Request new auth token for user `{_authCredentials.UserId}`");
#endif
            try
            {
                var token = await _tokenProvider.GetTokenAsync(_authCredentials.UserId);
                _authCredentials = _authCredentials.CreateWithNewUserToken(token);
                SeAuthorizationCredentials(_authCredentials);

#if STREAM_DEBUG_ENABLED
                _logs.Info($"auth token received for user `{_authCredentials.UserId}`: " + token);
#endif
            }
            catch (Exception e)
            {
                throw new TokenProviderException(
                    $"Failed to get token from the {nameof(ITokenProvider)}. Inspect {nameof(e.InnerException)} for more information. ",
                    e);
            }
        }

        private void TryCancelWaitingForUserConnection()
        {
            if (_connectUserTaskSource == null)
            {
                return;
            }

            var isConnectTaskRunning = _connectUserTaskSource.Task != null && !_connectUserTaskSource.Task.IsCompleted;
            var isCancellationRequested = _connectUserCancellationTokenSource.IsCancellationRequested;

            if (isConnectTaskRunning && !isCancellationRequested)
            {
#if STREAM_DEBUG_ENABLED
                _logs.Info($"Try Cancel {_connectUserTaskSource}");
#endif
                _connectUserTaskSource.TrySetCanceled();
            }
        }

        /// <summary>
        /// This event can be called by a background thread and we must propagate it on the main thread
        /// Otherwise any call to Unity API would result in Exception. Unity API can only be called from the main thread
        /// </summary>
        private void OnWebsocketDisconnected()
        {
#if STREAM_DEBUG_ENABLED
            _logs.Warning("Websocket Disconnected");
#endif

            if (Thread.CurrentThread.ManagedThreadId == _mainThreadId)
            {
                ConnectionState = ConnectionState.Disconnected;
                return;
            }

            lock (_websocketDisconnectedFlagLock)
            {
                _websocketDisconnected = true;
            }
        }

        private void TryHandleWebsocketDisconnected()
        {
            lock (_websocketDisconnectedFlagLock)
            {
                if (!_websocketDisconnected)
                {
                    return;
                }

                _websocketDisconnected = false;
            }

            if (ConnectionState == ConnectionState.Closing)
            {
                return;
            }

            ConnectionState = ConnectionState.Disconnected;
        }

        /// <summary>
        /// Subscribers are invoked one by one so that a throwing handler cannot stop the remaining ones -
        /// including the internal reconnect scheduling - from observing the transition
        /// </summary>
        private void RaiseConnectionStateChanged(ConnectionState previous, ConnectionState current)
        {
            var handler = ConnectionStateChanged;
            if (handler == null)
            {
                return;
            }

            foreach (var subscriber in handler.GetInvocationList())
            {
                try
                {
                    ((ConnectionStateChangeHandler)subscriber)(previous, current);
                }
                catch (Exception e)
                {
                    _logs.Exception(e);
                }
            }
        }

        /// <inheritdoc cref="RaiseConnectionStateChanged"/>
        private void RaiseDisconnected()
        {
            var handler = Disconnected;
            if (handler == null)
            {
                return;
            }

            foreach (var subscriber in handler.GetInvocationList())
            {
                try
                {
                    ((Action)subscriber)();
                }
                catch (Exception e)
                {
                    _logs.Exception(e);
                }
            }
        }

        /// <summary>
        /// This event can be called by a background thread and we must propagate it on the main thread
        /// Otherwise any call to Unity API would result in Exception. Unity API can only be called from the main thread
        /// </summary>
        private void OnWebsocketsConnectionFailed()
        {
            lock (_websocketConnectionFailedFlagLock)
            {
                _websocketConnectionFailed = true;
            }
        }

        private void TryHandleWebsocketsConnectionFailed()
        {
            lock (_websocketConnectionFailedFlagLock)
            {
                if (!_websocketConnectionFailed)
                {
                    return;
                }

                _websocketConnectionFailed = false;
            }

#if STREAM_DEBUG_ENABLED
            _logs.Warning("Websocket connection failed");
#endif

            ConnectionState = ConnectionState.Disconnected;
        }

        /// <summary>
        /// Based on receiving initial health check event from the server
        /// </summary>
        private void OnConnectionConfirmed(EventHealthCheck healthCheckEvent,
            HealthCheckEventInternalDTO eventHealthCheckInternalDto)
        {
            //StreamTodo: resolve issue that expired token also triggers connection confirmed that gets immediately disconnected

            _connectionId = healthCheckEvent.ConnectionId;
#pragma warning disable 0618
            LocalUser = healthCheckEvent.Me;
#pragma warning restore 0618
            _lastHealthCheckReceivedTime = _timeService.Time;

            ConnectionState = ConnectionState.Connected;

            _connectUserTaskSource?.SetResult(eventHealthCheckInternalDto.Me);

            _logs.Info("Connection confirmed by server with connection id: " + _connectionId);
            Connected?.Invoke(healthCheckEvent.Me);
            InternalConnected?.Invoke(eventHealthCheckInternalDto);
        }

        private void TryToReconnect()
        {
            if (!ConnectionState.IsValidToConnect() || !NextReconnectTime.HasValue)
            {
                return;
            }

            if (NextReconnectTime.Value > _timeService.Time)
            {
                return;
            }

            Reconnecting?.Invoke();

            if (_tokenProvider != null)
            {
                ConnectUserAsync(_authCredentials.ApiKey, _authCredentials.UserId, _tokenProvider).LogIfFailed();
            }
            else
            {
                Connect();
            }
        }

        private void RegisterEventHandlers()
        {
            RegisterEventType<HealthCheckEventInternalDTO, EventHealthCheck>(WSEventType.HealthCheck,
                HandleHealthCheckEvent);

            RegisterEventType<MessageNewEventInternalDTO, EventMessageNew>(WSEventType.MessageNew,
                (e, dto) => MessageReceived?.Invoke(e), dto => InternalMessageReceived?.Invoke(dto));
            RegisterEventType<MessageDeletedEventInternalDTO, EventMessageDeleted>(WSEventType.MessageDeleted,
                (e, dto) => MessageDeleted?.Invoke(e), dto => InternalMessageDeleted?.Invoke(dto));
            RegisterEventType<MessageUpdatedEventInternalDTO, EventMessageUpdated>(WSEventType.MessageUpdated,
                (e, dto) => MessageUpdated?.Invoke(e), dto => InternalMessageUpdated?.Invoke(dto));
            RegisterEventType<MessageReadEventInternalDTO, EventMessageRead>(WSEventType.MessageRead,
                (e, dto) => MessageRead?.Invoke(e), dto => InternalMessageRead?.Invoke(dto));

            RegisterEventType<ChannelUpdatedEventInternalDTO, EventChannelUpdated>(WSEventType.ChannelUpdated,
                (e, dto) => ChannelUpdated?.Invoke(e), dto => InternalChannelUpdated?.Invoke(dto));
            RegisterEventType<ChannelDeletedEventInternalDTO, EventChannelDeleted>(WSEventType.ChannelDeleted,
                (e, dto) => ChannelDeleted?.Invoke(e), dto => InternalChannelDeleted?.Invoke(dto));
            RegisterEventType<ChannelTruncatedEventInternalDTO, EventChannelTruncated>(WSEventType.ChannelTruncated,
                (e, dto) => ChannelTruncated?.Invoke(e), dto => InternalChannelTruncated?.Invoke(dto));
            RegisterEventType<ChannelVisibleEventInternalDTO, EventChannelVisible>(WSEventType.ChannelVisible,
                (e, dto) => ChannelVisible?.Invoke(e), dto => InternalChannelVisible?.Invoke(dto));
            RegisterEventType<ChannelHiddenEventInternalDTO, EventChannelHidden>(WSEventType.ChannelHidden,
                (e, dto) => ChannelHidden?.Invoke(e), dto => InternalChannelHidden?.Invoke(dto));

            RegisterEventType<ReactionNewEventInternalDTO, EventReactionNew>(WSEventType.ReactionNew,
                (e, dto) => ReactionReceived?.Invoke(e), dto => InternalReactionReceived?.Invoke(dto));
            RegisterEventType<ReactionUpdatedEventInternalDTO, EventReactionUpdated>(WSEventType.ReactionUpdated,
                (e, dto) => ReactionUpdated?.Invoke(e), dto => InternalReactionUpdated?.Invoke(dto));
            RegisterEventType<ReactionDeletedEventInternalDTO, EventReactionDeleted>(WSEventType.ReactionDeleted,
                (e, dto) => ReactionDeleted?.Invoke(e), dto => InternalReactionDeleted?.Invoke(dto));

            RegisterEventType<MemberAddedEventInternalDTO, EventMemberAdded>(WSEventType.MemberAdded,
                (e, dto) => MemberAdded?.Invoke(e), dto => InternalMemberAdded?.Invoke(dto));
            RegisterEventType<MemberRemovedEventInternalDTO, EventMemberRemoved>(WSEventType.MemberRemoved,
                (e, dto) => MemberRemoved?.Invoke(e), dto => InternalMemberRemoved?.Invoke(dto));
            RegisterEventType<MemberUpdatedEventInternalDTO, EventMemberUpdated>(WSEventType.MemberUpdated,
                (e, dto) => MemberUpdated?.Invoke(e), dto => InternalMemberUpdated?.Invoke(dto));

            RegisterEventType<UserPresenceChangedEventInternalDTO, EventUserPresenceChanged>(
                WSEventType.UserPresenceChanged,
                (e, dto) => UserPresenceChanged?.Invoke(e), dto => InternalUserPresenceChanged?.Invoke(dto));
            RegisterEventType<UserUpdatedEventInternalDTO, EventUserUpdated>(WSEventType.UserUpdated,
                (e, dto) => UserUpdated?.Invoke(e), dto => InternalUserUpdated?.Invoke(dto));
            RegisterEventType<UserDeletedEventInternalDTO, EventUserDeleted>(WSEventType.UserDeleted,
                (e, dto) => UserDeleted?.Invoke(e), dto => InternalUserDeleted?.Invoke(dto));
            RegisterEventType<UserBannedEventInternalDTO, EventUserBanned>(WSEventType.UserBanned,
                (e, dto) => UserBanned?.Invoke(e), dto => InternalUserBanned?.Invoke(dto));
            RegisterEventType<UserUnbannedEventInternalDTO, EventUserUnbanned>(WSEventType.UserUnbanned,
                (e, dto) => UserUnbanned?.Invoke(e), dto => InternalUserUnbanned?.Invoke(dto));

            RegisterEventType<UserWatchingStartEventInternalDTO, EventUserWatchingStart>(WSEventType.UserWatchingStart,
                (e, dto) => UserWatchingStart?.Invoke(e), dto => InternalUserWatchingStart?.Invoke(dto));
            RegisterEventType<UserWatchingStopEventInternalDTO, EventUserWatchingStop>(WSEventType.UserWatchingStop,
                (e, dto) => UserWatchingStop?.Invoke(e), dto => InternalUserWatchingStop?.Invoke(dto));

            RegisterEventType<TypingStartEventInternalDTO, EventTypingStart>(WSEventType.TypingStart,
                (e, dto) => TypingStarted?.Invoke(e), dto => InternalTypingStarted?.Invoke(dto));
            RegisterEventType<TypingStopEventInternalDTO, EventTypingStop>(WSEventType.TypingStop,
                (e, dto) => TypingStopped?.Invoke(e), dto => InternalTypingStopped?.Invoke(dto));

            // Notifications

            RegisterEventType<NotificationChannelMutesUpdatedEventInternalDTO, EventNotificationChannelMutesUpdated>(
                WSEventType.NotificationChannelMutesUpdated,
                (e, dto) => NotificationChannelMutesUpdated?.Invoke(e),
                dto => InternalNotificationChannelMutesUpdated?.Invoke(dto));
            RegisterEventType<NotificationMutesUpdatedEventInternalDTO, EventNotificationMutesUpdated>(
                WSEventType.NotificationMutesUpdated,
                (e, dto) => NotificationMutesUpdated?.Invoke(e), dto => InternalNotificationMutesUpdated?.Invoke(dto));

            RegisterEventType<NotificationMarkReadEventInternalDTO, EventNotificationMarkRead>(
                WSEventType.NotificationMarkRead,
                (e, dto) => NotificationMarkRead?.Invoke(e), dto => InternalNotificationMarkRead?.Invoke(dto),
                InternalNotificationsHelper.FixMissingChannelTypeAndId);
            RegisterEventType<NotificationNewMessageEventInternalDTO, EventNotificationMessageNew>(
                WSEventType.NotificationMessageNew,
                (e, dto) => NotificationMessageReceived?.Invoke(e),
                dto => InternalNotificationMessageReceived?.Invoke(dto),
                InternalNotificationsHelper.FixMissingChannelTypeAndId);

            RegisterEventType<NotificationChannelDeletedEventInternalDTO, EventNotificationChannelDeleted>(
                WSEventType.NotificationChannelDeleted,
                (e, dto) => NotificationChannelDeleted?.Invoke(e),
                dto => InternalNotificationChannelDeleted?.Invoke(dto),
                InternalNotificationsHelper.FixMissingChannelTypeAndId);
            RegisterEventType<NotificationChannelTruncatedEventInternalDTO, EventNotificationChannelTruncated>(
                WSEventType.NotificationChannelTruncated,
                (e, dto) => NotificationChannelTruncated?.Invoke(e),
                dto => InternalNotificationChannelTruncated?.Invoke(dto),
                InternalNotificationsHelper.FixMissingChannelTypeAndId);

            RegisterEventType<NotificationAddedToChannelEventInternalDTO, EventNotificationAddedToChannel>(
                WSEventType.NotificationAddedToChannel,
                (e, dto) => NotificationAddedToChannel?.Invoke(e),
                dto => InternalNotificationAddedToChannel?.Invoke(dto),
                InternalNotificationsHelper.FixMissingChannelTypeAndId);
            RegisterEventType<NotificationRemovedFromChannelEventInternalDTO, EventNotificationRemovedFromChannel>(
                WSEventType.NotificationRemovedFromChannel,
                (e, dto) => NotificationRemovedFromChannel?.Invoke(e),
                dto => InternalNotificationRemovedFromChannel?.Invoke(dto),
                InternalNotificationsHelper.FixMissingChannelTypeAndId);

            RegisterEventType<NotificationInvitedEventInternalDTO, EventNotificationInvited>(
                WSEventType.NotificationInvited,
                (e, dto) => NotificationInvited?.Invoke(e), dto => InternalNotificationInvited?.Invoke(dto),
                InternalNotificationsHelper.FixMissingChannelTypeAndId);
            RegisterEventType<NotificationInviteAcceptedEventInternalDTO, EventNotificationInviteAccepted>(
                WSEventType.NotificationInviteAccepted,
                (e, dto) => NotificationInviteAccepted?.Invoke(e),
                dto => InternalNotificationInviteAccepted?.Invoke(dto),
                InternalNotificationsHelper.FixMissingChannelTypeAndId);
            RegisterEventType<NotificationInviteRejectedEventInternalDTO, EventNotificationInviteRejected>(
                WSEventType.NotificationInviteRejected,
                (e, dto) => NotificationInviteRejected?.Invoke(e),
                dto => InternalNotificationInviteRejected?.Invoke(dto),
                InternalNotificationsHelper.FixMissingChannelTypeAndId);

            // Polls

            RegisterEventType<PollClosedEventInternalDTO, EventPollClosed>(WSEventType.PollClosed,
                (e, dto) => PollClosed?.Invoke(e), dto => InternalPollClosed?.Invoke(dto));
            RegisterEventType<PollDeletedEventInternalDTO, EventPollDeleted>(WSEventType.PollDeleted,
                (e, dto) => PollDeleted?.Invoke(e), dto => InternalPollDeleted?.Invoke(dto));
            RegisterEventType<PollUpdatedEventInternalDTO, EventPollUpdated>(WSEventType.PollUpdated,
                (e, dto) => PollUpdated?.Invoke(e), dto => InternalPollUpdated?.Invoke(dto));
            RegisterEventType<PollVoteCastedEventInternalDTO, EventPollVoteCasted>(WSEventType.PollVoteCasted,
                (e, dto) => PollVoteCasted?.Invoke(e), dto => InternalPollVoteCasted?.Invoke(dto));
            RegisterEventType<PollVoteChangedEventInternalDTO, EventPollVoteChanged>(WSEventType.PollVoteChanged,
                (e, dto) => PollVoteChanged?.Invoke(e), dto => InternalPollVoteChanged?.Invoke(dto));
            RegisterEventType<PollVoteRemovedEventInternalDTO, EventPollVoteRemoved>(WSEventType.PollVoteRemoved,
                (e, dto) => PollVoteRemoved?.Invoke(e), dto => InternalPollVoteRemoved?.Invoke(dto));

            // Threads

            RegisterEventType<ThreadUpdatedEventInternalDTO, EventThreadUpdated>(WSEventType.ThreadUpdated,
                (e, dto) => ThreadUpdated?.Invoke(e), dto => InternalThreadUpdated?.Invoke(dto));
            RegisterEventType<NotificationThreadMessageNewEventInternalDTO, EventNotificationThreadMessageNew>(
                WSEventType.NotificationThreadMessageNew,
                (e, dto) => NotificationThreadMessageNew?.Invoke(e),
                dto => InternalNotificationThreadMessageNew?.Invoke(dto));
            RegisterEventType<NotificationMarkUnreadEventInternalDTO, EventNotificationMarkUnread>(
                WSEventType.NotificationMarkUnread,
                (e, dto) => NotificationMarkUnread?.Invoke(e),
                dto => InternalNotificationMarkUnread?.Invoke(dto));
        }

        private void RegisterEventType<TDto, TEvent>(string key,
            Action<TEvent, TDto> handler, Action<TDto> internalHandler = null, Action<TDto> postprocess = null)
            where TEvent : EventBase, ILoadableFrom<TDto, TEvent>, new()
        {
            if (_eventKeyToHandler.ContainsKey(key))
            {
#if STREAM_DEBUG_ENABLED
                _logs.Error($"Event handler with key `{key}` is already registered. Ignored");
#endif
                return;
            }

            _eventKeyToHandler.Add(key, serializedContent =>
            {
                try
                {
#if STREAM_DEBUG_ENABLED
                    var ignoreKeys = new[] { WSEventType.HealthCheck };
                    if (!ignoreKeys.Contains(key))
                    {
                        _logs.Warning("WS event received KEY: " + key + " CONTENT: " + serializedContent);
                    }
#endif
                    var eventObj = DeserializeEvent<TDto, TEvent>(serializedContent, out var dto);
                    postprocess?.Invoke(dto);

                    if (_isApplyingHistoryEvents)
                    {
                        RecordHistoryWatermark(eventObj.CreatedAt);
                    }
                    else
                    {
                        TryAdvanceLastEventReceivedAt(eventObj.CreatedAt, key);

                        // The low-level client is event-only, so it has no state a consumer could read
                        // after a silent batch. Suppressing its callbacks keeps a low-level subscriber
                        // consistent with the stateful client's BatchStateUpdate contract.
                        handler?.Invoke(eventObj, dto);
                    }

                    // Always applied - this is what mutates local state.
                    internalHandler?.Invoke(dto);
                }
                catch (Exception e)
                {
                    _logs.Exception(e);

                    // A silent batch counts its failures and advances the watermark only to the newest
                    // event it applied, so it needs to see the throw. Live events stay isolated here.
                    if (_isApplyingHistoryEvents)
                    {
                        throw;
                    }
                }
            });
        }

        private TEvent DeserializeEvent<TDto, TEvent>(string content, out TDto dto)
            where TEvent : ILoadableFrom<TDto, TEvent>, new()
        {
            try
            {
                dto = _serializer.Deserialize<TDto>(content);
            }
            catch (Exception e)
            {
                throw new StreamDeserializationException(content, typeof(TDto), e);
            }

            var response = new TEvent();
            response.LoadFromDto(dto);

            return response;
        }

        private void HandleNewWebsocketMessage(string msg, bool isLiveEvent = false)
        {
            const string ErrorKey = "error";

            if (_serializer.TryPeekValue<APIError>(msg, ErrorKey, out var apiError))
            {
                _errorSb.Length = 0;
                apiError.AppendFullLog(_errorSb);

                _logs.Error($"{nameof(APIError)} returned: {_errorSb}");
                return;
            }

            const string TypeKey = "type";

            if (!_serializer.TryPeekValue<string>(msg, TypeKey, out var type))
            {
                _logs.Error($"Failed to find `{TypeKey}` in msg: " + msg);
                return;
            }

            // Stamp liveness here rather than from the health check handler: the handler runs after
            // every consumer callback registered ahead of it, so a slow consumer could push the gap
            // past HealthCheckMaxWaitingTime and make the client disconnect itself. Only events that
            // came off the live socket count - a health check replayed from /sync proves nothing.
            if (isLiveEvent && type == WSEventType.HealthCheck)
            {
                _lastHealthCheckReceivedTime = _timeService.Time;
            }

            if (EventReceived != null && !_isApplyingHistoryEvents)
            {
                var time = DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss");
                EventReceived.Invoke($"{time} - Event received: <b>{type}</b>");
            }

            if (!_eventKeyToHandler.TryGetValue(type, out var handler))
            {
                if (TryHandleCustomChannelEvent(msg, type))
                {
                    return;
                }

                if (_config.LogLevel.IsDebugEnabled())
                {
                    _logs.Warning($"No message handler registered for `{type}`. Message not handled: " + msg);
                }

                return;
            }

            handler(msg);
        }

        private bool TryHandleCustomChannelEvent(string serializedContent, string eventType)
        {
            if (!_serializer.TryPeekValue<string>(serializedContent, "cid", out var cid)
                || string.IsNullOrEmpty(cid))
            {
                return false;
            }

            try
            {
                var dto = _serializer.Deserialize<CustomEventInternalDTO>(serializedContent);

                if (_isApplyingHistoryEvents)
                {
                    RecordHistoryWatermark(dto.CreatedAt);
                }
                else
                {
                    TryAdvanceLastEventReceivedAt(dto.CreatedAt, eventType);
                }

                // Custom events are the one category with no representation in local state, so a
                // consumer cannot reconstruct them from IStreamChannel after a silent batch. They are
                // therefore delivered per event even during history sync - dropping them would be
                // silent data loss, and deferring them into the recovery signal would arrive after
                // the re-query and out of chronological order. The reference SDKs discard custom
                // events from /sync entirely; an app porting between SDKs must not rely on this.
                var evt = new EventCustom();
                ((ILoadableFrom<CustomEventInternalDTO, EventCustom>)evt).LoadFromDto(dto);
                CustomEventReceived?.Invoke(evt);
                InternalCustomEventReceived?.Invoke(dto);
                return true;
            }
            catch (Exception e)
            {
                _logs.Exception(e);
                return false;
            }
        }

        private void UpdateHealthCheck()
        {
            if (ConnectionState != ConnectionState.Connected)
            {
                return;
            }

            var timeSinceLastHealthCheckSent = _timeService.Time - _lastHealthCheckSendTime;
            if (timeSinceLastHealthCheckSent > HealthCheckSendInterval)
            {
                PingHealthCheck();
            }

            var timeSinceLastHealthCheck = _timeService.Time - _lastHealthCheckReceivedTime;
            if (timeSinceLastHealthCheck > HealthCheckMaxWaitingTime)
            {
                _logs.Warning($"Health check was not received since: {timeSinceLastHealthCheck}, resetting connection");
                _websocketClient
                    .DisconnectAsync(WebSocketCloseStatus.InternalServerError,
                        $"Health check was not received since: {timeSinceLastHealthCheck}")
                    .ContinueWith(_ => _logs.Exception(_.Exception), TaskContinuationOptions.OnlyOnFaulted);
            }
        }

        private void PingHealthCheck()
        {
            var healthCheck = new EventHealthCheck
            {
                Type = WSEventType.HealthCheck
            };

            _websocketClient.Send(_serializer.Serialize(healthCheck));
            _lastHealthCheckSendTime = _timeService.Time;
        }

        private void HandleHealthCheckEvent(EventHealthCheck healthCheckEvent, HealthCheckEventInternalDTO dto)
        {
            if (ConnectionState == ConnectionState.Connecting)
            {
                OnConnectionConfirmed(healthCheckEvent, dto);
            }
        }

        private void TryAdvanceLastEventReceivedAt(DateTimeOffset createdAt, string eventType)
        {
            if (createdAt == DateTimeOffset.MinValue)
            {
                if (_config.LogLevel.IsDebugEnabled())
                {
                    _logs.Warning(
                        $"WebSocket event `{eventType}` has no valid `created_at`; the /sync watermark was not advanced.");
                }

                return;
            }

            if (!_lastEventReceivedAt.HasValue || createdAt > _lastEventReceivedAt.Value)
            {
                _lastEventReceivedAt = createdAt;
            }
        }

        private static bool IsUserIdValid(string userId)
        {
            var r = new Regex("^[a-zA-Z0-9@_-]+$");
            return r.IsMatch(userId);
        }

        private static string Base64UrlEncode(byte[] input)
            => Convert.ToBase64String(input)
                .Replace('+', '-')
                .Replace('/', '_')
                .Trim('=');

        //StreamTodo: make it more clear that we either receive full set of credentials or apiKey, userId and the token provider
        private void SetPartialConnectionCredentials(string apiKey, string userId)
        {
            _authCredentials = new AuthCredentials(apiKey, userId, string.Empty);
        }

        private void LogErrorIfUpdateIsNotBeingCalled()
        {
            _updateMonitorCts = new CancellationTokenSource();

            //StreamTodo: temporarily disable update monitor when tests are enabled -> investigate why some tests trigger this error
#if !STREAM_TESTS_ENABLED
            const int timeout = 2;
            Task.Delay(timeout * 1000, _updateMonitorCts.Token).ContinueWith(t =>
            {
                if (!_updateCallReceived && !_updateMonitorCts.IsCancellationRequested && ConnectionState != ConnectionState.Closing)
                {
                    _logs.Error(
                        $"Connection is not being updated. Please call the `{nameof(StreamChatLowLevelClient)}.{nameof(Update)}` method per frame. Connection state: {ConnectionState}");
                }
            }, _updateMonitorCts.Token);
#endif
        }

        private static string BuildStreamClientHeader(IApplicationInfo applicationInfo)
        {
            var sb = new StringBuilder();
            sb.Append($"stream-chat-unity-client-");
            sb.Append(SDKVersion);
            sb.Append("|");

            sb.Append("os=");
            sb.Append(applicationInfo.OperatingSystem);
            sb.Append("|");

            sb.Append("platform=");
            sb.Append(applicationInfo.Platform);
            sb.Append("|");

            sb.Append("engine=");
            sb.Append(applicationInfo.Engine);
            sb.Append("|");

            sb.Append("engine_version=");
            sb.Append(applicationInfo.EngineVersion);
            sb.Append("|");

            sb.Append("screen_size=");
            sb.Append(applicationInfo.ScreenSize);
            sb.Append("|");

            sb.Append("memory_size=");
            sb.Append(applicationInfo.MemorySize);
            sb.Append("|");

            sb.Append("graphics_memory_size=");
            sb.Append(applicationInfo.GraphicsMemorySize);

            return sb.ToString();
        }

        private void OnReconnectionScheduled()
        {
            ConnectionState = ConnectionState.WaitToReconnect;
            var timeLeft = NextReconnectTime.Value - _timeService.Time;

            _logSb.Append("Reconnect scheduled to time: <b>");
            _logSb.Append(Math.Round(NextReconnectTime.Value));
            _logSb.Append(" seconds</b>, current time: <b>");
            _logSb.Append(Math.Round(_timeService.Time));
            _logSb.Append(" seconds</b>, time left: <b>");
            _logSb.Append(Math.Round(timeLeft));
            _logSb.Append(" seconds</b>");

            _logs.Info(_logSb.ToString());
            _logSb.Clear();
        }
    }
}
