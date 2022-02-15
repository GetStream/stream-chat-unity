using StreamChat.Core;
using StreamChat.Core.DTO.Requests;
using StreamChat.Core.Requests;

namespace StreamChat.Core.Requests
{
    public partial class ShowChannelRequest : RequestObjectBase, ISavableTo<ShowChannelRequestDTO>
    {
        ShowChannelRequestDTO ISavableTo<ShowChannelRequestDTO>.SaveToDto() =>
            new ShowChannelRequestDTO
            {
                AdditionalProperties = AdditionalProperties,
            };
    }
}