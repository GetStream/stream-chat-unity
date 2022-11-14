﻿using StreamChat.Core.State.Caches;
using StreamChat.Libs.Logs;

namespace StreamChat.Core.State
{
    internal sealed class TrackedObjectContext : ITrackedObjectContext
    {
        public ICache Cache { get; }
        public StreamChatStateClient StreamChatStateClient { get; }
        public ILogs Logs { get; }

        public TrackedObjectContext(ICache cache, StreamChatStateClient streamChatStateClient, ILogs logs)
        {
            Cache = cache;
            StreamChatStateClient = streamChatStateClient;
            Logs = logs;
        }
    }
}