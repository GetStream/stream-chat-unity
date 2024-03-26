using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.State;
using StreamChat.Core.State.Caches;
using StreamChat.Core.Models;
using StreamChat.Core.Requests;

namespace StreamChat.Core.StatefulModels
{
    internal sealed class StreamMessage : StreamStatefulModelBase<StreamMessage>,
        IUpdateableFrom<MessageInternalDTO, StreamMessage>, IStreamMessage
    {
        public event StreamMessageReactionHandler ReactionAdded;
        public event StreamMessageReactionHandler ReactionRemoved;
        public event StreamMessageReactionHandler ReactionUpdated;

        public IReadOnlyList<StreamMessageAttachment> Attachments => _attachments;

        /// <summary>
        /// Whether `before_message_send webhook` failed or not. Field is only accessible in push webhook
        /// </summary>
        //public bool? BeforeMessageSendFailed { get; private set; } //StreamTodo: verify this property

        public string Cid { get; private set; }

        public string Command { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        public DateTimeOffset? DeletedAt { get; private set; }

        public string Html { get; private set; }

        public IReadOnlyDictionary<string, string> I18n => _iI18n;

        public string Id { get; private set; }

        public IReadOnlyDictionary<string, IReadOnlyList<string>> ImageLabels => throw new NotImplementedException();

        public IReadOnlyList<StreamReaction> LatestReactions => _latestReactions;

        public IReadOnlyList<IStreamUser> MentionedUsers => _mentionedUsers;

        public IReadOnlyList<StreamReaction> OwnReactions => _ownReactions;

        public string ParentId { get; private set; }

        public DateTimeOffset? PinExpires { get; private set; }

        public bool Pinned { get; private set; }

        public DateTimeOffset? PinnedAt { get; private set; }

        public IStreamUser PinnedBy { get; private set; }

        public IStreamMessage QuotedMessage { get; private set; }

        public string QuotedMessageId { get; private set; }

        public IReadOnlyDictionary<string, int> ReactionCounts => _reactionCounts;

        public IReadOnlyDictionary<string, int> ReactionScores => _reactionScores;

        public int? ReplyCount { get; private set; }

        public bool? Shadowed { get; private set; }

        public bool? ShowInChannel { get; private set; }

        public bool? Silent { get; private set; }

        public string Text { get; private set; }

        public IReadOnlyList<IStreamUser> ThreadParticipants => _threadParticipants;

        public StreamMessageType? Type { get; private set; }

        public DateTimeOffset? UpdatedAt { get; private set; }

        public IStreamUser User { get; private set; }
        
        public bool IsDeleted => Type == StreamMessageType.Deleted;

        //Do not update message from response, the WS event might have been processed and we would overwrite it with an old state
        public Task SoftDeleteAsync() => LowLevelClient.InternalMessageApi.DeleteMessageAsync(Id, hard: false);

        //Do not update message from response, the WS event might have been processed and we would overwrite it with an old state
        public Task HardDeleteAsync() => LowLevelClient.InternalMessageApi.DeleteMessageAsync(Id, hard: true);

        public async Task UpdateAsync(StreamUpdateMessageRequest streamUpdateMessageRequest)
        {
            if (streamUpdateMessageRequest == null)
            {
                throw new ArgumentNullException(nameof(streamUpdateMessageRequest));
            }
            
            var requestDto = streamUpdateMessageRequest.TrySaveToDto();
            requestDto.Message.Id = Id;

            var response = await LowLevelClient.InternalMessageApi.UpdateMessageAsync(requestDto);
            Cache.TryCreateOrUpdate(response.Message);
        }

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
            await Client.RefreshChannelState(Cid);
        }

        public async Task UnpinAsync()
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
            await Client.RefreshChannelState(Cid);
        }

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

        //StreamTodo: docs calls it Remove
        public Task DeleteReactionAsync(string type)
        {
            StreamAsserts.AssertNotNullOrEmpty(type, nameof(type));
            return LowLevelClient.InternalMessageApi.DeleteReactionAsync(Id, type);
        }

        //StreamTodo: should we unwrap the response?
        public Task FlagAsync() => LowLevelClient.InternalModerationApi.FlagMessageAsync(Id);

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
            //BeforeMessageSendFailed = GetOrDefault(dto.BeforeMessageSendFailed, BeforeMessageSendFailed);
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
            //dto.Mml ignored because its only server-side
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
            
            LoadAdditionalProperties(dto.AdditionalProperties);
        }

        internal StreamMessage(string uniqueId, ICacheRepository<StreamMessage> repository, IStatefulModelContext context)
            : base(uniqueId, repository, context)
        {
        }

        internal void InternalHandleSoftDelete()
        {
            Text = string.Empty;
        }

        internal void HandleReactionNewEvent(ReactionNewEventInternalDTO eventDto, StreamChannel channel, StreamReaction reaction)
        {
            AssertCid(eventDto.Cid);
            AssertMessageId(eventDto.Message.Id);

            // Important! `reaction.new` has this field invalidly empty. This is for performance reasons but is left as empty for backward compatibility.
            // We rely on the fact that _ownReactions.TryReplaceRegularObjectsFromDto ignores null and does not clear the list when `own_reactions` is invalidly empty
            eventDto.Message.OwnReactions = null;

            //StreamTodo: verify if this how we should update the message + what about events for customer to get notified
            Cache.TryCreateOrUpdate(eventDto.Message);
            ReactionAdded?.Invoke(channel, this, reaction);
        }

        internal void HandleReactionUpdatedEvent(ReactionUpdatedEventInternalDTO eventDto, StreamChannel channel, StreamReaction reaction)
        {
            AssertCid(eventDto.Cid);
            AssertMessageId(eventDto.Message.Id);

            // Important! `reaction.new` has this field invalidly empty. This is for performance reasons but is left as empty for backward compatibility.
            // We rely on the fact that _ownReactions.TryReplaceRegularObjectsFromDto ignores null and does not clear the list when `own_reactions` is invalidly empty
            eventDto.Message.OwnReactions = null;

            Cache.TryCreateOrUpdate(eventDto.Message);
            ReactionUpdated?.Invoke(channel, this, reaction);
        }

        internal void HandleReactionDeletedEvent(ReactionDeletedEventInternalDTO eventDto, StreamChannel channel, StreamReaction reaction)
        {
            AssertCid(eventDto.Cid);
            AssertMessageId(eventDto.Message.Id);
            
            // Important! `reaction.new` has this field invalidly empty. This is for performance reasons but is left as empty for backward compatibility.
            // We rely on the fact that _ownReactions.TryReplaceRegularObjectsFromDto ignores null and does not clear the list when `own_reactions` is invalidly empty
            eventDto.Message.OwnReactions = null;

            Cache.TryCreateOrUpdate(eventDto.Message);
            ReactionRemoved?.Invoke(channel, this, reaction);
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