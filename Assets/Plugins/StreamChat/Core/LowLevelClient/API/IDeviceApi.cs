using System.Threading.Tasks;
using StreamChat.Core.LowLevelClient.Responses;
using StreamChat.Core.Requests;
using StreamChat.Core.Responses;

namespace StreamChat.Core.LowLevelClient.API
{
    /// <summary>
    /// API Client that can be used to retrieve, create and alter push notification devices of a Stream Chat application.
    /// </summary>
    /// <remarks>https://getstream.io/chat/docs/unity/push_devices/?language=unity</remarks>
    public interface IDeviceApi
    {
        /// <summary>
        /// <para>Adds a new device.</para>
        /// Registering a device associates it with a user and tells the
        /// push provider to send new message notifications to the device.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/push_devices/?language=unity</remarks>
        Task<ApiResponse> AddDeviceAsync(CreateDeviceRequest device);

        /// <summary>
        /// Provides a list of all devices associated with a user.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/push_devices/?language=unity</remarks>
        Task<ListDevicesResponse> ListDevicesAsync(string userId);

        /// <summary>
        /// <para>Removes a device.</para>
        /// Unregistering a device removes the device from the user and stops further new message notifications.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/push_devices/?language=unity</remarks>
        Task<ApiResponse> RemoveDeviceAsync(string deviceId, string userId);
    }
}