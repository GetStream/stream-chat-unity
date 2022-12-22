using StreamChat.Libs.Utils;

namespace StreamChat.Libs.Auth
{
    /// <summary>
    /// Wraps authorization data
    /// </summary>
    public readonly struct AuthCredentials
    {
        public readonly string ApiKey;
        public readonly string UserId;
        public readonly string UserToken;

        public AuthCredentials(string apiKey, string userId, string userToken)
        {
            ApiKey = apiKey;
            UserId = userId;
            UserToken = userToken;
        }

        public bool IsAnyEmpty() => ApiKey.IsNullOrEmpty() || UserId.IsNullOrEmpty() || UserToken.IsNullOrEmpty();

        public AuthCredentials CreateWithNewUserToken(string token) => new AuthCredentials(ApiKey, UserId, token);
    }
}