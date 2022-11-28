using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using StreamChat.Core.API;
using StreamChat.Core.API.Internal;
using StreamChat.Core.Auth;
using StreamChat.Core.Configs;
using StreamChat.Core.Exceptions;
using StreamChat.Core.InternalDTO.Events;
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
using StreamChat.Libs.Serialization;
using StreamChat.Libs.Time;
using StreamChat.Libs.Utils;
using StreamChat.Libs.Websockets;

#if STREAM_TESTS_ENABLED
[assembly: InternalsVisibleTo("StreamChat.Tests")] //StreamTodo: verify which Unity version introduced this
#endif

namespace StreamChat.Core.LowLevelClient
{
    /// <summary>
    /// Stream Chat Client - maintains WebSockets connection, executes API calls and exposes Stream events to which you can subscribe.
    /// There should be only one instance of this client in your application.
    /// </summary>
    public class StreamChatLowLevelClient : IStreamChatLowLevelClient
    {
        public const string MenuPrefix = "Stream/";

        public static readonly Uri ServerBaseUrl = new Uri("wss://chat.stream-io-api.com");

        public event ConnectionHandler Connected;
        public event Action Disconnected;
        public event Action<ConnectionState, ConnectionState> ConnectionStateChanged;

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

        #region Internal Events

        internal event Action<EventHealthCheckInternalDTO> InternalConnected;

        internal event Action<EventMessageNewInternalDTO> InternalMessageReceived;
        internal event Action<EventMessageUpdatedInternalDTO> InternalMessageUpdated;
        internal event Action<EventMessageDeletedInternalDTO> InternalMessageDeleted;
        internal event Action<EventMessageReadInternalDTO> InternalMessageRead;

        internal event Action<EventChannelUpdatedInternalDTO> InternalChannelUpdated;
        internal event Action<EventChannelDeletedInternalDTO> InternalChannelDeleted;
        internal event Action<EventChannelTruncatedInternalDTO> InternalChannelTruncated;
        internal event Action<EventChannelVisibleInternalDTO> InternalChannelVisible;
        internal event Action<EventChannelHiddenInternalDTO> InternalChannelHidden;

        internal event Action<EventMemberAddedInternalDTO> InternalMemberAdded;
        internal event Action<EventMemberRemovedInternalDTO> InternalMemberRemoved;
        internal event Action<EventMemberUpdatedInternalDTO> InternalMemberUpdated;

        internal event Action<EventUserPresenceChangedInternalDTO> InternalUserPresenceChanged;
        internal event Action<EventUserUpdatedInternalDTO> InternalUserUpdated;
        internal event Action<EventUserDeletedInternalDTO> InternalUserDeleted;
        internal event Action<EventUserBannedInternalDTO> InternalUserBanned;
        internal event Action<EventUserUnbannedInternalDTO> InternalUserUnbanned;

        internal event Action<EventUserWatchingStartInternalDTO> InternalUserWatchingStart;
        internal event Action<EventUserWatchingStopInternalDTO> InternalUserWatchingStop;

        internal event Action<EventReactionNewInternalDTO> InternalReactionReceived;
        internal event Action<EventReactionUpdatedInternalDTO> InternalReactionUpdated;
        internal event Action<EventReactionDeletedInternalDTO> InternalReactionDeleted;

        internal event Action<EventTypingStartInternalDTO> InternalTypingStarted;
        internal event Action<EventTypingStopInternalDTO> InternalTypingStopped;

        internal event Action<EventNotificationChannelMutesUpdatedInternalDTO> InternalNotificationChannelMutesUpdated;
        internal event Action<EventNotificationMutesUpdatedInternalDTO> InternalNotificationMutesUpdated;

        internal event Action<EventNotificationMessageNewInternalDTO> InternalNotificationMessageReceived;
        internal event Action<EventNotificationMarkReadInternalDTO> InternalNotificationMarkRead;

        internal event Action<EventNotificationChannelDeletedInternalDTO> InternalNotificationChannelDeleted;
        internal event Action<EventNotificationChannelTruncatedInternalDTO> InternalNotificationChannelTruncated;

        internal event Action<EventNotificationAddedToChannelInternalDTO> InternalNotificationAddedToChannel;
        internal event Action<EventNotificationRemovedFromChannelInternalDTO> InternalNotificationRemovedFromChannel;

        internal event Action<EventNotificationInvitedInternalDTO> InternalNotificationInvited;
        internal event Action<EventNotificationInviteAcceptedInternalDTO> InternalNotificationInviteAccepted;
        internal event Action<EventNotificationInviteRejectedInternalDTO> InternalNotificationInviteRejected;

        #endregion

        public IChannelApi ChannelApi { get; }
        public IMessageApi MessageApi { get; }
        public IModerationApi ModerationApi { get; }
        public IUserApi UserApi { get; }
        public IDeviceApi DeviceApi { get; }

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

                var prev = _connectionState;
                _connectionState = value;
                ConnectionStateChanged?.Invoke(prev, _connectionState);

                if (value == ConnectionState.Disconnected)
                {
                    Disconnected?.Invoke();
                }
            }
        }

        public ReconnectStrategy ReconnectStrategy { get; private set; }
        public float ReconnectConstantInterval { get; private set; } = 3;
        public float ReconnectExponentialMinInterval { get; private set; } = 1;
        public float ReconnectExponentialMaxInterval { get; private set; } = 1024;
        public double? NextReconnectTime { get; private set; }

        public static readonly Version SDKVersion = new Version(3, 9, 0);

        /// <summary>
        /// Use this method to create the main client instance or use StreamChatClient constructor to create a client instance with custom dependencies
        /// </summary>
        /// <param name="authCredentials">Authorization data with ApiKey, UserToken and UserId</param>
        public static IStreamChatLowLevelClient CreateDefaultClient(AuthCredentials authCredentials,
            IStreamClientConfig config = default)
        {
            config ??= StreamClientConfig.Default;
            var logs = StreamDependenciesFactory.CreateLogger(config.LogLevel.ToLogLevel());
            var applicationInfo = StreamDependenciesFactory.CreateApplicationInfo();
            var websocketClient
                = StreamDependenciesFactory.CreateWebsocketClient(logs, isDebugMode: config.LogLevel.IsDebugEnabled());
            var httpClient = StreamDependenciesFactory.CreateHttpClient();
            var serializer = StreamDependenciesFactory.CreateSerializer();
            var timeService = StreamDependenciesFactory.CreateTimeService();

            return new StreamChatLowLevelClient(authCredentials, websocketClient, httpClient, serializer,
                timeService, applicationInfo, logs, config);
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
            IHttpClient httpClient, ISerializer serializer, ITimeService timeService, IApplicationInfo applicationInfo,
            ILogs logs, IStreamClientConfig config)
        {
            _config = config;
            _authCredentials = authCredentials;
            _websocketClient = websocketClient ?? throw new ArgumentNullException(nameof(websocketClient));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _timeService = timeService ?? throw new ArgumentNullException(nameof(timeService));
            applicationInfo = applicationInfo ?? throw new ArgumentNullException(nameof(applicationInfo));
            _logs = logs ?? throw new ArgumentNullException(nameof(logs));
            _config = config ?? throw new ArgumentNullException(nameof(config));

            _logs.Prefix = "[Stream Chat] ";

            _requestUriFactory = new RequestUriFactory(authProvider: this, connectionProvider: this, _serializer);

            _httpClient.AddDefaultCustomHeader("stream-auth-type", DefaultStreamAuthType);
            var header = BuildStreamHeader(applicationInfo);
            _httpClient.AddDefaultCustomHeader("X-Stream-Client", header);

            _websocketClient.ConnectionFailed += OnWebsocketsConnectionFailed;
            _websocketClient.Connected += OnWebsocketsConnected;
            _websocketClient.Disconnected += OnWebsocketDisconnected;

            InternalChannelApi = new InternalChannelApi(httpClient, serializer, logs, _requestUriFactory);
            InternalMessageApi = new InternalMessageApi(httpClient, serializer, logs, _requestUriFactory);
            InternalModerationApi = new InternalModerationApi(httpClient, serializer, logs, _requestUriFactory);
            InternalUserApi = new InternalUserApi(httpClient, serializer, logs, _requestUriFactory);
            InternalDeviceApi = new InternalDeviceApi(httpClient, serializer, logs, _requestUriFactory);

            ChannelApi = new ChannelApi(InternalChannelApi);
            MessageApi = new MessageApi(InternalMessageApi);
            ModerationApi = new ModerationApi(InternalModerationApi);
            UserApi = new UserApi(InternalUserApi);
            DeviceApi = new DeviceApi(InternalDeviceApi);

            RegisterEventHandlers();

            LogErrorIfUpdateIsNotBeingCalled();
        }

        public void ConnectUser(AuthCredentials userAuthCredentials)
        {
            SetUser(userAuthCredentials);
            Connect();
        }

        public async Task DisconnectAsync()
        {
            //StreamTodo: remove this hack, if user issues a disconnect it should gracefully switch to permanent disconnect and to automatically reconnect
            NextReconnectTime = float.MaxValue;
            await _websocketClient.DisconnectAsync(WebSocketCloseStatus.NormalClosure, "User called Disconnect");
        }

        public void Connect()
        {
            SetUser(_authCredentials);

            if (!ConnectionState.IsValidToConnect())
            {
                throw new InvalidOperationException("Attempted to connect, but client is in state: " + ConnectionState);
            }

            NextReconnectTime = default;

            var connectionUri = _requestUriFactory.CreateConnectionUri();

            _logs.Info($"Attempt to connect");

            ConnectionState = ConnectionState.Connecting;

            _websocketClient.ConnectAsync(connectionUri).LogIfFailed(_logs);
        }

        public void Update(float deltaTime)
        {
            _updateCallReceived = true;

            TryHandleWebsocketsConnectionFailed();
            TryToReconnect();

            UpdateHealthCheck();

            _websocketClient.Update();

            while (_websocketClient.TryDequeueMessage(out var msg))
            {
                HandleNewWebsocketMessage(msg);
            }
        }

        public bool IsLocalUser(User user) => user.Id == _authCredentials.UserId;

        public bool IsLocalUser(ChannelMember channelMember) => channelMember.User.Id == _authCredentials.UserId;

        public void SetReconnectStrategySettings(ReconnectStrategy reconnectStrategy, float? exponentialMinInterval,
            float? exponentialMaxInterval, float? constantInterval)
        {
            ReconnectStrategy = reconnectStrategy;

            void ThrowIfLessOrEqualToZero(float value, string name)
            {
                if (value <= 0)
                {
                    throw new ArgumentException($"{name} needs to be greater than zero, given: " + value);
                }
            }

            if (exponentialMinInterval.HasValue)
            {
                ThrowIfLessOrEqualToZero(exponentialMinInterval.Value, nameof(exponentialMinInterval));
                ReconnectExponentialMinInterval = exponentialMinInterval.Value;
            }

            if (exponentialMaxInterval.HasValue)
            {
                ThrowIfLessOrEqualToZero(exponentialMaxInterval.Value, nameof(exponentialMaxInterval));
                ReconnectExponentialMaxInterval = exponentialMaxInterval.Value;
            }

            if (constantInterval.HasValue)
            {
                ThrowIfLessOrEqualToZero(constantInterval.Value, nameof(constantInterval));
                ReconnectConstantInterval = constantInterval.Value;
            }
        }

        public void Dispose()
        {
            ConnectionState = ConnectionState.Closing;

            _websocketClient.ConnectionFailed -= OnWebsocketsConnectionFailed;
            _websocketClient.Connected -= OnWebsocketsConnected;
            _websocketClient.Disconnected -= OnWebsocketDisconnected;
            _websocketClient?.Dispose();
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

        private const string DefaultStreamAuthType = "jwt";
        private const int HealthCheckMaxWaitingTime = 30;

        private const int HealthCheckSendInterval = HealthCheckMaxWaitingTime;

        private readonly IWebsocketClient _websocketClient;
        private readonly ISerializer _serializer;
        private readonly ILogs _logs;
        private readonly ITimeService _timeService;
        private readonly IRequestUriFactory _requestUriFactory;
        private readonly IHttpClient _httpClient;
        private readonly StringBuilder _errorSb = new StringBuilder();
        private readonly StringBuilder _logSb = new StringBuilder();
        private readonly IStreamClientConfig _config;

        private readonly Dictionary<string, Action<string>> _eventKeyToHandler =
            new Dictionary<string, Action<string>>();

        private readonly object _websocketConnectionFailedFlagLock = new object();

        private AuthCredentials _authCredentials;

        private ConnectionState _connectionState;
        private string _connectionId;
        private float _lastHealthCheckReceivedTime;
        private float _lastHealthCheckSendTime;
        private bool _updateCallReceived;

        private bool _websocketConnectionFailed;
        private int _reconnectAttempt;

        private void OnWebsocketsConnected() => _logs.Info("Websockets Connected");

        private void OnWebsocketDisconnected()
        {
            ConnectionState = ConnectionState.Disconnected;
            TryScheduleReconnect();
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

            ConnectionState = ConnectionState.Disconnected;

            TryScheduleReconnect();
        }

        /// <summary>
        /// Based on receiving initial health check event from the server
        /// </summary>
        private void OnConnectionConfirmed(EventHealthCheck healthCheckEvent,
            EventHealthCheckInternalDTO eventHealthCheckInternalDto)
        {
            _connectionId = healthCheckEvent.ConnectionId;
#pragma warning disable 0618
            LocalUser = healthCheckEvent.Me;
#pragma warning restore 0618
            _lastHealthCheckReceivedTime = _timeService.Time;
            _reconnectAttempt = 0;
            ConnectionState = ConnectionState.Connected;

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

            _reconnectAttempt++;
            Connect();
        }

        private bool TryScheduleReconnect()
        {
            if (NextReconnectTime.HasValue && NextReconnectTime.Value > _timeService.Time)
            {
                return false;
            }

            switch (ReconnectStrategy)
            {
                case ReconnectStrategy.Exponential:

                    var baseInterval = Math.Pow(2, _reconnectAttempt);
                    var interval = Math.Min(Math.Max(ReconnectExponentialMinInterval, baseInterval),
                        ReconnectExponentialMaxInterval);
                    NextReconnectTime = _timeService.Time + interval;

                    break;
                case ReconnectStrategy.Constant:
                    NextReconnectTime = _timeService.Time + ReconnectConstantInterval;
                    break;
                case ReconnectStrategy.Never:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (NextReconnectTime.HasValue)
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

            return NextReconnectTime.HasValue;
        }

        private void RegisterEventHandlers()
        {
            RegisterEventType<EventHealthCheckInternalDTO, EventHealthCheck>(WSEventType.HealthCheck,
                HandleHealthCheckEvent);

            RegisterEventType<EventMessageNewInternalDTO, EventMessageNew>(WSEventType.MessageNew,
                (e, dto) => MessageReceived?.Invoke(e), dto => InternalMessageReceived?.Invoke(dto));
            RegisterEventType<EventMessageDeletedInternalDTO, EventMessageDeleted>(WSEventType.MessageDeleted,
                (e, dto) => MessageDeleted?.Invoke(e), dto => InternalMessageDeleted?.Invoke(dto));
            RegisterEventType<EventMessageUpdatedInternalDTO, EventMessageUpdated>(WSEventType.MessageUpdated,
                (e, dto) => MessageUpdated?.Invoke(e), dto => InternalMessageUpdated?.Invoke(dto));
            RegisterEventType<EventMessageReadInternalDTO, EventMessageRead>(WSEventType.MessageRead,
                (e, dto) => MessageRead?.Invoke(e), dto => InternalMessageRead?.Invoke(dto));

            RegisterEventType<EventChannelUpdatedInternalDTO, EventChannelUpdated>(WSEventType.ChannelUpdated,
                (e, dto) => ChannelUpdated?.Invoke(e), dto => InternalChannelUpdated?.Invoke(dto));
            RegisterEventType<EventChannelDeletedInternalDTO, EventChannelDeleted>(WSEventType.ChannelDeleted,
                (e, dto) => ChannelDeleted?.Invoke(e), dto => InternalChannelDeleted?.Invoke(dto));
            RegisterEventType<EventChannelTruncatedInternalDTO, EventChannelTruncated>(WSEventType.ChannelTruncated,
                (e, dto) => ChannelTruncated?.Invoke(e), dto => InternalChannelTruncated?.Invoke(dto));
            RegisterEventType<EventChannelVisibleInternalDTO, EventChannelVisible>(WSEventType.ChannelVisible,
                (e, dto) => ChannelVisible?.Invoke(e), dto => InternalChannelVisible?.Invoke(dto));
            RegisterEventType<EventChannelHiddenInternalDTO, EventChannelHidden>(WSEventType.ChannelHidden,
                (e, dto) => ChannelHidden?.Invoke(e), dto => InternalChannelHidden?.Invoke(dto));

            RegisterEventType<EventReactionNewInternalDTO, EventReactionNew>(WSEventType.ReactionNew,
                (e, dto) => ReactionReceived?.Invoke(e), dto => InternalReactionReceived?.Invoke(dto));
            RegisterEventType<EventReactionUpdatedInternalDTO, EventReactionUpdated>(WSEventType.ReactionUpdated,
                (e, dto) => ReactionUpdated?.Invoke(e), dto => InternalReactionUpdated?.Invoke(dto));
            RegisterEventType<EventReactionDeletedInternalDTO, EventReactionDeleted>(WSEventType.ReactionDeleted,
                (e, dto) => ReactionDeleted?.Invoke(e), dto => InternalReactionDeleted?.Invoke(dto));

            RegisterEventType<EventMemberAddedInternalDTO, EventMemberAdded>(WSEventType.MemberAdded,
                (e, dto) => MemberAdded?.Invoke(e), dto => InternalMemberAdded?.Invoke(dto));
            RegisterEventType<EventMemberRemovedInternalDTO, EventMemberRemoved>(WSEventType.MemberRemoved,
                (e, dto) => MemberRemoved?.Invoke(e), dto => InternalMemberRemoved?.Invoke(dto));
            RegisterEventType<EventMemberUpdatedInternalDTO, EventMemberUpdated>(WSEventType.MemberUpdated,
                (e, dto) => MemberUpdated?.Invoke(e), dto => InternalMemberUpdated?.Invoke(dto));

            RegisterEventType<EventUserPresenceChangedInternalDTO, EventUserPresenceChanged>(WSEventType.UserPresenceChanged,
                (e, dto) => UserPresenceChanged?.Invoke(e), dto => InternalUserPresenceChanged?.Invoke(dto));
            RegisterEventType<EventUserUpdatedInternalDTO, EventUserUpdated>(WSEventType.UserUpdated,
                (e, dto) => UserUpdated?.Invoke(e), dto => InternalUserUpdated?.Invoke(dto));
            RegisterEventType<EventUserDeletedInternalDTO, EventUserDeleted>(WSEventType.UserDeleted,
                (e, dto) => UserDeleted?.Invoke(e), dto => InternalUserDeleted?.Invoke(dto));
            RegisterEventType<EventUserBannedInternalDTO, EventUserBanned>(WSEventType.UserBanned,
                (e, dto) => UserBanned?.Invoke(e), dto => InternalUserBanned?.Invoke(dto));
            RegisterEventType<EventUserUnbannedInternalDTO, EventUserUnbanned>(WSEventType.UserUnbanned,
                (e, dto) => UserUnbanned?.Invoke(e), dto => InternalUserUnbanned?.Invoke(dto));

            RegisterEventType<EventUserWatchingStartInternalDTO, EventUserWatchingStart>(WSEventType.UserWatchingStart,
                (e, dto) => UserWatchingStart?.Invoke(e), dto => InternalUserWatchingStart?.Invoke(dto));
            RegisterEventType<EventUserWatchingStopInternalDTO, EventUserWatchingStop>(WSEventType.UserWatchingStop,
                (e, dto) => UserWatchingStop?.Invoke(e), dto => InternalUserWatchingStop?.Invoke(dto));

            RegisterEventType<EventTypingStartInternalDTO, EventTypingStart>(WSEventType.TypingStart,
                (e, dto) => TypingStarted?.Invoke(e), dto => InternalTypingStarted?.Invoke(dto));
            RegisterEventType<EventTypingStopInternalDTO, EventTypingStop>(WSEventType.TypingStop,
                (e, dto) => TypingStopped?.Invoke(e), dto => InternalTypingStopped?.Invoke(dto));

            // Notifications

            RegisterEventType<EventNotificationChannelMutesUpdatedInternalDTO, EventNotificationChannelMutesUpdated>(
                WSEventType.NotificationChannelMutesUpdated,
                (e, dto) => NotificationChannelMutesUpdated?.Invoke(e),
                dto => InternalNotificationChannelMutesUpdated?.Invoke(dto));
            RegisterEventType<EventNotificationMutesUpdatedInternalDTO, EventNotificationMutesUpdated>(
                WSEventType.NotificationMutesUpdated,
                (e, dto) => NotificationMutesUpdated?.Invoke(e), dto => InternalNotificationMutesUpdated?.Invoke(dto));

            RegisterEventType<EventNotificationMarkReadInternalDTO, EventNotificationMarkRead>(
                WSEventType.NotificationMarkRead,
                (e, dto) => NotificationMarkRead?.Invoke(e), dto => InternalNotificationMarkRead?.Invoke(dto));
            RegisterEventType<EventNotificationMessageNewInternalDTO, EventNotificationMessageNew>(
                WSEventType.NotificationMessageNew,
                (e, dto) => NotificationMessageReceived?.Invoke(e),
                dto => InternalNotificationMessageReceived?.Invoke(dto));

            RegisterEventType<EventNotificationChannelDeletedInternalDTO, EventNotificationChannelDeleted>(
                WSEventType.NotificationChannelDeleted,
                (e, dto) => NotificationChannelDeleted?.Invoke(e),
                dto => InternalNotificationChannelDeleted?.Invoke(dto));
            RegisterEventType<EventNotificationChannelTruncatedInternalDTO, EventNotificationChannelTruncated>(
                WSEventType.NotificationChannelTruncated,
                (e, dto) => NotificationChannelTruncated?.Invoke(e),
                dto => InternalNotificationChannelTruncated?.Invoke(dto));

            RegisterEventType<EventNotificationAddedToChannelInternalDTO, EventNotificationAddedToChannel>(
                WSEventType.NotificationAddedToChannel,
                (e, dto) => NotificationAddedToChannel?.Invoke(e),
                dto => InternalNotificationAddedToChannel?.Invoke(dto));
            RegisterEventType<EventNotificationRemovedFromChannelInternalDTO, EventNotificationRemovedFromChannel>(
                WSEventType.NotificationRemovedFromChannel,
                (e, dto) => NotificationRemovedFromChannel?.Invoke(e),
                dto => InternalNotificationRemovedFromChannel?.Invoke(dto));

            RegisterEventType<EventNotificationInvitedInternalDTO, EventNotificationInvited>(
                WSEventType.NotificationInvited,
                (e, dto) => NotificationInvited?.Invoke(e), dto => InternalNotificationInvited?.Invoke(dto));
            RegisterEventType<EventNotificationInviteAcceptedInternalDTO, EventNotificationInviteAccepted>(
                WSEventType.NotificationInviteAccepted,
                (e, dto) => NotificationInviteAccepted?.Invoke(e),
                dto => InternalNotificationInviteAccepted?.Invoke(dto));
            RegisterEventType<EventNotificationInviteRejectedInternalDTO, EventNotificationInviteRejected>(
                WSEventType.NotificationInviteRejected,
                (e, dto) => NotificationInviteRejected?.Invoke(e),
                dto => InternalNotificationInviteRejected?.Invoke(dto));
        }

        private void RegisterEventType<TDto, TEvent>(string key,
            Action<TEvent, TDto> handler, Action<TDto> internalHandler = null)
            where TEvent : ILoadableFrom<TDto, TEvent>, new()
        {
            if (_eventKeyToHandler.ContainsKey(key))
            {
                _logs.Warning($"Event handler with key `{key}` is already registered. Ignored");
                return;
            }

            _eventKeyToHandler.Add(key, serializedContent =>
            {
                try
                {
                    var eventObj = DeserializeEvent<TDto, TEvent>(serializedContent, out var dto);
                    handler?.Invoke(eventObj, dto);
                    internalHandler?.Invoke(dto);
                }
                catch (Exception e)
                {
                    _logs.Exception(e);
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

        private void HandleNewWebsocketMessage(string msg)
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

            var time = DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss");
            EventReceived?.Invoke($"{time} - Event received: <b>{type}</b>");

            if (!_eventKeyToHandler.TryGetValue(type, out var handler))
            {
                if (_config.LogLevel.IsDebugEnabled())
                {
                    _logs.Warning($"No message handler registered for `{type}`. Message not handled: " + msg);
                }

                return;
            }

            handler(msg);
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
                _logs.Warning($"Health check was not received since: {timeSinceLastHealthCheck}, reset connection");
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

        private void HandleHealthCheckEvent(EventHealthCheck healthCheckEvent, EventHealthCheckInternalDTO dto)
        {
            _lastHealthCheckReceivedTime = _timeService.Time;

            if (ConnectionState == ConnectionState.Connecting)
            {
                OnConnectionConfirmed(healthCheckEvent, dto);
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

        private void SetUser(AuthCredentials credentials)
        {
            if (credentials.IsAnyEmpty())
            {
                throw new StreamMissingAuthCredentialsException(
                    "Please provide valid credentials: `Api Key`, 'User id`, `User token`");
            }

            _authCredentials = credentials;
            _httpClient.SetDefaultAuthenticationHeader(credentials.UserToken);
        }

        private void LogErrorIfUpdateIsNotBeingCalled()
        {
            const int Timeout = 2;
            Task.Delay(Timeout * 1000).ContinueWith(t =>
            {
                if (!_updateCallReceived && ConnectionState != ConnectionState.Disconnected)
                {
                    _logs.Error(
                        $"Connection is not being updated. Please call the `{nameof(StreamChatLowLevelClient)}.{nameof(Update)}` method per frame.");
                }
            });
        }
        
        private static string BuildStreamHeader(IApplicationInfo applicationInfo)
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
    }
}
