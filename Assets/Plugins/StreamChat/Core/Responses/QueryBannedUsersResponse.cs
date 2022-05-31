using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Helpers;

namespace StreamChat.Core.Responses
{
    public partial class QueryBannedUsersResponse : ResponseObjectBase, ILoadableFrom<QueryBannedUsersResponseDTO, QueryBannedUsersResponse>
    {
        public System.Collections.Generic.List<BanResponse> Bans { get; set; }

        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        QueryBannedUsersResponse ILoadableFrom<QueryBannedUsersResponseDTO, QueryBannedUsersResponse>.LoadFromDto(QueryBannedUsersResponseDTO dto)
        {
            Bans = Bans.TryLoadFromDtoCollection(dto.Bans);
            Duration = dto.Duration;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}