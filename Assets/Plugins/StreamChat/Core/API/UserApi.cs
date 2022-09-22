using System;
using System.Threading.Tasks;
using StreamChat.Core.API.Internal;
using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Helpers;
using StreamChat.Core.Requests;
using StreamChat.Core.Responses;

namespace StreamChat.Core.API
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