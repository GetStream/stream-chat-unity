using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.Models;
using StreamChat.Core.State.Models;
using StreamChat.Core.State.Requests;

namespace StreamChat.Core.State.TrackedObjects
{
    // StreamTodo: add IStreamChannel interface
    public interface IStreamChannel
    {
    }

    /// <summary>
    /// Stream channel where a group of <see cref="StreamUser"/>'s can chat
    ///
    /// This object is tracked by <see cref="StreamChatStateClient"/> meaning its state will be automatically updated
    /// </summary>
    public class StreamChannel : StreamTrackedObjectBase<StreamChannel>,
        IUpdateableFrom<ChannelStateResponseInternalDTO, StreamChannel>,
        IUpdateableFrom<ChannelResponseInternalDTO, StreamChannel>,
        IUpdateableFrom<ChannelStateResponseFieldsInternalDTO, StreamChannel>,
        IUpdateableFrom<UpdateChannelResponseInternalDTO, StreamChannel>,
        IStreamChannel
    {
        #region Channel

        /// <summary>
        /// Whether auto translation is enabled or not
        /// </summary>
        public bool? AutoTranslationEnabled { get; private set; }

        /// <summary>
        /// Language to translate to when auto translation is active
        /// </summary>
        public string AutoTranslationLanguage { get; private set; }

        /// <summary>
        /// Channel CID (type:id)
        /// </summary>
        public string Cid { get; private set; }

        /// <summary>
        /// Channel configuration
        /// </summary>
        public StreamChannelConfig Config { get; private set; }

        /// <summary>
        /// Cooldown period after sending each message
        /// </summary>
        public int? Cooldown { get; private set; }

        /// <summary>
        /// Date/time of creation
        /// </summary>
        public DateTimeOffset? CreatedAt { get; private set; }

        /// <summary>
        /// Creator of the channel
        /// </summary>
        public StreamUser CreatedBy { get; private set; }

        /// <summary>
        /// Date/time of deletion
        /// </summary>
        public DateTimeOffset? DeletedAt { get; private set; }

        public bool? Disabled { get; private set; }

        /// <summary>
        /// Whether channel is frozen or not
        /// </summary>
        public bool? Frozen { get; private set; }

        /// <summary>
        /// Whether this channel is hidden by current user or not
        /// </summary>
        public bool? Hidden { get; private set; }

        /// <summary>
        /// Date since when the message history is accessible
        /// </summary>
        public DateTimeOffset? HideMessagesBefore { get; private set; }

        /// <summary>
        /// Channel unique ID
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Date of the last message sent
        /// </summary>
        public DateTimeOffset? LastMessageAt { get; private set; }

        /// <summary>
        /// Number of members in the channel
        /// </summary>
        public int? MemberCount { get; private set; }

        /// <summary>
        /// List of channel members (max 100)
        /// </summary>
        public IReadOnlyList<StreamChannelMember> Members => _members;

        /// <summary>
        /// Date of mute expiration
        /// </summary>
        public DateTimeOffset? MuteExpiresAt { get; private set; }

        /// <summary>
        /// Whether this channel is muted or not
        /// </summary>
        public bool? Muted { get; private set; }

        /// <summary>
        /// List of channel capabilities of authenticated user
        /// </summary>
        public IReadOnlyList<string> OwnCapabilities => _ownCapabilities;

        /// <summary>
        /// Team the channel belongs to (multi-tenant only)
        /// </summary>
        public string Team { get; private set; }

        public DateTimeOffset? TruncatedAt { get; private set; }

        public StreamUser TruncatedBy { get; private set; }

        /// <summary>
        /// Type of the channel
        /// </summary>
        public string Type { get; private set; } //StreamTodo replace with ChannelType custom type

        /// <summary>
        /// Date/time of the last update
        /// </summary>
        public DateTimeOffset? UpdatedAt { get; private set; }

        public string Name { get; private set; } //StreamTodo: if custom name is not set should we return the ID?

        #endregion

        #region ChannelState

        /// <summary>
        /// Current user membership object
        /// </summary>
        public StreamChannelMember Membership { get; private set; }

        /// <summary>
        /// List of channel messages
        /// </summary>
        public IReadOnlyList<StreamMessage> Messages => _messages;

        /// <summary>
        /// Pending messages that this user has sent
        /// </summary>
        public IReadOnlyList<StreamPendingMessage> PendingMessages => _pendingMessages;

        /// <summary>
        /// List of pinned messages in the channel
        /// </summary>
        public IReadOnlyList<StreamMessage> PinnedMessages => _pinnedMessages;

        /// <summary>
        /// List of read states
        /// </summary>
        public IReadOnlyList<StreamRead> Read => _read;

        /// <summary>
        /// Number of channel watchers
        /// </summary>
        public int? WatcherCount { get; private set; }

        /// <summary>
        /// List of user who is watching the channel
        /// </summary>
        public IReadOnlyList<StreamUser> Watchers => _watchers;

        #endregion

        /// <summary>
        /// Basic send message method. If you want to set additional parameters like use the other <see cref="SendNewMessageAsync(StreamSendMessageRequest requestBody)"/> overload
        /// </summary>
        public Task<StreamMessage> SendNewMessageAsync(string message)
            => SendNewMessageAsync(new StreamSendMessageRequest
            {
                Text = message
            });

        /// <summary>
        /// Advanced send message method. Check out the <see cref="StreamSendMessageRequest"/> to see all of the parameters
        /// </summary>
        public async Task<StreamMessage> SendNewMessageAsync(StreamSendMessageRequest sendMessageRequest)
        {
            if (sendMessageRequest == null)
            {
                throw new ArgumentNullException(nameof(sendMessageRequest));
            }

            // StreamTodo: validate that Text and Mml should not be both set
            var response = await LowLevelClient.InternalMessageApi.SendNewMessageAsync(Type, Id,
                sendMessageRequest.TrySaveToDto());
            var streamMessage = AppendOrUpdateMessage(response.Message);
            return streamMessage;
        }

        /// <summary>
        /// Update channel in a complete overwrite mode.
        /// Important! Any data that is present on the channel and not included in a full update will be deleted.
        ///
        /// If you want to update only some fields of the channel use the <see cref="UpdatePartialAsync"/>
        /// </summary>
        public async Task UpdateOverwriteAsync() //StreamTodo: NOT IMPLEMENTED
        {
            var response = await LowLevelClient.InternalChannelApi.UpdateChannelAsync(Type, Id,
                new UpdateChannelRequestInternalDTO
                {
                });

            Cache.TryCreateOrUpdate(response.Channel);
        }

        /// <summary>
        /// Update channel in a partial mode. You can selectively set and unset fields of the channel
        ///
        /// If you want to completely overwrite the channel use the <see cref="UpdateOverwriteAsync"/>
        /// </summary>
        /// StreamTodo: this should be more high level, maybe use enum with predefined field names?
        public async Task UpdatePartialAsync(IDictionary<string, object> setFields,
            IEnumerable<string> unsetFields = null)
        {
            if (setFields == null && unsetFields == null)
            {
                throw new ArgumentNullException(
                    $"Either {nameof(setFields)} or {nameof(unsetFields)} must not be null");
            }

            if (unsetFields != null && !unsetFields.Any())
            {
                throw new ArgumentException($"{nameof(unsetFields)} cannot be empty");
            }

            if (setFields != null && !setFields.Any())
            {
                throw new ArgumentException($"{nameof(setFields)} cannot be empty");
            }

            var response = await LowLevelClient.InternalChannelApi.UpdateChannelPartialAsync(Type, Id,
                new UpdateChannelPartialRequestInternalDTO
                {
                    Set = setFields?.ToDictionary(p => p.Key, p => p.Value),
                    Unset = unsetFields?.ToList(),
                });

            Cache.TryCreateOrUpdate(response.Channel);
        }

        public void QueryMembers()
        {
        }

        public void QueryWatchers()
        {
        }

        /// <summary>
        /// Ban user from this channel
        /// </summary>
        /// <param name="user">User to ban from channel</param>
        /// <param name="isShadowBan">Shadow banned user is not notified about the ban. Read more: <remarks>https://getstream.io/chat/docs/unity/moderation/?language=unity#shadow-ban</remarks></param>
        /// <param name="reason">[Optional] reason description why user got banned</param>
        /// <param name="timeoutMinutes">[Optional] timeout in minutes after which ban is automatically expired</param>
        /// <param name="isIpBan">[Optional] Should ban apply to user's IP address</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Task BanUserAsync(StreamUser user, bool isShadowBan, string reason = "", int? timeoutMinutes = default,
            bool isIpBan = false)
        {
            if (user == null)
            {
                throw new ArgumentNullException($"{nameof(user)} cannot be null");
            }

            if (timeoutMinutes <= 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(timeoutMinutes)} must be greater than 0");
            }

            return LowLevelClient.InternalModerationApi.BanUserAsync(new BanRequestInternalDTO
            {
                //BannedBy = null,
                //BannedById = null,
                Id = Id,
                IpBan = isIpBan,
                Reason = reason,
                Shadow = isShadowBan,
                TargetUserId = user.Id,
                Timeout = timeoutMinutes,
                Type = Type,
                //User = null,
                //UserId = null,
                //AdditionalProperties = null
            });
        }

        /// <summary>
        /// Remove ban from the user on this channel
        /// </summary>
        public Task UnbanUserAsync(StreamUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException($"{nameof(user)} cannot be null");
            }

            return LowLevelClient.InternalModerationApi.UnbanUserAsync(user.Id, Type, Id);
        }

        public void ShadowBanUser()
        {
        }

        public void RemoveShadowBan()
        {
        }

        /// <summary>
        /// Mark this message as the last that was read by this user in this channel
        /// If you want to mark whole channel as read use the <see cref="MarkChannelReadAsync"/>
        ///
        /// This feature allows to track to which message users have read the channel
        /// </summary>
        public Task MarkMessageReadAsync(StreamMessage message)
        {
            StreamAsserts.AssertNotNull(message, nameof(message));
            if (message.Cid != Cid)
            {
                throw new InvalidOperationException(
                    $"Cid mismatch, expected: {Cid}, given: {message.Cid}. Passed {nameof(message)} does not belong to this channel.");
            }

            return message.MarkMessageReadAsync();
        }

        //StreamTodo: remove empty request object
        /// <summary>
        /// Mark this channel completely as read
        /// If you want to mark specific message as read use the <see cref="MarkMessageReadAsync"/>
        ///
        /// This feature allows to track to which message users have read the channel
        /// </summary>
        public Task MarkChannelReadAsync()
            => LowLevelClient.InternalChannelApi.MarkReadAsync(Type, Id, new MarkReadRequestInternalDTO());

        //StreamTodo: remove empty request object
        /// <summary>
        /// <para>Shows a previously hidden channel.</para>
        /// Use <see cref="HideAsync"/> to hide a channel.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/muting_channels/?language=unity</remarks>
        public Task ShowAsync()
            => LowLevelClient.InternalChannelApi.ShowChannelAsync(Type, Id, new ShowChannelRequestInternalDTO()
            {
            });

        /// <summary>
        /// <para>Removes a channel from query channel requests for that user until a new message is added.</para>
        /// Use <see cref="ShowAsync"/> to cancel this operation.
        /// </summary>
        /// <param name="clearHistory">Whether to clear message history of the channel or not</param>
        /// <remarks>https://getstream.io/chat/docs/unity/muting_channels/?language=unity</remarks>
        public Task HideAsync(bool? clearHistory = false)
            => LowLevelClient.InternalChannelApi.HideChannelAsync(Type, Id, new HideChannelRequestInternalDTO
            {
                ClearHistory = clearHistory
            });

        public async Task AddMembersAsync(IEnumerable<StreamUser> users)
        {
            StreamAsserts.AssertNotNull(users, nameof(users));

            var membersRequest = new List<ChannelMemberRequestInternalDTO>();
            foreach (var u in users)
            {
                membersRequest.Add(new ChannelMemberRequestInternalDTO
                {
                    UserId = u.Id,
                });
            }

            var response = await LowLevelClient.InternalChannelApi.UpdateChannelAsync(Type, Id,
                new UpdateChannelRequestInternalDTO
                {
                    AddMembers = membersRequest
                });
            Cache.TryCreateOrUpdate(response);
        }

        public async Task RemoveMembersAsync(IEnumerable<ChannelMember> members)
        {
            StreamAsserts.AssertNotNull(members, nameof(members));

            var response = await LowLevelClient.InternalChannelApi.UpdateChannelAsync(Type, Id,
                new UpdateChannelRequestInternalDTO
                {
                    RemoveMembers = members.Select(_ => _.UserId).ToList()
                });
            Cache.TryCreateOrUpdate(response);
        }

        /// <summary>
        /// Mute channel with optional duration in milliseconds
        /// </summary>
        /// <param name="milliseconds">[Optional] Duration in milliseconds</param>
        public async Task MuteChannelAsync(int? milliseconds = default)
        {
            var response = await LowLevelClient.InternalChannelApi.MuteChannelAsync(new MuteChannelRequestInternalDTO
            {
                ChannelCids = new List<string>
                {
                    Cid
                },
                Expiration = milliseconds,
            });

            StreamChatStateClient.UpdateLocalUser(response.OwnUser);
        }

        /// <summary>
        /// Unmute channel
        /// </summary>
        public Task UnmuteChannelAsync()
            => LowLevelClient.InternalChannelApi.UnmuteChannelAsync(new UnmuteChannelRequestInternalDTO
            {
                ChannelCids = new List<string>
                {
                    Cid
                },
            });


        //StreamTodo: write integration tests to check exactly how it works
        public async Task TruncateAsync(DateTimeOffset truncatedAt, string message = "", bool skipPushNotifications = false, bool isHardDelete = false)
        {
            var response = await LowLevelClient.InternalChannelApi.TruncateChannelAsync(Type, Id, new TruncateChannelRequestInternalDTO
            {
                HardDelete = isHardDelete,
                Message = new MessageRequestInternalInternalDTO
                {
                    Text = message
                },
                SkipPush = skipPushNotifications,
                TruncatedAt = truncatedAt,
            });
        }

        /// <summary>
        /// Delete this channel. By default channel is soft deleted. You may hard delete it by setting the <param name="isHardDelete"> argument to true
        /// </summary>
        /// <param name="isHardDelete">Hard delete completely removes channel with all its resources</param>
        /// <remarks>https://getstream.io/chat/docs/unity/channel_delete/?language=unity</remarks>
        public async Task DeleteAsync(bool isHardDelete) => LowLevelClient.ChannelApi.DeleteChannelAsync(Type, Id, isHardDelete);

        public Task NotifyTypingStarted() => LowLevelClient.InternalChannelApi.SendTypingStartEventAsync(Type, Id);

        public Task NotifyTypingStopped() => LowLevelClient.InternalChannelApi.SendTypingStopEventAsync(Type, Id);

        internal StreamChannel(string uniqueId, IRepository<StreamChannel> repository, ITrackedObjectContext context)
            : base(uniqueId, repository, context)
        {
        }

        void IUpdateableFrom<ChannelStateResponseInternalDTO, StreamChannel>.UpdateFromDto(
            ChannelStateResponseInternalDTO dto, ICache cache)
        {
            #region Channel

            AutoTranslationEnabled = dto.Channel.AutoTranslationEnabled;
            AutoTranslationLanguage = dto.Channel.AutoTranslationLanguage;
            Cid = dto.Channel.Cid;
            Config = Config.TryLoadFromDto(dto.Channel.Config, cache);
            Cooldown = dto.Channel.Cooldown;
            CreatedAt = dto.Channel.CreatedAt;
            CreatedBy = cache.TryCreateOrUpdate(dto.Channel.CreatedBy);
            DeletedAt = dto.Channel.DeletedAt;
            Disabled = dto.Channel.Disabled;
            Frozen = dto.Channel.Frozen;
            Hidden = dto.Channel.Hidden;
            HideMessagesBefore = dto.Channel.HideMessagesBefore;
            Id = dto.Channel.Id;
            LastMessageAt = dto.Channel.LastMessageAt;
            MemberCount = dto.Channel.MemberCount;
            // dto.Channel.Members handled below
            MuteExpiresAt = dto.Channel.MuteExpiresAt;
            Muted = dto.Channel.Muted;
            _ownCapabilities.TryReplaceValuesFromDto(dto.Channel.OwnCapabilities);
            Team = dto.Channel.Team;
            TruncatedAt = dto.Channel.TruncatedAt;
            TruncatedBy = cache.TryCreateOrUpdate(dto.Channel.TruncatedBy);
            Type = dto.Channel.Type;
            UpdatedAt = dto.Channel.UpdatedAt;

            LoadAdditionalProperties(dto.Channel.AdditionalProperties);

            //Not in API spec
            Name = dto.Channel.Name;

            #endregion

            #region ChannelState

            Hidden = dto.Hidden;
            HideMessagesBefore = dto.HideMessagesBefore;
            _members.TryReplaceTrackedObjects(dto.Members, cache.ChannelMembers);
            Membership = cache.TryCreateOrUpdate(dto.Membership);
            _messages.TryReplaceTrackedObjects(dto.Messages, cache.Messages);
            _pendingMessages.TryReplaceRegularObjectsFromDto(dto.PendingMessages, cache);
            _pinnedMessages.TryReplaceTrackedObjects(dto.PinnedMessages, cache.Messages);
            _read.TryReplaceRegularObjectsFromDto(dto.Read, cache);
            WatcherCount = dto.WatcherCount;
            _watchers.TryReplaceTrackedObjects(dto.Watchers, cache.Users);

            #endregion
        }

        void IUpdateableFrom<ChannelStateResponseFieldsInternalDTO, StreamChannel>.UpdateFromDto(
            ChannelStateResponseFieldsInternalDTO dto, ICache cache)
        {
            #region Channel

            AutoTranslationEnabled = dto.Channel.AutoTranslationEnabled;
            AutoTranslationLanguage = dto.Channel.AutoTranslationLanguage;
            Cid = dto.Channel.Cid;
            Config = Config.TryLoadFromDto(dto.Channel.Config, cache);
            Cooldown = dto.Channel.Cooldown;
            CreatedAt = dto.Channel.CreatedAt;
            CreatedBy = cache.TryCreateOrUpdate(dto.Channel.CreatedBy);
            DeletedAt = dto.Channel.DeletedAt;
            Disabled = dto.Channel.Disabled;
            Frozen = dto.Channel.Frozen;
            Hidden = dto.Channel.Hidden;
            HideMessagesBefore = dto.Channel.HideMessagesBefore;
            Id = dto.Channel.Id;
            LastMessageAt = dto.Channel.LastMessageAt;
            MemberCount = dto.Channel.MemberCount;
            // dto.Channel.Members handled below
            MuteExpiresAt = dto.Channel.MuteExpiresAt;
            Muted = dto.Channel.Muted;
            _ownCapabilities.TryReplaceValuesFromDto(dto.Channel.OwnCapabilities);
            Team = dto.Channel.Team;
            TruncatedAt = dto.Channel.TruncatedAt;
            TruncatedBy = cache.TryCreateOrUpdate(dto.Channel.TruncatedBy);
            Type = dto.Channel.Type;
            UpdatedAt = dto.Channel.UpdatedAt;

            LoadAdditionalProperties(dto.Channel.AdditionalProperties);

            //Not in API spec
            Name = dto.Channel.Name;

            #endregion

            #region ChannelState

            Hidden = dto.Hidden;
            HideMessagesBefore = dto.HideMessagesBefore;
            _members.TryReplaceTrackedObjects(dto.Members, cache.ChannelMembers);
            Membership = cache.TryCreateOrUpdate(dto.Membership);
            _messages.TryReplaceTrackedObjects(dto.Messages, cache.Messages);
            _pendingMessages.TryReplaceRegularObjectsFromDto(dto.PendingMessages, cache);
            _pinnedMessages.TryReplaceTrackedObjects(dto.PinnedMessages, cache.Messages);
            _read.TryReplaceRegularObjectsFromDto(dto.Read, cache);
            WatcherCount = dto.WatcherCount;
            _watchers.TryReplaceTrackedObjects(dto.Watchers, cache.Users);

            #endregion
        }

        void IUpdateableFrom<ChannelResponseInternalDTO, StreamChannel>.UpdateFromDto(ChannelResponseInternalDTO dto,
            ICache cache)
        {
            #region Channel

            AutoTranslationEnabled = dto.AutoTranslationEnabled;
            AutoTranslationLanguage = dto.AutoTranslationLanguage;
            Cid = dto.Cid;
            Config = Config.TryLoadFromDto(dto.Config, cache);
            Cooldown = dto.Cooldown;
            CreatedAt = dto.CreatedAt;
            CreatedBy = cache.TryCreateOrUpdate(dto.CreatedBy);
            DeletedAt = dto.DeletedAt;
            Disabled = dto.Disabled;
            Frozen = dto.Frozen;
            Hidden = dto.Hidden;
            HideMessagesBefore = dto.HideMessagesBefore;
            Id = dto.Id;
            LastMessageAt = dto.LastMessageAt;
            MemberCount = dto.MemberCount;
            _members.TryReplaceTrackedObjects(dto.Members, cache.ChannelMembers);
            MuteExpiresAt = dto.MuteExpiresAt;
            Muted = dto.Muted;
            _ownCapabilities.TryReplaceValuesFromDto(dto.OwnCapabilities);
            Team = dto.Team;
            TruncatedAt = dto.TruncatedAt;
            TruncatedBy = cache.TryCreateOrUpdate(dto.TruncatedBy);
            Type = dto.Type;
            UpdatedAt = dto.UpdatedAt;

            LoadAdditionalProperties(dto.AdditionalProperties);

            //Not in API spec
            Name = dto.Name;

            #endregion
        }

        void IUpdateableFrom<UpdateChannelResponseInternalDTO, StreamChannel>.UpdateFromDto(
            UpdateChannelResponseInternalDTO dto, ICache cache)
        {
            #region Channel

            AutoTranslationEnabled = dto.Channel.AutoTranslationEnabled;
            AutoTranslationLanguage = dto.Channel.AutoTranslationLanguage;
            Cid = dto.Channel.Cid;
            Config = Config.TryLoadFromDto(dto.Channel.Config, cache);
            Cooldown = dto.Channel.Cooldown;
            CreatedAt = dto.Channel.CreatedAt;
            CreatedBy = cache.TryCreateOrUpdate(dto.Channel.CreatedBy);
            DeletedAt = dto.Channel.DeletedAt;
            Disabled = dto.Channel.Disabled;
            Frozen = dto.Channel.Frozen;
            Hidden = dto.Channel.Hidden;
            HideMessagesBefore = dto.Channel.HideMessagesBefore;
            Id = dto.Channel.Id;
            LastMessageAt = dto.Channel.LastMessageAt;
            MemberCount = dto.Channel.MemberCount;
            // dto.Channel.Members handled below
            MuteExpiresAt = dto.Channel.MuteExpiresAt;
            Muted = dto.Channel.Muted;
            _ownCapabilities.TryReplaceValuesFromDto(dto.Channel.OwnCapabilities);
            Team = dto.Channel.Team;
            TruncatedAt = dto.Channel.TruncatedAt;
            TruncatedBy = cache.TryCreateOrUpdate(dto.Channel.TruncatedBy);
            Type = dto.Channel.Type;
            UpdatedAt = dto.Channel.UpdatedAt;

            LoadAdditionalProperties(dto.Channel.AdditionalProperties);

            //Not in API spec
            Name = dto.Channel.Name;

            #endregion

            #region ChannelState

            //StreamTodo: will this members list always contain all of them??
            _members.TryReplaceTrackedObjects(dto.Members, cache.ChannelMembers);

            #endregion
        }

        internal void HandleMessageNewEvent(EventMessageNewInternalDTO dto)
        {
            AssertCid(dto.Cid);
            AppendOrUpdateMessage(dto.Message);
        }

        internal void HandleMessageUpdatedEvent(EventMessageUpdatedInternalDTO dto)
        {
            AssertCid(dto.Cid);
            if (!Cache.Messages.TryGet(dto.Message.Id, out var message))
            {
                return;
            }

            message.TryUpdateFromDto(dto.Message, Cache);
        }

        //StreamTodo: consider using structs for MessageId, ChannelId, etc. this way we control in 1 place from which fields they are created + there will be no mistake on user
        internal void HandleMessageDeletedEvent(EventMessageDeletedInternalDTO dto)
        {
            AssertCid(dto.Cid);
            if (!Cache.Messages.TryGet(dto.Message.Id, out var message))
            {
                return;
            }

            //StramTodo: consider moving this logic into StreamMessage.HandleMessageDeletedEvent
            var isHardDelete = dto.HardDelete.GetValueOrDefault(false);
            if (isHardDelete)
            {
                Cache.Messages.Remove(message);
                _messages.Remove(message);
            }
            else
            {
                message.InternalHandleSoftDelete();
            }
        }

        internal void SetMuted(bool isMuted) => Muted = isMuted;

        protected override StreamChannel Self => this;

        protected override string InternalUniqueId
        {
            get => Cid;
            set => Cid = value;
        }

        private readonly List<StreamChannelMember> _members = new List<StreamChannelMember>();
        private readonly List<StreamMessage> _messages = new List<StreamMessage>();
        private readonly List<StreamMessage> _pinnedMessages = new List<StreamMessage>();
        private readonly List<StreamUser> _watchers = new List<StreamUser>();
        private readonly List<StreamRead> _read = new List<StreamRead>();
        private readonly List<string> _ownCapabilities = new List<string>();
        private readonly List<StreamPendingMessage> _pendingMessages = new List<StreamPendingMessage>();

        private void AssertCid(string cid)
        {
            if (cid != Cid)
            {
                throw new InvalidOperationException($"Cid mismatch, received: `{cid}` but current channel is: {Cid}");
            }
        }

        private StreamMessage AppendOrUpdateMessage(MessageInternalDTO dto)
        {
            var streamMessage = Cache.TryCreateOrUpdate(dto, out var wasCreated);
            if (wasCreated)
            {
                if (!_messages.Contains(streamMessage))
                {
                    _messages.Add(streamMessage);
                }
            }

            return streamMessage;
        }
    }
}