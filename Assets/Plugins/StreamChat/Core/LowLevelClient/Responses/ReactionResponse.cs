using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Responses
{
    public class ReactionResponse : ResponseObjectBase, ILoadableFrom<SendReactionResponseInternalDTO, ReactionResponse>
    {
        public Message Message { get; set; }

        public Reaction Reaction { get; set; }

        ReactionResponse ILoadableFrom<SendReactionResponseInternalDTO, ReactionResponse>.LoadFromDto(SendReactionResponseInternalDTO dto)
        {
            Message = Message.TryLoadFromDto<MessageResponseInternalDTO, Message>(dto.Message);
            Reaction = Reaction.TryLoadFromDto<ReactionResponseInternalDTO, Reaction>(dto.Reaction);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}