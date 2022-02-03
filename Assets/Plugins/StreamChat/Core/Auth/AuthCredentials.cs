namespace StreamChat.Core.Auth
{
    /// <summary>
    /// Wraps authorization data
    /// </summary>
    public readonly struct AuthCredentials
    {
        public readonly string UserToken;
        public readonly string ApiKey;
        public readonly string UserId;

        public AuthCredentials(string apiKey, string userToken, string userId)
        {
            ApiKey = apiKey;
            UserToken = userToken;
            UserId = userId;
        }
    }

    /// <summary>
    /// Extensions for <see cref="AuthCredentials"/>
    /// </summary>
    public static class AuthCredentialsExt
    {
        public static AuthCredentials WithUserCredentials(this AuthCredentials authCredentials, string userToken,
            string userId)
            => new AuthCredentials(authCredentials.ApiKey, userToken, userId);
    }
}