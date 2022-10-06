using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StreamChat.Core.Configs;
using StreamChat.Core.Events;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.Models;
using StreamChat.Core.Requests;
using StreamChat.Core.State.Models;
using StreamChat.Core.State;
using StreamChat.Libs;
using StreamChat.Libs.Auth;
using StreamChat.Libs.Http;
using StreamChat.Libs.Logs;
using StreamChat.Libs.Serialization;
using StreamChat.Libs.Time;
using StreamChat.Libs.Websockets;

namespace StreamChat.Core.State
{
    /// <summary>
    /// Connection state has changed
    /// </summary>
    public delegate void ConnectionChangeHandler(ConnectionState previous, ConnectionState current);

    /// <summary>
    /// Stateful client for the Stream Chat API. This is the recommended client
    /// </summary>
    public class StreamChatStateClient : IStreamChatStateClient
    {
        /// <summary>
        /// Triggered when connection with Stream Chat server is established
        /// </summary>
        public event Action<StreamLocalUser> Connected; // Todo: change to dedicated delegate?

        /// <summary>
        /// Triggered when connection with Stream Chat server is lost
        /// </summary>
        public event Action Disconnected;

        /// <summary>
        /// Triggered when connection state with Stream Chat server has changed
        /// </summary>
        public event ConnectionChangeHandler ConnectionStateChanged;

        /// <summary>
        /// Local user that is connected to the Stream Chat. This fields gets set after the client connection is established.
        /// </summary>
        public StreamLocalUser LocalUser { get; private set; } //Todo: observable

        /// <summary>
        /// Use this method to create a default Stream Chat Client instance.
        /// Alternatively you can use the constructor to provide non default dependencies for the client.
        /// </summary>
        /// <param name="config">Optional configuration</param>
        /// <returns>New instance of <see cref="IStreamChatStateClient"/></returns>
        public static IStreamChatStateClient CreateDefaultClient(IStreamClientConfig config = default)
        {
            config ??= StreamClientConfig.Default;
            var logs = LibsFactory.CreateDefaultLogs(config.LogLevel.ToLogLevel());
            var websocketClient
                = LibsFactory.CreateDefaultWebsocketClient(logs, isDebugMode: config.LogLevel.IsDebugEnabled());
            var httpClient = LibsFactory.CreateDefaultHttpClient();
            var serializer = LibsFactory.CreateDefaultSerializer();
            var timeService = LibsFactory.CreateDefaultTimeService();

            return new StreamChatStateClient(websocketClient, httpClient, serializer, timeService, logs, config);
        }

        public StreamChatStateClient(IWebsocketClient websocketClient,
            IHttpClient httpClient, ISerializer serializer, ITimeService timeService, ILogs logs,
            IStreamClientConfig config)
        {
            _timeService = timeService ?? throw new ArgumentNullException(nameof(timeService));
            _logs = logs ?? throw new ArgumentNullException(nameof(logs));

            _lowLevelClient = new StreamChatClient(authCredentials: default, websocketClient, httpClient, serializer,
                _timeService, logs, config);

            SubscribeTo(_lowLevelClient);
        }

        public void Update()
        {
            _lowLevelClient.Update(_timeService.DeltaTime);

            if (_lowLevelClient.ConnectionState == ConnectionState.Connecting &&
                _connectUserCancellationToken.IsCancellationRequested)
            {
                //Todo: cancel connection
            }
        }

        //Todo: maybe Async?
        //Todo: cancellation token?
        //Todo: timeout, like 5 seconds?
        public Task<StreamLocalUser> ConnectUserAsync(AuthCredentials userAuthCredentials,
            CancellationToken cancellationToken = default)
        {
            _lowLevelClient.ConnectUser(userAuthCredentials);

            _connectUserCancellationToken = cancellationToken;
            _connectUserTaskSource = new TaskCompletionSource<StreamLocalUser>();
            return _connectUserTaskSource.Task;
        }

        // TODO: Pagination should probably be removed here and only available through channel.GetNextMessages, channel.GetPreviousMessages
        // Otherwise we have problem that you fetch old messages and then WS event delivers a new one

        //Todo: consider changing ChannelGetOrCreateRequest to custom request structure
        public async Task<StreamChannel> GetOrCreateChannelAsync(string channelType, string channelId,
            ChannelGetOrCreateRequest requestBody = default)
        {
            requestBody ??= new ChannelGetOrCreateRequest();

            requestBody.State = true;
            requestBody.Watch = true;

            var channelResponseDto = await _lowLevelClient.InternalChannelApi.GetOrCreateChannelAsync(channelType,
                channelId, requestBody.TrySaveToDto());

            var uniqueId = GetUniqueId(channelResponseDto);
            var streamChannel = _cache.Channels.CreateOrUpdate<StreamChannel, ChannelStateResponseInternalDTO>(uniqueId, channelResponseDto);

            // Todo: maybe add UpdateOrCreate channelResponse.Channel.Cid
            // Todo: the ID mapping should be defined instead of manually passing the channel.Cid
            // var streamChannel = _channelsRepository.GetOrCreate(channelResponseDto.Channel.Cid);
            // streamChannel.UpdateFrom(channelResponseDto, repositories: this);

            return streamChannel;
        }

        // Todo: Add TrackedId with ctors for all data structures, this way we can consistently pick the same field as ID
        private string GetUniqueId(ChannelStateResponseInternalDTO dto) => dto.Channel.Cid;

        public Task<IEnumerable<StreamChannel>> QueryChannelsAsync()
        {
            return null;
        }

        public void Dispose()
        {
            if (_lowLevelClient != null)
            {
                UnsubscribeFrom(_lowLevelClient);
                _lowLevelClient.Dispose();
            }
        }



        private readonly StreamChatClient _lowLevelClient;
        private readonly ILogs _logs;
        private readonly ITimeService _timeService;
        private readonly ICache _cache = new Cache();

        private TaskCompletionSource<StreamLocalUser> _connectUserTaskSource;
        private CancellationToken _connectUserCancellationToken;

        #region Connection Events

        private void OnLowLevelClientConnected(OwnUser ownUser)
        {
            //Todo: get from repository, perhaps we're reconnecting and this object is already tracked
            LocalUser = _cache.LocalUser.CreateOrUpdate<StreamLocalUser, OwnUser>(ownUser.Id, ownUser);

            if (_connectUserTaskSource == null)
            {
                _logs.Error($"{nameof(OnLowLevelClientConnected)} expected {nameof(_connectUserTaskSource)} not null");
            }
            else
            {
                _connectUserTaskSource.SetResult(LocalUser);
            }

            Connected?.Invoke(LocalUser);
        }

        private void OnLowLevelClientDisconnected() => Disconnected?.Invoke();

        private void OnLowLevelClientConnectionStateChanged(ConnectionState previous, ConnectionState current)
            => ConnectionStateChanged?.Invoke(previous, current);

        #endregion

        #region Messages Events

        private void OnLowLevelClientMessageDeleted(EventMessageDeleted eventMessageDeleted)
        {
            if (_cache.Channels.TryGet(eventMessageDeleted.Cid, out var streamChannel))
            {
                var isHardDelete = eventMessageDeleted.HardDelete.GetValueOrDefault(false);
                streamChannel.DeleteMessage(eventMessageDeleted.Message.Id, isHardDelete);
            }
        }

        private void OnLowLevelClientMessageUpdated(EventMessageUpdated eventMessageUpdated)
        {
            if (_cache.Channels.TryGet(eventMessageUpdated.Cid, out var streamChannel))
            {
                //streamChannel.UpdateMessage();
            }
        }

        private void OnLowLevelClientMessageReceived(EventMessageNew obj)
        {
            if (_cache.Channels.TryGet(obj.Cid, out var streamChannel))
            {
            }
        }

        #endregion

        private void SubscribeTo(IStreamChatClient lowLevelClient)
        {
            lowLevelClient.Connected += OnLowLevelClientConnected;
            lowLevelClient.Disconnected += OnLowLevelClientDisconnected;
            lowLevelClient.ConnectionStateChanged += OnLowLevelClientConnectionStateChanged;

            lowLevelClient.MessageReceived += OnLowLevelClientMessageReceived;
            lowLevelClient.MessageUpdated += OnLowLevelClientMessageUpdated;
            lowLevelClient.MessageDeleted += OnLowLevelClientMessageDeleted;
        }

        private void UnsubscribeFrom(IStreamChatClient lowLevelClient)
        {
            lowLevelClient.Connected -= OnLowLevelClientConnected;
            lowLevelClient.Disconnected -= OnLowLevelClientDisconnected;
            lowLevelClient.ConnectionStateChanged -= OnLowLevelClientConnectionStateChanged;

            lowLevelClient.MessageReceived -= OnLowLevelClientMessageReceived;
            lowLevelClient.MessageUpdated -= OnLowLevelClientMessageUpdated;
            lowLevelClient.MessageDeleted -= OnLowLevelClientMessageDeleted;
        }
    }
}