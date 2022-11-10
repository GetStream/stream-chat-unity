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
using StreamChat.Core.State.Responses;
using StreamChat.Core.State.Caches;

namespace StreamChat.Core.State.TrackedObjects //StreamTodo: maybe some more intuitive namespace? Models? StateModels?
{
    //StreamTodo: rename all to add Stream prefix
    public delegate void ChannelVisibilityHandler(IStreamChannel channel, bool isHidden);

    public delegate void ChannelMuteHandler(IStreamChannel channel, bool isMuted);

    public delegate void ChannelMessageHandler(IStreamChannel channel, StreamMessage message);

    public delegate void MessageDeleteHandler(IStreamChannel channel, StreamMessage message, bool isHardDelete);

    public delegate void ChannelChangeHandler(IStreamChannel channel);

    public delegate void ChannelUserChangeHandler(IStreamChannel channel, StreamUser user);

    public delegate void ChannelMemberChangeHandler(IStreamChannel channel, StreamChannelMember member);

    /// <summary>
    /// Channel is where group of <see cref="StreamChannelMember"/> can chat with each other.
    /// Depending on <see cref="StreamChannel.Type"/>
    /// </summary>
    /// <remarks>https://getstream.io/chat/docs/unity/permissions_reference/?language=unity&q=hidden#default-grants</remarks>
    internal sealed class StreamChannel : StreamTrackedObjectBase<StreamChannel>,
        IUpdateableFrom<ChannelStateResponseInternalDTO, StreamChannel>,
        IUpdateableFrom<ChannelResponseInternalDTO, StreamChannel>,
        IUpdateableFrom<ChannelStateResponseFieldsInternalDTO, StreamChannel>,
        IUpdateableFrom<UpdateChannelResponseInternalDTO, StreamChannel>,
        IStreamChannel
    {
        /// <summary>
        /// Event fired when a new <see cref="StreamMessage"/> was received on this channel
        /// </summary>
        public event ChannelMessageHandler MessageReceived;
        
        /// <summary>
        /// Event fired when a <see cref="StreamMessage"/> from this channel was updated
        /// </summary>
        public event ChannelMessageHandler MessageUpdated;
        
        /// <summary>
        /// Event fired when a <see cref="StreamMessage"/> from this channel was deleted
        /// </summary>
        public event MessageDeleteHandler MessageDeleted;
        
        /// <summary>
        /// Event fired when a new <see cref="StreamChannelMember"/> joined this channel
        /// </summary>
        public event ChannelMemberChangeHandler MemberAdded;
        
        /// <summary>
        /// Event fired when a <see cref="StreamChannelMember"/> left this channel
        /// </summary>
        public event ChannelMemberChangeHandler MemberRemoved;
        
        /// <summary>
        /// Event fired when a <see cref="StreamChannelMember"/> was updated
        /// </summary>
        public event ChannelMemberChangeHandler MemberUpdated;
        
        /// <summary>
        /// Event fired when visibility of this channel changed. Check <see cref="Hidden"/> to know if channel is hidden
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/muting_channels/?language=unity&q=hidden#hiding-a-channel</remarks>
        public event ChannelVisibilityHandler VisibilityChanged;
        
        /// <summary>
        /// Event fired when channel got muted on unmuted. Check <see cref="Muted"/> and <see cref="MuteExpiresAt"/> to know if channel is muted
        /// </summary>
        public event ChannelMuteHandler MuteChanged;
        
        /// <summary>
        /// Event fired when this channel was truncated meaning that all or part of the messages where removed
        /// </summary>
        public event ChannelChangeHandler Truncated;
        
        /// <summary>
        /// Event fired when this channel data was updated
        /// </summary>
        public event ChannelChangeHandler Updated;
        
        /// <summary>
        /// Event fired when a <see cref="StreamUser"/> started watching this channel
        /// See also <see cref="see cref="WatcherCount"/>"/> and <see cref="Watchers"/>
        /// </summary>
        public event ChannelUserChangeHandler WatcherAdded;

        /// <summary>
        /// Event fired when a <see cref="StreamUser"/> stopped watching this channel
        /// See also <see cref="see cref="WatcherCount"/>"/> and <see cref="Watchers"/>
        /// </summary>
        public event ChannelUserChangeHandler WatcherRemoved;
        
        /// <summary>
        /// Event fired when a <see cref="StreamUser"/> in this channel starts typing
        /// </summary>
        public event ChannelUserChangeHandler UserStartedTyping;

        /// <summary>
        /// Event fired when a <see cref="StreamUser"/> in this channel stops typing
        /// </summary>
        public event ChannelUserChangeHandler UserStoppedTyping;

        #region Channel

        /// <summary>
        /// Whether auto translation is enabled or not
        /// </summary>
        public bool AutoTranslationEnabled { get; private set; }

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
        public DateTimeOffset CreatedAt { get; private set; }

        /// <summary>
        /// Creator of the channel
        /// </summary>
        public StreamUser CreatedBy { get; private set; }

        /// <summary>
        /// Date/time of deletion
        /// </summary>
        public DateTimeOffset? DeletedAt { get; private set; }

        public bool Disabled { get; private set; }

        /// <summary>
        /// Whether channel is frozen or not
        /// </summary>
        public bool Frozen { get; private set; }

        /// <summary>
        /// Whether this channel is hidden by current user or not. Subscribe to <see cref="VisibilityChanged"/> to get notified when this property changes
        /// </summary>
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
        public int MemberCount { get; private set; }

        /// <summary>
        /// List of channel members (max 100)
        /// </summary>
        public IReadOnlyList<StreamChannelMember> Members => _members;

        /// <summary>
        /// Date of mute expiration
        /// </summary>
        public DateTimeOffset? MuteExpiresAt { get; private set; }

        /// <summary>
        /// Whether this channel is muted or not. Subscribe to <see cref="MuteChanged"/> to get notified when this property changes
        /// </summary>
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
        public ChannelType Type { get; private set; }

        /// <summary>
        /// Date/time of the last update
        /// </summary>
        public DateTimeOffset? UpdatedAt { get; private set; }

        public string Name { get; private set; }

        #endregion

        #region ChannelState

        /// <summary>
        /// Current user membership object
        /// </summary>
        public StreamChannelMember Membership { get; private set; }

        /// <summary>
        /// List of channel messages. By default only latest messages are loaded. If you wish to load older messages user the <see cref="LoadOlderMessagesAsync"/>
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
        public int WatcherCount { get; private set; }

        /// <summary>
        /// List of user who is watching the channel
        /// Subscribe to <see cref="WatcherAdded"/> and <see cref="WatcherRemoved"/> events to know when this list changes.
        /// </summary>
        public IReadOnlyList<StreamUser> Watchers => _watchers; //StreamTodo: Mention that this is paginatable

        /// <summary>
        /// List of currently typing users.
        /// Subscribe to <see cref="UserStartedTyping"/> and <see cref="UserStoppedTyping"/> events to know when this list changes.
        /// </summary>
        public IReadOnlyList<StreamUser> TypingUsers => _typingUsers;

        #endregion
        
        /// <summary>
        /// Is this a direct message channel between the local and some other user
        /// </summary>
        public bool IsDirectMessage =>
            Members.Count == 2 && Members.Any(m => m.User == StreamChatStateClient.LocalUserData.User);

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
                Presence = true, //StreamTodo: presence could be optional in config
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

        /// <summary>
        /// Upload file to stream CDN. Returned file URL can be used as a message attachment.
        /// For image files use <see cref="UploadImageAsync"/> as it will generate the thumbnail and allow for image resize and crop operations
        /// </summary>
        /// <param name="fileContent">File bytes content (e.g. returned from <see cref="System.IO.File.ReadAllBytes"/></param>
        /// <param name="fileName">Name of the file</param>
        /// <remarks>https://getstream.io/chat/docs/unity/file_uploads/?language=unity</remarks>
        public async Task<StreamFileUploadResponse> UploadFileAsync(byte[] fileContent, string fileName)
        {
            StreamAsserts.AssertNotNullOrEmpty(fileContent, nameof(fileContent));
            StreamAsserts.AssertNotNullOrEmpty(fileName, nameof(fileName));

            var response = await LowLevelClient.InternalMessageApi.UploadFileAsync(Type, Id, fileContent, fileName);
            return new StreamFileUploadResponse(response.File);
        }

        /// <summary>
        /// Delete file of any type that was send to Stream CDN.
        /// This handles both files sent via <see cref="UploadFileAsync"/> and images sent via <see cref="UploadImageAsync"/>
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/file_uploads/?language=unity#deleting-files-and-images</remarks>
        public Task DeleteFileAsync(string fileUrl)
        {
            StreamAsserts.AssertNotNullOrEmpty(fileUrl, nameof(fileUrl));
            return LowLevelClient.InternalMessageApi.DeleteFileAsync(Type, Id, fileUrl);
        }

        /// <summary>
        /// Upload image file to stream CDN. The returned image URL can be injected into <see cref="StreamAttachmentRequest"/> when sending new message.
        /// </summary>
        /// <param name="imageContent"></param>
        /// <param name="imageName"></param>
        /// <remarks>https://getstream.io/chat/docs/unity/file_uploads/?language=unity#how-to-upload-a-file-or-image</remarks>
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
        
        /// <summary>
        /// Ban user from this channel.
        /// If you wish to ban user completely from all of the channels, this can be done only by server-side SDKs.
        /// </summary>
        /// <param name="user">User to ban from channel</param>
        /// <param name="isShadowBan">Shadow banned user is not notified about the ban. Read more: <remarks>https://getstream.io/chat/docs/unity/moderation/?language=unity#shadow-ban</remarks></param>
        /// <param name="reason">[Optional] reason description why user got banned</param>
        /// <param name="timeoutMinutes">[Optional] timeout in minutes after which ban is automatically expired</param>
        /// <param name="isIpBan">[Optional] Should ban apply to user's IP address</param>
        /// <remarks>https://getstream.io/chat/docs/unity/moderation/?language=unity#ban</remarks>
        public Task BanUserFromChannelAsync(StreamUser user, bool isShadowBan = false, string reason = "",
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

        /// <summary>
        /// Remove ban from the user on this channel
        /// </summary>
        public Task UnbanUserInChannelAsync(StreamUser user)
        {
            StreamAsserts.AssertNotNull(user, nameof(user));
            return LowLevelClient.InternalModerationApi.UnbanUserAsync(user.Id, Type, Id);
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

            return message.MarkMessageAsLastReadAsync();
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

        /// <summary>
        /// Add users as members to this channel
        /// </summary>
        /// <param name="users">Users to become members of this channel</param>
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

        /// <summary>
        /// Remove members from this channel
        /// </summary>
        /// <param name="members">Members to remove</param>
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


        /// <summary>
        /// Truncate removes all of the messages but does not affect the channel data or channel members. If you want to delete both messages and channel data then use the <see cref="DeleteAsync"/> method instead.
        /// </summary>
        /// <param name="truncatedAt">[Optional]truncate channel up to given time. If not set then all messages are truncated</param>
        /// <param name="systemMessage">A system message to be added via truncation.</param>
        /// <param name="skipPushNotifications">Don't send a push notification for <param name="systemMessage"/>.</param>
        /// <param name="isHardDelete">if truncation should delete messages instead of hiding</param>
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

        /// <summary>
        /// Delete this channel. By default channel is soft deleted. You may hard delete it by setting the <param name="isHardDelete"> argument to true
        /// </summary>
        /// <param name="isHardDelete">Hard delete completely removes channel with all its resources</param>
        /// <remarks>https://getstream.io/chat/docs/unity/channel_delete/?language=unity</remarks>
        public async Task DeleteAsync(bool isHardDelete)
            => LowLevelClient.InternalChannelApi.DeleteChannelAsync(Type, Id,
                isHardDelete);

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public Task SendTypingStartedEventAsync() =>
            LowLevelClient.InternalChannelApi.SendTypingStartEventAsync(Type, Id);

        public Task SendTypingStoppedEventAsync() =>
            LowLevelClient.InternalChannelApi.SendTypingStopEventAsync(Type, Id);

        internal StreamChannel(string uniqueId, ICacheRepository<StreamChannel> repository, ITrackedObjectContext context)
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

            bool wasRemoved = false;

            //We always reduce because watchers are paginatable so our partial _watchers state may not contain the removed on but count reflects all of them
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
        private readonly List<StreamUser> _typingUsers = new List<StreamUser>();
    }
}