using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.State;
using StreamChat.Core.State.Caches;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.Models
{
    /// <summary>
    /// Represents user reaction to a message
    /// </summary>
    public class StreamReaction : IStateLoadableFrom<ReactionInternalDTO, StreamReaction>
    {
        /// <summary>
        /// Date/time of creation
        /// </summary>
        public System.DateTimeOffset? CreatedAt { get; private set; }

        /// <summary>
        /// ID of a message user reacted to
        /// </summary>
        public string MessageId { get; private set; }

        /// <summary>
        /// Reaction score. If not specified reaction has score of 1
        /// </summary>
        public int? Score { get; private set; }

        /// <summary>
        /// The type of reaction (e.g. 'like', 'laugh', 'wow')
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// Date/time of the last update
        /// </summary>
        public System.DateTimeOffset? UpdatedAt { get; private set; }

        /// <summary>
        /// User who reacted to a message
        /// </summary>
        public IStreamUser User { get; private set; }

        /// <summary>
        /// ID of a user who reacted to a message
        /// </summary>
        public string UserId { get; private set; }

        StreamReaction IStateLoadableFrom<ReactionInternalDTO, StreamReaction>.LoadFromDto(ReactionInternalDTO dto, ICache cache)
        {
            CreatedAt = dto.CreatedAt;
            MessageId = dto.MessageId;
            Score = dto.Score;
            Type = dto.Type;
            UpdatedAt = dto.UpdatedAt;
            User = cache.TryCreateOrUpdate(dto.User);
            UserId = dto.UserId;

            return this;
        }
    }
}