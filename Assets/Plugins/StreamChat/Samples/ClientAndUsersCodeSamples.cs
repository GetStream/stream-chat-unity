using System.Collections.Generic;
using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Core.Requests;
using StreamChat.Core.StatefulModels;
using StreamChat.Libs.Auth;
using UnityEngine;

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
            var userId = StreamChatClient.SanitizeUserId(userName); // Remove disallowed characters
            var userToken = StreamChatClient.CreateDeveloperAuthToken(userId);
            var credentials = new AuthCredentials("API_KEY", userId, userToken);

// Create chat client
            var client = StreamChatClient.CreateDefaultClient();

// Connect user
            var localUserData = await client.ConnectUserAsync("API_KEY", userId, userToken);
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/init_and_users/?language=unity
        /// </summary>
        public void InitClient()
        {
            var client = StreamChatClient.CreateDefaultClient();
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/init_and_users/?language=unreal#connecting-the-user
        /// </summary>
        public async Task ConnectUser()
        {
            var client = StreamChatClient.CreateDefaultClient();

            var localUserData = await client.ConnectUserAsync("api_key", "chat_user", "chat_user_token");
// After await is complete the user is connected

// Alternatively, you subscribe to the IStreamChatClient.Connected event
            client.Connected += localUserData =>
            {
                // User is connected
            };
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/init_and_users/?language=unity#websocket-connections
        /// </summary>
        public async Task DisconnectUser()
        {
            var client = StreamChatClient.CreateDefaultClient();
            await client.DisconnectUserAsync();
        }

        // Managing users https://getstream.io/chat/docs/unity/update_users/?language=unity

        /// <summary>
        /// https://getstream.io/chat/docs/unity/update_users/?language=unity#delete-a-user
        /// </summary>
        public void DeleteUser()
        {
            //StreamTODO: Implement user delete
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
// Returns collection of IStreamUser
            var users = await Client.QueryUsersAsync(new Dictionary<string, object>
            {
                {
                    "id", new Dictionary<string, object>
                    {
                        {
                            "$in", new List<string>
                            {
                                "user-1", "user-2", "user-3"
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/query_users/?language=unity
        /// </summary>
        public async Task QueryBannedUsers()
        {
// Returns collection of StreamUserBanInfo
            var usersBanInfo = await Client.QueryBannedUsersAsync(new StreamQueryBannedUsersRequest
            {
                CreatedAtAfter = null, // Optional Banned after this date
                CreatedAtAfterOrEqual = null, // Optional Banned after or equal this date
                CreatedAtBefore = null, // Optional Banned before this date
                CreatedAtBeforeOrEqual = null, // Optional Banned before or equal this date
                FilterConditions = null, // Optional filter
                Limit = 30,
                Offset = 60,
                Sort = new List<StreamSortParam> // Optional sort
                {
                    new StreamSortParam
                    {
                        Field = "created_at",
                        Direction = -1,
                    }
                },
            });

            foreach (var banInfo in usersBanInfo)
            {
                Debug.Log(banInfo.User); // Which user
                Debug.Log(banInfo.Channel); // From which channel
                Debug.Log(banInfo.Reason); // Reason why banned
                Debug.Log(banInfo.Expires); // Optional expiry date
                Debug.Log(banInfo.Shadow); // Is this a shadow ban
                Debug.Log(banInfo.BannedBy); // Who created a ban
                Debug.Log(banInfo.CreatedAt); // Date when banned
            }
        }

        private IStreamChatClient Client { get; } = StreamChatClient.CreateDefaultClient();
    }
}