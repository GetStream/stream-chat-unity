using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.State.Models;
using StreamChat.Core.State.Requests;
using StreamChat.Core.State.Caches;

namespace StreamChat.Core.State.TrackedObjects
{
    /// <summary>
    /// Message belonging to a <see cref="IStreamChannel"/>
    /// </summary>
    public sealed class StreamMessage : StreamTrackedObjectBase<StreamMessage>,
        IUpdateableFrom<MessageInternalDTO, StreamMessage>
    {
        /// <summary>
        /// Array of message attachments
        /// </summary>
        public IReadOnlyList<StreamMessageAttachment> Attachments => _attachments;

        /// <summary>
        /// Whether `before_message_send webhook` failed or not. Field is only accessible in push webhook
        /// </summary>
        public bool? BeforeMessageSendFailed { get; private set; }

        /// <summary>
        /// Channel unique identifier in &lt;type&gt;:&lt;id&gt; format
        /// </summary>
        public string Cid { get; private set; }

        //StreamTodo: best to add StreamChannel reference
        public IStreamChannel Channel { get; private set; }

        /// <summary>
        /// Contains provided slash command
        /// </summary>
        public string Command { get; private set; }

        /// <summary>
        /// Date/time of creation
        /// </summary>
        public DateTimeOffset CreatedAt { get; private set; }

        /// <summary>
        /// Date/time of deletion
        /// </summary>
        public DateTimeOffset? DeletedAt { get; private set; }

        /// <summary>
        /// Contains HTML markup of the message. Can only be set when using server-side API
        /// </summary>
        public string Html { get; private set; }

        /// <summary>
        /// Object with translations. Key `language` contains the original language key. Other keys contain translations
        /// </summary>
        public IReadOnlyDictionary<string, string> I18n => _iI18n;

        /// <summary>
        /// Message ID is unique string identifier of the message
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Contains image moderation information
        /// *** NOT IMPLEMENTED *** PLEASE SEND SUPPORT TICKET IF YOU NEED THIS FEATURE
        /// </summary>
        public IReadOnlyDictionary<string, IReadOnlyList<string>> ImageLabels => throw new NotImplementedException();

        /// <summary>
        /// List of 10 latest reactions to this message
        /// </summary>
        public IReadOnlyList<StreamReaction> LatestReactions => _latestReactions;

        /// <summary>
        /// List of mentioned users
        /// </summary>
        public IReadOnlyList<IStreamUser> MentionedUsers => _mentionedUsers;

        /// <summary>
        /// Should be empty if `text` is provided. Can only be set when using server-side API
        /// </summary>
        public string Mml { get; private set; }

        /// <summary>
        /// List of 10 latest reactions of authenticated user to this message
        /// </summary>
        public IReadOnlyList<StreamReaction> OwnReactions => _ownReactions;

        /// <summary>
        /// ID of parent message (thread)
        /// </summary>
        public string ParentId { get; private set; }

        /// <summary>
        /// Date when pinned message expires
        /// </summary>
        public DateTimeOffset? PinExpires { get; private set; }

        /// <summary>
        /// Whether message is pinned or not
        /// </summary>
        public bool? Pinned { get; private set; }

        /// <summary>
        /// Date when message got pinned
        /// </summary>
        public DateTimeOffset? PinnedAt { get; private set; }

        /// <summary>
        /// Contains user who pinned the message
        /// </summary>
        public IStreamUser PinnedBy { get; private set; }

        /// <summary>
        /// Contains quoted message
        /// </summary>
        public StreamMessage QuotedMessage { get; private set; }

        public string QuotedMessageId { get; private set; }

        /// <summary>
        /// An object containing number of reactions of each type. Key: reaction type (string), value: number of reactions (int)
        /// </summary>
        public IReadOnlyDictionary<string, int> ReactionCounts => _reactionCounts;

        /// <summary>
        /// An object containing scores of reactions of each type. Key: reaction type (string), value: total score of reactions (int)
        /// </summary>
        public IReadOnlyDictionary<string, int> ReactionScores => _reactionScores;

        /// <summary>
        /// Number of replies to this message
        /// </summary>
        public int? ReplyCount { get; private set; }

        /// <summary>
        /// Whether the message was shadowed or not
        /// </summary>
        public bool? Shadowed { get; private set; }

        /// <summary>
        /// Whether thread reply should be shown in the channel as well
        /// </summary>
        public bool? ShowInChannel { get; private set; }

        /// <summary>
        /// Whether message is silent or not
        /// </summary>
        public bool? Silent { get; private set; }

        /// <summary>
        /// Text of the message. Should be empty if `mml` is provided
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// List of users who participate in thread
        /// </summary>
        public IReadOnlyList<IStreamUser> ThreadParticipants => _threadParticipants;

        /// <summary>
        /// Contains type of the message
        /// </summary>
        public StreamMessageType? Type { get; private set; }

        /// <summary>
        /// Date/time of the last update
        /// </summary>
        public DateTimeOffset? UpdatedAt { get; private set; }

        /// <summary>
        /// Sender of the message. Required when using server-side API
        /// </summary>
        public IStreamUser User { get; private set; }
        
        public bool IsDeleted => Type == StreamMessageType.Deleted;

        /// <summary>
        /// Clears the message text but leaves the rest of the message unchanged e.g. reaction, replies, attachments will be untouched
        ///
        /// If you want to remove the message and all its components completely use the <see cref="HardDeleteAsync"/>
        /// </summary>
        public Task SoftDeleteAsync() => LowLevelClient.InternalMessageApi.DeleteMessageAsync(Id, hard: false);

        /// <summary>
        /// Removes the message completely along with its reactions, replies, attachments, etc.
        ///
        /// If you want to clear the text only use the <see cref="SoftDeleteAsync"/>
        /// </summary>
        public Task HardDeleteAsync() => LowLevelClient.InternalMessageApi.DeleteMessageAsync(Id, hard: true);

        /// <summary>
        /// Update message text or other parameters
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task UpdateAsync(StreamUpdateMessageRequest streamUpdateMessageRequest)
        {
            if (streamUpdateMessageRequest == null)
            {
                throw new ArgumentNullException(nameof(streamUpdateMessageRequest));
            }

            // StreamTodo: validate that Text and Mml should not be both set

            var requestDto = streamUpdateMessageRequest.TrySaveToDto();
            requestDto.Message.Id = Id;

            var response = await LowLevelClient.InternalMessageApi.UpdateMessageAsync(requestDto);
            Cache.TryCreateOrUpdate(response.Message);
        }

        /// <summary>
        /// Pin this message to a channel with optional expiration date
        /// </summary>
        /// <param name="expiresAt">[Optional] UTC DateTime when pin will expire</param>
        public async Task PinAsync(DateTime? expiresAt = null)
        {
            var request = new UpdateMessagePartialRequestInternalDTO
            {
                Set = new Dictionary<string, object>
                {
                    { "pinned", true },
                }
            };

            if (expiresAt.HasValue)
            {
                request.Set["pin_expires"] = expiresAt;
            }

            var response = await LowLevelClient.InternalMessageApi.UpdateMessagePartialAsync(Id, request);
            Cache.TryCreateOrUpdate(response.Message);

            //StreamTodo: is this needed? How are other users notified about message pin?
            await StreamChatStateClient.RefreshChannelState(Cid);
        }

        /// <summary>
        /// Unpin this message from a channel
        /// </summary>
        public async Task Unpin()
        {
            var request = new UpdateMessagePartialRequestInternalDTO
            {
                Set = new Dictionary<string, object>
                {
                    { "pinned", false },
                }
            };
            var response = await LowLevelClient.InternalMessageApi.UpdateMessagePartialAsync(Id, request);
            Cache.TryCreateOrUpdate(response.Message);

            //StreamTodo: is this needed? How are other users notified about message pin?
            await StreamChatStateClient.RefreshChannelState(Cid);
        }

        /// <summary>
        /// Add reaction to this message
        /// You can view reactions via <see cref="ReactionScores"/>, <see cref="ReactionCounts"/>, <see cref="OwnReactions"/> and <see cref="LatestReactions"/>
        /// </summary>
        /// <param name="type">Reaction custom key, examples: like, smile, sad, etc. or any custom string</param>
        /// <param name="score">[Optional] Reaction score, by default it counts as 1</param>
        /// <param name="enforceUnique">[Optional] Whether to replace all existing user reactions</param>
        /// <param name="skipMobilePushNotifications">[Optional] Skips any mobile push notifications</param>
        public async Task SendReactionAsync(string type, int score = 1, bool enforceUnique = false,
            bool skipMobilePushNotifications = false)
        {
            StreamAsserts.AssertNotNullOrEmpty(type, nameof(type));

            var request = new SendReactionRequestInternalDTO
            {
                EnforceUnique = enforceUnique,
                Reaction = new ReactionRequestInternalDTO
                {
                    Score = score,
                    Type = type,
                },
                SkipPush = skipMobilePushNotifications,
            };

            var response = await LowLevelClient.InternalMessageApi.SendReactionAsync(Id, request);
            Cache.TryCreateOrUpdate(response.Message);
        }

        /// <summary>
        /// Delete reaction
        /// </summary>
        /// <param name="type">Reaction custom key, examples: like, smile, sad, etc. You can use any custom key</param>
        /// <returns></returns>
        public Task DeleteReactionAsync(string type)
        {
            StreamAsserts.AssertNotNullOrEmpty(type, nameof(type));
            return LowLevelClient.InternalMessageApi.DeleteReactionAsync(Id, type);
        }

        //StreamTodo: should we unwrap the response?
        /// <summary>
        /// Any user is allowed to flag a message. This triggers the message.flagged webhook event and adds the message to the inbox of your Stream Dashboard Chat Moderation view.
        /// </summary>
        public Task FlagAsync() => LowLevelClient.InternalModerationApi.FlagMessageAsync(Id);

        /// <summary>
        /// Mark this message as the last that was read by local user in this channel
        /// If you want to mark whole channel as read use the <see cref="IStreamChannel.MarkChannelReadAsync"/>
        ///
        /// This feature allows to track to which message users have read the channel
        /// </summary>
        public Task MarkMessageAsLastReadAsync()
        {
            if (!Cache.Channels.TryGet(Cid, out var streamChannel))
            {
                throw new Exception($"Failed to get channel with id {Cid} from cache. Please report this issue");
            }

            return LowLevelClient.InternalChannelApi.MarkReadAsync(streamChannel.Type, streamChannel.Id,
                new MarkReadRequestInternalDTO()
                {
                    MessageId = Id
                });
        }

        void IUpdateableFrom<MessageInternalDTO, StreamMessage>.UpdateFromDto(MessageInternalDTO dto, ICache cache)
        {
            _attachments.TryReplaceRegularObjectsFromDto(dto.Attachments, cache);
            BeforeMessageSendFailed = GetOrDefault(dto.BeforeMessageSendFailed, BeforeMessageSendFailed);
            Cid = GetOrDefault(dto.Cid, Cid);
            Command = GetOrDefault(dto.Command, Command);
            CreatedAt = GetOrDefault(dto.CreatedAt, CreatedAt);
            DeletedAt = GetOrDefault(dto.DeletedAt, DeletedAt);
            Html = GetOrDefault(dto.Html, Html);
            _iI18n.TryReplaceValuesFromDto(dto.I18n);
            Id = GetOrDefault(dto.Id, Id);
            //_imageLabels.TryReplaceValuesFromDto(dto.ImageLabels); //StreamTodo: NOT IMPLEMENTED
            _latestReactions.TryReplaceRegularObjectsFromDto(dto.LatestReactions, cache);
            _mentionedUsers.TryReplaceTrackedObjects(dto.MentionedUsers, cache.Users);
            Mml = GetOrDefault(dto.Mml, Mml);
            _ownReactions.TryReplaceRegularObjectsFromDto(dto.OwnReactions, cache);
            ParentId = GetOrDefault(dto.ParentId, ParentId);
            PinExpires = GetOrDefault(dto.PinExpires, PinExpires);
            Pinned = GetOrDefault(dto.Pinned, Pinned);
            PinnedAt = GetOrDefault(dto.PinnedAt, PinnedAt);
            PinnedBy = cache.TryCreateOrUpdate(dto.PinnedBy);
            QuotedMessage = cache.TryCreateOrUpdate(dto.QuotedMessage);
            QuotedMessageId = GetOrDefault(dto.QuotedMessageId, QuotedMessageId);
            _reactionCounts.TryReplaceValuesFromDto(dto.ReactionCounts); //StreamTodo: is this append only?
            _reactionScores.TryReplaceValuesFromDto(dto.ReactionScores);
            ReplyCount = GetOrDefault(dto.ReplyCount, ReplyCount);
            Shadowed = GetOrDefault(dto.Shadowed, Shadowed);
            ShowInChannel = GetOrDefault(dto.ShowInChannel, ShowInChannel);
            Silent = GetOrDefault(dto.Silent, Silent);
            Text = GetOrDefault(dto.Text, Text);
            _mentionedUsers.TryReplaceTrackedObjects(dto.MentionedUsers, cache.Users);
            _threadParticipants.TryReplaceTrackedObjects(dto.ThreadParticipants, cache.Users);
            Type = dto.Type.TryConvertToStreamMessageType();
            UpdatedAt = GetOrDefault(dto.UpdatedAt, UpdatedAt);
            User = cache.TryCreateOrUpdate(dto.User);
        }

        internal StreamMessage(string uniqueId, ICacheRepository<StreamMessage> repository, ITrackedObjectContext context)
            : base(uniqueId, repository, context)
        {
        }

        /// <summary>
        /// Clears the text only. Does not make an API call
        /// </summary>
        internal void InternalHandleSoftDelete()
        {
            Text = string.Empty;
        }

        internal void HandleReactionNewEvent(EventReactionNewInternalDTO eventDto)
        {
            AssertCid(eventDto.Cid);
            AssertMessageId(eventDto.Message.Id);

            //StreamTodo: verify if this how we should update the message + what about events for customer to get notified
            Cache.TryCreateOrUpdate(eventDto.Message);
        }

        internal void HandleReactionUpdatedEvent(EventReactionUpdatedInternalDTO eventDto)
        {
            AssertCid(eventDto.Cid);
            AssertMessageId(eventDto.Message.Id);

            //StreamTodo: verify if this how we should update the message + what about events for customer to get notified

            //Figure out what is this??? in Android SDK
            // // make sure we don't lose ownReactions
            // getMessage(message.id)?.let {
            //     message.ownReactions = it.ownReactions
            // }

            Cache.TryCreateOrUpdate(eventDto.Message);
        }

        internal void HandleReactionDeletedEvent(EventReactionDeletedInternalDTO eventDto)
        {
            AssertCid(eventDto.Cid);
            AssertMessageId(eventDto.Message.Id);

            //StreamTodo: verify if this how we should update the message + what about events for customer to get notified
            Cache.TryCreateOrUpdate(eventDto.Message);
        }

        protected override StreamMessage Self => this;

        protected override string InternalUniqueId
        {
            get => Id;
            set => Id = value;
        }

        // StreamTodo: change to lazy loading
        private readonly List<StreamMessageAttachment> _attachments = new List<StreamMessageAttachment>();
        private readonly List<StreamReaction> _latestReactions = new List<StreamReaction>();
        private readonly List<StreamUser> _mentionedUsers = new List<StreamUser>();
        private readonly List<StreamReaction> _ownReactions = new List<StreamReaction>();
        private readonly Dictionary<string, int> _reactionCounts = new Dictionary<string, int>();
        private readonly Dictionary<string, int> _reactionScores = new Dictionary<string, int>();
        private readonly List<StreamUser> _threadParticipants = new List<StreamUser>();

        private readonly Dictionary<string, string> _iI18n = new Dictionary<string, string>();

        private void AssertMessageId(string messageId)
        {
            if (messageId != Id)
            {
                throw new InvalidOperationException($"ID mismatch, received: `{messageId}` but current message ID is: {Id}");
            }
        }

        private void AssertCid(string cid)
        {
            if (cid != Cid)
            {
                throw new InvalidOperationException($"Cid mismatch, received: `{cid}` but current channel is: {Cid}");
            }
        }
    }
}