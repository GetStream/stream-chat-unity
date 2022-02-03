using System;

namespace StreamChat.Core.Auth
{
    /// <summary>
    /// Wraps authorization data
    /// </summary>
    public readonly struct AuthData
    {
        public readonly string UserToken;
        public readonly string ApiKey;
        public readonly string UserId;

        public AuthData(string userToken, string apiKey, string userId)
        {
            UserToken = userToken;
            ApiKey = apiKey;
            UserId = userId;
        }
    }
}