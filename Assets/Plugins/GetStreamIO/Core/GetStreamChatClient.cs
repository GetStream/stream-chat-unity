using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using GetStreamIO.Core.DTO.Models;
using GetStreamIO.Core.DTO.Requests;
using GetStreamIO.Core.DTO.Responses;
using Plugins.GetStreamIO.Core.API;
using Plugins.GetStreamIO.Core.Auth;
using Plugins.GetStreamIO.Core.Events;
using Plugins.GetStreamIO.Core.Events.DTO;
using Plugins.GetStreamIO.Core.Models;
using Plugins.GetStreamIO.Core.Models.V2;
using Plugins.GetStreamIO.Core.Requests;
using Plugins.GetStreamIO.Core.Requests.DTO;
using Plugins.GetStreamIO.Core.Requests.V2;
using Plugins.GetStreamIO.Core.Responses;
using Plugins.GetStreamIO.Libs.Http;
using Plugins.GetStreamIO.Libs.Logs;
using Plugins.GetStreamIO.Libs.Serialization;
using Plugins.GetStreamIO.Libs.Time;
using Plugins.GetStreamIO.Libs.Utils;
using Plugins.GetStreamIO.Libs.Websockets;

namespace Plugins.GetStreamIO.Core
{
    //Todo: rename to StreamChatClient + fix namespace -> StreamChat.Core etc. without Plugins
    /// <summary>
    /// GetStream.io main client
    /// </summary>
    public class GetStreamChatClient : IGetStreamChatClient
    {
        public const string MenuPrefix = "GetStream/";

        public event Action Connected;

        public event Action<string> EventReceived;

        public event Action<MessageNewEvent> MessageReceived;
        public event Action<MessageDeletedEvent> MessageDeleted;
        public event Action<MessageUpdated> MessageUpdated;

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
            _serverEventsMapping.Register<MessageDeletedEvent>(EventType.MessageDeleted,
                msg => Parse<MessageDeletedEvent>(msg, Handle));
            _serverEventsMapping.Register<MessageUpdated>(EventType.MessageUpdated,
                msg => Parse<MessageUpdated>(msg, Handle));

            _httpClient.SetDefaultAuthenticationHeader(authData.UserToken);
            _httpClient.AddDefaultCustomHeader("stream-auth-type", DefaultStreamAuthType);
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

        //Todo: move to ChannelApi
        public Task<ChannelsResponse> QueryChannelsAsync(QueryChannelsRequest queryChannelsRequest)
        {
            var endpoint = ChannelEndpoints.QueryChannels();

            return Post<QueryChannelsRequest, QueryChannelsRequestDTO, ChannelsResponse, ChannelsResponseDTO>(endpoint,
                queryChannelsRequest);
        }

        //Todo: move to MessageApi
        public Task<MessageResponse> SendNewMessageAsync(string channelType, string channelId, SendMessageRequest sendMessageRequest)
        {
            var endpoint = MessageEndpoints.SendMessage(channelType, channelId);

            return Post<SendMessageRequest, SendMessageRequestDTO, MessageResponse, MessageResponseDTO>(endpoint,
                sendMessageRequest);
        }

        private async Task<TResponse> Get<TResponse, TResponseDto>(string url)
            where TResponse : ILoadableFrom<TResponseDto, TResponse>, new()
        {
            var uri = _requestUriFactory.CreateDefaultEndpointUri(url);

            var httpResponse = await _httpClient.GetAsync(uri);
            var responseContent = await httpResponse.Content.ReadAsStringAsync();

            if (!httpResponse.IsSuccessStatusCode)
            {
                var apiError = _serializer.Deserialize<APIErrorDTO>(responseContent);
                throw new StreamApiException(apiError);
            }

            var responseDto = _serializer.Deserialize<TResponseDto>(responseContent);
            var response = new TResponse();
            response.LoadFromDto(responseDto);

            return response;
        }

        private async Task<TResponse> Post<TRequest, TRequestDto, TResponse, TResponseDto>(string url, TRequest request)
            where TRequest : ISavableTo<TRequestDto>
            where TResponse : ILoadableFrom<TResponseDto, TResponse>, new()
        {
            var uri = _requestUriFactory.CreateDefaultEndpointUri(url);
            var requestContent = _serializer.Serialize(request.SaveToDto());

            var httpResponse = await _httpClient.PostAsync(uri, requestContent);
            var responseContent = await httpResponse.Content.ReadAsStringAsync();

            if (!httpResponse.IsSuccessStatusCode)
            {
                var apiError = _serializer.Deserialize<APIErrorDTO>(responseContent);
                throw new StreamApiException(apiError);
            }

            var responseDto = _serializer.Deserialize<TResponseDto>(responseContent);
            var response = new TResponse();
            response.LoadFromDto(responseDto);

            return response;
        }

        [Obsolete]
        public async Task UpdateMessageAsync(Message message)
        {
            var uri = _requestUriFactory.CreateUpdateMessageUri(message);

            dynamic messageObj = new ExpandoObject();
            messageObj.id = message.Id;
            messageObj.text = message.Text;

            dynamic payload = new ExpandoObject();
            payload.message = messageObj;

            var requestContent = _serializer.Serialize(payload);

            var response = await _httpClient.PostAsync(uri, requestContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            LogRestCall(uri, requestContent, responseContent);

            if (!response.IsSuccessStatusCode)
            {
                _logs.Error("Failed to send message. Response: " + responseContent);
            }
        }

        [Obsolete]
        public async Task DeleteMessage(Message message, bool hard)
        {
            var uri = _requestUriFactory.CreateDeleteMessageUri(message, hard);

            var response = await _httpClient.DeleteAsync(uri);
            var responseContent = await response.Content.ReadAsStringAsync();

            LogRestCall(uri, responseContent);

            if (!response.IsSuccessStatusCode)
            {
                _logs.Error("Failed to send message. Response: " + responseContent);
            }
        }

        [Obsolete]
        public async Task Mute(User user)
        {
            var uri = _requestUriFactory.CreateMuteUserUri();

            var payload = new MuteUserObsolete()
            {
                Id = user.Id
            };

            var requestContent = _serializer.Serialize(payload);

            var response = await _httpClient.PostAsync(uri, requestContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            LogRestCall(uri, requestContent, responseContent);

            if (!response.IsSuccessStatusCode)
            {
                _logs.Error("Failed to send message. Response: " + responseContent);
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
        private readonly IEventHandlersRepository _serverEventsMapping = new EventHandlersRepository();
        private readonly IRequestUriFactory _requestUriFactory;
        private readonly IHttpClient _httpClient;

        private string _connectionId;
        private float _lastHealthCheckReceivedTime;
        private float _lastHealthCheckSendTime;

        private void OnWebsocketsConnected() => _logs.Info("Websockets Connected");

        private void OnConnectionConfirmed() => Connected?.Invoke();

        private void Reconnect()
        {
            ConnectionState = ConnectionState.Reconnecting;
            Connect();
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
            var msg = new HealthCheckRequestObsolete
            {
                Type = EventType.HealthCheck,

                //TOdo: is this valid? Seems to work, but for react SDK was `leia_organa--ce260d55-c24e-4963-887a-3b90a30a6175`
                ClientId = _authData.UserId
            };
            _websocketClient.Send(_serializer.Serialize(msg));

            _lastHealthCheckSendTime = _timeService.Time;
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

            MessageReceived?.Invoke(messageNewEvent);
        }

        private void Handle(MessageDeletedEvent messageDeletedEvent)
        {
            _logs.Info("New deleted event received");

            MessageDeleted?.Invoke(messageDeletedEvent);
        }

        private void Handle(MessageUpdated messageUpdatedEvent)
        {
            _logs.Info("New deleted event received");

            MessageUpdated?.Invoke(messageUpdatedEvent);
        }

        private void LogRestCall(Uri uri, string request, string response)
        {
            _logs.Info($"REST API Call: {uri}\n\nRequest:\n{request}\n\nResponse:\n{response}\n\n\n");
        }

        private void LogRestCall(Uri uri, string response)
        {
            _logs.Info($"REST API Call: {uri}\n\nResponse:\n{response}\n\n\n");
        }
    }
}