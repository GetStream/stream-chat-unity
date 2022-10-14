using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.Models;

namespace StreamChat.Core.State.Models
{
    /// <summary>
    /// Represents user reaction to a message
    /// </summary>
    public class StreamReaction : IStateLoadableFrom<ReactionInternalDTO, StreamReaction>
    {
        /// <summary>
        /// Date/time of creation
        /// </summary>
        public System.DateTimeOffset? CreatedAt { get; set; }

        /// <summary>
        /// ID of a message user reacted to
        /// </summary>
        public string MessageId { get; set; }

        /// <summary>
        /// Reaction score. If not specified reaction has score of 1
        /// </summary>
        public int? Score { get; set; }

        /// <summary>
        /// The type of reaction (e.g. 'like', 'laugh', 'wow')
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Date/time of the last update
        /// </summary>
        public System.DateTimeOffset? UpdatedAt { get; set; }

        public User User { get; set; }

        /// <summary>
        /// ID of a user who reacted to a message
        /// </summary>
        public string UserId { get; set; }

        StreamReaction IStateLoadableFrom<ReactionInternalDTO, StreamReaction>.LoadFromDto(ReactionInternalDTO dto, ICache cache)
        {
            CreatedAt = dto.CreatedAt;
            MessageId = dto.MessageId;
            Score = dto.Score;
            Type = dto.Type;
            UpdatedAt = dto.UpdatedAt;
            User = User.TryLoadFromDto<UserObjectInternalInternalDTO, User>(dto.User);
            UserId = dto.UserId;

            return this;
        }
    }
}