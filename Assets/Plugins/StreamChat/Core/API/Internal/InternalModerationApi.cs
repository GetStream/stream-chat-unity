using System.Threading.Tasks;
using StreamChat.Core.DTO.Requests;
using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Web;
using StreamChat.Libs.Http;
using StreamChat.Libs.Logs;
using StreamChat.Libs.Serialization;

namespace StreamChat.Core.API.Internal
{
    internal class InternalModerationApi : InternalApiClientBase, IInternalModerationApi
    {
        public InternalModerationApi(IHttpClient httpClient, ISerializer serializer, ILogs logs,
            IRequestUriFactory requestUriFactory)
            : base(httpClient, serializer, logs, requestUriFactory)
        {
        }

        public Task<MuteUserResponseDTO> MuteUserAsync(MuteUserRequestDTO muteUserRequest)
        {
            var endpoint = ModerationEndpoints.MuteUser();
            return Post<MuteUserRequestDTO, MuteUserResponseDTO>(endpoint, muteUserRequest);
        }

        public Task<UnmuteResponseDTO> UnmuteUserAsync(UnmuteUserRequestDTO unmuteUserRequest)
        {
            var endpoint = ModerationEndpoints.UnmuteUser();
            return Post<UnmuteUserRequestDTO, UnmuteResponseDTO>(endpoint, unmuteUserRequest);
        }

        public Task<ResponseDTO> BanUserAsync(BanRequestDTO banRequest)
        {
            const string endpoint = "/moderation/ban";
            return Post<BanRequestDTO, ResponseDTO>(endpoint, banRequest);
        }

        public Task<ResponseDTO> UnbanUserAsync(string targetUserId, string type, string id)
        {
            const string endpoint = "/moderation/ban";

            var parameters = QueryParameters.Default
                .Append("target_user_id", targetUserId)
                .Append("type", type)
                .Append("id", id);

            return Delete<ResponseDTO>(endpoint, parameters);
        }

        public Task<ResponseDTO> ShadowBanUserAsync(BanRequestDTO shadowBanRequest)
        {
            const string endpoint = "/moderation/ban";
            return Post<BanRequestDTO, ResponseDTO>(endpoint, shadowBanRequest);
        }

        public Task<ResponseDTO> RemoveUserShadowBanAsync(string targetUserId, string type, string id)
            => UnbanUserAsync(targetUserId, type, id);

        public Task<QueryBannedUsersResponseDTO> QueryBannedUsersAsync(QueryBannedUsersRequestDTO queryBannedUsersRequest)
        {
            const string endpoint = "/query_banned_users";
            return Get<QueryBannedUsersRequestDTO, QueryBannedUsersResponseDTO>(endpoint, queryBannedUsersRequest);
        }

        public Task<FlagResponseDTO> FlagUserAsync(string targetUserId)
        {
            const string endpoint = "/moderation/flag";

            var request = new FlagRequestDTO
            {
                TargetUserId = targetUserId
            };

            return Post<FlagRequestDTO, FlagResponseDTO>(endpoint, request);
        }

        public Task<FlagResponseDTO> FlagMessageAsync(string targetMessageId)
        {
            const string endpoint = "/moderation/flag";

            var request = new FlagRequestDTO
            {
                TargetMessageId = targetMessageId
            };

            return Post<FlagRequestDTO, FlagResponseDTO>(endpoint, request);
        }

        public Task<QueryMessageFlagsResponseDTO> QueryMessageFlagsAsync(QueryMessageFlagsRequestDTO queryMessageFlagsRequest)
        {
            const string endpoint = "/moderation/flags/message";
            return Get<QueryMessageFlagsRequestDTO, QueryMessageFlagsResponseDTO>(endpoint, queryMessageFlagsRequest);
        }
    }
}