namespace StreamChat.Core.State
{
    /// <summary>
    /// Object with its state being automatically tracked by the <see cref="StreamChatStateClient"/>
    /// </summary>
    public interface IStreamTrackedObject
    {
        string UniqueId { get; }
    }
}