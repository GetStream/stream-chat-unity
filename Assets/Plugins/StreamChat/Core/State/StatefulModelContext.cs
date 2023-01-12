using StreamChat.Core.State.Caches;
using StreamChat.Libs.Logs;
using StreamChat.Libs.Serialization;

namespace StreamChat.Core.State
{
    internal sealed class StatefulModelContext : IStatefulModelContext
    {
        public ICache Cache { get; }
        public StreamChatClient Client { get; }
        public ILogs Logs { get; }
        public ISerializer Serializer { get; }

        public StatefulModelContext(ICache cache, StreamChatClient streamChatClient, ISerializer serializer, ILogs logs)
        {
            Cache = cache;
            Client = streamChatClient;
            Serializer = serializer;
            Logs = logs;
        }
    }
}