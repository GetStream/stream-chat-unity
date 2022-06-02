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

        public Task<UnmuteResponse> UnmuteUserAsync(UnmuteUserRequest unmuteUserRequest)
        {
            var endpoint = ModerationEndpoints.UnmuteUser();

            return Post<UnmuteUserRequest, UnmuteUserRequestDTO, UnmuteResponse, UnmuteResponseDTO>(endpoint,
                unmuteUserRequest);
        }

        public Task<ApiResponse> BanUserAsync(BanRequest banRequest)
        {
            var endpoint = "/moderation/ban";

            return Post<BanRequest, BanRequestDTO, ApiResponse, ResponseDTO>(endpoint, banRequest);
        }

        public Task<ApiResponse> UnbanUserAsync(UnbanRequest unbanRequest)
        {
            var endpoint = "/moderation/ban";

            var parameters = QueryParameters.Default
                .Append("target_user_id", unbanRequest.TargetUserId)
                .Append("type", unbanRequest.Type)
                .Append("id", unbanRequest.Id);

            return Delete<ApiResponse, ResponseDTO>(endpoint, parameters);
        }

        public Task<ApiResponse> ShadowBanUserAsync(ShadowBanRequest shadowBanRequest)
        {
            var endpoint = "/moderation/ban";

            return Post<BanRequest, BanRequestDTO, ApiResponse, ResponseDTO>(endpoint, shadowBanRequest);
        }

        public Task<ApiResponse> RemoveUserShadowBanAsync(UnbanRequest unbanRequest)
            => UnbanUserAsync(unbanRequest);

        public Task<QueryBannedUsersResponse> QueryBannedUsersAsync(QueryBannedUsersRequest queryBannedUsersRequest)
        {
            var endpoint = "/query_banned_users";

            return Get<QueryBannedUsersRequest, QueryBannedUsersRequestDTO, QueryBannedUsersResponse,
                QueryBannedUsersResponseDTO>(endpoint, queryBannedUsersRequest);
        }

        public Task<FlagResponse> FlagUserAsync(string targetUserId)
        {
            var endpoint = "/moderation/flag";

            var request = new FlagRequest
            {
                TargetUserId = targetUserId
            };

            return Post<FlagRequest, FlagRequestDTO, FlagResponse,
                FlagResponseDTO>(endpoint, request);
        }

        public Task<FlagResponse> FlagMessageAsync(string targetMessageId)
        {
            var endpoint = "/moderation/flag";

            var request = new FlagRequest
            {
                TargetMessageId = targetMessageId
            };

            return Post<FlagRequest, FlagRequestDTO, FlagResponse,
                FlagResponseDTO>(endpoint, request);
        }

        public Task<QueryMessageFlagsResponse> QueryMessageFlagsAsync(QueryMessageFlagsRequest queryMessageFlagsRequest)
        {
            var endpoint = "/moderation/flags/message";

            return Get<QueryMessageFlagsRequest, QueryMessageFlagsRequestDTO, QueryMessageFlagsResponse,
                QueryMessageFlagsResponseDTO>(endpoint, queryMessageFlagsRequest);
        }
    }
}