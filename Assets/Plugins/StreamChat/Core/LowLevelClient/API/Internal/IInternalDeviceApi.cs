using System.Threading.Tasks;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.InternalDTO.Responses;

namespace StreamChat.Core.LowLevelClient.API.Internal
{
    internal interface IInternalDeviceApi
    {
        Task<ResponseInternalDTO> AddDeviceAsync(CreateDeviceRequestInternalDTO device);

        Task<ListDevicesResponseInternalDTO> ListDevicesAsync(string userId);

        Task<ResponseInternalDTO> RemoveDeviceAsync(string deviceId, string userId);
    }
}