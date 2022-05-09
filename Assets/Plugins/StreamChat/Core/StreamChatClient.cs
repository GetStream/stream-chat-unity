using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using StreamChat.Core.DTO.Events;
using StreamChat.Libs.Http;
using StreamChat.Libs.Logs;
using StreamChat.Libs.Serialization;
using StreamChat.Libs.Time;
using StreamChat.Libs.Utils;
using StreamChat.Libs.Websockets;
using StreamChat.Core.API;
using StreamChat.Core.Auth;
using StreamChat.Core.Events;
using StreamChat.Core.Exceptions;
using StreamChat.Core.Models;
using StreamChat.Core.Web;
using StreamChat.Libs;
using StreamChat.Libs.Auth;

namespace StreamChat.Core
{
    /// <summary>
    /// Stream Chat Client - maintains WebSockets connection, executes API calls and exposes Stream events to which you can subscribe.
    /// There should be only one instance of this client in your application.
    /// </summary>
    /// <remarks>
    /// The only case where you might want to have multiple instances is if you'd be creating an app which maintains multiple users connected simultaneously.
    /// </remarks>
    public class StreamChatClient : IStreamChatClient
    {
        public const string MenuPrefix = "Stream/";
        public const int ReconnectMaxAttempts = 5;

        public static readonly Uri ServerBaseUrl = new Uri("wss://chat.stream-io-api.com");

        public event Action Connected;
        public event Action<ConnectionState, ConnectionState> ConnectionStateChanged;

        public event Action<string> EventReceived;

        public event Action<EventMessageNew> MessageReceived;
        public event Action<EventMessageDeleted> MessageDeleted;
        public event Action<EventMessageUpdated> MessageUpdated;

        public event Action<EventReactionNew> ReactionReceived;
        public event Action<EventReactionUpdated> ReactionUpdated;
        public event Action<EventReactionDeleted> ReactionDeleted;

        public IChannelApi ChannelApi { get; }
        public IMessageApi MessageApi { get; }
        public IModerationApi ModerationApi { get; }
        public IUserApi UserApi { get; }

        public ConnectionState ConnectionState
        {
            get => _connectionState;
            private set
            {
                var prev = value;
                _connectionState = value;
                ConnectionStateChanged?.Invoke(prev, value);
            }
        }

        public static readonly Version SDKVersion = new Version(2, 4, 0);

        /// <summary>
        /// Use this method to create the main client instance or use StreamChatClient constructor to create a client instance with custom dependencies
        /// </summary>
        /// <param name="authCredentials">Authorization data with ApiKey, UserToken and UserId</param>
        public static IStreamChatClient CreateDefaultClient(AuthCredentials authCredentials)
        {
            var logs = LibsFactory.CreateDefaultLogs();
            var websocketClient = LibsFactory.CreateDefaultWebsocketClient(logs);
            var httpClient = LibsFactory.CreateDefaultHttpClient();
            var serializer = LibsFactory.CreateDefaultSerializer();
            var timeService = LibsFactory.CreateDefaultTimeService();

            return new StreamChatClient(authCredentials, websocketClient, httpClient, serializer,
                timeService, logs);
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

        public StreamChatClient(AuthCredentials authCredentials, IWebsocketClient websocketClient,
            IHttpClient httpClient, ISerializer serializer, ITimeService timeService, ILogs logs)
        {
            _authCredentials = authCredentials;
            _websocketClient = websocketClient ?? throw new ArgumentNullException(nameof(websocketClient));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _timeService = timeService ?? throw new ArgumentNullException(nameof(timeService));
            _logs = logs ?? throw new ArgumentNullException(nameof(logs));

            _logs.Prefix = "Stream Chat: ";

            _requestUriFactory = new RequestUriFactory(authProvider: this, connectionProvider: this, _serializer);

            _httpClient.AddDefaultCustomHeader("stream-auth-type", DefaultStreamAuthType);
            _httpClient.AddDefaultCustomHeader("X-Stream-Client", $"stream-chat-unity-client-{SDKVersion}");

            //Todo: move to factory
            ChannelApi = new ChannelApi(httpClient, serializer, logs, _requestUriFactory);
            MessageApi = new MessageApi(httpClient, serializer, logs, _requestUriFactory);
            ModerationApi = new ModerationApi(httpClient, serializer, logs, _requestUriFactory);
            UserApi = new UserApi(httpClient, serializer, logs, _requestUriFactory);

            RegisterEventHandlers();
        }

        public void Connect()
        {
            SetUser(_authCredentials);

            if (!ConnectionState.IsValidToConnect())
            {
                throw new InvalidOperationException("Attempted to connect, but client is in state: " + ConnectionState);
            }

            var connectionUri = _requestUriFactory.CreateConnectionUri();

            _logs.Info($"Attempt to connect");

            ConnectionState = ConnectionState.Connecting;

            _websocketClient.ConnectionFailed -= OnWebsocketsConnectionFailed;
            _websocketClient.ConnectionFailed += OnWebsocketsConnectionFailed;

            _websocketClient.Connected -= OnWebsocketsConnected;
            _websocketClient.Connected += OnWebsocketsConnected;

            _websocketClient.ConnectAsync(connectionUri).LogIfFailed(_logs);
        }

        public void Update(float deltaTime)
        {
            UpdateHealthCheck();

            while (_websocketClient.TryDequeueMessage(out var msg))
            {
                HandleNewWebsocketMessage(msg);
            }
        }

        public bool IsLocalUser(User user)
            => user.Id == _authCredentials.UserId;

        public bool IsLocalUser(ChannelMember channelMember)
            => channelMember.User.Id == _authCredentials.UserId;

        public void Dispose()
        {
            _websocketClient.ConnectionFailed -= OnWebsocketsConnectionFailed;
            _websocketClient.Connected -= OnWebsocketsConnected;
            _websocketClient?.Dispose();
        }

        string IAuthProvider.ApiKey => _authCredentials.ApiKey;
        string IAuthProvider.UserToken => _authCredentials.UserToken;
        string IAuthProvider.UserId => _authCredentials.UserId;
        string IAuthProvider.StreamAuthType => DefaultStreamAuthType;
        string IConnectionProvider.ConnectionId => _connectionId;
        Uri IConnectionProvider.ServerUri => ServerBaseUrl;

        private const string DefaultStreamAuthType = "jwt";
        private const int HealthCheckMaxWaitingTime = 30;

        private const int HealthCheckSendInterval = HealthCheckMaxWaitingTime;

        private readonly IWebsocketClient _websocketClient;
        private readonly ISerializer _serializer;
        private readonly ILogs _logs;
        private readonly ITimeService _timeService;
        private readonly AuthCredentials _authCredentials;
        private readonly IRequestUriFactory _requestUriFactory;
        private readonly IHttpClient _httpClient;
        private readonly StringBuilder _errorSb = new StringBuilder();

        private readonly Dictionary<string, Action<string>> _eventKeyToHandler =
            new Dictionary<string, Action<string>>();

        private ConnectionState _connectionState;
        private string _connectionId;
        private int _reconnectAttempt;
        private float _lastHealthCheckReceivedTime;
        private float _lastHealthCheckSendTime;

        private void OnWebsocketsConnected() => _logs.Info("Websockets Connected");

        private void OnWebsocketsConnectionFailed()
        {
            ConnectionState = ConnectionState.Disconnected;

            if (_reconnectAttempt >= ReconnectMaxAttempts)
            {
                return;
            }

            ConnectionState = ConnectionState.Reconnecting;
            _reconnectAttempt++;

            Connect();
        }

        private void OnConnectionConfirmed() => Connected?.Invoke();

        private void RegisterEventHandlers()
        {
            RegisterEventType<EventHealthCheckDTO, EventHealthCheck>(EventType.HealthCheck, HandleHealthCheckEvent);

            RegisterEventType<EventMessageNewDTO, EventMessageNew>(EventType.MessageNew,
                e => MessageReceived?.Invoke(e));
            RegisterEventType<EventMessageDeletedDTO, EventMessageDeleted>(EventType.MessageDeleted,
                e => MessageDeleted?.Invoke(e));
            RegisterEventType<EventMessageUpdatedDTO, EventMessageUpdated>(EventType.MessageUpdated,
                e => MessageUpdated?.Invoke(e));

            RegisterEventType<EventReactionNewDTO, EventReactionNew>(EventType.ReactionNew,
                e => ReactionReceived?.Invoke(e));
            RegisterEventType<EventReactionUpdatedDTO, EventReactionUpdated>(EventType.ReactionUpdated,
                e => ReactionUpdated?.Invoke(e));
            RegisterEventType<EventReactionDeletedDTO, EventReactionDeleted>(EventType.ReactionDeleted,
                e => ReactionDeleted?.Invoke(e));
        }

        private void Reconnect()
        {
            ConnectionState = ConnectionState.Reconnecting;
            Connect();
        }

        private void RegisterEventType<TDto, TEvent>(string key,
            Action<TEvent> handler)
            where TEvent : ILoadableFrom<TDto, TEvent>, new()
        {
            if (_eventKeyToHandler.ContainsKey(key))
            {
                _logs.Warning($"Event handler with key `{key}` is already registered. Ignored");
                return;
            }

            _eventKeyToHandler.Add(key, content =>
            {
                var eventObj = DeserializeEvent<TDto, TEvent>(content);
                handler?.Invoke(eventObj);
            });
        }

        private TEvent DeserializeEvent<TDto, TEvent>(string content)
            where TEvent : ILoadableFrom<TDto, TEvent>, new()
        {
            TDto responseDto;

            try
            {
                responseDto = _serializer.Deserialize<TDto>(content);
            }
            catch (Exception e)
            {
                throw new StreamDeserializationException(content, typeof(TDto), e);
            }

            var response = new TEvent();
            response.LoadFromDto(responseDto);

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
                //_logs.Warning($"No message handler registered for `{type}`. Message not handled: " + msg);
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
                _logs.Warning($"Health check was not received since: {timeSinceLastHealthCheck}, attempt to reconnect");
                Reconnect();
            }
        }

        private void PingHealthCheck()
        {
            //Todo: react demo also includes `client_id` but it's not in spec and health check seems to work without it
            var healthCheck = new EventHealthCheck
            {
                Type = EventType.HealthCheck
            };

            _websocketClient.Send(_serializer.Serialize(healthCheck));

            _lastHealthCheckSendTime = _timeService.Time;
        }

        private void HandleHealthCheckEvent(EventHealthCheck healthCheckEvent)
        {
            _lastHealthCheckReceivedTime = _timeService.Time;

            if (ConnectionState == ConnectionState.Connecting)
            {
                ConnectionState = ConnectionState.Connected;
                _connectionId = healthCheckEvent.ConnectionId;

                _logs.Info("Connection confirmed by server with connection id: " + _connectionId);
                OnConnectionConfirmed();
            }
        }

        private static bool IsUserIdValid(string userId)
        {
            var r = new Regex("^[a-zA-Z0-9@_-]+$");
            return r.IsMatch(userId);
        }

        private static string Base64UrlEncode(byte[] input) =>
            Convert.ToBase64String(input)
                .Replace('+', '-')
                .Replace('/', '_')
                .Trim('=');

        private void SetUser(AuthCredentials credentials)
        {
            if (credentials.IsAnyEmpty())
            {
                throw new StreamMissingAuthCredentialsException("Please provide valid credentials: `Api Key`, 'User id`, `User token`");
            }

            _httpClient.SetDefaultAuthenticationHeader(credentials.UserToken);
        }
    }
}