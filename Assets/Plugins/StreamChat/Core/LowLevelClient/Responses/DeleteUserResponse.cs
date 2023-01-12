﻿using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Responses
{
    public partial class DeleteUserResponse : ResponseObjectBase, ILoadableFrom<DeleteUserResponseInternalDTO, DeleteUserResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        public string TaskId { get; set; }

        public User User { get; set; }

        DeleteUserResponse ILoadableFrom<DeleteUserResponseInternalDTO, DeleteUserResponse>.LoadFromDto(DeleteUserResponseInternalDTO dto)
        {
            Duration = dto.Duration;
            TaskId = dto.TaskId;
            User = User.TryLoadFromDto<UserObjectInternalInternalDTO, User>(dto.User);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}