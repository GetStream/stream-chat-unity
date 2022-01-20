using System.Threading.Tasks;
using Plugins.GetStreamIO.Core.Requests;
using Plugins.GetStreamIO.Core.Responses;

namespace Plugins.GetStreamIO.Core.API
{
    public interface IModerationApi
    {
        Task<MuteUserResponse> MuteUserAsync(MuteUserRequest muteUserRequest);
    }
}