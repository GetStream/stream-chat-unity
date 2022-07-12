using StreamChat.Core.DTO.Requests;
using StreamChat.Core.Helpers;
using StreamChat.Core.Requests;

namespace StreamChat.Core.Requests
{
    public partial class UpdateUserPartialRequest : RequestObjectBase, ISavableTo<UpdateUserPartialRequestDTO>
    {
        public System.Collections.Generic.List<UpdateUserPartialRequestEntry> Users { get; set; }

        UpdateUserPartialRequestDTO ISavableTo<UpdateUserPartialRequestDTO>.SaveToDto()
        {
            return new UpdateUserPartialRequestDTO
            {
                Users = Users.TrySaveToDtoCollection<UpdateUserPartialRequestEntry, UpdateUserPartialRequestEntryDTO>(),
                AdditionalProperties = AdditionalProperties,
            };
        }
    }
}