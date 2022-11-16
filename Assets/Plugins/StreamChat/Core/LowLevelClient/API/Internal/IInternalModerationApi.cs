using System.Threading.Tasks;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.InternalDTO.Responses;

namespace StreamChat.Core.LowLevelClient.API.Internal
{
    internal interface IInternalModerationApi
    {
        Task<MuteUserResponseInternalDTO> MuteUserAsync(MuteUserRequestInternalDTO muteUserRequest);

        Task<UnmuteResponseInternalDTO> UnmuteUserAsync(UnmuteUserRequestInternalDTO unmuteUserRequest);

        Task<ResponseInternalDTO> BanUserAsync(BanRequestInternalDTO banRequest);

        Task<ResponseInternalDTO> UnbanUserAsync(string targetUserId, string type, string id);

        Task<ResponseInternalDTO> ShadowBanUserAsync(BanRequestInternalDTO shadowBanRequest);

        Task<ResponseInternalDTO> RemoveUserShadowBanAsync(string targetUserId, string type, string id);

        Task<QueryBannedUsersResponseInternalDTO> QueryBannedUsersAsync(QueryBannedUsersRequestInternalDTO queryBannedUsersRequest);

        Task<FlagResponseInternalDTO> FlagUserAsync(string targetUserId);

        Task<FlagResponseInternalDTO> FlagMessageAsync(string targetMessageId);

        Task<QueryMessageFlagsResponseInternalDTO> QueryMessageFlagsAsync(QueryMessageFlagsRequestInternalDTO queryMessageFlagsRequest);
    }
}