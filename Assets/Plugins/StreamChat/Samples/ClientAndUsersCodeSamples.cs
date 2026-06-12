using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Core.QueryBuilders.Filters;
using StreamChat.Core.QueryBuilders.Filters.Users;
using StreamChat.Core.QueryBuilders.Sort;
using StreamChat.Core.Requests;
using StreamChat.Libs.Auth;
using UnityEngine;

namespace StreamChat.Samples
{
    internal sealed class ClientAndUsersCodeSamples
    {
        /// <summary>
        /// https://getstream.io/chat/docs/unity/tokens-and-authentication/?language=unity#developer-tokens
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
            var localUserData = await client.ConnectUserAsync(credentials);
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/init-and-users/?language=unity
        /// </summary>
        public void InitClient()
        {
            var client = StreamChatClient.CreateDefaultClient();
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/init-and-users/?language=unity#connecting-users
        /// </summary>
        public async Task ConnectUser()
        {
            var client = StreamChatClient.CreateDefaultClient();

// 1. Static JWT token - prototyping/tests only. In production, issue tokens
//    from your backend.
            var localUserData = await client.ConnectUserAsync("api_key", "chat_user", "chat_user_token");
// Await returns once connected; you can also subscribe to client.Connected.
            client.Connected += connectedUser => { /* User is connected */ };

// 2. Production: implement ITokenProvider. The SDK calls GetTokenAsync on
//    initial connect, reconnect, and expiration, so the WebSocket stays
//    connected across token refreshes automatically.
            var tokenProvider = new YourTokenProvider();
            await client.ConnectUserAsync("api_key", "chat_user", tokenProvider);
        }

// Your backend MUST authenticate the caller before issuing a Stream token -
// never expose an endpoint that returns a token for any userId.
        public class YourTokenProvider : ITokenProvider
        {
            public Task<string> GetTokenAsync(string userId)
                => Task.FromResult("token-fetched-from-your-backend");
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/init-and-users/?language=unity#disconnecting-users
        /// </summary>
        public async Task DisconnectUser()
        {
            var client = StreamChatClient.CreateDefaultClient();
            await client.DisconnectUserAsync();
        }

        #region Managing Users

        /// <summary>
        /// https://getstream.io/chat/docs/unity/update-users/?language=unity#creating-and-updating-users-server-side
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
            var users = await Client.UpsertUsersAsync(new[] { createOrUpdateUser });
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/update-users/?language=unity#creating-and-updating-users-server-side
        /// </summary>
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
            var users = await Client.UpsertUsersAsync(usersToCreateOrUpdate);
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/update-users/?language=unity#server-side-partial-updates
        /// </summary>
        public void PartialUpdateUser()
        {
// This is a server-side only feature, choose any of our server-side SDKs to use it
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/update-users/?language=unity#unique-usernames
        /// </summary>
        public void UniqueUsernames()
        {
// This can be set in https://dashboard.getstream.io/ -> Open your application -> Overview -> Authentication
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/update-users/?language=unity#deactivate-a-user
        /// </summary>
        public void DeactivateUser()
        {
// This is a server-side only feature, choose any of our server-side SDKs to use it
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/update-users/?language=unity#deactivate-many-users
        /// </summary>
        public void DeactivateManyUsers()
        {
// This is a server-side only feature, choose any of our server-side SDKs to use it
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/update-users/?language=unity#reactivate-a-user
        /// </summary>
        public void ReactivateUser()
        {
// This is a server-side only feature, choose any of our server-side SDKs to use it
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/update-users/?language=unity#deleting-many-users
        /// </summary>
        public void DeleteUsers()
        {
// This is a server-side only feature, choose any of our server-side SDKs to use it
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/update-users/?language=unity#restoring-deleted-users
        /// </summary>
        public void RestoreUsers()
        {
// This is a server-side only feature, choose any of our server-side SDKs to use it
        }

        #endregion

        #region Querying Users

        /// <summary>
        /// https://getstream.io/chat/docs/unity/update-users/?language=unity#querying-users
        /// </summary>
        public async Task QueryUsers()
        {
            var filters = new IFieldFilterRule[]
            {
                UserFilter.Id.In("john", "jack", "jessie")
            };

            var sort = UsersSort.OrderByDescending(UserSortField.LastActive);
            var limit = 10;
            var offset = 0;

// Returns collection of IStreamUser
            var users = await Client.QueryUsersAsync(filters, sort, offset, limit);
        }

        // Code-only reference: the docs page does not host a separate offset/limit
        // pagination snippet for QueryUsers (the limit + offset arguments are
        // demonstrated inside the main "Querying Users" tab).
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
        /// https://getstream.io/chat/docs/unity/update-users/?language=unity#querying-with-autocomplete
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

        // Code-only reference: the docs page only hosts a single autocomplete
        // snippet (under "Querying with Autocomplete") which is mirrored by
        // QueryUsersUsingAutocompleteByName above.
        public async Task QueryUsersUsingAutocompleteById()
        {
            var filters = new IFieldFilterRule[]
            {
                // Return all users whose Id starts with `Ro` (e.g. Roxy, Roxanne, Rover)
                UserFilter.Id.Autocomplete("Ro")
            };
// Returns collection of IStreamUser
            var users = await Client.QueryUsersAsync(filters);
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/update-users/?language=unity#querying-inactive-users
        /// </summary>
        public void QueryInactiveUsers()
        {
// The $exists operator on last_active is not yet exposed in the Unity SDK.
// Use the UserFilter.LastActive comparison operators (e.g. LessThan / GreaterThanOrEquals)
// to filter users by their last-active timestamp.
        }

        // Code-only reference: QueryBannedUsersAsync targets a different endpoint
        // that is not documented on the Managing Users page.
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