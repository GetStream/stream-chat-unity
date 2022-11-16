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
using StreamChat.Core.LowLevelClient;
using StreamChat.Core.State;
using StreamChat.Core.State.Caches;
using StreamChat.Core.Models;
using StreamChat.Core.Requests;
using StreamChat.Core.Responses;
using StreamChat.Core.StatefulModels;
using StreamChat.Libs;
using StreamChat.Libs.Auth;
using StreamChat.Libs.ChatInstanceRunner;
using StreamChat.Libs.Http;
using StreamChat.Libs.Logs;
using StreamChat.Libs.Serialization;
using StreamChat.Libs.Time;
using StreamChat.Libs.Websockets;

namespace StreamChat.Core
{
    /// <summary>
    /// Connection has been established
    /// You can access local user data via <see cref="StreamChatClient.LocalUserData"/>
    /// </summary>
    public delegate void ConnectionMadeHandler(IStreamLocalUserData localUserData);

    /// <summary>
    /// Connection state change handler
    /// </summary>
    public delegate void ConnectionChangeHandler(ConnectionState previous, ConnectionState current);

    /// <summary>
    /// Channel deletion handler
    /// </summary>
    public delegate void ChannelDeleteHandler(string channelCid, string channelId, ChannelType channelType);
    
    //StreamTodo: Handle restoring state after lost connection + include Unity Network Monitor

    /// <summary>
    /// Stateful client for the Stream Chat API. This is the recommended client
    /// </summary>
    public sealed class StreamChatClient : IStreamChatClient
    {
        public event ConnectionMadeHandler Connected;
        
        public event Action Disconnected;

        public event Action Disposed;
        
        public event ConnectionChangeHandler ConnectionStateChanged;
        
        public event ChannelDeleteHandler ChannelDeleted;
        
        public ConnectionState ConnectionState => LowLevelClient.ConnectionState;

        public IStreamLocalUserData LocalUserData => _localUserData;

        private StreamLocalUserData _localUserData;

        public IReadOnlyList<IStreamChannel> WatchedChannels => _cache.Channels.AllItems;

        public double? NextReconnectTime => LowLevelClient.NextReconnectTime;

        /// <summary>
        /// Recommended method to create an instance of <see cref="IStreamChatClient"/>
        /// If you wish to create an instance with non default dependencies you can use the <see cref="CreateClientWithCustomDependencies"/>
        /// </summary>
        /// <param name="config">[Optional] configuration</param>
        public static IStreamChatClient CreateDefaultClient(IStreamClientConfig config = default)
        {
            config ??= StreamClientConfig.Default;
            var logs = LibsFactory.CreateDefaultLogs(config.LogLevel.ToLogLevel());
            var websocketClient = LibsFactory.CreateDefaultWebsocketClient(logs, config.LogLevel.IsDebugEnabled());
            var httpClient = LibsFactory.CreateDefaultHttpClient();
            var serializer = LibsFactory.CreateDefaultSerializer();
            var timeService = LibsFactory.CreateDefaultTimeService();
            var gameObjectRunner = LibsFactory.CreateChatClientRunner();

            var client = new StreamChatClient(websocketClient, httpClient, serializer, timeService, logs, config);
            gameObjectRunner.RunChatInstance(client);
            return client;
        }

        /// <summary>
        /// Create a new instance of <see cref="IStreamChatLowLevelClient"/> with custom provided dependencies.
        /// If you want to create a default new instance then just use the <see cref="CreateDefaultClient"/>.
        /// Important! Custom created client require calling the <see cref="Update"/> and <see cref="Destroy"/> methods.
        /// </summary>
        public static IStreamChatClient CreateClientWithCustomDependencies(IWebsocketClient websocketClient,
            IHttpClient httpClient, ISerializer serializer, ITimeService timeService, ILogs logs,
            IStreamClientConfig config) =>
            new StreamChatClient(websocketClient, httpClient, serializer, timeService, logs, config);

        public Task<IStreamLocalUserData> ConnectUserAsync(AuthCredentials userAuthCredentials,
            CancellationToken cancellationToken = default)
        {
            LowLevelClient.ConnectUser(userAuthCredentials);

            //StreamTodo: test calling this method multiple times in a row

            //StreamTodo: timeout, like 5 seconds?
            _connectUserCancellationToken = cancellationToken;

            _connectUserCancellationTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(_connectUserCancellationToken);
            _connectUserCancellationTokenSource.Token.Register(TryCancelWaitingForUserConnection);

            //StreamTodo: check if we can pass the cancellation token here
            _connectUserTaskSource = new TaskCompletionSource<IStreamLocalUserData>();
            return _connectUserTaskSource.Task;
        }

        public Task DisconnectUserAsync() => LowLevelClient.DisconnectAsync();

        public bool IsLocalUser(IStreamUser user) => LocalUserData.User == user;

        public async Task<IStreamChannel> GetOrCreateChannelAsync(ChannelType channelType, string channelId,
            string name = null, Dictionary<string, object> optionalCustomData = null)
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
                },
            };

            if (optionalCustomData != null && optionalCustomData.Any())
            {
                requestBodyDto.Data.AdditionalProperties = optionalCustomData;
            }

            var channelResponseDto = await LowLevelClient.InternalChannelApi.GetOrCreateChannelAsync(channelType,
                channelId, requestBodyDto);
            return _cache.TryCreateOrUpdate(channelResponseDto);
        }

        public async Task<IStreamChannel> GetOrCreateChannelAsync(ChannelType channelType,
            IEnumerable<IStreamUser> members, Dictionary<string, object> optionalCustomData = null)
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
                }
            };

            if (optionalCustomData != null && optionalCustomData.Any())
            {
                requestBodyDto.Data.AdditionalProperties = optionalCustomData;
            }

            var channelResponseDto =
                await LowLevelClient.InternalChannelApi.GetOrCreateChannelAsync(channelType, requestBodyDto);
            return _cache.TryCreateOrUpdate(channelResponseDto);
        }

        //StreamTodo: Filter object that contains a factory
        public async Task<IEnumerable<IStreamChannel>> QueryChannelsAsync(IDictionary<string, object> filters)
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

            var result = new List<IStreamChannel>();
            foreach (var channelDto in channelsResponseDto.Channels)
            {
                result.Add(_cache.TryCreateOrUpdate(channelDto));
            }

            return result;
        }

        public async Task<IEnumerable<IStreamUser>> QueryUsersAsync(IDictionary<string, object> filters)
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
                return Enumerable.Empty<IStreamUser>();
            }

            var result = new List<IStreamUser>();
            foreach (var userDto in response.Users)
            {
                result.Add(_cache.TryCreateOrUpdate(userDto));
            }

            return result;
        }

        public async Task<IEnumerable<IStreamUser>> UpsertUsers(IEnumerable<StreamUserUpsertRequest> userRequests)
        {
            StreamAsserts.AssertNotNullOrEmpty(userRequests, nameof(userRequests));

            //StreamTodo: items could be null
            var requestDtos = userRequests.Select(_ => _.TrySaveToDto()).ToDictionary(_ => _.Id, _ => _);

            var response = await LowLevelClient.InternalUserApi.UpsertManyUsersAsync(new UpdateUsersRequestInternalDTO
            {
                Users = requestDtos
            });

            var result = new List<IStreamUser>();
            foreach (var userDto in response.Users.Values)
            {
                result.Add(_cache.TryCreateOrUpdate(userDto));
            }

            return result;
        }

        public async Task MuteMultipleChannelsAsync(IEnumerable<IStreamChannel> channels, int? milliseconds = default)
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

            UpdateLocalUser(response.OwnUser);
        }

        public async Task UnmuteMultipleChannelsAsync(IEnumerable<IStreamChannel> channels)
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

        public async Task<StreamDeleteChannelsResponse> DeleteMultipleChannelsAsync(
            IEnumerable<IStreamChannel> channels,
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
        
        public async Task MuteMultipleUsersAsync(IEnumerable<IStreamUser> users, int? timeoutMinutes = default)
        {
            StreamAsserts.AssertNotNullOrEmpty(users, nameof(users));

            var responseDto = await LowLevelClient.InternalModerationApi.MuteUserAsync(new MuteUserRequestInternalDTO
            {
                TargetIds = users.Select(_ => _.Id).ToList(),
                Timeout = timeoutMinutes
            });

            UpdateLocalUser(responseDto.OwnUser);
        }

        private Task<IEnumerable<IStreamUser>> QueryBannedUsersAsync()
        {
            //StreamTodo: IMPLEMENT, should we allow for query
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            
            //StreamTodo: disconnect current user

            TryCancelWaitingForUserConnection();

            if (LowLevelClient != null)
            {
                UnsubscribeFrom(LowLevelClient);
                LowLevelClient.Dispose();
            }

            _isDisposed = true;
            Disposed?.Invoke();
        }

        void IStreamChatClientEventsListener.Destroy()
        {
            //StreamTodo: we should probably check: if waiting for connection -> cancel, if connected -> disconnect, etc
            DisconnectUserAsync().ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    _logs.Exception(t.Exception);
                    return;
                }

                Dispose();
            });
        }

        void IStreamChatClientEventsListener.Update() => LowLevelClient.Update(_timeService.DeltaTime);

        internal StreamChatLowLevelClient LowLevelClient { get; }

        internal void UpdateLocalUser(OwnUserInternalDTO ownUserInternalDto)
        {
            _localUserData = _cache.TryCreateOrUpdate(ownUserInternalDto);

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

        private TaskCompletionSource<IStreamLocalUserData> _connectUserTaskSource;
        private CancellationToken _connectUserCancellationToken;
        private CancellationTokenSource _connectUserCancellationTokenSource;
        private bool _isDisposed;
        
        /// <summary>
        /// Use the <see cref="CreateDefaultClient"/> to create the client instance
        /// </summary>
        private StreamChatClient(IWebsocketClient websocketClient,
            IHttpClient httpClient, ISerializer serializer, ITimeService timeService, ILogs logs,
            IStreamClientConfig config)
        {
            _timeService = timeService ?? throw new ArgumentNullException(nameof(timeService));
            _logs = logs ?? throw new ArgumentNullException(nameof(logs));

            LowLevelClient = new StreamChatLowLevelClient(authCredentials: default, websocketClient, httpClient, serializer,
                _timeService, logs, config);

            _cache = new Cache(this, _logs);

            SubscribeTo(LowLevelClient);
        }

        private void InternalDeleteChannel(StreamChannel channel)
        {
            //StreamTodo: mark StreamChannel object as deleted + probably silent clear all internal data?
            _cache.Channels.Remove(channel);
            ChannelDeleted?.Invoke(channel.Cid, channel.Id, channel.Type);
        }

        private void TryCancelWaitingForUserConnection()
        {
            var isConnectTaskRunning = _connectUserTaskSource?.Task != null && !_connectUserTaskSource.Task.IsCompleted;
            var isCancellationRequested = _connectUserCancellationTokenSource.IsCancellationRequested;

            if (isConnectTaskRunning && !isCancellationRequested)
            {
#if STREAM_DEBUG_ENABLED
                _logs.Info($"Try Cancel {_connectUserTaskSource}");
#endif
                _connectUserTaskSource.TrySetCanceled();
            }
        }

        #region Events

        private void OnConnected(EventHealthCheckInternalDTO dto)
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
                        $"{nameof(OnConnected)} expected {nameof(_connectUserTaskSource)} not null");
                }
                else
                {
                    _connectUserTaskSource.SetResult(LocalUserData);
                }
            }
        }

        private void OnDisconnected() => Disconnected?.Invoke();

        private void OnConnectionStateChanged(ConnectionState previous, ConnectionState current)
            => ConnectionStateChanged?.Invoke(previous, current);

        private void OnMessageDeleted(EventMessageDeletedInternalDTO eventMessageDeleted)
        {
            if (_cache.Channels.TryGet(eventMessageDeleted.Cid, out var streamChannel))
            {
                streamChannel.HandleMessageDeletedEvent(eventMessageDeleted);
            }
        }

        private void OnMessageUpdated(EventMessageUpdatedInternalDTO eventMessageUpdated)
        {
            if (_cache.Channels.TryGet(eventMessageUpdated.Cid, out var streamChannel))
            {
                streamChannel.HandleMessageUpdatedEvent(eventMessageUpdated);
            }
        }

        private void OnMessageReceived(EventMessageNewInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.HandleMessageNewEvent(eventDto);
            }
        }

        private void OnChannelTruncated(EventChannelTruncatedInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.HandleChannelTruncatedEvent(eventDto);
            }
        }

        private void OnChannelDeletedNotification(EventNotificationChannelDeletedInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                InternalDeleteChannel(streamChannel);
            }
        }

        private void OnChannelVisible(EventChannelVisibleInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.Hidden = false;
            }
        }

        private void OnChannelHidden(EventChannelHiddenInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.Hidden = true;
            }
        }

        private void OnChannelDeleted(EventChannelDeletedInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                InternalDeleteChannel(streamChannel);
            }
        }

        private void OnChannelUpdated(EventChannelUpdatedInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.HandleChannelUpdatedEvent(eventDto);
            }
        }

        private void OnChannelTruncatedNotification(
            EventNotificationChannelTruncatedInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.HandleChannelTruncatedEvent(eventDto);
            }
        }

        private void OnChannelMutesUpdatedNotification(EventNotificationChannelMutesUpdatedInternalDTO eventDto)
        {
            UpdateLocalUser(eventDto.Me);
        }

        private void OnMessageReceivedNotification(EventNotificationMessageNewInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.InternalHandleMessageNewNotification(eventDto);
            }
        }

        private void OnMutesUpdatedNotification(EventNotificationMutesUpdatedInternalDTO eventDto)
        {
            UpdateLocalUser(eventDto.Me);
        }

        private void OnMemberAdded(EventMemberAddedInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                var member = _cache.TryCreateOrUpdate(eventDto.Member);
                StreamAsserts.AssertNotNull(member, nameof(member));
                streamChannel.InternalAddMember(member);
            }
        }

        private void OnMemberUpdated(EventMemberUpdatedInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                var member = _cache.TryCreateOrUpdate(eventDto.Member);
                StreamAsserts.AssertNotNull(member, nameof(member));
                streamChannel.InternalUpdateMember(member);
            }
        }

        private void OnMemberRemoved(EventMemberRemovedInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                var member = _cache.TryCreateOrUpdate(eventDto.Member);
                StreamAsserts.AssertNotNull(member, nameof(member));
                streamChannel.InternalRemoveMember(member);
            }
        }

        private void OnMessageRead(EventMessageReadInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.InternalHandleMessageReadEvent(eventDto);
            }
        }
        
        private void OnMarkReadNotification(EventNotificationMarkReadInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.InternalHandleMessageReadNotification(eventDto);
            }

            _localUserData.InternalHandleMarkReadNotification(eventDto);
        }

        private void OnAddedToChannelNotification(EventNotificationAddedToChannelInternalDTO obj)
        {
            //StreamTodo: IMPLEMENT
        }

        private void OnRemovedFromChannelNotification(
            EventNotificationRemovedFromChannelInternalDTO obj)
        {
//StreamTodo: IMPLEMENT
        }

        private void OnInvitedNotification(EventNotificationInvitedInternalDTO obj)
        {
//StreamTodo: IMPLEMENT
        }

        private void OnInviteAcceptedNotification(EventNotificationInviteAcceptedInternalDTO obj)
        {
//StreamTodo: IMPLEMENT
        }

        private void OnInviteRejectedNotification(EventNotificationInviteRejectedInternalDTO obj)
        {
//StreamTodo: IMPLEMENT
        }

        private void OnReactionReceived(EventReactionNewInternalDTO eventDto)
        {
            if (!_cache.Channels.TryGet(eventDto.Cid, out var channel))
            {
                return;
            }

            if (_cache.Messages.TryGet(eventDto.Message.Id, out var message))
            {
                var reaction = new StreamReaction().TryLoadFromDto(eventDto.Reaction, _cache);
                message.HandleReactionNewEvent(eventDto, channel, reaction);
            }
        }

        private void OnReactionUpdated(EventReactionUpdatedInternalDTO eventDto)
        {
            if (!_cache.Channels.TryGet(eventDto.Cid, out var channel))
            {
                return;
            }

            if (_cache.Messages.TryGet(eventDto.Message.Id, out var message))
            {
                var reaction = new StreamReaction().TryLoadFromDto(eventDto.Reaction, _cache);
                message.HandleReactionUpdatedEvent(eventDto, channel, reaction);
            }
        }

        private void OnReactionDeleted(EventReactionDeletedInternalDTO eventDto)
        {
            if (!_cache.Channels.TryGet(eventDto.Cid, out var channel))
            {
                return;
            }

            if (_cache.Messages.TryGet(eventDto.Message.Id, out var message))
            {
                var reaction = new StreamReaction().TryLoadFromDto(eventDto.Reaction, _cache);
                message.HandleReactionDeletedEvent(eventDto, channel, reaction);
            }
        }

        private void OnUserWatchingStop(EventUserWatchingStopInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.InternalHandleUserWatchingStop(eventDto);
            }
        }

        private void OnUserWatchingStart(EventUserWatchingStartInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.InternalHandleUserWatchingStartEvent(eventDto);
            }
        }

        private void OnLowLevelClientUserUnbanned(EventUserUnbannedInternalDTO obj)
        {
            //StreamTodo: IMPLEMENT
        }

        private void OnLowLevelClientUserBanned(EventUserBannedInternalDTO obj)
        {
            //StreamTodo: IMPLEMENT
        }

        private void OnLowLevelClientUserDeleted(EventUserDeletedInternalDTO obj)
        {
            //StreamTodo: IMPLEMENT
        }

        private void OnLowLevelUserUpdated(EventUserUpdatedInternalDTO eventDto)
        {
            if (_cache.Users.TryGet(eventDto.User.Id, out var streamUser))
            {
                _cache.TryCreateOrUpdate(eventDto.User);
            }
        }

        private void OnUserPresenceChanged(EventUserPresenceChangedInternalDTO eventDto)
        {
            if (_cache.Users.TryGet(eventDto.User.Id, out var streamUser))
            {
                streamUser.InternalHandlePresenceChanged(eventDto);
            }
        }

        private void OnTypingStopped(EventTypingStopInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.InternalHandleTypingStopped(eventDto);
            }
        }

        private void OnTypingStarted(EventTypingStartInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.InternalHandleTypingStarted(eventDto);
            }
        }

        private void SubscribeTo(StreamChatLowLevelClient lowLevelClient)
        {
            lowLevelClient.InternalConnected += OnConnected;
            lowLevelClient.Disconnected += OnDisconnected;
            lowLevelClient.ConnectionStateChanged += OnConnectionStateChanged;

            lowLevelClient.InternalMessageReceived += OnMessageReceived;
            lowLevelClient.InternalMessageUpdated += OnMessageUpdated;
            lowLevelClient.InternalMessageDeleted += OnMessageDeleted;
            lowLevelClient.InternalMessageRead += OnMessageRead;

            lowLevelClient.InternalChannelUpdated += OnChannelUpdated;
            lowLevelClient.InternalChannelDeleted += OnChannelDeleted;
            lowLevelClient.InternalChannelTruncated += OnChannelTruncated;
            lowLevelClient.InternalChannelVisible += OnChannelVisible;
            lowLevelClient.InternalChannelHidden += OnChannelHidden;

            lowLevelClient.InternalMemberAdded += OnMemberAdded;
            lowLevelClient.InternalMemberRemoved += OnMemberRemoved;
            lowLevelClient.InternalMemberUpdated += OnMemberUpdated;

            lowLevelClient.InternalUserPresenceChanged += OnUserPresenceChanged;
            lowLevelClient.InternalUserUpdated += OnLowLevelUserUpdated;
            lowLevelClient.InternalUserDeleted += OnLowLevelClientUserDeleted;
            lowLevelClient.InternalUserBanned += OnLowLevelClientUserBanned;
            lowLevelClient.InternalUserUnbanned += OnLowLevelClientUserUnbanned;

            lowLevelClient.InternalUserWatchingStart += OnUserWatchingStart;
            lowLevelClient.InternalUserWatchingStop += OnUserWatchingStop;

            lowLevelClient.InternalReactionReceived += OnReactionReceived;
            lowLevelClient.InternalReactionUpdated += OnReactionUpdated;
            lowLevelClient.InternalReactionDeleted += OnReactionDeleted;

            lowLevelClient.InternalTypingStarted += OnTypingStarted;
            lowLevelClient.InternalTypingStopped += OnTypingStopped;

            lowLevelClient.InternalNotificationChannelMutesUpdated += OnChannelMutesUpdatedNotification;

            lowLevelClient.InternalNotificationMutesUpdated += OnMutesUpdatedNotification;
            lowLevelClient.InternalNotificationMessageReceived += OnMessageReceivedNotification;
            lowLevelClient.InternalNotificationMarkRead += OnMarkReadNotification;

            lowLevelClient.InternalNotificationChannelDeleted += OnChannelDeletedNotification;
            lowLevelClient.InternalNotificationChannelTruncated += OnChannelTruncatedNotification;

            lowLevelClient.InternalNotificationAddedToChannel += OnAddedToChannelNotification;
            lowLevelClient.InternalNotificationRemovedFromChannel += OnRemovedFromChannelNotification;

            lowLevelClient.InternalNotificationInvited += OnInvitedNotification;
            lowLevelClient.InternalNotificationInviteAccepted += OnInviteAcceptedNotification;
            lowLevelClient.InternalNotificationInviteRejected += OnInviteRejectedNotification;
        }

        private void UnsubscribeFrom(StreamChatLowLevelClient lowLevelClient)
        {
            lowLevelClient.InternalConnected -= OnConnected;
            lowLevelClient.Disconnected -= OnDisconnected;
            lowLevelClient.ConnectionStateChanged -= OnConnectionStateChanged;

            lowLevelClient.InternalMessageReceived -= OnMessageReceived;
            lowLevelClient.InternalMessageUpdated -= OnMessageUpdated;
            lowLevelClient.InternalMessageDeleted -= OnMessageDeleted;
            lowLevelClient.InternalMessageRead -= OnMessageRead;

            lowLevelClient.InternalChannelUpdated -= OnChannelUpdated;
            lowLevelClient.InternalChannelDeleted -= OnChannelDeleted;
            lowLevelClient.InternalChannelTruncated -= OnChannelTruncated;
            lowLevelClient.InternalChannelVisible -= OnChannelVisible;
            lowLevelClient.InternalChannelHidden -= OnChannelHidden;

            lowLevelClient.InternalMemberAdded -= OnMemberAdded;
            lowLevelClient.InternalMemberRemoved -= OnMemberRemoved;
            lowLevelClient.InternalMemberUpdated -= OnMemberUpdated;

            lowLevelClient.InternalUserPresenceChanged -= OnUserPresenceChanged;
            lowLevelClient.InternalUserUpdated -= OnLowLevelUserUpdated;
            lowLevelClient.InternalUserDeleted -= OnLowLevelClientUserDeleted;
            lowLevelClient.InternalUserBanned -= OnLowLevelClientUserBanned;
            lowLevelClient.InternalUserUnbanned -= OnLowLevelClientUserUnbanned;

            lowLevelClient.InternalUserWatchingStart -= OnUserWatchingStart;
            lowLevelClient.InternalUserWatchingStop -= OnUserWatchingStop;

            lowLevelClient.InternalReactionReceived -= OnReactionReceived;
            lowLevelClient.InternalReactionUpdated -= OnReactionUpdated;
            lowLevelClient.InternalReactionDeleted -= OnReactionDeleted;

            lowLevelClient.InternalTypingStarted -= OnTypingStarted;
            lowLevelClient.InternalTypingStopped -= OnTypingStopped;

            lowLevelClient.InternalNotificationChannelMutesUpdated -= OnChannelMutesUpdatedNotification;

            lowLevelClient.InternalNotificationMutesUpdated -= OnMutesUpdatedNotification;
            lowLevelClient.InternalNotificationMessageReceived -= OnMessageReceivedNotification;
            lowLevelClient.InternalNotificationMarkRead -= OnMarkReadNotification;

            lowLevelClient.InternalNotificationChannelDeleted -= OnChannelDeletedNotification;
            lowLevelClient.InternalNotificationChannelTruncated -= OnChannelTruncatedNotification;

            lowLevelClient.InternalNotificationAddedToChannel -= OnAddedToChannelNotification;
            lowLevelClient.InternalNotificationRemovedFromChannel -= OnRemovedFromChannelNotification;

            lowLevelClient.InternalNotificationInvited -= OnInvitedNotification;
            lowLevelClient.InternalNotificationInviteAccepted -= OnInviteAcceptedNotification;
            lowLevelClient.InternalNotificationInviteRejected -= OnInviteRejectedNotification;
        }

        #endregion
    }
}