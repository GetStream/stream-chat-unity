﻿using System;
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
using StreamChat.Core.State.Responses;
using StreamChat.Core.State.Caches;

//StreamTodo: maybe some more intuitive namespace? Models? StateModels?
namespace StreamChat.Core.State.TrackedObjects
{
    public delegate void StreamChannelVisibilityHandler(IStreamChannel channel, bool isHidden);

    public delegate void StreamChannelMuteHandler(IStreamChannel channel, bool isMuted);

    public delegate void StreamChannelMessageHandler(IStreamChannel channel, IStreamMessage message);

    public delegate void StreamMessageDeleteHandler(IStreamChannel channel, IStreamMessage message, bool isHardDelete);

    public delegate void StreamChannelChangeHandler(IStreamChannel channel);

    public delegate void StreamChannelUserChangeHandler(IStreamChannel channel, IStreamUser user);

    public delegate void StreamChannelMemberChangeHandler(IStreamChannel channel, IStreamChannelMember member);

    public delegate void StreamMessageReactionHandler(IStreamChannel channel, IStreamMessage message,
        StreamReaction reaction);

    internal sealed class StreamChannel : StreamTrackedObjectBase<StreamChannel>,
        IUpdateableFrom<ChannelStateResponseInternalDTO, StreamChannel>,
        IUpdateableFrom<ChannelResponseInternalDTO, StreamChannel>,
        IUpdateableFrom<ChannelStateResponseFieldsInternalDTO, StreamChannel>,
        IUpdateableFrom<UpdateChannelResponseInternalDTO, StreamChannel>,
        IStreamChannel
    {
        public event StreamChannelMessageHandler MessageReceived;

        public event StreamChannelMessageHandler MessageUpdated;

        public event StreamMessageDeleteHandler MessageDeleted;

        public event StreamChannelMemberChangeHandler MemberAdded;

        public event StreamChannelMemberChangeHandler MemberRemoved;

        public event StreamChannelMemberChangeHandler MemberUpdated;

        public event StreamChannelVisibilityHandler VisibilityChanged;

        public event StreamChannelMuteHandler MuteChanged;

        public event StreamChannelChangeHandler Truncated;

        public event StreamChannelChangeHandler Updated;

        public event StreamChannelUserChangeHandler WatcherAdded;

        public event StreamChannelUserChangeHandler WatcherRemoved;

        public event StreamChannelUserChangeHandler UserStartedTyping;

        public event StreamChannelUserChangeHandler UserStoppedTyping;

        #region Channel

        public bool AutoTranslationEnabled { get; private set; }

        public string AutoTranslationLanguage { get; private set; }

        public string Cid { get; private set; }

        public StreamChannelConfig Config { get; private set; }

        public int? Cooldown { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        public IStreamUser CreatedBy { get; private set; }

        public DateTimeOffset? DeletedAt { get; private set; }

        public bool Disabled { get; private set; }

        public bool Frozen { get; private set; }

        public bool Hidden
        {
            get => _hidden;
            internal set
            {
                if (TrySet(ref _hidden, value))
                {
                    VisibilityChanged?.Invoke(this, Hidden);
                }

                _hidden = value;
            }
        }

        public DateTimeOffset? HideMessagesBefore { get; private set; }

        public string Id { get; private set; }

        public DateTimeOffset? LastMessageAt { get; private set; }

        public int MemberCount { get; private set; }

        public IReadOnlyList<IStreamChannelMember> Members => _members;

        public DateTimeOffset? MuteExpiresAt { get; private set; }

        public bool Muted
        {
            get => _muted;
            internal set
            {
                if (_muted == value)
                {
                    return;
                }

                _muted = value;
                MuteChanged?.Invoke(this, value);
            }
        }

        public IReadOnlyList<string> OwnCapabilities => _ownCapabilities;

        public string Team { get; private set; }

        public DateTimeOffset? TruncatedAt { get; private set; }

        public IStreamUser TruncatedBy { get; private set; }

        public ChannelType Type { get; private set; }

        public DateTimeOffset? UpdatedAt { get; private set; }

        public string Name { get; private set; }

        #endregion

        #region ChannelState

        public IStreamChannelMember Membership { get; private set; }

        public IReadOnlyList<IStreamMessage> Messages => _messages;

        public IReadOnlyList<StreamPendingMessage> PendingMessages => _pendingMessages;

        public IReadOnlyList<IStreamMessage> PinnedMessages => _pinnedMessages;

        public IReadOnlyList<StreamRead> Read => _read;

        public int WatcherCount { get; private set; }

        public IReadOnlyList<IStreamUser> Watchers => _watchers; //StreamTodo: Mention that this is paginatable

        public IReadOnlyList<IStreamUser> TypingUsers => _typingUsers;

        #endregion

        public bool IsDirectMessage =>
            Members.Count == 2 && Members.Any(m => m.User == StreamChatStateClient.LocalUserData.User);

        public Task<IStreamMessage> SendNewMessageAsync(string message)
            => SendNewMessageAsync(new StreamSendMessageRequest
            {
                Text = message
            });

        public async Task<IStreamMessage> SendNewMessageAsync(StreamSendMessageRequest sendMessageRequest)
        {
            if (sendMessageRequest == null)
            {
                throw new ArgumentNullException(nameof(sendMessageRequest));
            }

            // StreamTodo: validate that Text and Mml should not be both set
            var response = await LowLevelClient.InternalMessageApi.SendNewMessageAsync(Type, Id,
                sendMessageRequest.TrySaveToDto());
            var streamMessage = InternalAppendOrUpdateMessage(response.Message);
            return streamMessage;
        }

        public async Task LoadOlderMessagesAsync()
        {
            var oldestMessage = _messages.OrderBy(_ => _.CreatedAt).FirstOrDefault();

            var request = new ChannelGetOrCreateRequestInternalDTO
            {
                Data = null,
                Members = null,
                Messages = new MessagePaginationParamsRequestInternalDTO
                {
                    CreatedAtAfter = null,
                    CreatedAtAfterOrEqual = null,
                    CreatedAtAround = null,
                    CreatedAtBefore = null,
                    CreatedAtBeforeOrEqual = null,
                    IdAround = null,
                    IdGt = null,
                    IdGte = null,
                    IdLt = null,
                    IdLte = null,
                    Limit = null,
                    Offset = null,
                },
                //StreamTodo: presence could be optional in config
                Presence = true,
                State = true,
                Watch = true,
            };

            if (oldestMessage != null)
            {
                request.Messages = new MessagePaginationParamsRequestInternalDTO
                {
                    IdLt = oldestMessage.Id,
                };
            }

            var response = await LowLevelClient.InternalChannelApi.GetOrCreateChannelAsync(Type, Id, request);
            Cache.TryCreateOrUpdate(response);
        }

        //StreamTodo: LoadNewerMessages? This would only make sense if we would start somewhere in the history. Maybe its possible with search? You jump in to past message and scroll to load newer
        public async Task UpdateOverwriteAsync() //StreamTodo: NOT IMPLEMENTED
        {
            var response = await LowLevelClient.InternalChannelApi.UpdateChannelAsync(Type, Id,
                new UpdateChannelRequestInternalDTO
                {
                });

            Cache.TryCreateOrUpdate(response.Channel);
        }

        public async Task UpdatePartialAsync(IDictionary<string, object> setFields,
            IEnumerable<string> unsetFields = null)
        {
            if (setFields == null && unsetFields == null)
            {
                throw new ArgumentNullException(
                    $"{nameof(setFields)} and {nameof(unsetFields)} cannot be both null");
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

        public async Task<StreamFileUploadResponse> UploadFileAsync(byte[] fileContent, string fileName)
        {
            StreamAsserts.AssertNotNullOrEmpty(fileContent, nameof(fileContent));
            StreamAsserts.AssertNotNullOrEmpty(fileName, nameof(fileName));

            var response = await LowLevelClient.InternalMessageApi.UploadFileAsync(Type, Id, fileContent, fileName);
            return new StreamFileUploadResponse(response.File);
        }

        public Task DeleteFileAsync(string fileUrl)
        {
            StreamAsserts.AssertNotNullOrEmpty(fileUrl, nameof(fileUrl));
            return LowLevelClient.InternalMessageApi.DeleteFileAsync(Type, Id, fileUrl);
        }

        public async Task<StreamImageUploadResponse> UploadImageAsync(byte[] imageContent, string imageName)
        {
            StreamAsserts.AssertNotNullOrEmpty(imageContent, nameof(imageContent));
            StreamAsserts.AssertNotNullOrEmpty(imageName, nameof(imageName));

            var response = await LowLevelClient.InternalMessageApi.UploadImageAsync(Type, Id, imageContent, imageName);
            return new StreamImageUploadResponse().LoadFromDto(response, Cache);
        }

        public void QueryMembers() //StreamTodo: IMPLEMENT
        {
        }

        public void QueryWatchers() //StreamTodo: IMPLEMENT
        {
        }

        public Task BanUserFromChannelAsync(IStreamUser user, bool isShadowBan = false, string reason = "",
            int? timeoutMinutes = default, bool isIpBan = false)
        {
            StreamAsserts.AssertNotNull(user, nameof(user));
            StreamAsserts.AssertGreaterThanZero(timeoutMinutes, nameof(timeoutMinutes));

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

        public Task UnbanUserInChannelAsync(IStreamUser user)
        {
            StreamAsserts.AssertNotNull(user, nameof(user));
            return LowLevelClient.InternalModerationApi.UnbanUserAsync(user.Id, Type, Id);
        }

        public Task MarkMessageReadAsync(IStreamMessage message)
        {
            StreamAsserts.AssertNotNull(message, nameof(message));
            if (message.Cid != Cid)
            {
                throw new InvalidOperationException(
                    $"Cid mismatch, expected: {Cid}, given: {message.Cid}. Passed {nameof(message)} does not belong to this channel.");
            }

            return message.MarkMessageAsLastReadAsync();
        }

        //StreamTodo: remove empty request object
        public Task MarkChannelReadAsync()
            => LowLevelClient.InternalChannelApi.MarkReadAsync(Type, Id, new MarkReadRequestInternalDTO());

        //StreamTodo: remove empty request object
        public Task ShowAsync()
            => LowLevelClient.InternalChannelApi.ShowChannelAsync(Type, Id, new ShowChannelRequestInternalDTO()
            {
            });

        public Task HideAsync(bool? clearHistory = false)
            => LowLevelClient.InternalChannelApi.HideChannelAsync(Type, Id, new HideChannelRequestInternalDTO
            {
                ClearHistory = clearHistory
            });

        public async Task AddMembersAsync(IEnumerable<IStreamUser> users)
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

        public Task UnmuteChannelAsync()
            => LowLevelClient.InternalChannelApi.UnmuteChannelAsync(new UnmuteChannelRequestInternalDTO
            {
                ChannelCids = new List<string>
                {
                    Cid
                },
            });

        public async Task TruncateAsync(DateTimeOffset? truncatedAt = default, string systemMessage = "",
            bool skipPushNotifications = false, bool isHardDelete = false)
        {
            var response = await LowLevelClient.InternalChannelApi.TruncateChannelAsync(Type, Id,
                new TruncateChannelRequestInternalDTO
                {
                    HardDelete = isHardDelete,
                    Message = new MessageRequestInternalInternalDTO
                    {
                        Text = systemMessage
                    },
                    SkipPush = skipPushNotifications,
                    TruncatedAt = truncatedAt,
                });
            Cache.TryCreateOrUpdate(response.Channel);
            //StreamTodo: check if we need to add response.Message or was it already contained in response.Channel
        }

        public Task DeleteAsync(bool isHardDelete)
            => LowLevelClient.InternalChannelApi.DeleteChannelAsync(Type, Id, isHardDelete);

        public Task SendTypingStartedEventAsync() =>
            LowLevelClient.InternalChannelApi.SendTypingStartEventAsync(Type, Id);

        public Task SendTypingStoppedEventAsync() =>
            LowLevelClient.InternalChannelApi.SendTypingStopEventAsync(Type, Id);

        internal StreamChannel(string uniqueId, ICacheRepository<StreamChannel> repository,
            ITrackedObjectContext context)
            : base(uniqueId, repository, context)
        {
        }

        void IUpdateableFrom<ChannelStateResponseInternalDTO, StreamChannel>.UpdateFromDto(
            ChannelStateResponseInternalDTO dto, ICache cache)
        {
            UpdateChannelFieldsFromDto(dto.Channel, cache);

            #region ChannelState

            //Hidden = dto.Hidden.GetValueOrDefault(); Updated from Channel
            //HideMessagesBefore = dto.HideMessagesBefore; Updated from Channel
            //_members.TryReplaceTrackedObjects(dto.Members, cache.ChannelMembers); Updated from Channel
            Membership = cache.TryCreateOrUpdate(dto.Membership);
            _messages.TryAppendUniqueTrackedObjects(dto.Messages, cache.Messages);
            _pendingMessages.TryReplaceRegularObjectsFromDto(dto.PendingMessages, cache);
            _pinnedMessages.TryReplaceTrackedObjects(dto.PinnedMessages, cache.Messages);
            _read.TryReplaceRegularObjectsFromDto(dto.Read, cache);
            WatcherCount = GetOrDefault(dto.WatcherCount, WatcherCount);
            _watchers.TryAppendUniqueTrackedObjects(dto.Watchers, cache.Users);

            #endregion

            //StreamTodo probably every UpdateFromDto should trigger Updated event
        }

        void IUpdateableFrom<ChannelStateResponseFieldsInternalDTO, StreamChannel>.UpdateFromDto(
            ChannelStateResponseFieldsInternalDTO dto, ICache cache)
        {
            UpdateChannelFieldsFromDto(dto.Channel, cache);

            #region ChannelState

            //Hidden = dto.Hidden.GetValueOrDefault(); Updated from Channel
            //HideMessagesBefore = dto.HideMessagesBefore; Updated from Channel
            //_members.TryReplaceTrackedObjects(dto.Members, cache.ChannelMembers); Updated from dto.Channel
            Membership = cache.TryCreateOrUpdate(dto.Membership);
            _messages.TryAppendUniqueTrackedObjects(dto.Messages, cache.Messages);
            _pendingMessages.TryReplaceRegularObjectsFromDto(dto.PendingMessages, cache);
            _pinnedMessages.TryReplaceTrackedObjects(dto.PinnedMessages, cache.Messages);
            _read.TryReplaceRegularObjectsFromDto(dto.Read, cache);
            WatcherCount = GetOrDefault(dto.WatcherCount, WatcherCount);
            _watchers.TryAppendUniqueTrackedObjects(dto.Watchers, cache.Users);

            #endregion
        }

        void IUpdateableFrom<ChannelResponseInternalDTO, StreamChannel>.UpdateFromDto(ChannelResponseInternalDTO dto,
            ICache cache)
            => UpdateChannelFieldsFromDto(dto, cache);

        void IUpdateableFrom<UpdateChannelResponseInternalDTO, StreamChannel>.UpdateFromDto(
            UpdateChannelResponseInternalDTO dto, ICache cache)
        {
            UpdateChannelFieldsFromDto(dto.Channel, cache);

            #region ChannelState

            //_members.TryReplaceTrackedObjects(dto.Members, cache.ChannelMembers); //Updated from Channel

            #endregion
        }

        internal void HandleMessageNewEvent(EventMessageNewInternalDTO dto)
        {
            AssertCid(dto.Cid);
            InternalAppendOrUpdateMessage(dto.Message);
            WatcherCount = GetOrDefault(dto.WatcherCount, WatcherCount);
        }

        internal void HandleMessageUpdatedEvent(EventMessageUpdatedInternalDTO dto)
        {
            AssertCid(dto.Cid);
            if (!Cache.Messages.TryGet(dto.Message.Id, out var message))
            {
                return;
            }

            message.TryUpdateFromDto(dto.Message, Cache);
            MessageUpdated?.Invoke(this, message);
        }

        internal void HandleMessageDeletedEvent(EventMessageDeletedInternalDTO dto)
        {
            AssertCid(dto.Cid);
            if (!Cache.Messages.TryGet(dto.Message.Id, out var message))
            {
                return;
            }

            //StreamTodo: consider moving this logic into StreamMessage.HandleMessageDeletedEvent
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

            MessageDeleted?.Invoke(this, message, isHardDelete);
        }

        internal void HandleChannelUpdatedEvent(EventChannelUpdatedInternalDTO eventDto)
        {
            Cache.TryCreateOrUpdate(eventDto.Channel);
            Updated?.Invoke(this);
        }

        internal void HandleChannelTruncatedEvent(EventChannelTruncatedInternalDTO eventDto)
        {
            AssertCid(eventDto.Cid);
            InternalTruncateMessages(eventDto.CreatedAt, eventDto.Message);
        }

        internal void HandleChannelTruncatedEvent(EventNotificationChannelTruncatedInternalDTO eventDto)
        {
            AssertCid(eventDto.Cid);
            InternalTruncateMessages(eventDto.CreatedAt);
        }

        internal void InternalAddMember(StreamChannelMember member)
        {
            if (_members.Contains(member))
            {
                return;
            }

            _members.Add(member);
            MemberAdded?.Invoke(this, member);
        }

        internal void InternalRemoveMember(StreamChannelMember member)
        {
            if (!_members.Contains(member))
            {
                return;
            }

            _members.Remove(member);
            MemberRemoved?.Invoke(this, member);
        }

        internal void InternalUpdateMember(StreamChannelMember member)
        {
            if (!_members.Contains(member))
            {
                _members.Add(member);
            }

            MemberUpdated?.Invoke(this, member);
        }

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

        private bool _muted;
        private bool _hidden;

        private void AssertCid(string cid)
        {
            if (cid != Cid)
            {
                throw new InvalidOperationException($"Cid mismatch, received: `{cid}` but current channel is: {Cid}");
            }
        }

        private StreamMessage InternalAppendOrUpdateMessage(MessageInternalDTO dto)
        {
            var streamMessage = Cache.TryCreateOrUpdate(dto, out var wasCreated);
            if (wasCreated)
            {
                if (!_messages.Contains(streamMessage))
                {
                    _messages.Add(streamMessage);
                    MessageReceived?.Invoke(this, streamMessage);
                }
            }

            return streamMessage;
        }

        private void InternalTruncateMessages(DateTimeOffset? deleteBeforeCreatedAt = null,
            MessageInternalDTO systemMessageDto = null)
        {
            if (deleteBeforeCreatedAt.HasValue)
            {
                for (int i = _messages.Count - 1; i >= 0; i--)
                {
                    var msg = _messages[i];
                    if (msg.CreatedAt < deleteBeforeCreatedAt)
                    {
                        _messages.RemoveAt(i);
                        Cache.Messages.Remove(msg);
                    }
                }
            }
            else
            {
                for (int i = _messages.Count - 1; i >= 0; i--)
                {
                    _messages.RemoveAt(i);
                    Cache.Messages.Remove(_messages[i]);
                }
            }

            if (systemMessageDto != null)
            {
                InternalAppendOrUpdateMessage(systemMessageDto);
            }

            Truncated?.Invoke(this);
        }

        private void UpdateChannelFieldsFromDto(ChannelResponseInternalDTO dto, ICache cache)
        {
            #region Channel

            //StreamTodo: we need to tell if something is purposely null or not available in json, check the nullable ref types or wrap reference type in custom nullable type
            AutoTranslationEnabled = GetOrDefault(dto.AutoTranslationEnabled, AutoTranslationEnabled);
            AutoTranslationLanguage = GetOrDefault(dto.AutoTranslationLanguage, AutoTranslationLanguage);
            Cid = GetOrDefault(dto.Cid, Cid);
            Config = Config.TryLoadFromDto(dto.Config, cache);
            Cooldown = GetOrDefault(dto.Cooldown, Cooldown);
            CreatedAt = GetOrDefault(dto.CreatedAt, CreatedAt);
            CreatedBy = cache.TryCreateOrUpdate(dto.CreatedBy);
            DeletedAt = GetOrDefault(dto.DeletedAt, DeletedAt);
            Disabled = GetOrDefault(dto.Disabled, Disabled);
            Frozen = GetOrDefault(dto.Frozen, Frozen);
            Hidden = dto.Hidden.GetValueOrDefault();
            HideMessagesBefore = GetOrDefault(dto.HideMessagesBefore, HideMessagesBefore);
            Id = GetOrDefault(dto.Id, Id);
            LastMessageAt = GetOrDefault(dto.LastMessageAt, LastMessageAt);
            MemberCount = GetOrDefault(dto.MemberCount, MemberCount);
            _members.TryAppendUniqueTrackedObjects(dto.Members, cache.ChannelMembers);
            MuteExpiresAt = GetOrDefault(dto.MuteExpiresAt, MuteExpiresAt);
            Muted = GetOrDefault(dto.Muted, Muted);
            _ownCapabilities.TryReplaceValuesFromDto(dto.OwnCapabilities);
            Team = GetOrDefault(dto.Team, Team);
            TruncatedAt = GetOrDefault(dto.TruncatedAt, TruncatedAt);
            TruncatedBy = cache.TryCreateOrUpdate(dto.TruncatedBy);
            Type = new ChannelType(GetOrDefault(dto.Type, Type));
            UpdatedAt = GetOrDefault(dto.UpdatedAt, UpdatedAt);

            LoadAdditionalProperties(dto.AdditionalProperties);

            //Not in API spec
            Name = GetOrDefault(dto.Name, Name);

            #endregion
        }

        internal void InternalHandleMessageReadEvent(EventMessageReadInternalDTO eventDto)
        {
            AssertCid(eventDto.Cid);
            //we can only mark messages based on created_at
            //we mark this per user

            var userRead = _read.FirstOrDefault(_ => _.User.Id == eventDto.User.Id);
            if (userRead == null)
            {
                return; //StreamTodo: do we add this user? We don't have his UnreadMessages count
            }

            userRead.Update(eventDto.CreatedAt.Value);
            //StreamTodo: IMPLEMENT we need to recalculate the unread counts and raise some event
        }

        internal void InternalHandleUserWatchingStart(EventUserWatchingStartInternalDTO eventDto)
        {
            AssertCid(eventDto.Cid);

            var user = Cache.TryCreateOrUpdate(eventDto.User, out var wasCreated);
            if (wasCreated || !_watchers.Contains(user))
            {
                WatcherCount += 1;
                _watchers.Add(user);
                WatcherAdded?.Invoke(this, user);
            }
        }

        internal void InternalHandleUserWatchingStop(EventUserWatchingStopInternalDTO eventDto)
        {
            AssertCid(eventDto.Cid);

            //We always reduce because watchers are paginatable so our partial _watchers state may not contain the removed one but count reflects all
            WatcherCount -= 1;

            for (int i = _watchers.Count - 1; i >= 0; i--)
            {
                if (_watchers[i].Id == eventDto.User.Id)
                {
                    var user = Cache.TryCreateOrUpdate(eventDto.User, out var wasCreated);
                    _watchers.RemoveAt(i);
                    WatcherRemoved?.Invoke(this, user);
                    return;
                }
            }
        }

        internal void InternalHandleTypingStopped(EventTypingStopInternalDTO eventDto)
        {
            AssertCid(eventDto.Cid);
            var user = Cache.TryCreateOrUpdate(eventDto.User);
            StreamAsserts.AssertNotNull(user, nameof(user));

            for (int i = _typingUsers.Count - 1; i >= 0; i--)
            {
                if (_typingUsers[i].Id == eventDto.User.Id)
                {
                    _typingUsers.RemoveAt(i);
                    UserStoppedTyping?.Invoke(this, user);
                    return;
                }
            }
        }

        internal void InternalHandleTypingStarted(EventTypingStartInternalDTO eventDto)
        {
            AssertCid(eventDto.Cid);
            var user = Cache.TryCreateOrUpdate(eventDto.User);
            StreamAsserts.AssertNotNull(user, nameof(user));

            if (!_typingUsers.Contains(user))
            {
                _typingUsers.Add(user);
                UserStartedTyping?.Invoke(this, user);
            }
        }

        //StreamTodo: implement some timeout for typing users in case we dont' receive, this could be configurable
        private readonly List<IStreamUser> _typingUsers = new List<IStreamUser>();
    }
}