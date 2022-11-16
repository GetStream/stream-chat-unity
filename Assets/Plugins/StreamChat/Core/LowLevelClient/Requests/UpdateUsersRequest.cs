using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Requests
{
    public partial class UpdateUsersRequest : RequestObjectBase, ISavableTo<UpdateUsersRequestInternalDTO>
    {
        /// <summary>
        /// Object containing users
        /// </summary>
        /// StreamTodo: Check if this could be list
        public System.Collections.Generic.Dictionary<string, UserObjectRequest> Users { get; set; }

        UpdateUsersRequestInternalDTO ISavableTo<UpdateUsersRequestInternalDTO>.SaveToDto() =>
            new UpdateUsersRequestInternalDTO
            {
                Users = Users.TrySaveToDtoDictionary<UserObjectRequestInternalDTO, UserObjectRequest, string>(),
                AdditionalProperties = AdditionalProperties,
            };
    }
}