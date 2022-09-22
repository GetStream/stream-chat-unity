using StreamChat.Core.DTO.Requests;

namespace StreamChat.Core.Requests
{
    public partial class ShowChannelRequest : RequestObjectBase, ISavableTo<ShowChannelRequestInternalDTO>
    {
        ShowChannelRequestInternalDTO ISavableTo<ShowChannelRequestInternalDTO>.SaveToDto() =>
            new ShowChannelRequestInternalDTO
            {
                AdditionalProperties = AdditionalProperties,
            };
    }
}