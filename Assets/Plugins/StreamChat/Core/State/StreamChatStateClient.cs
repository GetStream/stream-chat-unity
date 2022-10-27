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
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.State.Responses;
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

    public delegate void ChannelDeleteHandler(string channelCid, string channelId, string channelType);


    //StreamTodo: move all xml doc comments to interface and use <inheritdoc /> on implementation

    /// <summary>
    /// Stateful client for the Stream Chat API. This is the recommended client
    /// </summary>
    public class StreamChatStateClient : IStreamChatStateClient
    {
        /// <inheritdoc cref="IStreamChatStateClient.Connected"/>
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
        /// Channel was deleted
        /// </summary>
        public event ChannelDeleteHandler ChannelDeleted;

        /// <summary>
        /// Current connection state
        /// </summary>
        public ConnectionState ConnectionState => LowLevelClient.ConnectionState;

        /// <summary>
        /// Local user that is connected to the Stream Chat. This fields gets set after the client connection is established.
        /// You can access the local <see cref="StreamUser"/> via <see cref="LocalUserData"/> <see cref="StreamLocalUser.User"/> property
        /// </summary>
        public StreamLocalUser LocalUserData { get; private set; }

        /// <summary>
        /// Watched channels to which this client receives realtime events regarding messages, reactions and any other users activity
        ///
        /// These channels are being automatically updated by the StreamChatClient
        ///
        /// You can watch additional channels via <see cref="GetOrCreateChannelAsync"/> and <see cref="QueryChannelsAsync"/>
        /// </summary>
        public IEnumerable<StreamChannel> WatchedChannels => _cache.Channels.AllItems;

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
                //StreamTodo: cancel connection
            }
        }

        //StreamTodo: timeout, like 5 seconds?
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

        public async Task<StreamChannel> GetOrCreateChannelAsync(ChannelType channelType, string channelId,
            IChannelCustomData optionalCustomData = null)
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
                    AdditionalProperties = optionalCustomData?.Items.ToDictionary(x => x.Key, x => x.Value)
                }
            };

            var channelResponseDto = await LowLevelClient.InternalChannelApi.GetOrCreateChannelAsync(channelType,
                channelId, requestBodyDto);
            return _cache.Channels.CreateOrUpdate<StreamChannel, ChannelStateResponseInternalDTO>(channelResponseDto, out _);
        }

        public async Task<StreamChannel> GetOrCreateChannelAsync(ChannelType channelType,
            IEnumerable<StreamUser> members, IChannelCustomData optionalCustomData = null)
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
                    //StreamTodo: avoid this allocation, maybe method to  pass dictionary and write all items?
                    AdditionalProperties = optionalCustomData?.Items.ToDictionary(x => x.Key, x => x.Value)
                }
            };

            var channelResponseDto = await LowLevelClient.InternalChannelApi.GetOrCreateChannelAsync(channelType, requestBodyDto);
            return _cache.Channels.CreateOrUpdate<StreamChannel, ChannelStateResponseInternalDTO>(channelResponseDto, out _);
        }

        //StreamTodo: Filter object that contains a factory
        public async Task<IEnumerable<StreamChannel>> QueryChannelsAsync(IDictionary<string, object> filters)
        {
            var requestBodyDto = new QueryChannelsRequestInternalDTO
            {
                Watch = true,
                State = true,
                Presence = true,
                FilterConditions = filters.ToDictionary(x => x.Key, x => x.Value)

            };

            var channelsResponseDto = await LowLevelClient.InternalChannelApi.QueryChannelsAsync(requestBodyDto);
            if (channelsResponseDto.Channels != null && channelsResponseDto.Channels.Count == 0)
            {
                return Enumerable.Empty<StreamChannel>();
            }

            var result = new List<StreamChannel>();
            foreach (var channelDto in channelsResponseDto.Channels)
            {
                result.Add(_cache.TryCreateOrUpdate(channelDto));
            }

            return result;
        }

        public async Task<IEnumerable<StreamUser>> QueryUsersAsync(IDictionary<string, object> filters)
        {
            //StreamTodo: Missing filter, and stuff like IdGte etc
            var requestBodyDto = new QueryUsersRequestInternalDTO
            {
                Presence = true,
                FilterConditions = filters.ToDictionary(x => x.Key, x => x.Value)
            };

            var response = await LowLevelClient.InternalUserApi.QueryUsersAsync(requestBodyDto);
            if (response.Users != null && response.Users.Count == 0)
            {
                return Enumerable.Empty<StreamUser>();
            }

            var result = new List<StreamUser>();
            foreach (var userDto in response.Users)
            {
                result.Add(_cache.TryCreateOrUpdate(userDto));
            }

            return result;
        }

        /// <inheritdoc cref="IStreamChatStateClient.MuteMultipleChannelsAsync"/>
        public async Task MuteMultipleChannelsAsync(IEnumerable<StreamChannel> channels, int? milliseconds = default)
        {
            StreamAsserts.AssertNotNullOrEmpty(channels, nameof(channels));

            var channelCids = channels.Select(_ => _.Cid).ToList();
            if (channelCids.Count == 0)
            {
                throw new ArgumentException($"{nameof(channels)} is empty");
            }

            var response = await LowLevelClient.InternalChannelApi.MuteChannelAsync(new MuteChannelRequestInternalDTO
            {
                ChannelCids = channelCids,
                Expiration = milliseconds
            });

            //StreamTodo: verify OwnUser object contents
            UpdateLocalUser(response.OwnUser);
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

            await LowLevelClient.InternalChannelApi.UnmuteChannelAsync(new UnmuteChannelRequestInternalDTO
            {
                ChannelCids = channelCids,
                //StreamTodo: what is this Expiration here?
            });
        }

        /// <inheritdoc />
        public async Task<StreamDeleteChannelsResponse> DeleteMultipleChannelsAsync(IEnumerable<StreamChannel> channels, bool isHardDelete = false)
        {
            StreamAsserts.AssertNotNullOrEmpty(channels, nameof(channels));

            var responseDto = await LowLevelClient.InternalChannelApi.DeleteChannelsAsync(new DeleteChannelsRequestInternalDTO
            {
                Cids = channels.Select(_ => _.Cid).ToList(),
                HardDelete = isHardDelete
            });

            //StreamTodo: unnecessary allocation - TryLoadFromDto creates new object internally
            var response = new StreamDeleteChannelsResponse().TryLoadFromDto(responseDto);
            return response;
        }

        public async Task MuteMultipleUsersAsync(IEnumerable<StreamUser> users, int? timeoutMinutes = default)
        {
            StreamAsserts.AssertNotNullOrEmpty(users, nameof(users));

            var responseDto = await LowLevelClient.InternalModerationApi.MuteUserAsync(new MuteUserRequestInternalDTO
            {
                TargetIds = users.Select(_ => _.Id).ToList(),
                Timeout = timeoutMinutes
            });

            UpdateLocalUser(responseDto.OwnUser);
        }

        public async Task<IEnumerable<StreamUser>> QueryBannedUsersAsync()
        {
            //StreamTodo: implement, should we allow for query
            throw new NotImplementedException();
        }

        //StreamTODO: Check ChannelLogic.kt and implement reacting to all possible events

        public void Dispose()
        {
            if (LowLevelClient != null)
            {
                UnsubscribeFrom(LowLevelClient);
                LowLevelClient.Dispose();
            }
        }

        internal StreamChatClient LowLevelClient { get; }

        internal void UpdateLocalUser(OwnUserInternalDTO ownUserInternalDto)
        {
            LocalUserData = _cache.TryCreateOrUpdate(ownUserInternalDto);

            //StreamTodo: Can we not rely on whoever called TryCreateOrUpdate to update this but make it more reliable? Better to react to some event
            // This could be solved if ChannelMutes would be an observable collection
            foreach (var channel in _cache.Channels.AllItems)
            {
                var isMuted = LocalUserData.ChannelMutes.Any(_ => _.Channel == channel);
                channel.Muted = isMuted;
            }
        }

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
                UpdateLocalUser(localUserDto);
                Connected?.Invoke(LocalUserData);
            }
            finally
            {
                if (_connectUserTaskSource == null)
                {
                    _logs.Error($"{nameof(OnLowLevelClientConnected)} expected {nameof(_connectUserTaskSource)} not null");
                }
                else
                {
                    _connectUserTaskSource.SetResult(LocalUserData);
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

        private void OnLowLevelClientChannelTruncated(EventChannelTruncatedInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.HandleChannelTruncatedEvent(eventDto);
            }
        }

        private void OnLowLevelClientNotificationChannelDeleted(EventNotificationChannelDeletedInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                DeleteChannel(streamChannel);
            }
        }

        private void OnLowLevelClientChannelVisible(EventChannelVisibleInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.Hidden = false;
            }
        }

        private void OnLowLevelClientChannelHidden(EventChannelHiddenInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.Hidden = true;
            }
        }

        private void OnLowLevelClientChannelDeleted(EventChannelDeletedInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                DeleteChannel(streamChannel);
            }
        }

        private void OnLowLevelClientChannelUpdated(EventChannelUpdatedInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.HandleChannelUpdatedEvent(eventDto);
            }
        }

        private void LowLevelClientOnInternalNotificationChannelTruncated(EventNotificationChannelTruncatedInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.HandleChannelTruncatedEvent(eventDto);
            }
        }

        private void OnLowLevelClientChannelMutesUpdated(EventNotificationChannelMutesUpdatedInternalDTO eventDto)
        {
            UpdateLocalUser(eventDto.Me);
        }

        private void SubscribeTo(StreamChatClient lowLevelClient)
        {
            lowLevelClient.InternalConnected += OnLowLevelClientConnected;
            lowLevelClient.Disconnected += OnLowLevelClientDisconnected;
            lowLevelClient.ConnectionStateChanged += OnLowLevelClientConnectionStateChanged;

            lowLevelClient.InternalMessageReceived += OnLowLevelClientMessageReceived;
            lowLevelClient.InternalMessageUpdated += OnLowLevelClientMessageUpdated;
            lowLevelClient.InternalMessageDeleted += OnLowLevelClientMessageDeleted;
            lowLevelClient.InternalMessageRead += OnLowLevelClientMessageRead;

            lowLevelClient.InternalChannelUpdated += OnLowLevelClientChannelUpdated;
            lowLevelClient.InternalChannelDeleted += OnLowLevelClientChannelDeleted;
            lowLevelClient.InternalChannelTruncated += OnLowLevelClientChannelTruncated;
            lowLevelClient.InternalChannelVisible += OnLowLevelClientChannelVisible;
            lowLevelClient.InternalChannelHidden += OnLowLevelClientChannelHidden;

            lowLevelClient.InternalMemberAdded += OnLowLevelClientMemberAdded;
            lowLevelClient.InternalMemberRemoved += OnLowLevelClientMemberRemoved;
            lowLevelClient.InternalMemberUpdated += OnLowLevelClientMemberUpdated;

            lowLevelClient.InternalNotificationChannelDeleted += OnLowLevelClientNotificationChannelDeleted;
            lowLevelClient.InternalNotificationChannelTruncated += LowLevelClientOnInternalNotificationChannelTruncated;

            lowLevelClient.InternalNotificationChannelMutesUpdated += OnLowLevelClientChannelMutesUpdated;
        }

        private void OnLowLevelClientMemberAdded(EventMemberAddedInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                var member = _cache.TryCreateOrUpdate(eventDto.Member);
                StreamAsserts.AssertNotNull(member, nameof(member));
                streamChannel.InternalAddMember(member);
            }
        }

        private void OnLowLevelClientMemberUpdated(EventMemberUpdatedInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                var member = _cache.TryCreateOrUpdate(eventDto.Member);
                StreamAsserts.AssertNotNull(member, nameof(member));
                streamChannel.InternalUpdateMember(member);
            }
        }

        private void OnLowLevelClientMemberRemoved(EventMemberRemovedInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                var member = _cache.TryCreateOrUpdate(eventDto.Member);
                StreamAsserts.AssertNotNull(member, nameof(member));
                streamChannel.InternalRemoveMember(member);
            }
        }

        private void OnLowLevelClientMessageRead(EventMessageReadInternalDTO obj)
        {
            //StreamTodo: IMPLEMENT
            // is MessageReadEvent -> {
            //     channelStateLogic.updateRead(ChannelUserRead(event.user, event.createdAt))
            // }
            // is NotificationMarkReadEvent -> {
            //     channelStateLogic.updateRead(ChannelUserRead(event.user, event.createdAt))
            // }
        }

        private void UnsubscribeFrom(StreamChatClient lowLevelClient)
        {
            lowLevelClient.InternalConnected -= OnLowLevelClientConnected;
            lowLevelClient.Disconnected -= OnLowLevelClientDisconnected;
            lowLevelClient.ConnectionStateChanged -= OnLowLevelClientConnectionStateChanged;

            lowLevelClient.InternalMessageReceived -= OnLowLevelClientMessageReceived;
            lowLevelClient.InternalMessageUpdated -= OnLowLevelClientMessageUpdated;
            lowLevelClient.InternalMessageDeleted -= OnLowLevelClientMessageDeleted;

            lowLevelClient.InternalChannelUpdated -= OnLowLevelClientChannelUpdated;
            lowLevelClient.InternalChannelDeleted -= OnLowLevelClientChannelDeleted;
            lowLevelClient.InternalChannelTruncated -= OnLowLevelClientChannelTruncated;
            lowLevelClient.InternalChannelVisible -= OnLowLevelClientChannelVisible;
            lowLevelClient.InternalChannelHidden -= OnLowLevelClientChannelHidden;

            lowLevelClient.InternalNotificationChannelDeleted -= OnLowLevelClientNotificationChannelDeleted;
            lowLevelClient.InternalNotificationChannelTruncated -= LowLevelClientOnInternalNotificationChannelTruncated;

            lowLevelClient.InternalNotificationChannelMutesUpdated -= OnLowLevelClientChannelMutesUpdated;
        }

        private void DeleteChannel(StreamChannel channel)
        {
            //StreamTodo: probably silent clear all internal data?
            _cache.Channels.Remove(channel);
            ChannelDeleted?.Invoke(channel.Cid, channel.Id, channel.Type);
        }
    }
}