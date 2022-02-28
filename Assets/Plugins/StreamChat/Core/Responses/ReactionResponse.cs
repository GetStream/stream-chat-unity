using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Helpers;
using StreamChat.Core.Models;

namespace StreamChat.Core.Responses
{
    public class ReactionResponse : ResponseObjectBase, ILoadableFrom<ReactionResponseDTO, ReactionResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        public Message Message { get; set; }

        public Reaction Reaction { get; set; }

        ReactionResponse ILoadableFrom<ReactionResponseDTO, ReactionResponse>.LoadFromDto(ReactionResponseDTO dto)
        {
            Duration = dto.Duration;
            Message = Message.TryLoadFromDto(dto.Message);
            Reaction = Reaction.TryLoadFromDto(dto.Reaction);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}