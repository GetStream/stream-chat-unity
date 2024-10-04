using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Requests
{
    public partial class CreateDeviceRequest : RequestObjectBase, ISavableTo<CreateDeviceRequestInternalDTO>
    {
        public string Id { get; set; }

        public PushProviderType PushProvider { get; set; }

        public string PushProviderName { get; set; }

        CreateDeviceRequestInternalDTO ISavableTo<CreateDeviceRequestInternalDTO>.SaveToDto()
            => new CreateDeviceRequestInternalDTO
            {
                Id = Id,
                PushProvider = PushProvider.TrySaveToDto(),
                PushProviderName = PushProviderName,
            };
    }
}