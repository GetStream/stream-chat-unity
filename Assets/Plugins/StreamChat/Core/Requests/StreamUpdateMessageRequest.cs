using System;
using System.Collections.Generic;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.LowLevelClient;
using StreamChat.Core.State;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.Requests
{
    public class StreamUpdateMessageRequest : ISavableTo<UpdateMessageRequestInternalDTO>
    {
        public Dictionary<string, string> PendingMessageMetadata { get; set; }

        /// <summary>
        /// Do not try to enrich the links within message
        /// </summary>
        public bool? SkipEnrichUrl { get; set; }

        #region MessageRequest

        /// <summary>
        /// Array of message attachments
        /// </summary>
        public List<StreamAttachmentRequest> Attachments { get; set; } = new List<StreamAttachmentRequest>();

        /// <summary>
        /// List of mentioned users
        /// </summary>
        public List<IStreamUser> MentionedUsers { get; set; }

        /// <summary>
        /// ID of parent message (thread)
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// Date when pinned message expires
        /// </summary>
        public DateTimeOffset? PinExpires { get; set; }

        /// <summary>
        /// Whether message is pinned or not
        /// </summary>
        public bool? Pinned { get; set; }

        /// <summary>
        /// Date when message got pinned
        /// </summary>
        public DateTimeOffset? PinnedAt { get; set; }

        /// <summary>
        /// Contains user who pinned the message
        /// </summary>
        public IStreamUser PinnedBy { get; set; }

        //StreamTodo: best to have StreamMessage QuotedMessage but we may not always have it in our cache
        public string QuotedMessageId { get; set; }

        /// <summary>
        /// Whether thread reply should be shown in the channel as well
        /// </summary>
        public bool? ShowInChannel { get; set; }

        /// <summary>
        /// Whether message is silent or not
        /// </summary>
        public bool? Silent { get; set; }

        /// <summary>
        /// Text of the message. Should be empty if `mml` is provided
        /// </summary>
        public string Text { get; set; }

        #endregion
        
        /// <summary>
        /// Add or update custom data associated with this message. This will be accessible through <see cref="IStreamMessage.CustomData"/>
        /// </summary>
        public StreamCustomDataRequest CustomData { get; set; }

        UpdateMessageRequestInternalDTO ISavableTo<UpdateMessageRequestInternalDTO>.SaveToDto()
        {
            var messageRequestDto = new MessageRequestInternalDTO
            {
                Attachments = Attachments?.TrySaveToDtoCollection<StreamAttachmentRequest, AttachmentRequestInternalDTO>(),
                // Cid = Cid, Purposely ignored because it has no effect and endpoint already contains channel type&id
                //Html = Html, Only server-side field
                //Id = Id, Purposely ignored so it can be set by the StreamMessage.UpdateAsync
                MentionedUsers = MentionedUsers.ToUserIdsListOrNull(),
                //Mml = Mml, Only server-side field
                ParentId = ParentId,
                PinExpires = PinExpires,
                Pinned = Pinned,
                PinnedAt = PinnedAt,
                PinnedBy = PinnedBy?.Id,
                QuotedMessageId = QuotedMessageId,
                //ReactionScores = ReactionScores, Purposely ignored because it has no effect, probably ignored by backend
                ShowInChannel = ShowInChannel,
                Silent = Silent,
                Text = Text,
                AdditionalProperties = CustomData?.ToDictionary()
            };

            return new UpdateMessageRequestInternalDTO
            {
                Message = messageRequestDto,
                PendingMessageMetadata = PendingMessageMetadata,
                SkipEnrichUrl = SkipEnrichUrl,
            };
        }
    }
}