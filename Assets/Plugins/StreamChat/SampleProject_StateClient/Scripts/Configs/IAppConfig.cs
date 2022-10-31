namespace StreamChat.SampleProject_StateClient.Configs
{
    public interface IAppConfig
    {
        IViewFactoryConfig ViewFactoryConfig { get; }
        IEmojiConfig Emojis { get; }
    }
}