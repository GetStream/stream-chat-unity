using System.Threading.Tasks;
using StreamChat.Core.API;
using StreamChat.Core.DTO.Requests;
using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Requests;
using StreamChat.Core.Responses;
using StreamChat.Core.Web;
using StreamChat.Libs.Http;
using StreamChat.Libs.Logs;
using StreamChat.Libs.Serialization;

namespace Plugins.StreamChat.Core.API
{
    public class UserApi : ApiClientBase, IUserApi
    {
        public UserApi(IHttpClient httpClient, ISerializer serializer, ILogs logs,
            IRequestUriFactory requestUriFactory)
            : base(httpClient, serializer, logs, requestUriFactory)
        {
        }

        public Task<UsersResponse> QueryUsersAsync(QueryUsersRequest queryUsersRequest) =>
            Get<QueryUsersRequest, QueryUsersRequestDTO, UsersResponse, UsersResponseDTO>("/users",
                queryUsersRequest);
    }
}