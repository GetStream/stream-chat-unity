using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Helpers;
using StreamChat.Core.Responses;

namespace StreamChat.Core.Responses
{
    public partial class MarkReadResponse : ResponseObjectBase, ILoadableFrom<MarkReadResponseDTO, MarkReadResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        /// <summary>
        /// Mark read event
        /// </summary>
        public Event Event { get; set; }

        MarkReadResponse ILoadableFrom<MarkReadResponseDTO, MarkReadResponse>.LoadFromDto(MarkReadResponseDTO dto)
        {
            Duration = dto.Duration;
            Event = Event.TryLoadFromDto(dto.Event);
            AdditionalProperties = dto.AdditionalProperties;
                
            return this;
        }
    }
}