using StreamChat.Core.DTO.Models;
using StreamChat.Core.Helpers;
using StreamChat.Core.Responses;

namespace StreamChat.Core.Models
{
    /// <summary>
    /// Represents user reaction to a message
    /// </summary>
    public class Reaction : ResponseObjectBase, ILoadableFrom<ReactionDTO, Reaction>
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
        public double? Score { get; set; }

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

        Reaction ILoadableFrom<ReactionDTO, Reaction>.LoadFromDto(ReactionDTO dto)
        {
            AdditionalProperties = dto.AdditionalProperties;
            CreatedAt = dto.CreatedAt;
            MessageId = dto.MessageId;
            Score = dto.Score;
            Type = dto.Type;
            UpdatedAt = dto.UpdatedAt;
            User = User.TryLoadFromDto<UserObjectDTO, User>(dto.User);
            UserId = dto.UserId;

            return this;
        }
    }
}