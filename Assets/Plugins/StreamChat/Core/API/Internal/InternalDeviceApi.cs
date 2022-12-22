using System.Threading.Tasks;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.LowLevelClient;
using StreamChat.Core.LowLevelClient.API.Internal;
using StreamChat.Core.Web;
using StreamChat.Libs.Http;
using StreamChat.Libs.Logs;
using StreamChat.Libs.Serialization;

namespace StreamChat.Core.API.Internal
{
    internal class InternalDeviceApi : InternalApiClientBase, IInternalDeviceApi
    {
        public InternalDeviceApi(IHttpClient httpClient, ISerializer serializer, ILogs logs,
            IRequestUriFactory requestUriFactory, IStreamChatLowLevelClient lowLevelClient)
            : base(httpClient, serializer, logs, requestUriFactory, lowLevelClient)
        {
        }

        public Task<ResponseInternalDTO> AddDeviceAsync(CreateDeviceRequestInternalDTO device)
            => Post<CreateDeviceRequestInternalDTO, ResponseInternalDTO>("devices", device);

        public Task<ListDevicesResponseInternalDTO> ListDevicesAsync(string userId)
        {
            var parameters = QueryParameters.Default.Append("user_id", userId);
            return Get<ListDevicesResponseInternalDTO>("devices", parameters);
        }

        public Task<ResponseInternalDTO> RemoveDeviceAsync(string deviceId, string userId)
        {
            var parameters = QueryParameters.Default
                .Append("id", deviceId)
                .Append("user_id", userId);

            return Delete<ResponseInternalDTO>("devices", parameters);
        }
    }
}