using StreamChat.Libs.Utils;

namespace StreamChat.Core.Auth
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
    }

    /// <summary>
    /// Extensions for <see cref="AuthCredentials"/>
    /// </summary>
    internal static class AuthCredentialsExt
    {
        public static AuthCredentials WithUserCredentials(this AuthCredentials authCredentials, string userId,
            string userToken)
            => new AuthCredentials(authCredentials.ApiKey, userId, userToken);
    }
}