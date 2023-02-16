using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Core.QueryBuilders.Filters;
using StreamChat.Core.QueryBuilders.Filters.Users;
using StreamChat.Core.QueryBuilders.Sort;
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


        #region Managing Users

        /// <summary>
        /// https://getstream.io/chat/docs/unity/update_users/?language=unity#server-side-user-updates-(batch)
        /// </summary>
        public async Task UserUpdates()
        {
// Only Id field is required, the rest is optional
            var createOrUpdateUser = new StreamUserUpsertRequest
            {
                Id = "my-user-id",
                // BanExpires = DateTimeOffset.Now.AddDays(7),
                // Banned = true,
                // Invisible = true,
                // Role = "user",
                // Name = "David",
                // Image = "image-url", // You can upload image to Stream CDN or your own
                // CustomData = new StreamCustomDataRequest
                //{
                //    { "Age", 24 },
                //    { "Passions", new string[] { "Tennis", "Football", "Basketball" } }
                //}
            };

// Upsert means: update user with a given ID or create a new one if it doesn't exist
            var users = await Client.UpsertUsers(new[] { createOrUpdateUser });
        }

        public async Task UserUpdatesMultiple()
        {
            var usersToCreateOrUpdate = new[]
            {
                new StreamUserUpsertRequest
                {
                    Id = "my-user-id",
                    Role = "user",
                },
                new StreamUserUpsertRequest
                {
                    Id = "my-user-id-2",
                    // BanExpires = DateTimeOffset.Now.AddDays(7),
                    // Banned = true,
                    // Invisible = true,
                    // Role = "user",
                    // Name = "David",
                    // Image = "image-url", // You can upload image to Stream CDN or your own
                    // CustomData = new StreamCustomDataRequest
                    //{
                    //    { "Age", 24 },
                    //    { "Passions", new string[] { "Tennis", "Football", "Basketball" } }
                    //}
                },
            };

// Upsert means: update user with a given ID or create a new one if it doesn't exist
            var users = await Client.UpsertUsers(usersToCreateOrUpdate);
        }

        #endregion

        #region Querying Users

        /// <summary>
        /// https://getstream.io/chat/docs/unity/query_users/?language=unity
        /// </summary>
        public async Task QueryUsers()
        {
            var filters = new IFieldFilterRule[]
            {
                UserFilter.Id.In("user-1", "user-2", "user-3")
            };
// Returns collection of IStreamUser
            var users = await Client.QueryUsersAsync(filters);
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/query_users/?language=unity
        /// </summary>
        public async Task QueryUsersPagination()
        {
            var lastWeek = DateTime.Now.AddDays(-7);
            var filters = new IFieldFilterRule[]
            {
                UserFilter.CreatedAt.GreaterThanOrEquals(lastWeek)
            };

            // Order results by one or multiple fields e.g
            var sort = UsersSort.OrderByDescending(UserSortField.CreatedAt);

            var limit = 30; // How many records per page
            var offset = 0; // How many records to skip e.g. offset = 30 -> page 2, offset = 60 -> page 3, etc.
            
            // Returns collection of IStreamUser
            var users = await Client.QueryUsersAsync(filters, sort, offset, limit);
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/query_users/?language=unity#1.-by-name
        /// </summary>
        public async Task QueryUsersUsingAutocompleteByName()
        {
            var filters = new IFieldFilterRule[]
            {
                UserFilter.Name.Autocomplete("Ro")
            };
// Returns collection of IStreamUser
            var users = await Client.QueryUsersAsync(filters);
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/query_users/?language=unity#2.-by-id
        /// </summary>
        public async Task QueryUsersUsingAutocompleteById()
        {
            var filters = new IFieldFilterRule[]
            {
                // Return all users with Id starting with `Ro` like: Roxy, Roxanne, Rover
                UserFilter.Name.Autocomplete("Ro")
            };
// Returns collection of IStreamUser
            var users = await Client.QueryUsersAsync(filters);
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

        #endregion

        private IStreamChatClient Client { get; } = StreamChatClient.CreateDefaultClient();
    }
}