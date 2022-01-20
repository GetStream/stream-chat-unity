namespace StreamChat.Core.Auth
{
    /// <summary>
    /// Provides authorization details
    /// </summary>
    public interface IAuthProvider
    {
        string ApiKey { get; }
        string UserToken { get; }
        string UserId { get; }
        string StreamAuthType { get; }
    }
}