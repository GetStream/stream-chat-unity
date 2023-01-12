using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Requests
{
    public partial class GuestRequest : RequestObjectBase, ISavableTo<GuestRequestInternalDTO>
    {
        public UserObjectRequest User { get; set; } = new UserObjectRequest();

        GuestRequestInternalDTO ISavableTo<GuestRequestInternalDTO>.SaveToDto() =>
            new GuestRequestInternalDTO
            {
                User = User.TrySaveToDto(),
                AdditionalProperties = AdditionalProperties,
            };
    }
}