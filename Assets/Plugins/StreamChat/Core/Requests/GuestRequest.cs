using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.Helpers;

namespace StreamChat.Core.Requests
{
    public partial class GuestRequest : RequestObjectBase, ISavableTo<GuestRequestInternalDTO>
    {
        public UserObjectRequest User { get; set; } = new UserObjectRequest();

        GuestRequestInternalDTO ISavableTo<GuestRequestInternalDTO>.SaveToDto() =>
            new GuestRequestInternalDTO
            {
                User = User.TrySaveToDto<UserObjectRequestDTO>(),
                AdditionalProperties = AdditionalProperties,
            };
    }
}