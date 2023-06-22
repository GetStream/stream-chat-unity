using System.Collections.Generic;
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

        UpdateUsersRequestInternalDTO ISavableTo<UpdateUsersRequestInternalDTO>.SaveToDto()
        {
            var dto = new UpdateUsersRequestInternalDTO
            {
                AdditionalProperties = AdditionalProperties,
            };

            // Ticket#38178 TrySaveToDtoDictionary caused crashes on old Android versions with IL2CPP
            if (Users != null)
            {
                var dict = new Dictionary<string, UserObjectRequestInternalDTO>();
            
                foreach (var sourceKeyValue in Users)
                {
                    if (sourceKeyValue.Value == null)
                    {
                        continue;
                    }
                    
                    dict.Add(sourceKeyValue.Key,
                        ((ISavableTo<UserObjectRequestInternalDTO>)sourceKeyValue.Value).SaveToDto());
                }
            
                dto.Users = dict;
            }

            return dto;
        }
    }
}