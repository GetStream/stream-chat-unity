using StreamChat.Core.Helpers;
using StreamChat.Core.DTO.Requests;

namespace StreamChat.Core.Requests
{
    public partial class UpdateUsersRequest : RequestObjectBase, ISavableTo<UpdateUsersRequestDTO>
    {
        /// <summary>
        /// Object containing users
        /// </summary>
        public System.Collections.Generic.Dictionary<string, UserObjectRequest> Users { get; set; }

        UpdateUsersRequestDTO ISavableTo<UpdateUsersRequestDTO>.SaveToDto() =>
            new UpdateUsersRequestDTO
            {
                Users = Users.TrySaveToDtoDictionary<UserObjectRequestDTO, UserObjectRequest, string>(),
                AdditionalProperties = AdditionalProperties,
            };
    }
}