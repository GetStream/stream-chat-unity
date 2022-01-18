namespace Plugins.GetStreamIO.Core.API
{
    /// <summary>
    ///
    /// </summary>
    public static class ModerationEndpoints
    {
        public static string MuteUser() => "/moderation/mute";
        public static string UnmuteUser() => "/moderation/unmute";
    }
}