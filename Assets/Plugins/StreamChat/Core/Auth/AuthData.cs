using System;

namespace StreamChat.Core.Auth
{
    /// <summary>
    /// Wraps authorization data
    /// </summary>
    public readonly struct AuthData
    {
        public readonly Uri ServerUri;
        public readonly string UserToken;
        public readonly string ApiKey;
        public readonly string UserId;

        public AuthData(Uri serverUri, string userToken, string apiKey, string userId)
        {
            ServerUri = serverUri;
            UserToken = userToken;
            ApiKey = apiKey;
            UserId = userId;
        }
    }
}