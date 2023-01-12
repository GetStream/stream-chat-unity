using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.LowLevelClient;
using StreamChat.Core.LowLevelClient.Requests;

namespace StreamChat.Core.Requests
{
    public partial class CreateDeviceRequest : RequestObjectBase, ISavableTo<CreateDeviceRequestInternalDTO>
    {
        public string Id { get; set; }

        public PushProviderType? PushProvider { get; set; }

        public string PushProviderName { get; set; }

        CreateDeviceRequestInternalDTO ISavableTo<CreateDeviceRequestInternalDTO>.SaveToDto()
            => new CreateDeviceRequestInternalDTO
            {
                Id = Id,
                PushProvider = PushProvider,
                PushProviderName = PushProviderName,
            };
    }
}