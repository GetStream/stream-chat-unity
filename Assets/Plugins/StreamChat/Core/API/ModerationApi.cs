﻿using System;
using System.Threading.Tasks;
using StreamChat.Core.API.Internal;
using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Helpers;
using StreamChat.Core.Requests;
using StreamChat.Core.Responses;

namespace StreamChat.Core.API
{
    internal class ModerationApi : IModerationApi
    {
        public ModerationApi(IInternalModerationApi internalModerationApi)
        {
            _internalModerationApi =
                internalModerationApi ?? throw new ArgumentNullException(nameof(internalModerationApi));
        }

        public async Task<MuteUserResponse> MuteUserAsync(MuteUserRequest muteUserRequest)
        {
            var dto = await _internalModerationApi.MuteUserAsync(muteUserRequest.TrySaveToDto());
            return dto.ToDomain<MuteUserResponseDTO, MuteUserResponse>();
        }

        public async Task<UnmuteResponse> UnmuteUserAsync(UnmuteUserRequest unmuteUserRequest)
        {
            var dto = await _internalModerationApi.UnmuteUserAsync(unmuteUserRequest.TrySaveToDto());
            return dto.ToDomain<UnmuteResponseDTO, UnmuteResponse>();
        }

        public async Task<ApiResponse> BanUserAsync(BanRequest banRequest)
        {
            var dto = await _internalModerationApi.BanUserAsync(banRequest.TrySaveToDto());
            return dto.ToDomain<ResponseDTO, ApiResponse>();
        }

        public async Task<ApiResponse> UnbanUserAsync(UnbanRequest unbanRequest)
        {
            var dto = await _internalModerationApi.UnbanUserAsync(unbanRequest.TargetUserId, unbanRequest.Type,
                unbanRequest.Id);
            return dto.ToDomain<ResponseDTO, ApiResponse>();
        }

        public async Task<ApiResponse> ShadowBanUserAsync(ShadowBanRequest shadowBanRequest)
        {
            var dto = await _internalModerationApi.ShadowBanUserAsync(shadowBanRequest.TrySaveToDto());
            return dto.ToDomain<ResponseDTO, ApiResponse>();
        }

        public Task<ApiResponse> RemoveUserShadowBanAsync(UnbanRequest unbanRequest) => UnbanUserAsync(unbanRequest);

        public async Task<QueryBannedUsersResponse> QueryBannedUsersAsync(
            QueryBannedUsersRequest queryBannedUsersRequest)
        {
            var dto = await _internalModerationApi.QueryBannedUsersAsync(queryBannedUsersRequest.TrySaveToDto());
            return dto.ToDomain<QueryBannedUsersResponseDTO, QueryBannedUsersResponse>();
        }

        public async Task<FlagResponse> FlagUserAsync(string targetUserId)
        {
            var dto = await _internalModerationApi.FlagUserAsync(targetUserId);
            return dto.ToDomain<FlagResponseDTO, FlagResponse>();
        }

        public async Task<FlagResponse> FlagMessageAsync(string targetMessageId)
        {
            var dto = await _internalModerationApi.FlagMessageAsync(targetMessageId);
            return dto.ToDomain<FlagResponseDTO, FlagResponse>();
        }

        public async Task<QueryMessageFlagsResponse> QueryMessageFlagsAsync(
            QueryMessageFlagsRequest queryMessageFlagsRequest)
        {
            var dto = await _internalModerationApi.QueryMessageFlagsAsync(queryMessageFlagsRequest.TrySaveToDto());
            return dto.ToDomain<QueryMessageFlagsResponseDTO, QueryMessageFlagsResponse>();
        }

        private readonly IInternalModerationApi _internalModerationApi;
    }
}