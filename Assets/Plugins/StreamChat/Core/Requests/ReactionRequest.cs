using StreamChat.Core.DTO.Requests;
using StreamChat.Core.Helpers;

namespace StreamChat.Core.Requests
{
    public class ReactionRequest : RequestObjectBase, ISavableTo<ReactionRequestDTO>
    {
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

        public UserObjectRequest User { get; set; }

        /// <summary>
        /// ID of a user who reacted to a message
        /// </summary>
        public string UserId { get; set; }

        ReactionRequestDTO ISavableTo<ReactionRequestDTO>.SaveToDto() =>
            new ReactionRequestDTO
            {
                MessageId = MessageId,
                Score = Score,
                Type = Type,
                User = User.TrySaveToDto(),
                UserId = UserId,
                AdditionalProperties = AdditionalProperties,
            };
    }
}