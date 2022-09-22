using System.Collections.Generic;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.Helpers;
using StreamChat.Core.Models;

namespace StreamChat.Core.Responses
{
    public partial class UsersResponse : ResponseObjectBase, ILoadableFrom<UsersResponseInternalDTO, UsersResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        /// <summary>
        /// List of found users
        /// </summary>
        public List<User> Users { get; set; }

        UsersResponse ILoadableFrom<UsersResponseInternalDTO, UsersResponse>.LoadFromDto(UsersResponseInternalDTO dto)
        {
            Duration = dto.Duration;
            Users = Users.TryLoadFromDtoCollection<UserResponseInternalDTO, User>(dto.Users);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}