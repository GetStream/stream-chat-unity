using System.Threading.Tasks;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.Web;
using StreamChat.Libs.Http;
using StreamChat.Libs.Logs;
using StreamChat.Libs.Serialization;

namespace StreamChat.Core.LowLevelClient.API.Internal
{
    internal class InternalUserApi : InternalApiClientBase, IInternalUserApi
    {
        public InternalUserApi(IHttpClient httpClient, ISerializer serializer, ILogs logs,
            IRequestUriFactory requestUriFactory, IStreamChatLowLevelClient lowLevelClient)
            : base(httpClient, serializer, logs, requestUriFactory, lowLevelClient)
        {
        }

        public Task<UsersResponseInternalDTO> QueryUsersAsync(QueryUsersRequestInternalDTO queryUsersRequest)
            => Get<QueryUsersRequestInternalDTO, UsersResponseInternalDTO>("/users", queryUsersRequest);

        public Task<GuestResponseInternalDTO> CreateGuestAsync(GuestRequestInternalDTO createGuestRequest)
            => Post<GuestRequestInternalDTO, GuestResponseInternalDTO>("/guest", createGuestRequest);

        public Task<UpdateUsersResponseInternalDTO>
            UpsertManyUsersAsync(UpdateUsersRequestInternalDTO updateUsersRequest)
            => Post<UpdateUsersRequestInternalDTO, UpdateUsersResponseInternalDTO>("/users", updateUsersRequest);

        public Task<UpdateUsersResponseInternalDTO>
            UpdateUserPartialAsync(UpdateUserPartialRequestInternalDTO updateUserPartialRequest)
            => Patch<UpdateUserPartialRequestInternalDTO, UpdateUsersResponseInternalDTO>("/users",
                updateUserPartialRequest);
    }
}