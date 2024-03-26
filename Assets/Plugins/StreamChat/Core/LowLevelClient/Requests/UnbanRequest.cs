namespace StreamChat.Core.LowLevelClient.Requests
{
    public partial class UnbanRequest
    {
        /// <summary>
        /// Channel ID to unban user in
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Channel type to ban user in
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// ID of user to unban
        /// </summary>
        public string TargetUserId { get; set; }
    }
}