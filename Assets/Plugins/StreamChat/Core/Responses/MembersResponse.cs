﻿using StreamChat.Core;
using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Helpers;
using StreamChat.Core.Models;
using StreamChat.Core.Responses;

namespace StreamChat.Core.Responses
{
    public partial class MembersResponse : ResponseObjectBase, ILoadableFrom<MembersResponseDTO, MembersResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        /// <summary>
        /// List of found members
        /// </summary>
        public System.Collections.Generic.ICollection<ChannelMember> Members { get; set; }


        public MembersResponse LoadFromDto(MembersResponseDTO dto)
        {
            Duration = dto.Duration;
            Members = Members.TryLoadFromDtoCollection(dto.Members);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}