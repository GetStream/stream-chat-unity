using System;
using System.Collections.Generic;
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

namespace StreamChat.Core
{
    /// <summary>
    /// GetStream.io main client
    /// </summary>
    public class StreamChatClient : IStreamChatClient
    {
        public const string MenuPrefix = "GetStream/";

        public event Action Connected;

        public event Action<string> EventReceived;

        public event Action<EventMessageNew> MessageReceived;
        public event Action<EventMessageDeleted> MessageDeleted;
        public event Action<EventMessageUpdated> MessageUpdated;

        public IChannelApi ChannelApi { get; }
        public IMessageApi MessageApi { get; }
        public IModerationApi ModerationApi { get; }

        public ConnectionState ConnectionState { get; private set; }

        /// <summary>
        /// Use this method to create the main client instance
        /// </summary>
        /// <param name="authData">Authorization data with Server Url, ApiKey, UserToken and UserId</param>
        public static IStreamChatClient CreateDefaultClient(AuthData authData)
        {
            var unityLogs = new UnityLogs();
            var websocketClient = new WebsocketClient(unityLogs);
            var httpClient = new HttpClientAdapter();
            var serializer = new NewtonsoftJsonSerializer();
            var timeService = new UnityTime();
            var getStreamClient = new StreamChatClient(authData, websocketClient, httpClient, serializer,
                timeService, unityLogs);

            return getStreamClient;
        }

        public StreamChatClient(AuthData authData, IWebsocketClient websocketClient, IHttpClient httpClient,
            ISerializer serializer, ITimeService timeService, ILogs logs)
        {
            _authData = authData;
            _websocketClient = websocketClient ?? throw new ArgumentNullException(nameof(websocketClient));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _timeService = timeService ?? throw new ArgumentNullException(nameof(timeService));
            _logs = logs ?? throw new ArgumentNullException(nameof(logs));

            _requestUriFactory = new RequestUriFactory(authProvider: this, connectionProvider: this, _serializer);

            _httpClient.SetDefaultAuthenticationHeader(authData.UserToken);
            _httpClient.AddDefaultCustomHeader("stream-auth-type", DefaultStreamAuthType);

            //Todo: move to factory
            ChannelApi = new ChannelApi(httpClient, serializer, logs, _requestUriFactory);
            MessageApi = new MessageApi(httpClient, serializer, logs, _requestUriFactory);
            ModerationApi = new ModerationApi(httpClient, serializer, logs, _requestUriFactory);

            RegisterEventHandlers();
        }

        public void Connect()
        {
            if (!ConnectionState.IsValidToConnect())
            {
                throw new InvalidOperationException("Attempted to connect, but client is in state: " + ConnectionState);
            }

            var connectionUri = _requestUriFactory.CreateConnectionUri();

            _logs.Info($"Connect with uri: " + connectionUri);

            ConnectionState = ConnectionState.Connecting;
            _websocketClient.ConnectAsync(connectionUri).LogIfFailed();

            _websocketClient.Connected -= OnWebsocketsConnected;
            _websocketClient.Connected += OnWebsocketsConnected;
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
            => user.Id == _authData.UserId;

        public bool IsLocalUser(ChannelMember channelMember)
            => channelMember.User.Id == _authData.UserId;

        public void Dispose()
        {
            _websocketClient.Connected -= OnWebsocketsConnected;
            _websocketClient?.Dispose();
        }

        string IAuthProvider.ApiKey => _authData.ApiKey;
        string IAuthProvider.UserToken => _authData.UserToken;
        string IAuthProvider.UserId => _authData.UserId;
        string IAuthProvider.StreamAuthType => DefaultStreamAuthType;
        string IConnectionProvider.ConnectionId => _connectionId;
        Uri IConnectionProvider.ServerUri => _authData.ServerUri;

        private const string DefaultStreamAuthType = "jwt";
        private const int HealthCheckMaxWaitingTime = 30;

        //Todo: is it uniformly defined for all SDKs?
        private const int HealthCheckSendInterval = 25;

        private readonly IWebsocketClient _websocketClient;
        private readonly ISerializer _serializer;
        private readonly ILogs _logs;
        private readonly ITimeService _timeService;
        private readonly AuthData _authData;
        private readonly IRequestUriFactory _requestUriFactory;
        private readonly IHttpClient _httpClient;

        private readonly Dictionary<string, Action<string>> _eventKeyToHandler = new Dictionary<string, Action<string>>();

        private string _connectionId;
        private float _lastHealthCheckReceivedTime;
        private float _lastHealthCheckSendTime;

        private void OnWebsocketsConnected() => _logs.Info("Websockets Connected");

        private void OnConnectionConfirmed() => Connected?.Invoke();

        private void RegisterEventHandlers()
        {
            RegisterEventType<EventHealthCheckDTO, EventHealthCheck>(EventType.HealthCheck, Handle);

            RegisterEventType<EventMessageNewDTO, EventMessageNew>(EventType.MessageNew, Handle);
            RegisterEventType<EventMessageDeletedDTO, EventMessageDeleted>(EventType.MessageDeleted, Handle);
            RegisterEventType<EventMessageUpdatedDTO, EventMessageUpdated>(EventType.MessageUpdated, Handle);
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
            _eventKeyToHandler.Add(key, content =>
            {
                var eventObj = DeserializeEvent<TDto, TEvent>(content);
                handler(eventObj);
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
            _logs.Info("Message received: " + msg);

            const string TypeKey = "type";

            if (!_serializer.TryPeekValue<string>(msg, TypeKey, out var type))
            {
                _logs.Error($"Failed to find `{TypeKey}` in msg: " + msg);
                return;
            }

            var time = DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss");
            EventReceived?.Invoke($"{time} - Event received: <b>{type}</b>");

            if(!_eventKeyToHandler.TryGetValue(type, out var handler))
            {
                _logs.Warning($"No message handler registered for `{type}`. Message not handled: " + msg);
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

        private void Handle(EventHealthCheck healthCheckEvent)
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

        private void Handle(EventMessageNew messageNewEvent)
        {
            _logs.Info("New message event received");

            MessageReceived?.Invoke(messageNewEvent);
        }

        private void Handle(EventMessageDeleted messageDeletedEvent)
        {
            _logs.Info("New deleted event received");

            MessageDeleted?.Invoke(messageDeletedEvent);
        }

        private void Handle(EventMessageUpdated messageUpdatedEvent)
        {
            _logs.Info("New deleted event received");

            MessageUpdated?.Invoke(messageUpdatedEvent);
        }
    }
}