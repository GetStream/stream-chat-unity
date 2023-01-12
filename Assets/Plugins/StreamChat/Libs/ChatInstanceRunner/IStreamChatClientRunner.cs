namespace StreamChat.Libs.ChatInstanceRunner
{
    /// <summary>
    /// Runner is responsible for calling callbacks on the <see cref="IStreamChatClientEventsListener"/>
    /// </summary>
    public interface IStreamChatClientRunner
    {
        /// <summary>
        /// Pass environment callbacks to the <see cref="IStreamChatClientEventsListener"/> and react to its events
        /// </summary>
        void RunChatInstance(IStreamChatClientEventsListener streamChatInstance);
    }
}