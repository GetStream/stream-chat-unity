namespace StreamChat.Core.LowLevelClient.API
{
    /// <summary>
    /// Moderation API Endpoints
    /// </summary>
    internal static class ModerationEndpoints
    {
        public static string MuteUser() => "/moderation/mute";

        public static string UnmuteUser() => "/moderation/unmute";
    }
}