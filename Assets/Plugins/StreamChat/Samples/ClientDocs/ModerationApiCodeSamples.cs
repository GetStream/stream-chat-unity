using System.Collections.Generic;
using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Core.Requests;

namespace Plugins.StreamChat.Samples.ClientDocs
{
    public class ModerationApiCodeSamples
    {
        public async Task MuteUser()
        {
            var muteUserRequest = new MuteUserRequest
            {
                TargetIds = new List<string> { "user-id-1" }
            };

            var muteUserResponse = await Client.ModerationApi.MuteUserAsync(muteUserRequest);
        }

        private IStreamChatClient Client;
    }
}