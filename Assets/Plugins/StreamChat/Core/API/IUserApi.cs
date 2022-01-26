using System.Threading.Tasks;
using StreamChat.Core.Requests;
using StreamChat.Core.Responses;

namespace Plugins.StreamChat.Core.API
{
    public interface IUserApi
    {
        Task<UsersResponse> QueryUsersAsync(QueryUsersRequest queryUsersRequest);
    }
}