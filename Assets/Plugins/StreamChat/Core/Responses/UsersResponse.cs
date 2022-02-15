using System.Collections.Generic;
using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Helpers;
using StreamChat.Core.Models;

namespace StreamChat.Core.Responses
{
    public partial class UsersResponse : ResponseObjectBase, ILoadableFrom<UsersResponseDTO, UsersResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        /// <summary>
        /// List of found users
        /// </summary>
        public ICollection<User> Users { get; set; }

        UsersResponse ILoadableFrom<UsersResponseDTO, UsersResponse>.LoadFromDto(UsersResponseDTO dto)
        {
            Duration = dto.Duration;
            Users = Users.TryLoadFromDtoCollection<UserResponseDTO, User>(dto.Users);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}