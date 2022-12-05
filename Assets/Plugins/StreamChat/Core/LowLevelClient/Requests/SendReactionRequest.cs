using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Requests
{
    public class SendReactionRequest : RequestObjectBase, ISavableTo<SendReactionRequestInternalDTO>
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

        SendReactionRequestInternalDTO ISavableTo<SendReactionRequestInternalDTO>.SaveToDto() =>
            new SendReactionRequestInternalDTO
            {
                ID = ID,
                EnforceUnique = EnforceUnique,
                Reaction = Reaction.TrySaveToDto(),
                SkipPush = SkipPush,
                AdditionalProperties = AdditionalProperties,
            };
    }
}