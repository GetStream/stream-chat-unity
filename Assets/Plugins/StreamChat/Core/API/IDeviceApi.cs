using System.Threading.Tasks;
using StreamChat.Core.Requests;
using StreamChat.Core.Responses;

namespace StreamChat.Core.API
{
    internal interface IDeviceApi
    {
        Task<ApiResponse> AddDeviceAsync(CreateDeviceRequest device);

        Task<ListDevicesResponse> ListDevicesAsync(string userId);

        Task<ApiResponse> RemoveDeviceAsync(string deviceId, string userId);
    }
}