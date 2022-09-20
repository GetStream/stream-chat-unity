using StreamChat.Core.DTO.Requests;
using StreamChat.Core.Helpers;

namespace StreamChat.Core.Requests
{
    public class SendReactionRequest : RequestObjectBase, ISavableTo<SendReactionRequestDTO>
    {
        public string ID { get; set; }

        /// <summary>
        /// Whether to replace all existing user reactions
        /// </summary>
        public bool? EnforceUnique { get; set; }

        public ReactionRequest Reaction { get; set; }

        /// <summary>
        /// Skips any mobile push notifications
        /// </summary>
        public bool? SkipPush { get; set; }

        SendReactionRequestDTO ISavableTo<SendReactionRequestDTO>.SaveToDto() =>
            new SendReactionRequestDTO
            {
                ID = ID,
                EnforceUnique = EnforceUnique,
                Reaction = Reaction.TrySaveToDto(),
                SkipPush = SkipPush,
                AdditionalProperties = AdditionalProperties,
            };
    }
}