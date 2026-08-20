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
using StreamChat.Core.QueryBuilders.Filters.Channels;
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

    /// <inheritdoc cref="IStreamChatClient.StateRecovered"/>
    public delegate void StateRecoveredHandler(StreamStateRecoveredEventArgs eventArgs);

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

        public event StateRecoveredHandler StateRecovered;

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

            // Ends the session, so the next Connected transition is a fresh login rather than a
            // reconnect and must not run recovery or raise StateRecovered.
            _hasConnectedBefore = false;

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
                await WatchResultChannelsAsync(distinctChannels.Values, cancellationToken);
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

        // The /search endpoint returns channel data but does not start watching those channels,
        // so search hits don't receive realtime updates on their own. We watch them with as few
        // requests as possible: a single QueryChannels with a `cid IN (...)` filter, batched in
        // groups of 30 (the server's page limit) to stay clear of per-request limits. Channels
        // that are already watched are skipped.
        private async Task WatchResultChannelsAsync(IEnumerable<IStreamChannel> channels,
            CancellationToken cancellationToken)
        {
            var cidsToWatch = channels.Where(c => !c.IsWatched).Select(c => c.Cid).ToList();
            if (cidsToWatch.Count == 0)
            {
                return;
            }

            const int maxChannelsPerQuery = 30;
            for (var i = 0; i < cidsToWatch.Count; i += maxChannelsPerQuery)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var chunk = cidsToWatch.Skip(i).Take(maxChannelsPerQuery).ToList();
                var filters = new IFieldFilterRule[]
                {
                    ChannelFilter.Cid.In(chunk),
                };

                await QueryChannelsAsync(filters, limit: chunk.Count);
            }
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

        internal ICache InternalCache => _cache;

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

        /// <summary>
        /// Cids that were being watched when the connection dropped, most recently active first.
        /// Reconnect recovery restores state and watches from this, not from
        /// <see cref="_watchedChannels"/>, which is cleared on the disconnect.
        /// </summary>
        private readonly List<string> _recoveryChannelCids = new List<string>();

        /// <summary>
        /// Cids the <c>/sync</c> response reported as inaccessible - deleted, or no longer readable by
        /// the local user. Never re-queried again for the lifetime of the client.
        /// </summary>
        private readonly HashSet<string> _inaccessibleCids = new HashSet<string>();

        private int _recoveryGeneration;
        private bool _hasConnectedBefore;

        /// <summary>
        /// Recovering more channels than this would mean an unbounded number of sequential queries on
        /// every reconnect, which invites rate limiting. Matches the <c>/sync</c> cid cap so both
        /// halves of recovery cover the same set. JS and Android both cap lower, at 30.
        /// </summary>
        internal const int MaxRecoveredChannels = StreamChatLowLevelClient.MaxSyncChannelCids;

        /// <summary>
        /// <c>QueryChannelsAsync</c> asserts a limit of at most 30, so a longer recovery set has to be
        /// chunked. Same chunk size as <see cref="WatchResultChannelsAsync"/>.
        /// </summary>
        private const int MaxChannelsPerRecoveryQuery = 30;

        // Ties are broken arbitrarily - List.Sort is unstable - which only matters for channels with
        // an equal LastMessageAt, where there is no meaningful "more recently active" anyway.
        private static readonly Comparison<IStreamChannel> ByLastMessageAtDescending = (a, b)
            => (b.LastMessageAt ?? DateTimeOffset.MinValue).CompareTo(a.LastMessageAt ?? DateTimeOffset.MinValue);

        /// <inheritdoc cref="StreamChatLowLevelClient.IsApplyingHistoryEvents"/>
        internal bool IsApplyingHistorySync => InternalLowLevelClient.IsApplyingHistoryEvents;

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
            if (channel == null)
            {
                return;
            }

            // Drop it from the recovery snapshot as well, or an unwatch performed while disconnected
            // would be undone by the next reconnect re-watching it.
            _recoveryChannelCids.Remove(channel.Cid);

            if (!channel.IsWatched)
            {
                return;
            }

            channel.IsWatched = false;
            _watchedChannels.Remove(channel);
        }

        private void OnChannelLeftCache(StreamChannel channel)
        {
            _watchedChannels.Remove(channel);
            _recoveryChannelCids.Remove(channel.Cid);
        }

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

        /// <summary>
        /// Watches are bound to a websocket connection, and a reconnect always gets a new one, so a
        /// reconnected client is watching nothing until something re-watches for it. Without this the
        /// channels stay in local state but stop receiving events - the chat looks alive and silently
        /// never updates again.
        ///
        /// This holds no matter how briefly the socket was down. The handshake payload
        /// (<c>ConnectPayload</c>) carries only the user and token plus
        /// <c>server_determines_connection_id</c>; there is no session or resume token, so the client
        /// cannot ask the server to continue a previous connection, and the server mints a fresh
        /// <c>connection_id</c> that every subsequent request is then tagged with. Do not confuse this
        /// with the server-side health check grace period: that governs when the server notices a
        /// silently dropped socket, which affects presence and the cleanup of the stale watcher entry.
        /// It does not hand the old connection's watches to the new one - if anything a fast reconnect
        /// is the worse case, because for a while the channel counts you as a watcher twice while the
        /// connection you are actually reading receives nothing.
        ///
        /// Runs after every reconnect, in this order:
        ///
        /// 1. <c>/sync</c> catch-up, best effort. This is what makes a short outage recover with no
        ///    hole in the message list. It has to run first because a replayed <c>channel.truncated</c>
        ///    wipes the local message list, and doing that after step 2 would discard the page step 2
        ///    just fetched.
        /// 2. Re-query and re-watch, unconditionally, whatever step 1 did. One query per 30 cids, with
        ///    <c>State</c> and <c>Watch</c> set, so a single request both re-hydrates and re-watches.
        /// 3. Raise <see cref="StateRecovered"/> once.
        ///
        /// Steps 1 and 2 are individually fault-tolerant: a failure in one channel or one request must
        /// not abandon the others, because this is the only recovery this reconnect gets.
        /// </summary>
        private async Task RestoreStateLostDuringDisconnect()
        {
            // A fresh login is not a recovery: there is no prior state to restore and no consumer
            // expects a recovery signal for it. Anything left in the snapshot belongs to the previous
            // session, and possibly to a different user, so drop it.
            if (!_hasConnectedBefore)
            {
                _hasConnectedBefore = true;
                _recoveryChannelCids.Clear();
                _inaccessibleCids.Clear();
                return;
            }

            if (InternalLowLevelClient.Config.StateRecoveryStrategy == StateRecoveryStrategy.Disabled)
            {
                return;
            }

            var generation = ++_recoveryGeneration;

            // Pooled - never leaves this method and the steps below only read it. The two collections
            // handed to StateRecovered are not pooled, because subscribers keep them for as long as
            // they like.
            using (new ListPoolScope<string>(out var recoverSet))
            {
                FillRecoverySet(recoverSet);

                var refreshedChannels = new List<IStreamChannel>();

                try
                {
                    if (recoverSet.Count > 0)
                    {
                        await TryCatchUpWithHistoryAsync(recoverSet, generation);
                        if (!IsRecoveryGenerationCurrent(generation))
                        {
                            return;
                        }

                        await RehydrateAndRewatchChannelsAsync(recoverSet, generation, refreshedChannels);
                        if (!IsRecoveryGenerationCurrent(generation))
                        {
                            return;
                        }
                    }
                }
                catch (Exception e)
                {
                    // Defence in depth - every step already handles its own failures. Whatever happened,
                    // the consumer still gets told that recovery finished and which channels are stale.
                    _logs.Exception(e);
                }

                if (!IsRecoveryGenerationCurrent(generation))
                {
                    return;
                }

                var unrecovered = new List<string>();

                using (new HashSetPoolScope<string>(out var recovered))
                {
                    for (var i = 0; i < refreshedChannels.Count; i++)
                    {
                        recovered.Add(refreshedChannels[i].Cid);
                    }

                    for (var i = 0; i < recoverSet.Count; i++)
                    {
                        if (!recovered.Contains(recoverSet[i]))
                        {
                            unrecovered.Add(recoverSet[i]);
                        }
                    }
                }

                if (unrecovered.Count > 0)
                {
                    _logs.Warning(
                        $"Reconnect recovery could not restore {unrecovered.Count} channel(s): {string.Join(", ", unrecovered)}. " +
                        "Their local state is stale and they are no longer watched. See " +
                        nameof(StreamStateRecoveredEventArgs) + "." + nameof(StreamStateRecoveredEventArgs.UnrecoveredChannelCids));
                }

                StateRecovered?.Invoke(new StreamStateRecoveredEventArgs(refreshedChannels, unrecovered));
            }
        }

        /// <summary>
        /// Copy the most recently active <see cref="MaxRecoveredChannels"/> cids from the snapshot
        /// captured on the disconnect into <paramref name="recoverSet"/>.
        /// </summary>
        private void FillRecoverySet(List<string> recoverSet)
        {
            if (_recoveryChannelCids.Count > MaxRecoveredChannels)
            {
                _logs.Warning(
                    $"{_recoveryChannelCids.Count} channels were being watched when the connection dropped, but reconnect " +
                    $"recovery restores at most {MaxRecoveredChannels}. The {MaxRecoveredChannels} most recently active are " +
                    "recovered; the rest keep stale state and are not re-watched. Watch fewer channels concurrently, or set " +
                    nameof(IStreamClientConfig) + "." + nameof(IStreamClientConfig.StateRecoveryStrategy) + " to " +
                    nameof(StateRecoveryStrategy.Disabled) + " and recover them yourself.");
            }

            var count = Math.Min(_recoveryChannelCids.Count, MaxRecoveredChannels);
            for (var i = 0; i < count; i++)
            {
                recoverSet.Add(_recoveryChannelCids[i]);
            }
        }

        private async Task TryCatchUpWithHistoryAsync(IReadOnlyList<string> recoverSet, int generation)
        {
            try
            {
                var response = await InternalLowLevelClient.TrySyncHistoryAsync(recoverSet);

                // null means the catch-up was skipped: no sync point, or one older than the 30 days
                // the server accepts. Both used to return before any recovery ran; now step 2 still
                // runs, which is the whole point of making it unconditional.
                if (response?.Events == null || response.Events.Count == 0)
                {
                    return;
                }

                if (!IsRecoveryGenerationCurrent(generation))
                {
                    return;
                }

                if (response.InaccessibleCids != null)
                {
                    // The server is telling us these will never come back. Recording them keeps the
                    // re-query from asking about deleted channels and stops us reporting them as a
                    // recovery failure every reconnect.
                    foreach (var cid in response.InaccessibleCids)
                    {
                        _inaccessibleCids.Add(cid);
                    }
                }

                if (InternalLowLevelClient.Config.StateRecoveryStrategy == StateRecoveryStrategy.BatchStateUpdate)
                {
                    InternalLowLevelClient.ApplyHistoryEvents(response.Events);
                }
                else
                {
                    InternalLowLevelClient.ReplayHistoryEvents(response.Events);
                }
            }
            catch (StreamApiException ex) when (ex.IsInputError())
            {
                // HTTP 400 / code 4, "too many events to sync". The server counts events summed across
                // every requested cid against a ceiling of roughly 1000 and refuses the whole request,
                // so this is the normal outcome of a long outage on busy channels, not an anomaly.
                // The re-query below is the fallback and recovers the same state minus the events that
                // did not fit in the latest page.
                _logs.Warning("The /sync catch-up was refused because too many events accumulated during the outage. " +
                              "Recovering channel state with a re-query instead. " + ex.Message);
            }
            catch (Exception ex)
            {
                _logs.Warning("The /sync catch-up failed. Recovering channel state with a re-query instead. " +
                              ex.Message);
            }
        }

        /// <summary>
        /// Re-hydrate and re-watch in one request per <see cref="MaxChannelsPerRecoveryQuery"/> cids.
        /// </summary>
        /// <remarks>
        /// Unlike Android, this does not follow up with a per-channel re-watch for cids the query did
        /// not return. Android needs that because it recovers through the customer's own channel-list
        /// queries, which need not cover every active cid; this queries the recovery set by cid, so it
        /// is exhaustive by construction. A cid the query omits is one the server will not return at
        /// all - deleted, or no longer readable - and the only per-channel watch primitive available
        /// is get-or-create, which would recreate a channel that was deleted while we were offline.
        /// Such cids are reported through
        /// <see cref="StreamStateRecoveredEventArgs.UnrecoveredChannelCids"/> instead.
        /// </remarks>
        private async Task RehydrateAndRewatchChannelsAsync(IReadOnlyList<string> recoverSet, int generation,
            List<IStreamChannel> refreshed)
        {
            var sort = ChannelSort.OrderByDescending(ChannelSortFieldName.LastMessageAt);

            for (var i = 0; i < recoverSet.Count; i += MaxChannelsPerRecoveryQuery)
            {
                if (!IsRecoveryGenerationCurrent(generation))
                {
                    return;
                }

                // Released only once the query has completed: the filter holds this list and the
                // request body is serialized from it.
                using (new ListPoolScope<string>(out var chunk))
                {
                    var chunkEnd = Math.Min(i + MaxChannelsPerRecoveryQuery, recoverSet.Count);
                    for (var j = i; j < chunkEnd; j++)
                    {
                        if (!_inaccessibleCids.Contains(recoverSet[j]))
                        {
                            chunk.Add(recoverSet[j]);
                        }
                    }

                    if (chunk.Count == 0)
                    {
                        continue;
                    }

                    var filters = new IFieldFilterRule[]
                    {
                        ChannelFilter.Cid.In(chunk),
                    };

                    IEnumerable<IStreamChannel> channels;
                    try
                    {
                        channels = await QueryChannelsAsync(filters, sort, limit: chunk.Count);
                    }
                    catch (Exception e)
                    {
                        // One failed chunk (a rate limit part-way through a long watch list, a channel
                        // torn down while offline) must not cost the remaining chunks their recovery -
                        // there is no later retry this connection.
                        _logs.Warning($"Recovery query failed for {chunk.Count} channel(s). Continuing with the rest. " +
                                      e.Message);
                        continue;
                    }

                    if (!IsRecoveryGenerationCurrent(generation))
                    {
                        return;
                    }

                    foreach (var channel in channels)
                    {
                        // The query merge path goes through UpdateFromDto, which does not trim, so a
                        // recovery merge can push Messages past MessageCacheWindow.MaxMessages.
                        ((StreamChannel)channel).InternalTrimMessageCache();
                        refreshed.Add(channel);
                    }
                }
            }
        }

        private bool IsRecoveryGenerationCurrent(int generation) => generation == _recoveryGeneration;

        /// <summary>
        /// Capture what was being watched when the connection dropped, then stop claiming those
        /// watches: the server has dropped them, so <see cref="IStreamChannel.IsWatched"/> would
        /// otherwise report watches that no longer exist. Recovery restores both from the snapshot.
        /// </summary>
        private void SnapshotRecoverySetAndClearWatches()
        {
            // A reconnect attempt that fails transitions Connecting -> Disconnected again, and by then
            // the watch list is already empty. Overwriting the snapshot at that point would throw away
            // the only record of what needs recovering, and the reconnect that eventually succeeds
            // would restore nothing at all - which is exactly the flaky-mobile-network case.
            if (_watchedChannels.Count == 0)
            {
                return;
            }

            _recoveryChannelCids.Clear();

            using (new ListPoolScope<IStreamChannel>(out var ordered))
            {
                ordered.AddRange(_watchedChannels);
                ordered.Sort(ByLastMessageAtDescending);

                for (var i = 0; i < ordered.Count; i++)
                {
                    _recoveryChannelCids.Add(ordered[i].Cid);
                }
            }

            for (var i = 0; i < _watchedChannels.Count; i++)
            {
                ((StreamChannel)_watchedChannels[i]).IsWatched = false;
            }

            _watchedChannels.Clear();
        }

        private void OnDisconnected() => Disconnected?.Invoke();

        private void OnConnectionStateChanged(ConnectionState previous, ConnectionState current)
        {
            if (current == ConnectionState.Disconnected)
            {
                // Supersede any recovery still in flight before its responses can land on top of the
                // state the next recovery is about to fetch. Some channel fields (read state, members,
                // pinned messages) are replaced wholesale by a query response rather than merged, so a
                // late response is not merely redundant, it can overwrite newer state.
                _recoveryGeneration++;

                if (InternalLowLevelClient.Config.StateRecoveryStrategy != StateRecoveryStrategy.Disabled)
                {
                    SnapshotRecoverySetAndClearWatches();
                }
            }

            ConnectionStateChanged?.Invoke(previous, current);
        }

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
            var channel = _cache.TryCreateOrUpdate(eventDto.Channel);

#if STREAM_TESTS_ENABLED
            sb.Length = 0;
            sb.AppendLine("Channel returned FROM CACHE:");
            sb.AppendLine($"{nameof(channel.Type)}: {channel.Type}");
            sb.AppendLine($"{nameof(channel.Id)}: {channel.Id}");
            sb.AppendLine($"{nameof(channel.Cid)}: {channel.Cid}");
            _logs.Info(sb.ToString());
#endif

            _cache.TryCreateOrUpdate(eventDto.User);
            if (eventDto.Member != null && eventDto.Member.User == null && eventDto.User != null)
            {
                eventDto.Member.User = eventDto.User;
            }

            var member = _cache.TryCreateOrUpdate(eventDto.Member);

            // Watched channels receive member.removed -> IStreamChannel.MemberRemoved instead.
            // The server may still deliver notification.removed_from_channel to the removed user.
            if (channel.IsWatched)
            {
                return;
            }

            // Unlike notification.added_to_channel, do not watch here — the user was just removed
            // and no longer has ReadChannel. The notification payload is sufficient to raise the event.
            RemovedFromChannelAsMember?.Invoke(channel, member);
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

        // Who is currently watching is live presence, like typing: replaying it would leave watchers
        // listed who left during the outage. The recovery query returns the authoritative watcher set.
        private void OnUserWatchingStop(UserWatchingStopEventInternalDTO eventDto)
        {
            if (IsApplyingHistorySync)
            {
                return;
            }

            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.InternalHandleUserWatchingStop(eventDto);
            }
        }

        /// <inheritdoc cref="OnUserWatchingStop"/>
        private void OnUserWatchingStart(UserWatchingStartEventInternalDTO eventDto)
        {
            if (IsApplyingHistorySync)
            {
                return;
            }

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
            // Typing is live presence with no meaning in a history replay, and applying it is not
            // merely redundant but wrong: a typing.start whose matching typing.stop fell outside the
            // synced window would leave a user typing forever. Skipped entirely, state included.
            if (IsApplyingHistorySync)
            {
                return;
            }

            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.InternalHandleTypingStopped(eventDto);
            }
        }

        /// <inheritdoc cref="OnTypingStopped"/>
        private void OnTypingStarted(TypingStartEventInternalDTO eventDto)
        {
            if (IsApplyingHistorySync)
            {
                return;
            }

            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.InternalHandleTypingStarted(eventDto);
            }
        }

        private void OnCustomEventReceived(CustomEventInternalDTO eventDto)
        {
            if (_cache.Channels.TryGet(eventDto.Cid, out var streamChannel))
            {
                streamChannel.InternalHandleCustomEvent(eventDto);
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

            lowLevelClient.InternalCustomEventReceived += OnCustomEventReceived;

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

            lowLevelClient.InternalCustomEventReceived -= OnCustomEventReceived;

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