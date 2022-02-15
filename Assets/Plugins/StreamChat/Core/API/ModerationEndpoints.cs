namespace StreamChat.Core.API
{
    /// <summary>
    ///
    /// </summary>
    internal static class ModerationEndpoints
    {
        public static string MuteUser() => "/moderation/mute";
        public static string UnmuteUser() => "/moderation/unmute";
    }
}