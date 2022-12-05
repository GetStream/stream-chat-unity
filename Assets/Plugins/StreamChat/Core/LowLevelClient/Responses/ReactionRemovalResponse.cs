using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Responses
{
    public class ReactionRemovalResponse : ResponseObjectBase, ILoadableFrom<ReactionRemovalResponseInternalDTO, ReactionRemovalResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        public Message Message { get; set; }

        public Reaction Reaction { get; set; }

        ReactionRemovalResponse ILoadableFrom<ReactionRemovalResponseInternalDTO, ReactionRemovalResponse>.LoadFromDto(ReactionRemovalResponseInternalDTO dto)
        {
            Duration = Duration;
            Message = Message.TryLoadFromDto<MessageInternalDTO, Message>(dto.Message);
            Reaction = Reaction.TryLoadFromDto(dto.Reaction);
            AdditionalProperties = AdditionalProperties;

            return this;
        }
    }
}