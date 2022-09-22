﻿using StreamChat.Core.Helpers;
using StreamChat.Core.DTO.Requests;

namespace StreamChat.Core.Requests
{
    public partial class UpdateUsersRequest : RequestObjectBase, ISavableTo<UpdateUsersRequestInternalDTO>
    {
        /// <summary>
        /// Object containing users
        /// </summary>
        public System.Collections.Generic.Dictionary<string, UserObjectRequest> Users { get; set; }

        UpdateUsersRequestInternalDTO ISavableTo<UpdateUsersRequestInternalDTO>.SaveToDto() =>
            new UpdateUsersRequestInternalDTO
            {
                Users = Users.TrySaveToDtoDictionary<UserObjectRequestInternalDTO, UserObjectRequest, string>(),
                AdditionalProperties = AdditionalProperties,
            };
    }
}