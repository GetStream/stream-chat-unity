using StreamChat.Core.State.Caches;
using StreamChat.Libs.Logs;

namespace StreamChat.Core.State
{
    internal sealed class TrackedObjectContext : ITrackedObjectContext
    {
        public ICache Cache { get; }
        public StreamChatClient Client { get; }
        public ILogs Logs { get; }

        public TrackedObjectContext(ICache cache, StreamChatClient streamChatClient, ILogs logs)
        {
            Cache = cache;
            Client = streamChatClient;
            Logs = logs;
        }
    }
}