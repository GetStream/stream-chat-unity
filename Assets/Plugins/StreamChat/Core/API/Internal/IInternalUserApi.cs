using System.Threading.Tasks;
using StreamChat.Core.DTO.Requests;
using StreamChat.Core.DTO.Responses;

namespace StreamChat.Core.API.Internal
{
    internal interface IInternalUserApi
    {
        Task<UsersResponseDTO> QueryUsersAsync(QueryUsersRequestDTO queryUsersRequest);

        Task<GuestResponseDTO> CreateGuestAsync(GuestRequestDTO createGuestRequest);

        Task<UpdateUsersResponseDTO> UpsertManyUsersAsync(UpdateUsersRequestDTO updateUsersRequest);

        Task<UpdateUsersResponseDTO> UpdateUserPartialAsync(UpdateUserPartialRequestDTO updateUserPartialRequest);
    }
}