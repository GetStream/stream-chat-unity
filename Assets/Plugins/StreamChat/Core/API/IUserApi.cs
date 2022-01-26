using System.Threading.Tasks;
using StreamChat.Core.Plugins.StreamChat.Core.Requests.DTO;
using StreamChat.Core.Plugins.StreamChat.Core.Responses;
using StreamChat.Core.Requests;
using StreamChat.Core.Responses;

namespace Plugins.StreamChat.Core.API
{
    public interface IUserApi
    {
        Task<UsersResponse> QueryUsersAsync(QueryUsersRequest queryUsersRequest);

        Task<GuestResponse> CreateGuestAsync(GuestRequest createGuestRequest);

        Task<UpdateUsersResponse> UpdateUsersAsync(UpdateUsersRequest updateUsersRequest);

        Task<UpdateUsersResponse> UpdateUserPartialAsync(UpdateUserPartialRequest updateUserPartialRequest);

        Task<DeleteUserResponse>
            DeleteUserAsync(string userId, DeleteUserRequestParameters deleteUserRequestParameters);

        Task<DeleteUsersResponse> DeleteUsersAsync(DeleteUsersRequest deleteUsersRequest);
    }
}