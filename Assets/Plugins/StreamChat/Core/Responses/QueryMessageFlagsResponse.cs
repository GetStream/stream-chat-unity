using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Helpers;
using StreamChat.Core.Models;

namespace StreamChat.Core.Responses
{
    public partial class QueryMessageFlagsResponse : ResponseObjectBase, ILoadableFrom<QueryMessageFlagsResponseDTO, QueryMessageFlagsResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        public System.Collections.Generic.List<MessageFlag> Flags { get; set; }

        QueryMessageFlagsResponse ILoadableFrom<QueryMessageFlagsResponseDTO, QueryMessageFlagsResponse>.LoadFromDto(QueryMessageFlagsResponseDTO dto)
        {
            Duration = dto.Duration;
            Flags = Flags.TryLoadFromDtoCollection(dto.Flags);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}