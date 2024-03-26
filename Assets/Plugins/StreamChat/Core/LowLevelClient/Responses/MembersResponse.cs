using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Responses
{
    public partial class MembersResponse : ResponseObjectBase, ILoadableFrom<MembersResponseInternalDTO, MembersResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        /// <summary>
        /// List of found members
        /// </summary>
        public System.Collections.Generic.List<ChannelMember> Members { get; set; }


        MembersResponse ILoadableFrom<MembersResponseInternalDTO, MembersResponse>.LoadFromDto(MembersResponseInternalDTO dto)
        {
            Duration = dto.Duration;
            Members = Members.TryLoadFromDtoCollection(dto.Members);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}