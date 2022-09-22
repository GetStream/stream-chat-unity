using StreamChat.Core.DTO.Requests;
using StreamChat.Core.Helpers;
using StreamChat.Core.Requests;

namespace StreamChat.Core.Requests
{
    public partial class UpdateUserPartialRequest : RequestObjectBase, ISavableTo<UpdateUserPartialRequestInternalDTO>
    {
        public System.Collections.Generic.List<UpdateUserPartialRequestEntry> Users { get; set; }

        UpdateUserPartialRequestInternalDTO ISavableTo<UpdateUserPartialRequestInternalDTO>.SaveToDto()
        {
            return new UpdateUserPartialRequestInternalDTO
            {
                Users = Users.TrySaveToDtoCollection<UpdateUserPartialRequestEntry, UpdateUserPartialRequestEntryInternalDTO>(),
                AdditionalProperties = AdditionalProperties,
            };
        }
    }
}