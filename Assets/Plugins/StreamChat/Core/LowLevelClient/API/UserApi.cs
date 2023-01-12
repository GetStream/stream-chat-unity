using System;
using System.Threading.Tasks;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.LowLevelClient.API.Internal;
using StreamChat.Core.LowLevelClient.Requests;
using StreamChat.Core.LowLevelClient.Responses;

namespace StreamChat.Core.LowLevelClient.API
{
    internal class UserApi : IUserApi
    {
        public UserApi(IInternalUserApi internalUserApi)
        {
            _internalUserApi = internalUserApi ?? throw new ArgumentNullException(nameof(internalUserApi));
        }

        public async Task<UsersResponse> QueryUsersAsync(QueryUsersRequest queryUsersRequest)
        {
            var dto = await _internalUserApi.QueryUsersAsync(queryUsersRequest.TrySaveToDto());
            return dto.ToDomain<UsersResponseInternalDTO, UsersResponse>();
        }

        public async Task<GuestResponse> CreateGuestAsync(GuestRequest createGuestRequest)
        {
            var dto = await _internalUserApi.CreateGuestAsync(createGuestRequest.TrySaveToDto());
            return dto.ToDomain<GuestResponseInternalDTO, GuestResponse>();
        }

        public Task<UpdateUsersResponse> UpsertUsersAsync(UpdateUsersRequest updateUsersRequest)
            => UpsertManyUsersAsync(updateUsersRequest);

        public async Task<UpdateUsersResponse> UpsertManyUsersAsync(UpdateUsersRequest updateUsersRequest)
        {
            var dto = await _internalUserApi.UpsertManyUsersAsync(updateUsersRequest.TrySaveToDto());
            return dto.ToDomain<UpdateUsersResponseInternalDTO, UpdateUsersResponse>();
        }

        public async Task<UpdateUsersResponse> UpdateUserPartialAsync(UpdateUserPartialRequest updateUserPartialRequest)
        {
            var dto = await _internalUserApi.UpdateUserPartialAsync(updateUserPartialRequest.TrySaveToDto());
            return dto.ToDomain<UpdateUsersResponseInternalDTO, UpdateUsersResponse>();
        }

        private readonly IInternalUserApi _internalUserApi;
    }
}