using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StreamChat.Core.Configs;
using StreamChat.Core.Exceptions;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.LowLevelClient;
using StreamChat.Core.State;
using StreamChat.Core.State.Caches;
using StreamChat.Core.Models;
using StreamChat.Core.QueryBuilders.Filters;
using StreamChat.Core.QueryBuilders.Sort;
using StreamChat.Core.Requests;
using StreamChat.Core.Responses;
using StreamChat.Core.StatefulModels;
using StreamChat.Libs;
using StreamChat.Libs.AppInfo;
using StreamChat.Libs.Auth;
using StreamChat.Libs.ChatInstanceRunner;
using StreamChat.Libs.Http;
using StreamChat.Libs.Logs;
using StreamChat.Libs.NetworkMonitors;
using StreamChat.Libs.Serialization;
using StreamChat.Libs.Time;
using StreamChat.Libs.Websockets;
using StreamChat.Libs.Utils;
#if STREAM_TESTS_ENABLED
using System.Text;
#endif

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

    //StreamTodo: Handle restoring state after lost connection

    public delegate void ChannelInviteHandler(IStreamChannel channel, IStreamUser invitee);

    /// <summary>
    /// Member added to the channel handler
    /// </summary>
    public delegate void ChannelMemberAddedHandler(IStreamChannel channel, IStreamChannelMember member);

    /// <summary>
    /// Member removed from the channel handler
    /// </summary>
    public delegate void ChannelMemberRemovedHandler(IStreamChannel channel, IStreamChannelMember member);

    /// <inheritdoc cref="IStreamChatClient"/>
    public sealed class StreamChatClient : IStreamChatClient
    {
        public event ConnectionMadeHandler Connected;

        public event Action Disconnected;

        public event Action Disposed;

        public event ConnectionChangeHandler ConnectionStateChanged;

        public event ChannelDeleteHandler ChannelDeleted;

        public event ChannelInviteHandler ChannelInviteReceived;
        public event ChannelInviteHandler ChannelInviteAccepted;
        public event ChannelInviteHandler ChannelInviteRejected;

        public event ChannelMemberAddedHandler AddedToChannelAsMember;
        public event ChannelMemberRemovedHandler RemovedFromChannelAsMember;

        public event StreamThreadChangeHandler ThreadTracked;
        public event StreamThreadChangeHandler ThreadUntracked;

        public const int QueryUsersLimitMaxValue = 30;
        public const int QueryUsersOffsetMaxValue = 1000;

        public ConnectionState ConnectionState => InternalLowLevelClient.ConnectionState;

        public bool IsConnected => InternalLowLevelClient.ConnectionState == ConnectionState.Connected;
        public bool IsConnecting => InternalLowLevelClient.ConnectionState == ConnectionState.Connecting;

        public IStreamLocalUserData LocalUserData => _localUserData;

        private StreamLocalUserData _localUserData;

        public IReadOnlyList<IStreamChannel> WatchedChannels => _watchedChannels;

        public double? NextReconnectTime => InternalLowLevelClient.NextReconnectTime;

        public IStreamChatLowLevelClient LowLevelClient => InternalLowLevelClient;

        public IStreamPollsApi Polls => _pollsApi;

        /// <inheritdoc cref="StreamChatLowLevelClient.SDKVersion"/>
        public static Version SDKVersion => StreamChatLowLevelClient.SDKVersion;

        /// <summary>
        /// Recommended method to create an instance of <see cref="IStreamChatClient"/>
        /// If you wish to create an instance with non default dependencies you can use the <see cref="CreateClientWithCustomDependencies"/>
        /// </summary>
        /// <param name="config">[Optional] configuration</param>
        public static IStreamChatClient CreateDefaultClient(IStreamClientConfig config = default)
        {
            if (config == null)
            {
                config = StreamClientConfig.Default;
            }

            var logs = StreamDependenciesFactory.CreateLogger(config.LogLevel.ToLogLevel());
            var websocketClient
                = StreamDependenciesFactory.CreateWebsocketClient(logs, config.LogLevel.IsDebugEnabled());
            var httpClient = StreamDependenciesFactory.CreateHttpClient();
            var serializer = StreamDependenciesFactory.CreateSerializer();
            var timeService = StreamDependenciesFactory.CreateTimeService();
            var applicationInfo = StreamDependenciesFactory.CreateApplicationInfo();
            var gameObjectRunner = StreamDependenciesFactory.CreateChatClientRunner();
            var networkMonitor = StreamDependenciesFactory.CreateNetworkMonitor();

            var client = new StreamChatClient(websocketClient, httpClient, serializer, timeService, networkMonitor,
                applicationInfo, logs, config);

            gameObjectRunner?.RunChatInstance(client);
            return client;
        }

        /// <summary>
        /// Create instance of <see cref="ITokenProvider"/>
        /// </summary>
        /// <param name="urlFactory">Delegate that will return a valid url that return JWT auth token for a given user ID</param>
        /// <example>
        /// <code>
        /// StreamChatClient.CreateDefaultTokenProvider(userId => new Uri($"https:your-awesome-page.com/get_token?userId={userId}"));
        /// </code>
        /// </example>
        public static ITokenProvider CreateDefaultTokenProvider(TokenProvider.TokenUriHandler urlFactory)
            => StreamDependenciesFactory.CreateTokenProvider(urlFactory);

        /// <summary>
        /// Create a new instance of <see cref="IStreamChatLowLevelClient"/> with custom provided dependencies.
        /// If you want to create a default new instance then just use the <see cref="CreateDefaultClient"/>.
        /// Important! Custom created client require calling the <see cref="Update"/> and <see cref="Destroy"/> methods.
        /// </summary>
        public static IStreamChatClient CreateClientWithCustomDependencies(IWebsocketClient websocketClient,
            IHttpClient httpClient, ISerializer serializer, ITimeService timeService, INetworkMonitor networkMonitor,
            IApplicationInfo applicationInfo, ILogs logs, IStreamClientConfig config)
            => new StreamChatClient(websocketClient, httpClient, serializer, timeService, networkMonitor,
                applicationInfo, logs, config);

        /// <inheritdoc cref="StreamChatLowLevelClient.CreateDeveloperAuthToken"/>
        public static string CreateDeveloperAuthToken(string userId)
            => StreamChatLowLevelClient.CreateDeveloperAuthToken(userId);

        /// <inheritdoc cref="StreamChatLowLevelClient.SanitizeUserId"/>
        public static string SanitizeUserId(string userId) => StreamChatLowLevelClient.SanitizeUserId(userId);

        public void SetAuthorizationCredentials(AuthCredentials authCredentials)
            => InternalLowLevelClient.SeAuthorizationCredentials(authCredentials);

        public Task<IStreamLocalUserData> ConnectUserAsync(AuthCredentials userAuthCredentials,
            CancellationToken cancellationToken = default)
        {
            InternalLowLevelClient.ConnectUser(userAuthCredentials);

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

        public Task<IStreamLocalUserData> ConnectUserAsync(string apiKey, string userId, string userAuthToken,
            CancellationToken cancellationToken = default)
        {
            StreamAsserts.AssertNotNullOrEmpty(apiKey, nameof(apiKey));
            StreamAsserts.AssertNotNullOrEmpty(userId, nameof(userId));
            StreamAsserts.AssertNotNullOrEmpty(userAuthToken, nameof(userAuthToken));

            return ConnectUserAsync(new AuthCredentials(apiKey, userId, userAuthToken), cancellationToken);
        }

        public async Task<IStreamLocalUserData> ConnectUserAsync(string apiKey, string userId,
            ITokenProvider tokenProvider,
            CancellationToken cancellationToken = default)
        {
            StreamAsserts.AssertNotNullOrEmpty(apiKey, nameof(apiKey));
            StreamAsserts.AssertNotNullOrEmpty(userId, nameof(userId));
            StreamAsserts.AssertNotNull(tokenProvider, nameof(tokenProvider));

            var ownUserDto
                = await InternalLowLevelClient.ConnectUserAsync(apiKey, userId, tokenProvider, cancellationToken);
            return UpdateLocalUser(ownUserDto);
        }

        //StreamTodo: test scenario: ConnectUserAsync and immediately call DisconnectUserAsync
        //StreamTodo: this should cancel token that would be globally passed to all async tasks so the moment we disconnect all async tasks are cancelled
        public Task DisconnectUserAsync()
        {
            TryCancelWaitingForUserConnection();
            return InternalLowLevelClient.DisconnectAsync(permanent: true);
        }

        public async Task<StreamCurrentUnreadCounts> GetLatestUnreadCountsAsync()
        {
            var dto = await InternalLowLevelClient.InternalChannelApi.GetUnreadCountsAsync();
            var response = dto.ToDomain<WrappedUnreadCountsResponseInternalDTO, StreamCurrentUnreadCounts>();

            ((IUpdateableFrom2<WrappedUnreadCountsResponseInternalDTO, StreamLocalUserData>)_localUserData).TryUpdateFromDto(dto, _cache);

            return response;
        }

        public bool IsLocalUser(IStreamUser user) => LocalUserData.User == user;

        public Task<IStreamChannel> GetOrCreateChannelWithIdAsync(ChannelType channelType, string channelId,
            string name = null, IDictionary<string, object> optionalCustomData = null)
            => InternalGetOrCreateChannelWithIdAsync(channelType, channelId, name, presence: true, state: true,
                watch: true, optionalCustomData);

        public async Task<IStreamChannel> GetOrCreateChannelWithMembersAsync(ChannelType channelType,
            IEnumerable<IStreamUser> members, IDictionary<string, object> optionalCustomData = null)
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
                requestBodyDto.Data.AdditionalProperties = optionalCustomData?.ToDictionary(x => x.Key, x => x.Value);
            }

            var channelResponseDto =
                await InternalLowLevelClient.InternalChannelApi.GetOrCreateChannelAsync(channelType, requestBodyDto);
            var channel = _cache.TryCreateOrUpdate(channelResponseDto);
            MarkChannelWatched(channel);
            return channel;
        }

        public async Task<IEnumerable<IStreamChannel>> QueryChannelsAsync(IEnumerable<IFieldFilterRule> filters = null,
            ChannelSortObject sort = null, int limit = 30, int offset = 0)
        {
            StreamAsserts.AssertWithinRange(limit, 0, 30, nameof(limit));
            StreamAsserts.AssertGreaterThanOrEqualZero(offset, nameof(offset));

            //StreamTodo: Perhaps MessageLimit and MemberLimit should be configurable
            var requestBodyDto = new QueryChannelsRequestInternalDTO
            {
                FilterConditions = filters?.Select(_ => _.GenerateFilterEntry()).ToDictionary(x => x.Key, x => x.Value),
                Limit = limit,
                MemberLimit = null,
                MessageLimit = null,
                Offset = offset,
                Presence = true,

                /*
                 * StreamTodo: Allowing to sort query can potentially lead to mixed sorting in WatchedChannels
                 * But there seems no other choice because its too limiting to force only a global sorting for channels
                 * e.g. user may want to show channels in multiple ways with different sorting which would not work with global only sorting
                 */
                Sort = sort?.ToSortParamRequestList(),
                State = true,
                Watch = true,
            };

            var channelsResponseDto
                = await InternalLowLevelClient.InternalChannelApi.QueryChannelsAsync(requestBodyDto);
            if (channelsResponseDto.Channels == null || channelsResponseDto.Channels.Count == 0)
            {
                return Enumerable.Empty<StreamChannel>();
            }

            var result = new List<IStreamChannel>();
            foreach (var channelDto in channelsResponseDto.Channels)
            {
                var channel = _cache.TryCreateOrUpdate(channelDto);
                MarkChannelWatched(channel);
                result.Add(channel);
            }

            return result;
        }

        [Obsolete("This method will be removed in the future. Please use the other overload method that uses " +
                  nameof(IFieldFilterRule) + " type filters")]
        public async Task<IEnumerable<IStreamChannel>> QueryChannelsAsync(IDictionary<string, object> filters,
            ChannelSortObject sort = null, int limit = 30, int offset = 0)
        {
            StreamAsserts.AssertWithinRange(limit, 0, 30, nameof(limit));
            StreamAsserts.AssertGreaterThanOrEqualZero(offset, nameof(offset));

            //StreamTodo: Perhaps MessageLimit and MemberLimit should be configurable
            var requestBodyDto = new QueryChannelsRequestInternalDTO
            {
                FilterConditions = filters?.ToDictionary(x => x.Key, x => x.Value),
                Limit = limit,
                MemberLimit = null,
                MessageLimit = null,
                Offset = offset,
                Presence = true,

                /*
                 * StreamTodo: Allowing to sort query can potentially lead to mixed sorting in WatchedChannels
                 * But there seems no other choice because its too limiting to force only a global sorting for channels
                 * e.g. user may want to show channels in multiple ways with different sorting which would not work with global only sorting
                 */
                Sort = sort?.ToSortParamRequestList(),
                State = true,
                Watch = true,
            };

            var channelsResponseDto
                = await InternalLowLevelClient.InternalChannelApi.QueryChannelsAsync(requestBodyDto);
            if (channelsResponseDto.Channels == null || channelsResponseDto.Channels.Count == 0)
            {
                return Enumerable.Empty<StreamChannel>();
            }

            var result = new List<IStreamChannel>();
            foreach (var channelDto in channelsResponseDto.Channels)
            {
                var channel = _cache.TryCreateOrUpdate(channelDto);
                MarkChannelWatched(channel);
                result.Add(channel);
            }

            return result;
        }

        [Obsolete("This method will be removed in the future. Please use the other overload method that uses " +
                  nameof(IFieldFilterRule) + " type filters")]
        public async Task<IEnumerable<IStreamUser>> QueryUsersAsync(IDictionary<string, object> filters = null)
        {
            //StreamTodo: Missing filter, and stuff like IdGte etc
            var requestBodyDto = new QueryUsersRequestInternalDTO
            {
                FilterConditions = filters?.ToDictionary(x => x.Key, x => x.Value) ?? new Dictionary<string, object>(),
                IdGt = null,
                IdGte = null,
                IdLt = null,
                IdLte = null,
                Limit = null,
                Offset = null,
                Presence = true, //StreamTodo: research whether user should be allowed to control this
                Sort = null,
            };

            var response = await InternalLowLevelClient.InternalUserApi.QueryUsersAsync(requestBodyDto);
            if (response == null || response.Users == null || response.Users.Count == 0)
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

        public async Task<IEnumerable<IStreamUser>> QueryUsersAsync(IEnumerable<IFieldFilterRule> filters = null,
            UsersSortObject sort = null, int offset = 0, int limit = 30)
        {
            StreamAsserts.AssertWithinRange(limit, 0, QueryUsersLimitMaxValue, nameof(limit));
            StreamAsserts.AssertWithinRange(offset, 0, QueryUsersOffsetMaxValue, nameof(offset));

            //StreamTodo: Missing IdGt, IdLt, etc. We could wrap all pagination parameters in a single struct
            var requestBodyDto = new QueryUsersRequestInternalDTO
            {
                FilterConditions
                    = filters?.Select(f => f.GenerateFilterEntry()).ToDictionary(x => x.Key, x => x.Value) ??
                      new Dictionary<string, object>(),
                IdGt = null,
                IdGte = null,
                IdLt = null,
                IdLte = null,
                Limit = limit,
                Offset = offset,
                Presence = true, //StreamTodo: research whether user should be allowed to control this
                Sort = sort?.ToSortParamInternalDTOs(),
            };

            var response = await InternalLowLevelClient.InternalUserApi.QueryUsersAsync(requestBodyDto);
            if (response == null || response.Users == null || response.Users.Count == 0)
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

        //StreamTodo: write tests
        public async Task<IEnumerable<StreamUserBanInfo>> QueryBannedUsersAsync(
            StreamQueryBannedUsersRequest streamQueryBannedUsersRequest)
        {
            StreamAsserts.AssertNotNull(streamQueryBannedUsersRequest, nameof(streamQueryBannedUsersRequest));

            var response =
                await InternalLowLevelClient.InternalModerationApi.QueryBannedUsersAsync(streamQueryBannedUsersRequest
                    .TrySaveToDto());
            if (response.Bans == null || response.Bans.Count == 0)
            {
                return Enumerable.Empty<StreamUserBanInfo>();
            }

            var result = new List<StreamUserBanInfo>();
            foreach (var userDto in response.Bans)
            {
                var banInfo = new StreamUserBanInfo().LoadFromDto(userDto, _cache);
                result.Add(banInfo);
            }

            return result;
        }

        public async Task<IStreamThread> GetThreadAsync(string parentMessageId,
            int? replyLimit = null,
            int? participantLimit = null,
            int? memberLimit = null,
            bool watch = true)
        {
            StreamAsserts.AssertNotNullOrEmpty(parentMessageId, nameof(parentMessageId));

            var response = await InternalLowLevelClient.InternalThreadsApi.GetThreadAsync(parentMessageId,
                replyLimit: replyLimit,
                participantLimit: participantLimit,
                memberLimit: memberLimit,
                watch: watch);

            var thread = _cache.TryCreateOrUpdate(response.Thread);

            // The /threads response always embeds the parent channel; only flip IsWatched
            // when the caller actually requested watch (preserve any prior IsWatched=true).
            if (watch)
            {
                MarkChannelWatched(thread?.Channel as StreamChannel);
            }

            return thread;
        }

        public async Task<StreamQueryThreadsResponse> QueryThreadsAsync(StreamQueryThreadsRequest request)
        {
            StreamAsserts.AssertNotNull(request, nameof(request));

            var requestDto = request.TrySaveToDto();
            var response = await InternalLowLevelClient.InternalThreadsApi.QueryThreadsAsync(requestDto);

            var threads = new List<IStreamThread>();
            if (response.Threads != null)
            {
                foreach (var threadDto in response.Threads)
                {
                    var thread = _cache.TryCreateOrUpdate(threadDto);
                    if (thread != null)
                    {
                        // Same as GetThreadAsync: only mark watched when Watch=true was requested.
                        if (request.Watch)
                        {
                            MarkChannelWatched(thread.Channel as StreamChannel);
                        }
                        threads.Add(thread);
                    }
                }
            }

            return new StreamQueryThreadsResponse
            {
                Threads = threads,
                Next = response.Next,
                Prev = response.Prev,
            };
        }

        public async Task<StreamSearchMessagesResponse> SearchMessagesAsync(
            StreamSearchMessagesRequest request,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ValidateSearchMessagesRequest(request);

            cancellationToken.ThrowIfCancellationRequested();

            var requestDto = request.TrySaveToDto();
            var responseDto =
                await InternalLowLevelClient.InternalMessageApi.SearchMessagesAsync(requestDto);

            cancellationToken.ThrowIfCancellationRequested();

            var results = new List<StreamSearchMessageResult>();
            var distinctChannels = new Dictionary<string, IStreamChannel>();

            if (responseDto?.Results != null)
            {
                foreach (var resultDto in responseDto.Results)
                {
                    var searchMsgDto = resultDto?.Message;
                    if (searchMsgDto == null)
                    {
                        continue;
                    }

                    IStreamChannel channel = null;
                    if (searchMsgDto.Channel != null)
                    {
                        // Cache for identity reuse only - /search does NOT start a server-side
                        // watch. Newly-cached channels stay IsWatched=false; already-watched
                        // ones keep their flag. WatchResultChannels=true upgrades below.
                        channel = _cache.TryCreateOrUpdate(searchMsgDto.Channel);
                        if (channel != null && !distinctChannels.ContainsKey(channel.Cid))
                        {
                            distinctChannels.Add(channel.Cid, channel);
                        }
                    }

                    var messageDto = ProjectSearchResultToMessageDto(searchMsgDto);
                    var message = _cache.TryCreateOrUpdate(messageDto);

                    results.Add(new StreamSearchMessageResult
                    {
                        Message = message,
                        Channel = channel,
                    });
                }
            }

            if (request.WatchResultChannels && distinctChannels.Count > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Watch all distinct result channels in parallel. Default-true means this fires
                // on every search; serial would multiply latency by the number of distinct
                // channels in the result set (typically 1-10 for a 30-result page).
                // Skip channels that are already watched - WatchAsync is idempotent but the
                // extra round-trip is wasteful when there's nothing to upgrade.
                var watchTasks = new List<Task>(distinctChannels.Count);
                foreach (var channel in distinctChannels.Values)
                {
                    if (channel.IsWatched)
                    {
                        continue;
                    }

                    watchTasks.Add(InternalGetOrCreateChannelWithIdAsync(channel.Type, channel.Id));
                }

                if (watchTasks.Count > 0)
                {
                    await Task.WhenAll(watchTasks);
                }

                cancellationToken.ThrowIfCancellationRequested();
            }

            return new StreamSearchMessagesResponse
            {
                Results = results,
                Next = responseDto?.Next,
                Previous = responseDto?.Previous,
                Duration = responseDto?.Duration,
                ResultsWarning = BuildSearchWarning(responseDto?.ResultsWarning),
            };
        }

        private static void ValidateSearchMessagesRequest(StreamSearchMessagesRequest request)
        {
            StreamAsserts.AssertNotNull(request, nameof(request));

            var hasChannelFilter = request.ChannelFilter != null && request.ChannelFilter.Any();
            if (!hasChannelFilter)
            {
                throw new ArgumentException(
                    "ChannelFilter is required for SearchMessagesAsync. Add at least one rule, " +
                    "e.g. ChannelFilter.Members.In(Client.LocalUserData.User).",
                    nameof(request));
            }

            if (request.Offset.HasValue && !string.IsNullOrEmpty(request.Next))
            {
                throw new ArgumentException(
                    "Offset and Next pagination are mutually exclusive on SearchMessagesAsync.",
                    nameof(request));
            }

            if (request.Sort != null && request.Offset.HasValue && request.Offset.Value > 0)
            {
                throw new ArgumentException(
                    "Sort cannot be combined with a non-zero Offset on SearchMessagesAsync. " +
                    "Use the Next cursor for sorted pagination.",
                    nameof(request));
            }

            if (request.Limit.HasValue && request.Limit.Value < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(request),
                    "Limit must be greater than or equal to 1.");
            }

            if (!string.IsNullOrEmpty(request.Query) && request.MessageFilter != null &&
                request.MessageFilter.Any(r => r != null))
            {
                throw new ArgumentException(
                    "Query and MessageFilter cannot be combined on SearchMessagesAsync. " +
                    "The server rejects requests that specify both a free-text query and " +
                    "message_filter_conditions. Pick one - either pass a free-text Query, or " +
                    "express the same constraint via MessageFilter (e.g. MessageFilter.Text.Contains(...)).",
                    nameof(request));
            }
        }

        private static MessageInternalDTO ProjectSearchResultToMessageDto(SearchResultMessageInternalDTO source)
        {
            // Project the search-specific payload onto the canonical message DTO so that the cache
            // can reuse the existing StreamMessage create/update path. Every field on
            // SearchResultMessageInternalDTO has a one-to-one counterpart on MessageInternalDTO
            // except for the embedded Channel, which is cached separately.
            return new MessageInternalDTO
            {
                Attachments = source.Attachments,
                BeforeMessageSendFailed = source.BeforeMessageSendFailed,
                Cid = source.Cid,
                Command = source.Command,
                CreatedAt = source.CreatedAt,
                Custom = source.Custom,
                DeletedAt = source.DeletedAt,
                DeletedReplyCount = source.DeletedReplyCount,
                Html = source.Html,
                I18n = source.I18n,
                Id = source.Id,
                ImageLabels = source.ImageLabels,
                LatestReactions = source.LatestReactions,
                MentionedUsers = source.MentionedUsers,
                MessageTextUpdatedAt = source.MessageTextUpdatedAt,
                Mml = source.Mml,
                OwnReactions = source.OwnReactions,
                ParentId = source.ParentId,
                PinExpires = source.PinExpires,
                Pinned = source.Pinned,
                PinnedAt = source.PinnedAt,
                PinnedBy = source.PinnedBy,
                Poll = source.Poll,
                PollId = source.PollId,
                QuotedMessage = source.QuotedMessage,
                QuotedMessageId = source.QuotedMessageId,
                ReactionCounts = source.ReactionCounts,
                ReactionGroups = source.ReactionGroups,
                ReactionScores = source.ReactionScores,
                ReplyCount = source.ReplyCount,
                Shadowed = source.Shadowed,
                ShowInChannel = source.ShowInChannel,
                Silent = source.Silent,
                Text = source.Text,
                ThreadParticipants = source.ThreadParticipants,
                Type = source.Type,
                UpdatedAt = source.UpdatedAt,
                User = source.User,
                AdditionalProperties = source.AdditionalProperties,
            };
        }

        private static StreamSearchWarning BuildSearchWarning(SearchWarningInternalDTO dto)
        {
            if (dto == null)
            {
                return null;
            }

            return new StreamSearchWarning
            {
                Code = dto.WarningCode,
                Description = dto.WarningDescription,
                ChannelSearchCount = dto.ChannelSearchCount,
                ChannelIds = dto.ChannelSearchCids,
            };
        }

        public Task<IEnumerable<IStreamUser>> UpsertUsersAsync(IEnumerable<StreamUserUpsertRequest> userRequests)
            => UpsertUsers(userRequests);

        public async Task<IEnumerable<IStreamUser>> UpsertUsers(IEnumerable<StreamUserUpsertRequest> userRequests)
        {
            StreamAsserts.AssertNotNullOrEmpty(userRequests, nameof(userRequests));

            //StreamTodo: items could be null
            var requestDtos = userRequests.Select(_ => _.TrySaveToDto<UserRequestInternalDTO>())
                .ToDictionary(_ => _.Id, _ => _);

            var response = await InternalLowLevelClient.InternalUserApi.UpsertManyUsersAsync(
                new UpdateUsersRequestInternalDTO
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

            var response = await InternalLowLevelClient.InternalChannelApi.MuteChannelAsync(
                new MuteChannelRequestInternalDTO
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

            await InternalLowLevelClient.InternalChannelApi.UnmuteChannelAsync(new UnmuteChannelRequestInternalDTO
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

            var responseDto = await InternalLowLevelClient.InternalChannelApi.DeleteChannelsAsync(
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

            var responseDto = await InternalLowLevelClient.InternalModerationApi.MuteUserAsync(
                new MuteUserRequestInternalDTO
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

            if (InternalLowLevelClient != null)
            {
                UnsubscribeFrom(InternalLowLevelClient);
                InternalLowLevelClient.Dispose();
            }

            if (_cache?.Threads != null)
            {
                _cache.Threads.Tracked -= OnThreadEnteredCache;
                _cache.Threads.Untracked -= OnThreadLeftCache;
            }

            if (_cache?.Channels != null)
            {
                _cache.Channels.Untracked -= OnChannelLeftCache;
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
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        void IStreamChatClientEventsListener.Update() => InternalLowLevelClient.Update(_timeService.DeltaTime);

        internal StreamChatLowLevelClient InternalLowLevelClient { get; }

        // We probably don't want to expose the presence, state, watch params to the public API
        internal async Task<IStreamChannel> InternalGetOrCreateChannelWithIdAsync(ChannelType channelType,
            string channelId,
            string name = null, bool presence = true, bool state = true, bool watch = true,
            IDictionary<string, object> optionalCustomData = null)
        {
            StreamAsserts.AssertChannelTypeIsValid(channelType);
            StreamAsserts.AssertChannelIdLength(channelId);

            var requestBodyDto = new ChannelGetOrCreateRequestInternalDTO
            {
                Presence = presence,
                State = state,
                Watch = watch,
                Data = new ChannelRequestInternalDTO
                {
                    Name = name,
                },
            };

            if (optionalCustomData != null && optionalCustomData.Any())
            {
                requestBodyDto.Data.AdditionalProperties = optionalCustomData?.ToDictionary(x => x.Key, x => x.Value);
            }

            var channelResponseDto = await InternalLowLevelClient.InternalChannelApi.GetOrCreateChannelAsync(
                channelType,
                channelId, requestBodyDto);
            var channel = _cache.TryCreateOrUpdate(channelResponseDto);
            if (watch)
            {
                MarkChannelWatched(channel);
            }
            return channel;
        }

        internal IStreamLocalUserData UpdateLocalUser(OwnUserInternalDTO ownUserInternalDto)
        {
            _localUserData = _cache.TryCreateOrUpdate(ownUserInternalDto);

            if (LocalUserData == null)
            {
                _logs.Error("Local User Data is null");
                return _localUserData;
            }

            if (LocalUserData.ChannelMutes != null)
            {
                //StreamTodo: Can we not rely on whoever called TryCreateOrUpdate to update this but make it more reliable? Better to react to some event
                // This could be solved if ChannelMutes would be an observable collection
                foreach (var channel in _cache.Channels.AllItems)
                {
                    var isMuted = LocalUserData.ChannelMutes.Any(_ => _.Channel == channel);
                    channel.Muted = isMuted;
                }
            }
            else
            {
                _logs.Info("ChannelMutes is null");
            }

            return _localUserData;
        }

        internal Task RefreshChannelState(string cid)
        {
            if (!_cache.Channels.TryGet(cid, out var channel))
            {
                _logs.Error($"Tried to refresh state of channel with {cid} but no such channel was found in the cache");
                return Task.CompletedTask;
            }

            return GetOrCreateChannelWithIdAsync(channel.Type, channel.Id);
        }

        private readonly ILogs _logs;
        private readonly ITimeService _timeService;
        private readonly ICache _cache;
        private readonly StreamPollsApi _pollsApi;
        private readonly List<IStreamChannel> _watchedChannels = new List<IStreamChannel>();

        private TaskCompletionSource<IStreamLocalUserData> _connectUserTaskSource;
        private CancellationToken _connectUserCancellationToken;
        private CancellationTokenSource _connectUserCancellationTokenSource;
        private bool _isDisposed;

        /// <summary>
        /// Use the <see cref="CreateDefaultClient"/> to create the default client instance.
        /// <example>
        /// Default example::
        /// <code>
        /// var streamChatClient = StreamChatClient.CreateDefaultClient();
        /// </code>
        /// </example>
        /// <example>
        /// Example with custom config:
        /// <code>
        /// var streamChatClient = StreamChatClient.CreateDefaultClient(new StreamClientConfig
        /// {
        ///     LogLevel = StreamLogLevel.Debug
        /// });
        /// </code>
        /// </example>
        /// In case you want to inject custom dependencies into the chat client you can use the <see cref="CreateClientWithCustomDependencies"/>
        /// </summary>
        private StreamChatClient(IWebsocketClient websocketClient, IHttpClient httpClient, ISerializer serializer,
            ITimeService timeService, INetworkMonitor networkMonitor, IApplicationInfo applicationInfo, ILogs logs,
            IStreamClientConfig config)
        {
            _timeService = timeService ?? throw new ArgumentNullException(nameof(timeService));
            _logs = logs ?? throw new ArgumentNullException(nameof(logs));

            InternalLowLevelClient = new StreamChatLowLevelClient(authCredentials: default, websocketClient, httpClient,
                serializer, _timeService, networkMonitor, applicationInfo, logs, config);

            _cache = new Cache(this, serializer, _logs);
            _pollsApi = new StreamPollsApi(InternalLowLevelClient, _cache);

            _cache.Threads.Tracked += OnThreadEnteredCache;
            _cache.Threads.Untracked += OnThreadLeftCache;
            _cache.Channels.Untracked += OnChannelLeftCache;

            SubscribeTo(InternalLowLevelClient);
        }

        private void OnThreadEnteredCache(StreamThread thread) => ThreadTracked?.Invoke(thread);

        private void OnThreadLeftCache(StreamThread thread) => ThreadUntracked?.Invoke(thread);

        private void InternalDeleteChannel(StreamChannel channel)
        {
            //StreamTodo: mark StreamChannel object as deleted + probably silent clear all internal data?
            _cache.Channels.Remove(channel);
            ChannelDeleted?.Invoke(channel.Cid, channel.Id, channel.Type);
        }

        // Flip IsWatched=true and add to _watchedChannels. Call from every path that issued
        // Watch=true to the server. Channels that land in the cache via non-watching paths
        // (search hits, threads with Watch=false, ban-info / mute payloads) stay IsWatched=false.
        // Idempotent: a no-op when the channel is already watched.
        private void MarkChannelWatched(StreamChannel channel)
        {
            if (channel == null || channel.IsWatched)
            {
                return;
            }

            channel.IsWatched = true;
            _watchedChannels.Add(channel);
        }

        // Counterpart to MarkChannelWatched. Called from StreamChannel.StopWatchingAsync
        // after the server confirms the unwatch. Idempotent.
        internal void InternalMarkChannelUnwatched(StreamChannel channel)
        {
            if (channel == null || !channel.IsWatched)
            {
                return;
            }

            channel.IsWatched = false;
            _watchedChannels.Remove(channel);
        }

        private void OnChannelLeftCache(StreamChannel channel) => _watchedChannels.Remove(channel);

        private void TryCancelWaitingForUserConnection()
        {
            var isConnectTaskRunning = _connectUserTaskSource?.Task != null && !_connectUserTaskSource.Task.IsCompleted;
            var isCancellationRequested = _connectUserCancellationTokenSource?.IsCancellationRequested ?? false;

            if (isConnectTaskRunning && !isCancellationRequested)
            {
#if STREAM_DEBUG_ENABLED
                _logs.Info($"Try Cancel {_connectUserTaskSource}");
#endif
                _connectUserTaskSource.TrySetCanceled();
            }
        }

        private async Task InternalGetOrCreateChannelAsync(ChannelType channelType, string channelId)
        {
#if STREAM_TESTS_ENABLED
            const int maxAttempts = 10;
#else
            const int maxAttempts = 1;
#endif

            for (int i = 1; i <= maxAttempts; i++)
            {
                try
                {
                    await GetOrCreateChannelWithIdAsync(channelType, channelId);
                }
                catch (StreamApiException streamException)
                {
                    if (!streamException.IsRateLimitExceededError() || i == maxAttempts)
                    {
                        throw;
                    }

                    if (ConnectionState != ConnectionState.Connected)
                    {
                        break;
                    }

                    var delay = 4 * i;
#if STREAM_TESTS_ENABLED
                    _logs.Warning(
                        $"InternalGetOrCreateChannelAsync attempt failed due to rate limit. Wait {delay} seconds and try again");
#endif
                    //StreamTodo: pass CancellationToken
                    await Task.Delay(delay * 1000);

                    if (ConnectionState != ConnectionState.Connected)
                    {
                        break;
                    }
                }
            }
        }

        #region Events

        private void OnConnected(HealthCheckEventInternalDTO dto)
        {
            try
            {
                var localUserDto = dto.Me;

                // This can sometimes be null. I think it's when the client lost network and believes he's reconnecting
                // but the healthcheck timeout didn't pass on server and from the server perspective the client never disconnected
                if (localUserDto != null)
                {
                    UpdateLocalUser(localUserDto);
                }
                else
                {
                    _logs.Warning("OnConnected localUserDto was NULL and current LocalUserData is " +
                                  (LocalUserData != null) + " value " + LocalUserData);
                }

                Connected?.Invoke(LocalUserData);
            }
            finally
            {
                // This will be null if the ConnectUserAsync with token provider was used
                if (_connectUserTaskSource != null)
                {
                    _connectUserTaskSource.SetResult(LocalUserData);
                    _connectUserTaskSource = null;
                }
            }

            RestoreStateLostDuringDisconnect().LogIfFailed();
        }

        private Task RestoreStateLostDuringDisconnect()
        {
            if (!WatchedChannels.Any())
            {
                return Task.CompletedTask;
            }

            return LowLevelClient.FetchAndProcessEventsSinceLastReceivedEvent(WatchedChannels.Select(c => c.Cid));
        }

        private void OnDisconnected() => Disconnected?.Invoke();

        private void OnConnectionStateChanged(ConnectionState previous, ConnectionState current)
            => ConnectionStateChanged?.Invoke(previous, current);

        private void OnMessageDeleted(MessageDeletedEventInternalDTO eventMessageDeleted)
        {
            if (_cache.Channels.TryGet(eventMessageDeleted.Cid, out var streamChannel))
            {
                streamChannel.HandleMessageDeletedEvent(eventMessageDeleted);
            }

            var deletedMessage = eventMessageDeleted.Message;
            if (deletedMessage == null)
            {
                return;
            }

            var isHardDelete = eventMessageDeleted.HardDelete;

            // Reply: drop it from its thread's LatestReplies. Mirrors Android's
            // QueryThreadsStateLogic.deleteMessageFromThread.
            if (!string.IsNullOrEmpty(deletedMessage.ParentId)
                && _cache.Threads.TryGet(deletedMessage.ParentId, out var thread))
            {
                thread.HandleReplyDeleted(deletedMessage.Id, isHardDelete);
            }

            // Parent: hard-deleting the parent destroys the thread. Soft-delete leaves
            // the thread in place with the parent message marked as deleted.
            if (string.IsNullOrEmpty(deletedMessage.ParentId)
                && isHardDelete
                && _cache.Threads.TryGet(deletedMessage.Id, out var deletedThread))
            {
                _cache.Threads.Remove(deletedThread);
            }
        }

        private void OnMessageUpdated(MessageUpdatedEventInternalDTO eventMessageUpdated)
        {
            if (_cache.Channels.TryGet(eventMessageUpdated.Cid, out var streamChannel))
            {
                streamChannel.HandleMessageUpdatedEvent(eventMessageUpdated);
            }
        }

        private void OnMessageReceived(MessageNewEventInternalDTO eventDto)
        {
            var messageDto = eventDto.Message;
            var messageId = messageDto?.Id;

            // Snapshot insert state BEFORE HandleMessageNewEvent populates the cache, so the
            // first delivery of a given reply (whether via this event or notification.thread_message_new)
            // bumps parent.ReplyCount exactly once. Mirrors Android's updateParentOrReply, which gates
            // the parent counter on a true insert so duplicate or overlapping deliveries are safe.
            var isInsert = !string.IsNullOrEmpty(messageId) && !_cache.Messages.TryGet(messageId, out _);

            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.HandleMessageNewEvent(eventDto);
            }

            var parentId = messageDto?.ParentId;
            if (string.IsNullOrEmpty(parentId))
            {
                return;
            }

            // Watching clients receive message.new but not notification.thread_message_new, so without
            // this bump parent.ReplyCount drifts below the true value until the next REST refresh.
            // Done unconditionally on the parent (independent of thread tracking) to match the
            // notification.thread_message_new path.
            if (isInsert && _cache.Messages.TryGet(parentId, out var parent))
            {
                parent.InternalIncrementReplyCount();
            }

            if (_cache.Threads.TryGet(parentId, out var thread)
                && _cache.Messages.TryGet(messageId, out var reply))
            {
                thread.HandleNewReply(reply);
            }
        }

        private void OnChannelTruncated(ChannelTruncatedEventInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.HandleChannelTruncatedEvent(eventDto);
            }
        }

        private void OnChannelDeletedNotification(NotificationChannelDeletedEventInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                InternalDeleteChannel(streamChannel);
            }
        }

        private void OnChannelVisible(ChannelVisibleEventInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.Hidden = false;
            }
        }

        private void OnChannelHidden(ChannelHiddenEventInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.Hidden = true;
            }
        }

        private void OnChannelDeleted(ChannelDeletedEventInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                InternalDeleteChannel(streamChannel);
            }
        }

        private void OnChannelUpdated(ChannelUpdatedEventInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.HandleChannelUpdatedEvent(eventDto);
            }
        }

        private void OnChannelTruncatedNotification(
            NotificationChannelTruncatedEventInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.HandleChannelTruncatedEvent(eventDto);
            }
        }

        private void OnChannelMutesUpdatedNotification(NotificationChannelMutesUpdatedEventInternalDTO eventDto)
        {
            UpdateLocalUser(eventDto.Me);
        }

        private void OnMessageReceivedNotification(NotificationNewMessageEventInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.InternalHandleMessageNewNotification(eventDto);
            }
        }

        private void OnMutesUpdatedNotification(NotificationMutesUpdatedEventInternalDTO eventDto)
        {
            UpdateLocalUser(eventDto.Me);
        }

        private void OnMemberAdded(MemberAddedEventInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                var member = _cache.TryCreateOrUpdate(eventDto.Member);
                StreamAsserts.AssertNotNull(member, nameof(member));
                streamChannel.InternalAddMember(member);
            }
        }

        private void OnMemberUpdated(MemberUpdatedEventInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                var member = _cache.TryCreateOrUpdate(eventDto.Member);
                StreamAsserts.AssertNotNull(member, nameof(member));
                streamChannel.InternalUpdateMember(member);
            }
        }

        private void OnMemberRemoved(MemberRemovedEventInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                var member = _cache.TryCreateOrUpdate(eventDto.Member);
                StreamAsserts.AssertNotNull(member, nameof(member));
                streamChannel.InternalRemoveMember(member);
            }
        }

        private void OnMessageRead(MessageReadEventInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.InternalHandleMessageReadEvent(eventDto);
            }

            // Thread read propagation: only mutate a thread we're already tracking.
            // Matches Android's QueryThreadsLogic.markThreadAsReadByUser which early-returns for unknown threads.
            if (eventDto.Thread != null
                && _cache.Threads.TryGet(eventDto.Thread.ParentMessageId, out var thread))
            {
                ((IUpdateableFrom2<ThreadResponseInternalDTO, StreamThread>)thread)
                    .UpdateFromDto(eventDto.Thread, _cache);
                thread.HandleMarkReadByUser(eventDto.User?.Id, eventDto.CreatedAt);
            }
        }

        private void OnMarkReadNotification(NotificationMarkReadEventInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.InternalHandleMessageReadNotification(eventDto);
            }

            _localUserData.InternalHandleMarkReadNotification(eventDto);

            // Thread mark-read propagation: only mutate a thread we're already tracking.
            // Matches Android's QueryThreadsLogic.markThreadAsReadByUser which early-returns for unknown threads.
            var threadId = eventDto.Thread?.ParentMessageId ?? eventDto.ThreadId;
            if (!string.IsNullOrEmpty(threadId) && _cache.Threads.TryGet(threadId, out var thread))
            {
                if (eventDto.Thread != null)
                {
                    ((IUpdateableFrom2<ThreadResponseInternalDTO, StreamThread>)thread)
                        .UpdateFromDto(eventDto.Thread, _cache);
                }

                thread.HandleMarkReadByUser(eventDto.User?.Id, eventDto.CreatedAt);
            }
        }

        private void OnAddedToChannelNotification(NotificationAddedToChannelEventInternalDTO eventDto)
        {
#if STREAM_TESTS_ENABLED
            var sb = new StringBuilder();
            sb.AppendLine("OnAddedToChannelNotification");
            sb.AppendLine($"{nameof(eventDto.ChannelType)}: {eventDto.ChannelType}");
            sb.AppendLine($"{nameof(eventDto.Channel.Type)}: {eventDto.Channel.Type}");
            sb.AppendLine($"{nameof(eventDto.Channel.Id)}: {eventDto.Channel.Id}");
            sb.AppendLine($"{nameof(eventDto.Channel.Cid)}: {eventDto.Channel.Cid}");
#endif

            var channel = _cache.TryCreateOrUpdate(eventDto.Channel, out var wasCreated);

#if STREAM_TESTS_ENABLED
            sb.Length = 0;
            sb.AppendLine("Channel returned from cache:");
            sb.AppendLine($"{nameof(channel.Type)}: {channel.Type}");
            sb.AppendLine($"{nameof(channel.Id)}: {channel.Id}");
            sb.AppendLine($"{nameof(channel.Cid)}: {channel.Cid}");
            _logs.Info(sb.ToString());
#endif

            var member = _cache.TryCreateOrUpdate(eventDto.Member);
            _cache.TryCreateOrUpdate(eventDto.Member.User);

            if (!wasCreated)
            {
                AddedToChannelAsMember?.Invoke(channel, member);
                return;
            }

            // Watch channel, otherwise WS events won't be received
            InternalGetOrCreateChannelAsync(channel.Type, channel.Id).ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    _logs.Error($"Failed to watch channel with type: {channel.Type} & id: {channel.Id} " +
                                $"before triggering the {nameof(AddedToChannelAsMember)} event. Inspect the following exception: " +
                                t.Exception);
                    _logs.Exception(t.Exception);
                    return;
                }

                AddedToChannelAsMember?.Invoke(channel, member);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void OnRemovedFromChannelNotification(
            NotificationRemovedFromChannelEventInternalDTO eventDto)
        {
#if STREAM_TESTS_ENABLED
            var sb = new StringBuilder();
            sb.AppendLine("OnRemovedFromChannelNotification BEFORE CACHE");
            sb.AppendLine($"{nameof(eventDto.ChannelType)}: {eventDto.ChannelType}");
            sb.AppendLine($"{nameof(eventDto.Channel.Type)}: {eventDto.Channel.Type}");
            sb.AppendLine($"{nameof(eventDto.Channel.Id)}: {eventDto.Channel.Id}");
            sb.AppendLine($"{nameof(eventDto.Channel.Cid)}: {eventDto.Channel.Cid}");
            _logs.Info(sb.ToString());
#endif
            var channel = _cache.TryCreateOrUpdate(eventDto.Channel, out var wasCreated);

#if STREAM_TESTS_ENABLED
            sb.Length = 0;
            sb.AppendLine("Channel returned FROM CACHE:");
            sb.AppendLine($"{nameof(channel.Type)}: {channel.Type}");
            sb.AppendLine($"{nameof(channel.Id)}: {channel.Id}");
            sb.AppendLine($"{nameof(channel.Cid)}: {channel.Cid}");
            _logs.Info(sb.ToString());
#endif

            var member = _cache.TryCreateOrUpdate(eventDto.Member);
            _cache.TryCreateOrUpdate(eventDto.Member.User);

            if (!wasCreated)
            {
                RemovedFromChannelAsMember?.Invoke(channel, member);
                return;
            }

            // Watch channel, otherwise WS events won't be received
            InternalGetOrCreateChannelAsync(channel.Type, channel.Id).ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    _logs.Error($"Failed to watch channel with type: {channel.Type} & id: {channel.Id} " +
                                $"before triggering the {nameof(RemovedFromChannelAsMember)} event. Inspect the following exception: " +
                                t.Exception);
                    _logs.Exception(t.Exception);
                    return;
                }

                RemovedFromChannelAsMember?.Invoke(channel, member);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void OnInvitedNotification(NotificationInvitedEventInternalDTO eventDto)
        {
            var channel = _cache.TryCreateOrUpdate(eventDto.Channel, out var wasCreated);
            var user = _cache.TryCreateOrUpdate(eventDto.User);

            if (!wasCreated)
            {
                ChannelInviteReceived?.Invoke(channel, user);
                return;
            }

            // Watch channel, otherwise WS events won't be received
            InternalGetOrCreateChannelAsync(channel.Type, channel.Id).ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    _logs.Error($"Failed to watch channel with type: {channel.Type} & id: {channel.Id} " +
                                $"before triggering the {nameof(ChannelInviteReceived)} event. Inspect the following exception: " +
                                t.Exception);
                    _logs.Exception(t.Exception);
                    return;
                }

                ChannelInviteReceived?.Invoke(channel, user);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void OnInviteAcceptedNotification(NotificationInviteAcceptedEventInternalDTO eventDto)
        {
            var channel = _cache.TryCreateOrUpdate(eventDto.Channel, out var wasCreated);
            var user = _cache.TryCreateOrUpdate(eventDto.User);

            if (!wasCreated)
            {
                ChannelInviteAccepted?.Invoke(channel, user);
                return;
            }

            // Watch channel, otherwise WS events won't be received
            InternalGetOrCreateChannelAsync(channel.Type, channel.Id).ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    _logs.Error($"Failed to watch channel with type: {channel.Type} & id: {channel.Id} " +
                                $"before triggering the {nameof(ChannelInviteAccepted)} event. Inspect the following exception: " +
                                t.Exception);
                    _logs.Exception(t.Exception);
                    return;
                }

                ChannelInviteAccepted?.Invoke(channel, user);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void OnInviteRejectedNotification(NotificationInviteRejectedEventInternalDTO eventDto)
        {
            var channel = _cache.TryCreateOrUpdate(eventDto.Channel, out var wasCreated);
            var user = _cache.TryCreateOrUpdate(eventDto.User);

            if (!wasCreated)
            {
                ChannelInviteRejected?.Invoke(channel, user);
                return;
            }

            // Watch channel, otherwise WS events won't be received
            InternalGetOrCreateChannelAsync(channel.Type, channel.Id).ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    _logs.Error($"Failed to watch channel with type: {channel.Type} & id: {channel.Id} " +
                                $"before triggering the {nameof(ChannelInviteRejected)} event. Inspect the following exception: " +
                                t.Exception);
                    _logs.Exception(t.Exception);
                    return;
                }

                ChannelInviteRejected?.Invoke(channel, user);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void OnReactionReceived(ReactionNewEventInternalDTO eventDto)
        {
            if (!_cache.Channels.TryGet(eventDto.Cid, out var channel))
            {
                return;
            }

            if (_cache.Messages.TryGet(eventDto.Message.Id, out var message))
            {
                var reaction
                    = new StreamReaction().TryLoadFromDto<ReactionInternalDTO, StreamReaction>(eventDto.Reaction,
                        _cache);
                message.HandleReactionNewEvent(eventDto, channel, reaction);
                channel.InternalNotifyReactionReceived(message, reaction);
            }
        }

        private void OnReactionUpdated(ReactionUpdatedEventInternalDTO eventDto)
        {
            if (!_cache.Channels.TryGet(eventDto.Cid, out var channel))
            {
                return;
            }

            if (_cache.Messages.TryGet(eventDto.Message.Id, out var message))
            {
                var reaction
                    = new StreamReaction().TryLoadFromDto<ReactionInternalDTO, StreamReaction>(eventDto.Reaction,
                        _cache);
                message.HandleReactionUpdatedEvent(eventDto, channel, reaction);
                channel.InternalNotifyReactionUpdated(message, reaction);
            }
        }

        private void OnReactionDeleted(ReactionDeletedEventInternalDTO eventDto)
        {
            if (!_cache.Channels.TryGet(eventDto.Cid, out var channel))
            {
                return;
            }

            if (_cache.Messages.TryGet(eventDto.Message.Id, out var message))
            {
                var reaction
                    = new StreamReaction().TryLoadFromDto<ReactionInternalDTO, StreamReaction>(eventDto.Reaction,
                        _cache);
                message.HandleReactionDeletedEvent(eventDto, channel, reaction);
                channel.InternalNotifyReactionDeleted(message, reaction);
            }
        }

        private void OnUserWatchingStop(UserWatchingStopEventInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.InternalHandleUserWatchingStop(eventDto);
            }
        }

        private void OnUserWatchingStart(UserWatchingStartEventInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.InternalHandleUserWatchingStartEvent(eventDto);
            }
        }

        private void OnLowLevelClientUserUnbanned(UserUnbannedEventInternalDTO obj)
        {
            //StreamTodo: IMPLEMENT
        }

        private void OnLowLevelClientUserBanned(UserBannedEventInternalDTO obj)
        {
            //StreamTodo: IMPLEMENT
        }

        private void OnLowLevelClientUserDeleted(UserDeletedEventInternalDTO obj)
        {
            //StreamTodo: IMPLEMENT
        }

        private void OnLowLevelUserUpdated(UserUpdatedEventInternalDTO eventDto)
        {
            if (_cache.Users.TryGet(eventDto.User.Id, out var streamUser))
            {
                _cache.TryCreateOrUpdate(eventDto.User);
            }
        }

        private void OnUserPresenceChanged(UserPresenceChangedEventInternalDTO eventDto)
        {
            if (_cache.Users.TryGet(eventDto.User.Id, out var streamUser))
            {
                streamUser.InternalHandlePresenceChanged(eventDto);
            }
        }

        private void OnTypingStopped(TypingStopEventInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.InternalHandleTypingStopped(eventDto);
            }
        }

        private void OnTypingStarted(TypingStartEventInternalDTO eventDto)
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

            lowLevelClient.InternalPollClosed += OnPollClosed;
            lowLevelClient.InternalPollDeleted += OnPollDeleted;
            lowLevelClient.InternalPollUpdated += OnPollUpdated;
            lowLevelClient.InternalPollVoteCasted += OnPollVoteCasted;
            lowLevelClient.InternalPollVoteChanged += OnPollVoteChanged;
            lowLevelClient.InternalPollVoteRemoved += OnPollVoteRemoved;

            lowLevelClient.InternalThreadUpdated += OnThreadUpdated;
            lowLevelClient.InternalNotificationThreadMessageNew += OnNotificationThreadMessageNew;
            lowLevelClient.InternalNotificationMarkUnread += OnNotificationMarkUnread;
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

            lowLevelClient.InternalPollClosed -= OnPollClosed;
            lowLevelClient.InternalPollDeleted -= OnPollDeleted;
            lowLevelClient.InternalPollUpdated -= OnPollUpdated;
            lowLevelClient.InternalPollVoteCasted -= OnPollVoteCasted;
            lowLevelClient.InternalPollVoteChanged -= OnPollVoteChanged;
            lowLevelClient.InternalPollVoteRemoved -= OnPollVoteRemoved;

            lowLevelClient.InternalThreadUpdated -= OnThreadUpdated;
            lowLevelClient.InternalNotificationThreadMessageNew -= OnNotificationThreadMessageNew;
            lowLevelClient.InternalNotificationMarkUnread -= OnNotificationMarkUnread;
        }

        private void OnPollClosed(PollClosedEventInternalDTO eventDto)
        {
            if (!_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
#if STREAM_DEBUG_ENABLED
                _logs.Warning($"[{nameof(OnPollClosed)}] Poll WS event received but ignored because channel with ID {eventDto.Cid} was not found in cache");
#endif
                return;
            }

            var streamPoll = _cache.TryCreateOrUpdate(eventDto.Poll);
            streamPoll.InternalSetChannel(streamChannel);

            streamPoll.HandlePollClosedEvent(eventDto);
        }

        private void OnPollDeleted(PollDeletedEventInternalDTO eventDto)
        {
            if (!_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
#if STREAM_DEBUG_ENABLED
                _logs.Warning($"[{nameof(OnPollDeleted)}] Poll WS event received but ignored because channel with ID {eventDto.Cid} was not found in cache");
#endif
                return;
            }

            if (_cache.Polls.TryGet(eventDto.Poll.Id, out var streamPoll))
            {
                // Remove poll from cache when deleted
                _cache.Polls.Remove(streamPoll);
            }
        }

        private void OnPollUpdated(PollUpdatedEventInternalDTO eventDto)
        {
            if (!_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
#if STREAM_DEBUG_ENABLED
                _logs.Warning($"[{nameof(OnPollUpdated)}] Poll WS event received but ignored because channel with ID {eventDto.Cid} was not found in cache");
#endif
                return;
            }

            var streamPoll = _cache.TryCreateOrUpdate(eventDto.Poll);
            streamPoll.InternalSetChannel(streamChannel);

            streamPoll.HandlePollUpdatedEvent(eventDto);
        }

        private void OnPollVoteCasted(PollVoteCastedEventInternalDTO eventDto)
        {
            if (!_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
#if STREAM_DEBUG_ENABLED
                _logs.Warning($"[{nameof(OnPollVoteCasted)}] Poll WS event received but ignored because channel with ID {eventDto.Cid} was not found in cache");
#endif
                return;
            }

            var streamPoll = _cache.TryCreateOrUpdate(eventDto.Poll);
            streamPoll.InternalSetChannel(streamChannel);

            streamPoll.HandlePollVoteCastedEvent(eventDto);
        }

        private void OnPollVoteChanged(PollVoteChangedEventInternalDTO eventDto)
        {
            if (!_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
#if STREAM_DEBUG_ENABLED
                _logs.Warning($"[{nameof(OnPollVoteChanged)}] Poll WS event received but ignored because channel with ID {eventDto.Cid} was not found in cache");
#endif
                return;
            }

            var streamPoll = _cache.TryCreateOrUpdate(eventDto.Poll);
            streamPoll.InternalSetChannel(streamChannel);

            streamPoll.HandlePollVoteChangedEvent(eventDto);
        }

        private void OnPollVoteRemoved(PollVoteRemovedEventInternalDTO eventDto)
        {
            if (!_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
#if STREAM_DEBUG_ENABLED
                _logs.Warning($"[{nameof(OnPollVoteRemoved)}] Poll WS event received but ignored because channel with ID {eventDto.Cid} was not found in cache");
#endif
                return;
            }

            var streamPoll = _cache.TryCreateOrUpdate(eventDto.Poll);
            streamPoll.InternalSetChannel(streamChannel);

            streamPoll.HandlePollVoteRemovedEvent(eventDto);
        }

        private void OnThreadUpdated(ThreadUpdatedEventInternalDTO eventDto)
        {
            // Thread update propagation: only mutate a thread we're already tracking.
            // Matches Android's QueryThreadsStateLogic.updateThreadFromEvent which early-returns for unknown threads.
            if (eventDto.Thread == null
                || !_cache.Threads.TryGet(eventDto.Thread.ParentMessageId, out var thread))
            {
                return;
            }

            // UpdateFromDto raises the public Updated event at the end; no need to invoke it again here.
            ((IUpdateableFrom2<ThreadResponseInternalDTO, StreamThread>)thread)
                .UpdateFromDto(eventDto.Thread, _cache);
        }

        private void OnNotificationThreadMessageNew(NotificationThreadMessageNewEventInternalDTO eventDto)
        {
            var messageDto = eventDto.Message;
            if (messageDto == null)
            {
                return;
            }

            // Snapshot insert state before TryCreateOrUpdate creates the cache entry. Pairs with
            // the same gate in OnMessageReceived so a reply delivered via both event paths bumps
            // parent.ReplyCount exactly once.
            var isInsert = !string.IsNullOrEmpty(messageDto.Id) && !_cache.Messages.TryGet(messageDto.Id, out _);

            var reply = _cache.TryCreateOrUpdate(messageDto);

            // Update parent's reply count if we know it
            if (isInsert && !string.IsNullOrEmpty(reply?.ParentId)
                && _cache.Messages.TryGet(reply.ParentId, out var parent))
            {
                parent.InternalIncrementReplyCount();
            }

            // If we track this thread, update it with the new reply
            var threadId = eventDto.ThreadId ?? reply?.ParentId;
            if (!string.IsNullOrEmpty(threadId) && _cache.Threads.TryGet(threadId, out var thread))
            {
                thread.HandleNewReply(reply);
            }

            if (eventDto.UnreadThreads.HasValue || eventDto.UnreadThreadMessages.HasValue)
            {
                _localUserData?.InternalHandleThreadMessageNewNotification(eventDto);
            }
        }

        private void OnNotificationMarkUnread(NotificationMarkUnreadEventInternalDTO eventDto)
        {
            _localUserData?.InternalHandleMarkUnreadNotification(eventDto);

            if (_cache.Channels.TryGet(eventDto.Cid, out var channel))
            {
                channel.InternalHandleMarkUnreadNotification(eventDto);
            }

            // Thread mark-unread propagation: only mutate a thread we're already tracking.
            // Matches Android's QueryThreadsLogic.markThreadAsUnreadByUser which early-returns
            // for unknown threads. The event payload omits the read array, so HandleMarkUnreadByUser
            // mutates the acting user's StreamRead in place before raising ReadStateChanged.
            if (!string.IsNullOrEmpty(eventDto.ThreadId)
                && _cache.Threads.TryGet(eventDto.ThreadId, out var thread))
            {
                thread.HandleMarkUnreadByUser(eventDto.User?.Id, eventDto.LastReadAt);
            }
        }

        #endregion
    }
}