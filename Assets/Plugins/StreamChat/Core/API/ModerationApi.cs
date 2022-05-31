using System;
using System.Threading.Tasks;
using StreamChat.Core.DTO.Requests;
using StreamChat.Core.DTO.Responses;
using StreamChat.Libs.Http;
using StreamChat.Libs.Logs;
using StreamChat.Libs.Serialization;
using StreamChat.Core.Requests;
using StreamChat.Core.Responses;
using StreamChat.Core.Web;

namespace StreamChat.Core.API
{
    internal class ModerationApi : ApiClientBase, IModerationApi
    {
        public ModerationApi(IHttpClient httpClient, ISerializer serializer, ILogs logs,
            IRequestUriFactory requestUriFactory)
            : base(httpClient, serializer, logs, requestUriFactory)
        {
        }

        public Task<MuteUserResponse> MuteUserAsync(MuteUserRequest muteUserRequest)
        {
            var endpoint = ModerationEndpoints.MuteUser();

            return Post<MuteUserRequest, MuteUserRequestDTO, MuteUserResponse, MuteUserResponseDTO>(endpoint,
                muteUserRequest);
        }

        public Task<MuteUserResponse> UnmuteUserAsync(MuteUserRequest muteUserRequest)
        {
            var endpoint = ModerationEndpoints.UnmuteUser();

            return Post<MuteUserRequest, MuteUserRequestDTO, MuteUserResponse, MuteUserResponseDTO>(endpoint,
                muteUserRequest);
        }

        public Task<ApiResponse> BanAsync(BanRequest banRequest)
        {
            var endpoint = "/moderation/ban";

            return Post<BanRequest, BanRequestDTO, ApiResponse, ResponseDTO>(endpoint, banRequest);
        }

        public Task<ApiResponse> UnbanAsync(string userId, string channelId, string channelType)
        {
            var endpoint = "/moderation/ban";

            var parameters = QueryParameters.Default
                .Append("target_user_id", userId)
                .Append("type", channelType)
                .Append("id", channelId);

            return Delete<ApiResponse, ResponseDTO>(endpoint, parameters);
        }

        public Task<ApiResponse> ShadowBanAsync(ShadowBanRequest shadowBanRequest)
        {
            var endpoint = "/moderation/ban";

            return Post<BanRequest, BanRequestDTO, ApiResponse, ResponseDTO>(endpoint, shadowBanRequest);
        }

        public Task<ApiResponse> RemoveShadowBanAsync(string userId, string channelId, string channelType)
            => UnbanAsync(userId, channelId, channelType);

        public Task<QueryBannedUsersResponse> QueryBannedUsersAsync(QueryBannedUsersRequest queryBannedUsersRequest)
        {
            var endpoint = "/query_banned_users";

            return Get<QueryBannedUsersRequest, QueryBannedUsersRequestDTO, QueryBannedUsersResponse,
                QueryBannedUsersResponseDTO>(endpoint, queryBannedUsersRequest);
        }
    }
}