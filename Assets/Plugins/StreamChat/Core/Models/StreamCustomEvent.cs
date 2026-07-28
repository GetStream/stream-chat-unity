using System;
using StreamChat.Core;
using StreamChat.Core.State;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.Models
{
    internal sealed class StreamCustomEvent : IStreamCustomEvent
    {
        public string Type { get; }

        public IStreamUser User { get; }

        public DateTimeOffset CreatedAt { get; }

        public IStreamCustomData CustomData { get; }

        internal StreamCustomEvent(string type, IStreamUser user, DateTimeOffset createdAt,
            StreamCustomData customData)
        {
            Type = type;
            User = user;
            CreatedAt = createdAt;
            CustomData = customData;
        }
    }
}
