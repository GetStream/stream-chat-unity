using System;
using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Requests
{
    public class ReactionRequest : RequestObjectBase, ISavableTo<ReactionRequestInternalDTO>
    {
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

        [Obsolete("Has no effect and will be removed in a future release")] //StreamTODO: remove this in a major release
        public UserObjectRequest User { get; set; }

        [Obsolete("Has no effect and will be removed in a future release")] //StreamTODO: remove this in a major release
        public string UserId { get; set; }

        ReactionRequestInternalDTO ISavableTo<ReactionRequestInternalDTO>.SaveToDto() =>
            new ReactionRequestInternalDTO
            {
                MessageId = MessageId,
                Score = Score,
                Type = Type,
                AdditionalProperties = AdditionalProperties,
            };
    }
}