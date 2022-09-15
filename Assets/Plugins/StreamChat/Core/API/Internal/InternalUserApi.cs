using System.Threading.Tasks;
using StreamChat.Core.DTO.Requests;
using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Web;
using StreamChat.Libs.Http;
using StreamChat.Libs.Logs;
using StreamChat.Libs.Serialization;

namespace StreamChat.Core.API.Internal
{
    internal class InternalUserApi : InternalApiClientBase, IInternalUserApi
    {
        public InternalUserApi(IHttpClient httpClient, ISerializer serializer, ILogs logs,
            IRequestUriFactory requestUriFactory)
            : base(httpClient, serializer, logs, requestUriFactory)
        {
        }

        public Task<UsersResponseDTO> QueryUsersAsync(QueryUsersRequestDTO queryUsersRequest) =>
            Get<QueryUsersRequestDTO, UsersResponseDTO>("/users", queryUsersRequest);

        public Task<GuestResponseDTO> CreateGuestAsync(GuestRequestDTO createGuestRequest) =>
            Post<GuestRequestDTO, GuestResponseDTO>("/guest", createGuestRequest);

        public Task<UpdateUsersResponseDTO> UpsertManyUsersAsync(UpdateUsersRequestDTO updateUsersRequest) =>
            Post<UpdateUsersRequestDTO, UpdateUsersResponseDTO>("/users", updateUsersRequest);

        public Task<UpdateUsersResponseDTO> UpdateUserPartialAsync(UpdateUserPartialRequestDTO updateUserPartialRequest) =>
            Patch<UpdateUserPartialRequestDTO, UpdateUsersResponseDTO>("/users", updateUserPartialRequest);
    }
}