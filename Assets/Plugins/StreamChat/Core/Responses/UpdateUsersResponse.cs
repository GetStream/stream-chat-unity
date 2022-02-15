﻿using StreamChat.Core.Helpers;
using StreamChat.Core;
using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Models;
using StreamChat.Core.Responses;

namespace StreamChat.Core.Responses
{
    public partial class UpdateUsersResponse : ResponseObjectBase, ILoadableFrom<UpdateUsersResponseDTO, UpdateUsersResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        /// <summary>
        /// Object containing users
        /// </summary>
        public System.Collections.Generic.IDictionary<string, User> Users { get; set; }

        UpdateUsersResponse ILoadableFrom<UpdateUsersResponseDTO, UpdateUsersResponse>.LoadFromDto(UpdateUsersResponseDTO dto)
        {
            Duration = dto.Duration;
            Users = Users.TryLoadFromDtoDictionary(dto.Users);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}