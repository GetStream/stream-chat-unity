using System.Threading.Tasks;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.InternalDTO.Responses;

namespace StreamChat.Core.LowLevelClient.API.Internal
{
    internal interface IInternalUserApi
    {
        Task<UsersResponseInternalDTO> QueryUsersAsync(QueryUsersRequestInternalDTO queryUsersRequest);

        Task<GuestResponseInternalDTO> CreateGuestAsync(GuestRequestInternalDTO createGuestRequest);

        /// <summary>
        /// <para>Creates or updates users.</para>
        /// Any user present in the payload will have its data replaced with the new version.
        /// For partial updates, use <see cref="UpdateUserPartialAsync"/> method.
        /// You can send up to 100 users per API request in both upsert and partial update API.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/update_users/?language=unity#server-side-user-updates-(batch)</remarks>
        Task<UpdateUsersResponseInternalDTO> UpsertManyUsersAsync(UpdateUsersRequestInternalDTO updateUsersRequest);

        Task<UpdateUsersResponseInternalDTO> UpdateUserPartialAsync(UpdateUserPartialRequestInternalDTO updateUserPartialRequest);
    }
}