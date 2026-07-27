using System;
using StreamChat.Core.StatefulModels;
using StreamChat.Core;

namespace StreamChat.Core.Models
{
    /// <summary>
    /// A custom event received on a channel.
    /// </summary>
    /// <remarks>https://getstream.io/chat/docs/unity/event_object/?language=unity#custom-events</remarks>
    public interface IStreamCustomEvent
    {
        /// <summary>Custom event type, e.g. "friendship-request".</summary>
        string Type { get; }

        /// <summary>User who sent the event (resolved from cache).</summary>
        IStreamUser User { get; }

        /// <summary>Server timestamp of the event.</summary>
        DateTimeOffset CreatedAt { get; }

        /// <summary>Optional thread parent id if the event was scoped to a thread.</summary>
        string ParentId { get; }

        /// <summary>Custom payload delivered with the event (top-level custom fields).</summary>
        IStreamCustomData CustomData { get; }
    }
}
