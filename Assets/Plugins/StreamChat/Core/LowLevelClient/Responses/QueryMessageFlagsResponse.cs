using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Responses
{
    public partial class QueryMessageFlagsResponse : ResponseObjectBase, ILoadableFrom<QueryMessageFlagsResponseInternalDTO, QueryMessageFlagsResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        public System.Collections.Generic.List<MessageFlag> Flags { get; set; }

        QueryMessageFlagsResponse ILoadableFrom<QueryMessageFlagsResponseInternalDTO, QueryMessageFlagsResponse>.LoadFromDto(QueryMessageFlagsResponseInternalDTO dto)
        {
            Duration = dto.Duration;
            Flags = Flags.TryLoadFromDtoCollection(dto.Flags);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}