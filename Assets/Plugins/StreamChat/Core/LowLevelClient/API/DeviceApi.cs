using System;
using System.Threading.Tasks;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.LowLevelClient.API.Internal;
using StreamChat.Core.LowLevelClient.Responses;
using StreamChat.Core.Requests;
using StreamChat.Core.Responses;

namespace StreamChat.Core.LowLevelClient.API
{
    internal class DeviceApi : IDeviceApi
    {
        public DeviceApi(IInternalDeviceApi internalDeviceApi)
        {
            _internalDeviceApi = internalDeviceApi ?? throw new ArgumentNullException(nameof(internalDeviceApi));
        }

        public async Task<ApiResponse> AddDeviceAsync(CreateDeviceRequest device)
        {
            var dto = await _internalDeviceApi.AddDeviceAsync(device.TrySaveToDto());
            return dto.ToDomain<ResponseInternalDTO, ApiResponse>();
        }

        public async Task<ListDevicesResponse> ListDevicesAsync(string userId)
        {
            var dto = await _internalDeviceApi.ListDevicesAsync(userId);
            return dto.ToDomain<ListDevicesResponseInternalDTO, ListDevicesResponse>();
        }


        public async Task<ApiResponse> RemoveDeviceAsync(string deviceId, string userId)
        {
            var dto = await _internalDeviceApi.RemoveDeviceAsync(deviceId, userId);
            return dto.ToDomain<ResponseInternalDTO, ApiResponse>();
        }

        private readonly IInternalDeviceApi _internalDeviceApi;
    }
}