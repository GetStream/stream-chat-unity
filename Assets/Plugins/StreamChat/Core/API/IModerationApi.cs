using System.Threading.Tasks;
using StreamChat.Core.Requests;
using StreamChat.Core.Responses;

namespace StreamChat.Core.API
{
    public interface IModerationApi
    {
        Task<MuteUserResponse> MuteUserAsync(MuteUserRequest muteUserRequest);
    }
}