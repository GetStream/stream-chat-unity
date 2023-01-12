using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Requests
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