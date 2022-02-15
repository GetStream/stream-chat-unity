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
    }
}