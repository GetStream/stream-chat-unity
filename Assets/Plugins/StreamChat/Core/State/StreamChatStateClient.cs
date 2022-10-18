using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StreamChat.Core.Configs;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.Requests;
using StreamChat.Core.State.TrackedObjects;
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
    public delegate void ConnectionMadeHandler(StreamLocalUser localUser);

    /// <summary>
    /// Connection state has changed
    /// </summary>
    public delegate void ConnectionChangeHandler(ConnectionState previous, ConnectionState current);

    //StreamTodo: move all xml doc comments to interface and use <inheritdoc /> on implementation

    /// <summary>
    /// Stateful client for the Stream Chat API. This is the recommended client
    /// </summary>
    public class StreamChatStateClient : IStreamChatStateClient
    {
        /// <summary>
        /// Triggered when connection with Stream Chat server is established
        /// </summary>
        public event ConnectionMadeHandler Connected;

        /// <summary>
        /// Triggered when connection with Stream Chat server is lost
        /// </summary>
        public event Action Disconnected;

        /// <summary>
        /// Triggered when connection state with Stream Chat server has changed
        /// </summary>
        public event ConnectionChangeHandler ConnectionStateChanged;

        /// <summary>
        /// Current connection state
        /// </summary>
        public ConnectionState ConnectionState => LowLevelClient.ConnectionState;

        /// <summary>
        /// Local user that is connected to the Stream Chat. This fields gets set after the client connection is established.
        /// </summary>
        public StreamLocalUser LocalUser { get; private set; }

        /// <summary>
        /// Channels loaded via <see cref="GetOrCreateChannelAsync"/> and <see cref="QueryChannelsAsync"/>
        ///
        /// These channels are receiving automatic updates
        /// </summary>
        public IEnumerable<StreamChannel> LoadedChannels => _cache.Channels.AllItems;

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
            var websocketClient = LibsFactory.CreateDefaultWebsocketClient(logs, config.LogLevel.IsDebugEnabled());
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

            LowLevelClient = new StreamChatClient(authCredentials: default, websocketClient, httpClient, serializer,
                _timeService, logs, config);

            _cache = new Cache(this, _logs);

            SubscribeTo(LowLevelClient);
        }

        public void Update()
        {
            LowLevelClient.Update(_timeService.DeltaTime);

            if (LowLevelClient.ConnectionState == ConnectionState.Connecting &&
                _connectUserCancellationToken.IsCancellationRequested)
            {
                //Todo: cancel connection
            }
        }

        //Todo: timeout, like 5 seconds?
        public Task<StreamLocalUser> ConnectUserAsync(AuthCredentials userAuthCredentials,
            CancellationToken cancellationToken = default)
        {
            LowLevelClient.ConnectUser(userAuthCredentials);

            _connectUserCancellationToken = cancellationToken;
            _connectUserTaskSource = new TaskCompletionSource<StreamLocalUser>();
            return _connectUserTaskSource.Task;
        }

        public Task DisconnectUserAsync() => LowLevelClient.DisconnectAsync();

        // StreamTodo: Pagination should probably be removed here and only available through channel.GetNextMessages, channel.GetPreviousMessages
        // Otherwise we have problem that you fetch old messages and then WS event delivers a new one

        //StreamTodo: remove this method, use GetOrCreateChannelAsync only
        public async Task<StreamChannel> GetOrCreateChannelAsync(string channelType, string channelId)
        {
            var requestBody = new ChannelGetOrCreateRequest
            {
                Presence = true,
                State = true,
                Watch = true
            };

            var channelResponseDto = await LowLevelClient.InternalChannelApi.GetOrCreateChannelAsync(channelType,
                channelId, requestBody.TrySaveToDto());

            return _cache.Channels.CreateOrUpdate<StreamChannel, ChannelStateResponseInternalDTO>(channelResponseDto, out _);
        }

        public Task<StreamChannel> GetOrCreateChannelAsync(ChannelType channelType, string channelId)
        {
            if (!channelType.IsValid)
            {
                throw new ArgumentException($"Invalid {nameof(channelType)} - internal key is empty");
            }

            return GetOrCreateChannelAsync(channelType.ToString(), channelId);
        }

        public Task<IEnumerable<StreamChannel>> QueryChannelsAsync()
        {
            return null;
        }

        public async Task MuteMultipleChannelsAsync(IEnumerable<StreamChannel> channels, int? milliseconds = default)
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

            // StreamTodo: unpack response
            var response = await LowLevelClient.InternalChannelApi.MuteChannelAsync(new MuteChannelRequestInternalDTO
            {
                ChannelCids = channelCids,
                Expiration = milliseconds
            });
        }

        public async Task UnmuteMultipleChannelsAsync(IEnumerable<StreamChannel> channels)
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

            // StreamTodo: unpack response
            var response = await LowLevelClient.InternalChannelApi.UnmuteChannelAsync(new UnmuteChannelRequestInternalDTO
            {
                ChannelCids = channelCids,
                //StreamTodo: what is this Expiration here?
            });
        }

        /// <summary>
        /// Delete multiple channels
        /// </summary>
        /// <param name="cids">Collection of <see cref="StreamChannel.Cid"/></param>
        /// <param name="isHardDelete">Hard delete removes channels entirely whereas Soft Delete deletes them from client but still allows to access them from the server-side SDK</param>
        public async Task DeleteMultipleChannelsAsync(IEnumerable<string> cids, bool isHardDelete = false)
        {
            // StreamTodo: unpack response
            await LowLevelClient.InternalChannelApi.DeleteChannelsAsync(new DeleteChannelsRequestInternalDTO
            {
                Cids = cids.ToList(),
                HardDelete = isHardDelete
            });
        }

        public void Dispose()
        {
            if (LowLevelClient != null)
            {
                UnsubscribeFrom(LowLevelClient);
                LowLevelClient.Dispose();
            }
        }

        internal ICache Cache => _cache;

        public StreamChatClient LowLevelClient { get; } //StreamTodo: change to internal

        private readonly ILogs _logs;
        private readonly ITimeService _timeService;
        private readonly ICache _cache;

        private TaskCompletionSource<StreamLocalUser> _connectUserTaskSource;
        private CancellationToken _connectUserCancellationToken;

        #region Connection Events

        private void OnLowLevelClientConnected(EventHealthCheckInternalDTO dto)
        {
            try
            {
                var localUserDto = dto.Me;
                LocalUser = _cache.TryCreateOrUpdate(localUserDto);
                Connected?.Invoke(LocalUser);
            }
            finally
            {
                if (_connectUserTaskSource == null)
                {
                    _logs.Error($"{nameof(OnLowLevelClientConnected)} expected {nameof(_connectUserTaskSource)} not null");
                }
                else
                {
                    _connectUserTaskSource.SetResult(LocalUser);
                }
            }
        }

        private void OnLowLevelClientDisconnected() => Disconnected?.Invoke();

        private void OnLowLevelClientConnectionStateChanged(ConnectionState previous, ConnectionState current)
            => ConnectionStateChanged?.Invoke(previous, current);

        #endregion

        #region Messages Events

        private void OnLowLevelClientMessageDeleted(EventMessageDeletedInternalDTO eventMessageDeleted)
        {
            if (_cache.Channels.TryGet(eventMessageDeleted.Cid, out var streamChannel))
            {
                streamChannel.HandleMessageDeletedEvent(eventMessageDeleted);
            }
        }

        private void OnLowLevelClientMessageUpdated(EventMessageUpdatedInternalDTO eventMessageUpdated)
        {
            if (_cache.Channels.TryGet(eventMessageUpdated.Cid, out var streamChannel))
            {
                streamChannel.HandleMessageUpdatedEvent(eventMessageUpdated);
            }
        }

        private void OnLowLevelClientMessageReceived(EventMessageNewInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.HandleMessageNewEvent(eventDto);
            }
        }

        #endregion

        private void SubscribeTo(StreamChatClient lowLevelClient)
        {
            lowLevelClient.InternalConnected += OnLowLevelClientConnected;
            lowLevelClient.Disconnected += OnLowLevelClientDisconnected;
            lowLevelClient.ConnectionStateChanged += OnLowLevelClientConnectionStateChanged;

            lowLevelClient.InternalMessageReceived += OnLowLevelClientMessageReceived;
            lowLevelClient.InternalMessageUpdated += OnLowLevelClientMessageUpdated;
            lowLevelClient.InternalMessageDeleted += OnLowLevelClientMessageDeleted;
        }

        private void UnsubscribeFrom(StreamChatClient lowLevelClient)
        {
            lowLevelClient.InternalConnected -= OnLowLevelClientConnected;
            lowLevelClient.Disconnected -= OnLowLevelClientDisconnected;
            lowLevelClient.ConnectionStateChanged -= OnLowLevelClientConnectionStateChanged;

            lowLevelClient.InternalMessageReceived -= OnLowLevelClientMessageReceived;
            lowLevelClient.InternalMessageUpdated -= OnLowLevelClientMessageUpdated;
            lowLevelClient.InternalMessageDeleted -= OnLowLevelClientMessageDeleted;
        }
    }
}