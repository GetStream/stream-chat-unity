using System;
using System.Collections.Generic;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Responses;

namespace StreamChat.Core.LowLevelClient.Models
{
    /// <summary>
    /// Represents a thread. A thread groups replies to a parent <see cref="Message"/> in a <see cref="Channel"/>.
    /// </summary>
    public class Thread : ModelBase, ILoadableFrom<ThreadResponseInternalDTO, Thread>
    {
        /// <summary>
        /// Number of currently active participants in the thread.
        /// </summary>
        public int ActiveParticipantCount { get; set; }

        /// <summary>
        /// The channel this thread belongs to.
        /// </summary>
        public Channel Channel { get; set; }

        /// <summary>
        /// CID of the channel this thread belongs to.
        /// </summary>
        public string ChannelCid { get; set; }

        /// <summary>
        /// Date/time the thread was created.
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// User who created the thread.
        /// </summary>
        public User CreatedBy { get; set; }

        /// <summary>
        /// Id of the user who created the thread.
        /// </summary>
        public string CreatedByUserId { get; set; }

        /// <summary>
        /// Custom data attached to this thread.
        /// </summary>
        public Dictionary<string, object> Custom { get; set; }

        /// <summary>
        /// Date/time the thread was deleted (null if not deleted).
        /// </summary>
        public DateTimeOffset? DeletedAt { get; set; }

        /// <summary>
        /// Date/time of the last reply in the thread.
        /// </summary>
        public DateTimeOffset? LastMessageAt { get; set; }

        /// <summary>
        /// The parent <see cref="Message"/> of this thread.
        /// </summary>
        public Message ParentMessage { get; set; }

        /// <summary>
        /// Id of the parent message (unique identifier of this thread).
        /// </summary>
        public string ParentMessageId { get; set; }

        /// <summary>
        /// Total number of participants in this thread.
        /// </summary>
        public int ParticipantCount { get; set; }

        /// <summary>
        /// Total number of replies in this thread.
        /// </summary>
        public int? ReplyCount { get; set; }

        /// <summary>
        /// All thread participants (including those who left).
        /// </summary>
        public List<ThreadParticipant> ThreadParticipants { get; set; }

        /// <summary>
        /// Optional title of the thread.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Date/time of the last update.
        /// </summary>
        public DateTimeOffset UpdatedAt { get; set; }

        Thread ILoadableFrom<ThreadResponseInternalDTO, Thread>.LoadFromDto(ThreadResponseInternalDTO dto)
        {
            ActiveParticipantCount = dto.ActiveParticipantCount;
            Channel = Channel.TryLoadFromDto(dto.Channel);
            ChannelCid = dto.ChannelCid;
            CreatedAt = dto.CreatedAt;
            CreatedBy = CreatedBy.TryLoadFromDto<UserResponseInternalDTO, User>(dto.CreatedBy);
            CreatedByUserId = dto.CreatedByUserId;
            Custom = dto.Custom;
            DeletedAt = dto.DeletedAt;
            LastMessageAt = dto.LastMessageAt;
            ParentMessage = ParentMessage.TryLoadFromDto<MessageResponseInternalDTO, Message>(dto.ParentMessage);
            ParentMessageId = dto.ParentMessageId;
            ParticipantCount = dto.ParticipantCount;
            ReplyCount = dto.ReplyCount;
            ThreadParticipants = ThreadParticipants.TryLoadFromDtoCollection(dto.ThreadParticipants);
            Title = dto.Title;
            UpdatedAt = dto.UpdatedAt;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}
