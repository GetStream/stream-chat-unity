using StreamChat.Core.State;
using StreamChat.Libs.Logs;

namespace StreamChat.Core.State
{
    internal class TrackedObjectContext : ITrackedObjectContext
    {
        public ITrackedObjectsFactory TrackedObjectsFactory { get; }
        public StreamChatStateClient StreamChatStateClient { get; }
        public ILogs Logs { get; }

        public TrackedObjectContext(ITrackedObjectsFactory trackedObjectsFactory, StreamChatStateClient streamChatStateClient, ILogs logs)
        {
            TrackedObjectsFactory = trackedObjectsFactory;
            StreamChatStateClient = streamChatStateClient;
            Logs = logs;
        }
    }
}