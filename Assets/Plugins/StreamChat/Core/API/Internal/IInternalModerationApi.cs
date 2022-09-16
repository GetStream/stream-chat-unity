using System.Threading.Tasks;
using StreamChat.Core.DTO.Requests;
using StreamChat.Core.DTO.Responses;

namespace StreamChat.Core.API.Internal
{
    internal interface IInternalModerationApi
    {
        Task<MuteUserResponseDTO> MuteUserAsync(MuteUserRequestDTO muteUserRequest);

        Task<UnmuteResponseDTO> UnmuteUserAsync(UnmuteUserRequestDTO unmuteUserRequest);

        Task<ResponseDTO> BanUserAsync(BanRequestDTO banRequest);

        Task<ResponseDTO> UnbanUserAsync(string targetUserId, string type, string id);

        Task<ResponseDTO> ShadowBanUserAsync(BanRequestDTO shadowBanRequest);

        Task<ResponseDTO> RemoveUserShadowBanAsync(string targetUserId, string type, string id);

        Task<QueryBannedUsersResponseDTO> QueryBannedUsersAsync(QueryBannedUsersRequestDTO queryBannedUsersRequest);

        Task<FlagResponseDTO> FlagUserAsync(string targetUserId);

        Task<FlagResponseDTO> FlagMessageAsync(string targetMessageId);

        Task<QueryMessageFlagsResponseDTO> QueryMessageFlagsAsync(QueryMessageFlagsRequestDTO queryMessageFlagsRequest);
    }
}