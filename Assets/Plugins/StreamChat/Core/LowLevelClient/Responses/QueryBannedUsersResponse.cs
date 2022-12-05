using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Responses;

namespace StreamChat.Core.LowLevelClient.Responses
{
    public partial class QueryBannedUsersResponse : ResponseObjectBase, ILoadableFrom<QueryBannedUsersResponseInternalDTO, QueryBannedUsersResponse>
    {
        public System.Collections.Generic.List<BanResponse> Bans { get; set; }

        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        QueryBannedUsersResponse ILoadableFrom<QueryBannedUsersResponseInternalDTO, QueryBannedUsersResponse>.LoadFromDto(QueryBannedUsersResponseInternalDTO dto)
        {
            Bans = Bans.TryLoadFromDtoCollection(dto.Bans);
            Duration = dto.Duration;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}