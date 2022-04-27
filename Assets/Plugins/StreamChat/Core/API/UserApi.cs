using System.Threading.Tasks;
using StreamChat.Core.DTO.Requests;
using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Requests.DTO;
using StreamChat.Core.Requests;
using StreamChat.Core.Responses;
using StreamChat.Core.Web;
using StreamChat.Libs.Http;
using StreamChat.Libs.Logs;
using StreamChat.Libs.Serialization;

namespace StreamChat.Core.API
{
    internal class UserApi : ApiClientBase, IUserApi
    {
        public UserApi(IHttpClient httpClient, ISerializer serializer, ILogs logs,
            IRequestUriFactory requestUriFactory)
            : base(httpClient, serializer, logs, requestUriFactory)
        {
        }

        public Task<UsersResponse> QueryUsersAsync(QueryUsersRequest queryUsersRequest) =>
            Get<QueryUsersRequest, QueryUsersRequestDTO, UsersResponse, UsersResponseDTO>("/users",
                queryUsersRequest);

        public Task<GuestResponse> CreateGuestAsync(GuestRequest createGuestRequest) =>
            Post<GuestRequest, GuestRequestDTO, GuestResponse, GuestResponseDTO>("/guest", createGuestRequest);

        public Task<UpdateUsersResponse> UpsertUsersAsync(UpdateUsersRequest updateUsersRequest) =>
            UpsertManyUsersAsync(updateUsersRequest);

        public Task<UpdateUsersResponse> UpsertManyUsersAsync(UpdateUsersRequest updateUsersRequest) =>
            Post<UpdateUsersRequest, UpdateUsersRequestDTO, UpdateUsersResponse, UpdateUsersResponseDTO>("/users",
                updateUsersRequest);

        public Task<UpdateUsersResponse> UpdateUserPartialAsync(UpdateUserPartialRequest updateUserPartialRequest) =>
            Patch<UpdateUserPartialRequest, UpdateUserPartialRequestDTO, UpdateUsersResponse, UpdateUsersResponseDTO>(
                "/users", updateUserPartialRequest);
    }
}