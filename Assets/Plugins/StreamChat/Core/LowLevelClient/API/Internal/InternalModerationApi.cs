using System.Threading.Tasks;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.Web;
using StreamChat.Libs.Http;
using StreamChat.Libs.Logs;
using StreamChat.Libs.Serialization;

namespace StreamChat.Core.LowLevelClient.API.Internal
{
    internal class InternalModerationApi : InternalApiClientBase, IInternalModerationApi
    {
        public InternalModerationApi(IHttpClient httpClient, ISerializer serializer, ILogs logs,
            IRequestUriFactory requestUriFactory, IStreamChatLowLevelClient lowLevelClient)
            : base(httpClient, serializer, logs, requestUriFactory, lowLevelClient)
        {
        }

        public Task<MuteUserResponseInternalDTO> MuteUserAsync(MuteUserRequestInternalDTO muteUserRequest)
        {
            var endpoint = ModerationEndpoints.MuteUser();
            return Post<MuteUserRequestInternalDTO, MuteUserResponseInternalDTO>(endpoint, muteUserRequest);
        }

        public Task<UnmuteResponseInternalDTO> UnmuteUserAsync(UnmuteUserRequestInternalDTO unmuteUserRequest)
        {
            var endpoint = ModerationEndpoints.UnmuteUser();
            return Post<UnmuteUserRequestInternalDTO, UnmuteResponseInternalDTO>(endpoint, unmuteUserRequest);
        }

        public Task<ResponseInternalDTO> BanUserAsync(BanRequestInternalDTO banRequest)
        {
            const string endpoint = "/moderation/ban";
            return Post<BanRequestInternalDTO, ResponseInternalDTO>(endpoint, banRequest);
        }

        public Task<ResponseInternalDTO> UnbanUserAsync(string targetUserId, string type, string id)
        {
            const string endpoint = "/moderation/ban";

            var parameters = QueryParameters.Default
                .Append("target_user_id", targetUserId)
                .Append("type", type)
                .Append("id", id);

            return Delete<ResponseInternalDTO>(endpoint, parameters);
        }

        public Task<ResponseInternalDTO> ShadowBanUserAsync(BanRequestInternalDTO shadowBanRequest)
        {
            const string endpoint = "/moderation/ban";
            return Post<BanRequestInternalDTO, ResponseInternalDTO>(endpoint, shadowBanRequest);
        }

        public Task<ResponseInternalDTO> RemoveUserShadowBanAsync(string targetUserId, string type, string id)
            => UnbanUserAsync(targetUserId, type, id);

        public Task<QueryBannedUsersResponseInternalDTO> QueryBannedUsersAsync(QueryBannedUsersRequestInternalDTO queryBannedUsersRequest)
        {
            const string endpoint = "/query_banned_users";
            return Get<QueryBannedUsersRequestInternalDTO, QueryBannedUsersResponseInternalDTO>(endpoint, queryBannedUsersRequest);
        }

        public Task<FlagResponseInternalDTO> FlagUserAsync(string targetUserId)
        {
            const string endpoint = "/moderation/flag";

            var request = new FlagRequestInternalDTO
            {
                TargetUserId = targetUserId
            };

            return Post<FlagRequestInternalDTO, FlagResponseInternalDTO>(endpoint, request);
        }

        public Task<FlagResponseInternalDTO> FlagMessageAsync(string targetMessageId)
        {
            const string endpoint = "/moderation/flag";

            var request = new FlagRequestInternalDTO
            {
                TargetMessageId = targetMessageId
            };

            return Post<FlagRequestInternalDTO, FlagResponseInternalDTO>(endpoint, request);
        }

        public Task<QueryMessageFlagsResponseInternalDTO> QueryMessageFlagsAsync(QueryMessageFlagsRequestInternalDTO queryMessageFlagsRequest)
        {
            const string endpoint = "/moderation/flags/message";
            return Get<QueryMessageFlagsRequestInternalDTO, QueryMessageFlagsResponseInternalDTO>(endpoint, queryMessageFlagsRequest);
        }
    }
}