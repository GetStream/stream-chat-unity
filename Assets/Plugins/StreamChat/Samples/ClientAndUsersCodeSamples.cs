using System.Collections.Generic;
using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Libs.Auth;

namespace StreamChat.Samples
{
    internal sealed class ClientAndUsersCodeSamples
    {
        /// <summary>
        /// https://getstream.io/chat/docs/unity/tokens_and_authentication/?language=unity#developer-tokens
        /// </summary>
        public async Task DeveloperTokens()
        {
            var userName = "The Amazing Tom";
            var userId = StreamChatClient.SanitizeUserId(userName);
            var userToken = StreamChatClient.CreateDeveloperAuthToken(userId);
            var credentials = new AuthCredentials("API_KEY", userId, userToken);

            // Create chat client
            var client = StreamChatClient.CreateDefaultClient();
            
            // Connect user
            var localUserData = await client.ConnectUserAsync(credentials);
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/logout/?language=unity
        /// </summary>
        public async Task LogoutUser()
        {
            await Client.DisconnectUserAsync();
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/query_users/?language=unity
        /// </summary>
        public async Task QueryUsers()
        {
            var users = await Client.QueryUsersAsync(new Dictionary<string, object>
            {
                {
                    "id", new Dictionary<string, object>
                    {
                        {
                            "$in", new List<string>
                            {
                                "new-user-3", "new-user-4"
                            }
                        }
                    }
                }
            });
        }
        
        private IStreamChatClient Client { get; } = StreamChatClient.CreateDefaultClient();
    }
}