namespace StreamChat.Core.LowLevelClient.Requests
{
    public class ShadowBanRequest : BanRequest
    {
        public new bool? Shadow => true;
    }
}