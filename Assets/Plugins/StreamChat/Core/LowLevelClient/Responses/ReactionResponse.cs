using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Responses
{
    public class ReactionResponse : ResponseObjectBase, ILoadableFrom<ReactionResponseInternalDTO, ReactionResponse>
    {
        public Message Message { get; set; }

        public Reaction Reaction { get; set; }

        ReactionResponse ILoadableFrom<ReactionResponseInternalDTO, ReactionResponse>.LoadFromDto(ReactionResponseInternalDTO dto)
        {
            Message = Message.TryLoadFromDto<MessageInternalDTO, Message>(dto.Message);
            Reaction = Reaction.TryLoadFromDto(dto.Reaction);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}