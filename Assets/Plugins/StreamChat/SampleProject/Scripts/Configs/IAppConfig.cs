namespace StreamChat.SampleProject.Configs
{
    public interface IAppConfig
    {
        IViewFactoryConfig ViewFactoryConfig { get; }
        IEmojiConfig Emojis { get; }
    }
}