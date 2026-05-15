using System;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Responses;

namespace StreamChat.Core.LowLevelClient.Models
{
    /// <summary>
    /// Represents a user that is participating in a thread.
    /// </summary>
    public class ThreadParticipant : ModelBase, ILoadableFrom<ThreadParticipantInternalDTO, ThreadParticipant>
    {
        /// <summary>
        /// CID of the channel the thread belongs to.
        /// </summary>
        public string ChannelCid { get; set; }

        /// <summary>
        /// Date/time the participant joined the thread.
        /// </summary>
        public DateTimeOffset? CreatedAt { get; set; }

        /// <summary>
        /// Custom data attached to this participant.
        /// </summary>
        public System.Collections.Generic.Dictionary<string, object> Custom { get; set; }

        /// <summary>
        /// Date/time the participant last read the thread.
        /// </summary>
        public DateTimeOffset? LastReadAt { get; set; }

        /// <summary>
        /// Date/time of the last message authored by this participant in the thread.
        /// </summary>
        public DateTimeOffset? LastThreadMessageAt { get; set; }

        /// <summary>
        /// Date/time the participant left the thread (null if still active).
        /// </summary>
        public DateTimeOffset? LeftThreadAt { get; set; }

        /// <summary>
        /// Id of the thread (matches the parent message id).
        /// </summary>
        public string ThreadId { get; set; }

        /// <summary>
        /// The participating user.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Id of the participating user.
        /// </summary>
        public string UserId { get; set; }

        ThreadParticipant ILoadableFrom<ThreadParticipantInternalDTO, ThreadParticipant>.LoadFromDto(
            ThreadParticipantInternalDTO dto)
        {
            ChannelCid = dto.ChannelCid;
            CreatedAt = dto.CreatedAt;
            Custom = dto.Custom;
            LastReadAt = dto.LastReadAt;
            LastThreadMessageAt = dto.LastThreadMessageAt;
            LeftThreadAt = dto.LeftThreadAt;
            ThreadId = dto.ThreadId;
            User = User.TryLoadFromDto<UserResponseInternalDTO, User>(dto.User);
            UserId = dto.UserId;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}
