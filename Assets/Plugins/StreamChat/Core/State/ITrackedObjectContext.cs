using StreamChat.Core.State;
using StreamChat.Libs.Logs;

namespace StreamChat.Core.State
{
    internal interface ITrackedObjectContext
    {
        ITrackedObjectsFactory TrackedObjectsFactory { get; }
        StreamChatStateClient StreamChatStateClient { get; }
        ILogs Logs { get; }
    }
}