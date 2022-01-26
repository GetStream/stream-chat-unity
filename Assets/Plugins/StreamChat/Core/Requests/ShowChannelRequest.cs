using StreamChat.Core;
using StreamChat.Core.DTO.Requests;
using StreamChat.Core.Requests;

namespace Plugins.StreamChat.Core.Requests
{
    public partial class ShowChannelRequest : RequestObjectBase, ISavableTo<ShowChannelRequestDTO>
    {
        public ShowChannelRequestDTO SaveToDto() =>
            new ShowChannelRequestDTO
            {
                AdditionalProperties = AdditionalProperties,
            };
    }
}