﻿using StreamChat.Core.Helpers;
using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Models;

namespace StreamChat.Core.Responses
{
    public partial class UpdateUsersResponse : ResponseObjectBase, ILoadableFrom<UpdateUsersResponseInternalDTO, UpdateUsersResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        /// <summary>
        /// Object containing users
        /// </summary>
        public System.Collections.Generic.Dictionary<string, User> Users { get; set; }

        UpdateUsersResponse ILoadableFrom<UpdateUsersResponseInternalDTO, UpdateUsersResponse>.LoadFromDto(UpdateUsersResponseInternalDTO dto)
        {
            Duration = dto.Duration;
            Users = Users.TryLoadFromDtoDictionary(dto.Users);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}