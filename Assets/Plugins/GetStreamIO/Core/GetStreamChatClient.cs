using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plugins.GetStreamIO.Core.Auth;
using Plugins.GetStreamIO.Core.Events;
using Plugins.GetStreamIO.Core.Events.DTO;
using Plugins.GetStreamIO.Core.Models;
using Plugins.GetStreamIO.Core.Requests;
using Plugins.GetStreamIO.Core.Requests.DTO;
using Plugins.GetStreamIO.Libs.Http;
using Plugins.GetStreamIO.Libs.Logs;
using Plugins.GetStreamIO.Libs.Serialization;
using Plugins.GetStreamIO.Libs.Time;
using Plugins.GetStreamIO.Libs.Websockets;

namespace Plugins.GetStreamIO.Core
{
    /// <summary>
    /// GetStream.io main client
    /// </summary>
    public class GetStreamChatClient : IGetStreamChatClient
    {
        public const string MenuPrefix = "GetStream/";

        public event Action ChannelsUpdated;
        public event Action<Channel> ActiveChanelChanged;
        public event Action<string> EventReceived;

        public IReadOnlyList<Channel> Channels => _channels;

        public Channel ActiveChannel
        {
            get => _activeChannel;
            private set
            {
                var prevValue = _activeChannel;
                _activeChannel = value;

                if (prevValue != value)
                {
                    ActiveChanelChanged?.Invoke(_activeChannel);
                }
            }
        }

        public ConnectionState ConnectionState { get; private set; }

        /// <summary>
        /// Use this method to create the main client instance
        /// </summary>
        /// <param name="authData">Authorization data with Server Url, ApiKey, UserToken and UserId</param>
        public static IGetStreamChatClient CreateDefaultClient(AuthData authData)
        {
            var unityLogs = new UnityLogs();
            var websocketClient = new WebsocketClient(unityLogs);
            var httpClient = new HttpClientAdapter();
            var serializer = new NewtonsoftJsonSerializer();
            var timeService = new UnityTime();
            var getStreamClient = new GetStreamChatClient(authData, websocketClient, httpClient, serializer,
                timeService, unityLogs);

            return getStreamClient;
        }

        public GetStreamChatClient(AuthData authData, IWebsocketClient websocketClient, IHttpClient httpClient,
            ISerializer serializer, ITimeService timeService, ILogs logs)
        {
            _authData = authData;
            _websocketClient = websocketClient ?? throw new ArgumentNullException(nameof(websocketClient));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _timeService = timeService ?? throw new ArgumentNullException(nameof(timeService));
            _logs = logs ?? throw new ArgumentNullException(nameof(logs));

            _requestUriFactory = new RequestUriFactory(authProvider: this, connectionProvider: this, _serializer);

            _serverEventsMapping.Register<HealthCheckEvent>(EventType.HealthCheck,
                msg => Parse<HealthCheckEvent>(msg, Handle));
            _serverEventsMapping.Register<MessageNewEvent>(EventType.MessageNew,
                msg => Parse<MessageNewEvent>(msg, Handle));

            _httpClient.SetDefaultAuthenticationHeader(authData.UserToken);
            _httpClient.AddDefaultCustomHeader("stream-auth-type", DefaultStreamAuthType);
        }

        public void Start()
        {
            if (!ConnectionState.IsValidToConnect())
            {
                throw new InvalidOperationException("Attempted to connect, but client is in state: " + ConnectionState);
            }

            var connectionUri = _requestUriFactory.CreateConnectionUri();

            _logs.Info($"Connect with uri: " + connectionUri);

            ConnectionState = ConnectionState.Connecting;
            _websocketClient.ConnectAsync(connectionUri).ContinueWith(_ => _logs.Exception(_.Exception),
                TaskContinuationOptions.OnlyOnFaulted);

            //Todo: reset health check timers

            _websocketClient.Connected -= OnWebsocketsConnected;
            _websocketClient.Connected += OnWebsocketsConnected;
        }

        //TOdo: replace with our inner coroutine?
        public void Update(float deltaTime)
        {
            UpdateHealthCheck();

            while (_websocketClient.TryDequeueMessage(out var msg))
            {
                OnMessageReceived(msg);
            }
        }

        public void OpenChannel(Channel channel)
        {
            ActiveChannel = channel;
        }

        public void SendMessage(string message)
        {
            SendMessageAsync(message)
                .ContinueWith(_ => _logs.Exception(_.Exception), TaskContinuationOptions.OnlyOnFaulted);
        }

        public bool IsLocalUser(User user)
            => user.Id == _authData.UserId;

        public bool IsLocalUser(Member member)
            => member.User.Id == _authData.UserId;

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
        private readonly IEventHandlersRepository _serverEventsMapping = new EventHandlersRepository();
        private readonly IRequestUriFactory _requestUriFactory;
        private readonly IHttpClient _httpClient;

        //Todo: remove -> LLC stateless
        private readonly List<Channel> _channels = new List<Channel>();

        private string _connectionId;
        private float _lastHealthCheckReceivedTime;
        private float _lastHealthCheckSendTime;
        private Channel _activeChannel;

        private void OnWebsocketsConnected() => _logs.Info("Websockets Connected");

        private void OnConnectionConfirmed()
            => GetChannelsAsync()
                .ContinueWith(_ => _logs.Exception(_.Exception), TaskContinuationOptions.OnlyOnFaulted);

        private void Reconnect()
        {
            ConnectionState = ConnectionState.Reconnecting;
            Start();
        }

        private void OnMessageReceived(string msg)
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

            if (!_serverEventsMapping.TryHandleEvent(type, msg))
            {
                _logs.Warning($"No message handler registered for `{type}`. Message not handled: " + msg);
                return;
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
                _logs.Warning($"Health check was not received since: {timeSinceLastHealthCheck}, attempt to reconnect");
                Reconnect();
            }
        }

        private void PingHealthCheck()
        {
            var msg = new HealthCheckRequest
            {
                Type = EventType.HealthCheck,

                //TOdo: is this valid? Seems to work, but for react SDK was `leia_organa--ce260d55-c24e-4963-887a-3b90a30a6175`
                ClientId = _authData.UserId
            };
            _websocketClient.Send(_serializer.Serialize(msg));

            _lastHealthCheckSendTime = _timeService.Time;
        }

        //Todo: refactor SendMessageAsync & GetChannelsAsync

        private async Task SendMessageAsync(string message)
        {
            if (ActiveChannel == null)
            {
                _logs.Error("Tried to send message, but no channel is active");
                return;
            }

            var uri = _requestUriFactory.CreateSendMessageUri(ActiveChannel);

            var messagePayload = new MessageRequest
            {
                Message = new SendMessage
                {
                    Id = _authData.UserId + "-" + Guid.NewGuid(),
                    Text = message
                }
            };

            var content = _serializer.Serialize(messagePayload);
            var response = await _httpClient.PostAsync(uri, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logs.Error("Failed to send message. Response: " + error);
            }
        }

        private async Task GetChannelsAsync()
        {
            var queryOptions = QueryChannelsOptions.Default.SortBy(SortFieldId.LastMessageAt, SortDirection.Descending);
            var content = _serializer.Serialize(queryOptions);

            var uri = _requestUriFactory.CreateChannelsUri();

            var response = await _httpClient.PostAsync(uri, content);

            if (!response.IsSuccessStatusCode)
            {
                _logs.Error("Failed to get channels response");
                return;
            }

            var responseText = await response.Content.ReadAsStringAsync();

            var channelsResponse = _serializer.Deserialize<ChannelsResponse>(responseText);

            _channels.Clear();
            _channels.AddRange(channelsResponse.Channels);

            OpenChannel(_channels.FirstOrDefault());

            ChannelsUpdated?.Invoke();
        }

        private void Parse<TType>(string msg, Action<TType> handler)
        {
            var parsed = _serializer.Deserialize<TType>(msg);
            handler(parsed);
        }

        private void Handle(HealthCheckEvent healthCheckEvent)
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

        private void Handle(MessageNewEvent messageNewEvent)
        {
            _logs.Info("New message event received");

            var channel = _channels.FirstOrDefault(_ => _.Details.Id == messageNewEvent.ChannelId);

            if (channel == null)
            {
                _logs.Error("Failed to find channel with id: " + messageNewEvent.ChannelId);
                return;
            }

            channel.AppendMessage(messageNewEvent.Message);
        }
    }
}