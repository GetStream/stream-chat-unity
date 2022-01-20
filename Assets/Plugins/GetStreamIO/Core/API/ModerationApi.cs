using System.Threading.Tasks;
using GetStreamIO.Core.DTO.Requests;
using GetStreamIO.Core.DTO.Responses;
using Plugins.GetStreamIO.Core.Requests;
using Plugins.GetStreamIO.Core.Responses;
using Plugins.GetStreamIO.Core.Web;
using Plugins.GetStreamIO.Libs.Http;
using Plugins.GetStreamIO.Libs.Logs;
using Plugins.GetStreamIO.Libs.Serialization;

namespace Plugins.GetStreamIO.Core.API
{
    public class ModerationApi : ApiClientBase, IModerationApi
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