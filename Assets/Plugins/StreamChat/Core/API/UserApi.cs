using System.Threading.Tasks;
using StreamChat.Core.API;
using StreamChat.Core.DTO.Requests;
using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Plugins.StreamChat.Core.Requests.DTO;
using StreamChat.Core.Plugins.StreamChat.Core.Responses;
using StreamChat.Core.Requests;
using StreamChat.Core.Responses;
using StreamChat.Core.Web;
using StreamChat.Libs.Http;
using StreamChat.Libs.Logs;
using StreamChat.Libs.Serialization;

namespace Plugins.StreamChat.Core.API
{
    public class UserApi : ApiClientBase, IUserApi
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

        public Task<UpdateUsersResponse> UpdateUsersAsync(UpdateUsersRequest updateUsersRequest) =>
            Post<UpdateUsersRequest, UpdateUsersRequestDTO, UpdateUsersResponse, UpdateUsersResponseDTO>("/users",
                updateUsersRequest);

        public Task<UpdateUsersResponse> UpdateUserPartialAsync(UpdateUserPartialRequest updateUserPartialRequest) =>
            Patch<UpdateUserPartialRequest, UpdateUserPartialRequestDTO, UpdateUsersResponse, UpdateUsersResponseDTO>(
                "/users", updateUserPartialRequest);

        public Task<DeleteUserResponse>
            DeleteUserAsync(string userId, DeleteUserRequestParameters deleteUserRequestParameters) =>
            Delete<DeleteUserResponse, DeleteUserResponseDTO>($"/users/{userId}",
                QueryParameters.Default.AppendFrom(deleteUserRequestParameters));

        public Task<DeleteUsersResponse> DeleteUsersAsync(DeleteUsersRequest deleteUsersRequest) =>
            Post<DeleteUsersRequest, DeleteUsersRequestDTO, DeleteUsersResponse, DeleteUsersResponseDTO>(
                "/users/delete", deleteUsersRequest);
    }
}