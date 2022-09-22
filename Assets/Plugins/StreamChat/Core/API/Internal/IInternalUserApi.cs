using System.Threading.Tasks;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.InternalDTO.Responses;

namespace StreamChat.Core.API.Internal
{
    internal interface IInternalUserApi
    {
        Task<UsersResponseInternalDTO> QueryUsersAsync(QueryUsersRequestInternalDTO queryUsersRequest);

        Task<GuestResponseInternalDTO> CreateGuestAsync(GuestRequestInternalDTO createGuestRequest);

        Task<UpdateUsersResponseInternalDTO> UpsertManyUsersAsync(UpdateUsersRequestInternalDTO updateUsersRequest);

        Task<UpdateUsersResponseInternalDTO> UpdateUserPartialAsync(UpdateUserPartialRequestInternalDTO updateUserPartialRequest);
    }
}