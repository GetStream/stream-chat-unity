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
using StreamChat.Core.State.Requests;
using StreamChat.Core.State.Responses;
using StreamChat.Core.State.TrackedObjects;
using StreamChat.Core.State.Caches;
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
    /// You can always access local user data via <see cref="StreamChatStateClient.LocalUserData"/>
    /// </summary>
    public delegate void ConnectionMadeHandler(StreamLocalUserData localUserData);

    /// <summary>
    /// Connection state has changed
    /// </summary>
    public delegate void ConnectionChangeHandler(ConnectionState previous, ConnectionState current);

    /// <summary>
    /// Channel was deleted
    /// </summary>
    public delegate void ChannelDeleteHandler(string channelCid, string channelId, ChannelType channelType);


    //StreamTodo: move all xml doc comments to interface and use <inheritdoc /> on implementation
    //StreamTodo: Handle restoring state after lost connection + include Unity Network Monitor
    //When connection is lost we potentially lose events 

    /// <summary>
    /// Stateful client for the Stream Chat API. This is the recommended client
    /// </summary>
    public sealed class StreamChatStateClient : IStreamChatStateClient
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

        /// <inheritdoc cref="IStreamChatStateClient.LocalUserData"/>
        public StreamLocalUserData LocalUserData { get; private set; }

        /// <inheritdoc cref="IStreamChatStateClient.WatchedChannels"/>
        public IReadOnlyList<StreamChannel> WatchedChannels => _cache.Channels.AllItems;

        public double? NextReconnectTime => LowLevelClient.NextReconnectTime;
        
        /// <summary>
        /// Recommended method to create an instance of <see cref="IStreamChatStateClient"/>
        /// If you wish to create an instance with non default dependencies you can use the constructor
        /// </summary>
        /// <param name="config">[Optional] configuration</param>
        /// <returns></returns>
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

        //StreamTodo: consider having constructor private and add static CreateCustomizedClient()
        /// <summary>
        /// Use this only if you wish to provide non default arguments. Otherwise use the <see cref="CreateDefaultClient"/> to create the client instance.
        /// </summary>
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
        public Task<StreamLocalUserData> ConnectUserAsync(AuthCredentials userAuthCredentials,
            CancellationToken cancellationToken = default)
        {
            LowLevelClient.ConnectUser(userAuthCredentials);

            _connectUserCancellationToken = cancellationToken;
            _connectUserTaskSource = new TaskCompletionSource<StreamLocalUserData>();
            return _connectUserTaskSource.Task;
        }

        public Task DisconnectUserAsync() => LowLevelClient.DisconnectAsync();

        public bool IsLocalUser(StreamUser user) => LocalUserData.User == user;

        // StreamTodo: Pagination should probably be removed here and only available through channel.GetNextMessages, channel.GetPreviousMessages
        // Otherwise we have problem that you fetch old messages and then WS event delivers a new one

        //StreamTodo: how about we add name as argument?
        /// <inheritdoc cref="IStreamChatStateClient.GetOrCreateChannelAsync(StreamChat.Core.State.ChannelType,string,IStreamChannelCustomData)"/>
        public async Task<StreamChannel> GetOrCreateChannelAsync(ChannelType channelType, string channelId,
            string name = null,
            IStreamChannelCustomData optionalCustomData = null)
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
                    AdditionalProperties = optionalCustomData?.Items.ToDictionary(x => x.Key, x => x.Value)
                },
            };

            var channelResponseDto = await LowLevelClient.InternalChannelApi.GetOrCreateChannelAsync(channelType,
                channelId, requestBodyDto);
            return _cache.Channels.CreateOrUpdate<StreamChannel, ChannelStateResponseInternalDTO>(channelResponseDto,
                out _);
        }

        /// <inheritdoc cref="IStreamChatStateClient.GetOrCreateChannelAsync(StreamChat.Core.State.ChannelType,System.Collections.Generic.IEnumerable{StreamChat.Core.State.TrackedObjects.StreamUser},IStreamChannelCustomData)"/>
        public async Task<StreamChannel> GetOrCreateChannelAsync(ChannelType channelType,
            IEnumerable<StreamUser> members, IStreamChannelCustomData optionalCustomData = null)
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

            var channelResponseDto =
                await LowLevelClient.InternalChannelApi.GetOrCreateChannelAsync(channelType, requestBodyDto);
            return _cache.Channels.CreateOrUpdate<StreamChannel, ChannelStateResponseInternalDTO>(channelResponseDto,
                out _);
        }

        //StreamTodo: Filter object that contains a factory
        public async Task<IEnumerable<StreamChannel>> QueryChannelsAsync(IDictionary<string, object> filters)
        {
            //StreamTodo: Perhaps MessageLimit and MemberLimit should be configurable
            var requestBodyDto = new QueryChannelsRequestInternalDTO
            {
                FilterConditions = filters.ToDictionary(x => x.Key, x => x.Value),
                Limit = null,
                MemberLimit = null,
                MessageLimit = null,
                Offset = null,
                Presence = true,
                Sort = null, //StreamTodo: sorting could be controlled in global config, we definitely don't want to control this per request as this could break data integrity
                State = true,
                Watch = true,
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

        public async Task<IEnumerable<StreamUser>> UpsertUsers(IEnumerable<StreamUserUpsertRequest> userRequests)
        {
            StreamAsserts.AssertNotNullOrEmpty(userRequests, nameof(userRequests));

            var requestDtos = userRequests.Select(_ => _.TrySaveToDto()).ToDictionary(_ => _.Id, _ => _);

            var response = await LowLevelClient.InternalUserApi.UpsertManyUsersAsync(new UpdateUsersRequestInternalDTO
            {
                Users = requestDtos
            });

            var result = new List<StreamUser>();
            foreach (var userDto in response.Users.Values)
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
        public async Task<StreamDeleteChannelsResponse> DeleteMultipleChannelsAsync(IEnumerable<StreamChannel> channels,
            bool isHardDelete = false)
        {
            StreamAsserts.AssertNotNullOrEmpty(channels, nameof(channels));

            var responseDto = await LowLevelClient.InternalChannelApi.DeleteChannelsAsync(
                new DeleteChannelsRequestInternalDTO
                {
                    Cids = channels.Select(_ => _.Cid).ToList(),
                    HardDelete = isHardDelete
                });

            var response = new StreamDeleteChannelsResponse().UpdateFromDto(responseDto);
            return response;
        }

        /// <summary>
        /// You mute single user by using <see cref="StreamUser.MuteAsync"/>
        /// </summary>
        /// <param name="users"></param>
        /// <param name="timeoutMinutes"></param>
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

        internal Task RefreshChannelState(string cid)
        {
            if (!_cache.Channels.TryGet(cid, out var channel))
            {
                _logs.Error($"Tried to refresh state of channel with {cid} but no such channel was found in the cache");
                return Task.CompletedTask;
            }

            return GetOrCreateChannelAsync(channel.Type, channel.Id);
        }

        private readonly ILogs _logs;
        private readonly ITimeService _timeService;
        private readonly ICache _cache;

        private TaskCompletionSource<StreamLocalUserData> _connectUserTaskSource;
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
                    _logs.Error(
                        $"{nameof(OnLowLevelClientConnected)} expected {nameof(_connectUserTaskSource)} not null");
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
                InternalDeleteChannel(streamChannel);
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
                InternalDeleteChannel(streamChannel);
            }
        }

        private void OnLowLevelClientChannelUpdated(EventChannelUpdatedInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.HandleChannelUpdatedEvent(eventDto);
            }
        }

        private void LowLevelClientOnInternalNotificationChannelTruncated(
            EventNotificationChannelTruncatedInternalDTO eventDto)
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

            lowLevelClient.InternalUserPresenceChanged += OnLowLevelClientUserPresenceChanged;
            lowLevelClient.InternalUserUpdated += OnLowLevelUserUpdated;
            lowLevelClient.InternalUserDeleted += OnLowLevelClientUserDeleted;
            lowLevelClient.InternalUserBanned += OnLowLevelClientUserBanned;
            lowLevelClient.InternalUserUnbanned += OnLowLevelClientUserUnbanned;

            lowLevelClient.InternalUserWatchingStart += OnLowLevelClientUserWatchingStart;
            lowLevelClient.InternalUserWatchingStop += OnLowLevelClientUserWatchingStop;

            lowLevelClient.InternalReactionReceived += OnLowLevelClientReactionReceived;
            lowLevelClient.InternalReactionUpdated += OnLowLevelClientReactionUpdated;
            lowLevelClient.InternalReactionDeleted += OnLowLevelClientReactionDeleted;

            lowLevelClient.InternalTypingStarted += OnLowLevelClientTypingStarted;
            lowLevelClient.InternalTypingStopped += OnLowLevelClientTypingStopped;

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

        private void OnLowLevelClientMessageRead(EventMessageReadInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.InternalHandleMessageReadEvent(eventDto);
            }

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

            lowLevelClient.InternalMemberAdded -= OnLowLevelClientMemberAdded;
            lowLevelClient.InternalMemberRemoved -= OnLowLevelClientMemberRemoved;
            lowLevelClient.InternalMemberUpdated -= OnLowLevelClientMemberUpdated;

            lowLevelClient.InternalUserPresenceChanged -= OnLowLevelClientUserPresenceChanged;
            lowLevelClient.InternalUserUpdated -= OnLowLevelUserUpdated;
            lowLevelClient.InternalUserDeleted -= OnLowLevelClientUserDeleted;
            lowLevelClient.InternalUserBanned -= OnLowLevelClientUserBanned;
            lowLevelClient.InternalUserUnbanned -= OnLowLevelClientUserUnbanned;

            lowLevelClient.InternalUserWatchingStart -= OnLowLevelClientUserWatchingStart;
            lowLevelClient.InternalUserWatchingStop -= OnLowLevelClientUserWatchingStop;

            lowLevelClient.InternalReactionReceived -= OnLowLevelClientReactionReceived;
            lowLevelClient.InternalReactionUpdated -= OnLowLevelClientReactionUpdated;
            lowLevelClient.InternalReactionDeleted -= OnLowLevelClientReactionDeleted;

            lowLevelClient.InternalTypingStarted -= OnLowLevelClientTypingStarted;
            lowLevelClient.InternalTypingStopped -= OnLowLevelClientTypingStopped;

            lowLevelClient.InternalNotificationChannelDeleted -= OnLowLevelClientNotificationChannelDeleted;
            lowLevelClient.InternalNotificationChannelTruncated -= LowLevelClientOnInternalNotificationChannelTruncated;

            lowLevelClient.InternalNotificationChannelMutesUpdated -= OnLowLevelClientChannelMutesUpdated;
        }

        private void OnLowLevelClientReactionReceived(EventReactionNewInternalDTO eventDto)
        {
            if (_cache.Messages.TryGet(eventDto.Message.Id, out var message))
            {
                message.HandleReactionNewEvent(eventDto);
            }
        }

        private void OnLowLevelClientReactionDeleted(EventReactionDeletedInternalDTO eventDto)
        {
            if (_cache.Messages.TryGet(eventDto.Message.Id, out var message))
            {
                message.HandleReactionDeletedEvent(eventDto);
            }
        }

        private void OnLowLevelClientReactionUpdated(EventReactionUpdatedInternalDTO eventDto)
        {
            if (_cache.Messages.TryGet(eventDto.Message.Id, out var message))
            {
                message.HandleReactionUpdatedEvent(eventDto);
            }
        }

        private void OnLowLevelClientUserWatchingStop(EventUserWatchingStopInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.InternalHandleUserWatchingStop(eventDto);
            }
        }

        private void OnLowLevelClientUserWatchingStart(EventUserWatchingStartInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.InternalHandleUserWatchingStart(eventDto);
            }
        }

        private void OnLowLevelClientUserUnbanned(EventUserUnbannedInternalDTO obj)
        {
            //throw new NotImplementedException();
        }

        private void OnLowLevelClientUserBanned(EventUserBannedInternalDTO obj)
        {
        }

        private void OnLowLevelClientUserDeleted(EventUserDeletedInternalDTO obj)
        {
            //throw new NotImplementedException();
        }

        private void OnLowLevelUserUpdated(EventUserUpdatedInternalDTO eventDto)
        {
            if (_cache.Users.TryGet(eventDto.User.Id, out var streamUser))
            {
                _cache.TryCreateOrUpdate(eventDto.User);
            }
        }

        private void OnLowLevelClientUserPresenceChanged(EventUserPresenceChangedInternalDTO eventDto)
        {
            if (_cache.Users.TryGet(eventDto.User.Id, out var streamUser))
            {
                streamUser.InternalHandlePresenceChanged(eventDto);
            }
        }

        private void OnLowLevelClientTypingStopped(EventTypingStopInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.InternalHandleTypingStopped(eventDto);
            }
        }

        private void OnLowLevelClientTypingStarted(EventTypingStartInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.InternalHandleTypingStarted(eventDto);
            }
        }

        private void InternalDeleteChannel(StreamChannel channel)
        {
            //StreamTodo: probably silent clear all internal data?
            _cache.Channels.Remove(channel);
            ChannelDeleted?.Invoke(channel.Cid, channel.Id, channel.Type);
        }
    }
}