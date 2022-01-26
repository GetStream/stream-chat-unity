using Plugins.StreamChat.Core.Helpers;
using StreamChat.Core;
using StreamChat.Core.DTO.Requests;
using StreamChat.Core.Requests;

namespace Plugins.StreamChat.Core.Requests
{
    public partial class UpdateUsersRequest : RequestObjectBase, ISavableTo<UpdateUsersRequestDTO>
    {
        /// <summary>
        /// Object containing users
        /// </summary>
        public System.Collections.Generic.IDictionary<string, UserObjectRequest> Users { get; set; } = new System.Collections.Generic.Dictionary<string, UserObjectRequest>();

        public UpdateUsersRequestDTO SaveToDto() =>
            new UpdateUsersRequestDTO
            {
                Users = Users.TrySaveToDtoDictionary<UserObjectRequestDTO, UserObjectRequest, string>(),
                AdditionalProperties = AdditionalProperties,
            };
    }
}