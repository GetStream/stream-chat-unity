﻿using StreamChat.Core.State.Caches;
using StreamChat.Libs.Logs;

namespace StreamChat.Core.State
{
    internal interface ITrackedObjectContext
    {
        ICache Cache { get; }
        StreamChatStateClient StreamChatStateClient { get; }
        ILogs Logs { get; }
    }
}