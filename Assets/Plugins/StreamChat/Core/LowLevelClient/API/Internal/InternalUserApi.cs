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

        public Task<QueryUsersResponseInternalDTO> QueryUsersAsync(QueryUsersRequestInternalDTO queryUsersRequest)
            => Get<QueryUsersRequestInternalDTO, QueryUsersResponseInternalDTO>("/users", queryUsersRequest);

        public Task<CreateGuestResponseInternalDTO> CreateGuestAsync(CreateGuestRequestInternalDTO createGuestRequest)
            => Post<CreateGuestRequestInternalDTO, CreateGuestResponseInternalDTO>("/guest", createGuestRequest);

        public Task<UpdateUsersResponseInternalDTO>
            UpsertManyUsersAsync(UpdateUsersRequestInternalDTO updateUsersRequest)
            => Post<UpdateUsersRequestInternalDTO, UpdateUsersResponseInternalDTO>("/users", updateUsersRequest);

        public Task<UpdateUsersResponseInternalDTO>
            UpdateUserPartialAsync(UpdateUserPartialRequestInternalDTO updateUserPartialRequest)
            => Patch<UpdateUserPartialRequestInternalDTO, UpdateUsersResponseInternalDTO>("/users",
                updateUserPartialRequest);
    }
}