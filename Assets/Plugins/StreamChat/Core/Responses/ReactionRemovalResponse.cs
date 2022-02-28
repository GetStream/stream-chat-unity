using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Helpers;
using StreamChat.Core.Models;

namespace StreamChat.Core.Responses
{
    public class ReactionRemovalResponse : ResponseObjectBase, ILoadableFrom<ReactionRemovalResponseDTO, ReactionRemovalResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        public Message Message { get; set; }

        public Reaction Reaction { get; set; }

        ReactionRemovalResponse ILoadableFrom<ReactionRemovalResponseDTO, ReactionRemovalResponse>.LoadFromDto(ReactionRemovalResponseDTO dto)
        {
            Duration = Duration;
            Message = Message.TryLoadFromDto(dto.Message);
            Reaction = Reaction.TryLoadFromDto(dto.Reaction);
            AdditionalProperties = AdditionalProperties;

            return this;
        }
    }
}