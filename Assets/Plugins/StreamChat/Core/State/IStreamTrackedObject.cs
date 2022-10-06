namespace StreamChat.Core.State
{
    /// <summary>
    /// Object with its state being automatically tracked by the <see cref="StreamChatStateClient"/>
    ///
    /// This means that this objects corresponds to an object on the Stream Chat server with the same ID
    /// its stated will be automatically updated whenever new information is received from the server
    /// </summary>
    public interface IStreamTrackedObject
    {
        string UniqueId { get; }
    }
}