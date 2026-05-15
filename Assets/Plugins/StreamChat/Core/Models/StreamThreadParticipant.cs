using System;
using System.Collections.Generic;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.State;
using StreamChat.Core.State.Caches;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.Models
{
    /// <summary>
    /// Represents a user that is participating in a thread.
    /// </summary>
    public class StreamThreadParticipant : IStateLoadableFrom<ThreadParticipantInternalDTO, StreamThreadParticipant>
    {
        /// <summary>
        /// Channel CID of the channel the thread belongs to
        /// </summary>
        public string ChannelCid { get; private set; }

        /// <summary>
        /// Date/time of when the user joined the thread
        /// </summary>
        public DateTimeOffset CreatedAt { get; private set; }

        /// <summary>
        /// Custom data attached to this thread participant
        /// </summary>
        public IReadOnlyDictionary<string, object> Custom => _custom;

        /// <summary>
        /// Date/time of the last time the user read the thread
        /// </summary>
        public DateTimeOffset LastReadAt { get; private set; }

        /// <summary>
        /// Date/time of the last message in the thread for this participant
        /// </summary>
        public DateTimeOffset? LastThreadMessageAt { get; private set; }

        /// <summary>
        /// Date/time the user left the thread
        /// </summary>
        public DateTimeOffset? LeftThreadAt { get; private set; }

        /// <summary>
        /// Thread (parent message) id
        /// </summary>
        public string ThreadId { get; private set; }

        /// <summary>
        /// The participating user
        /// </summary>
        public IStreamUser User { get; private set; }

        /// <summary>
        /// User id
        /// </summary>
        public string UserId { get; private set; }

        public StreamThreadParticipant()
        {
        }

        internal StreamThreadParticipant(IStreamUser user, string threadId, string channelCid,
            DateTimeOffset? lastThreadMessageAt)
        {
            User = user;
            UserId = user?.Id;
            ThreadId = threadId;
            ChannelCid = channelCid;
            LastThreadMessageAt = lastThreadMessageAt;
        }

        // Mirrors Android's local upsert in Thread.upsertReply: when a tracked participant
        // posts again, only their recency marker (and the freshest user snapshot) are touched;
        // ThreadId/ChannelCid/CreatedAt/LeftThreadAt remain authoritative from the server.
        internal void UpdateForNewReply(IStreamUser user, DateTimeOffset? lastThreadMessageAt)
        {
            if (user != null)
            {
                User = user;
                UserId = user.Id;
            }

            LastThreadMessageAt = lastThreadMessageAt;
        }

        StreamThreadParticipant IStateLoadableFrom<ThreadParticipantInternalDTO, StreamThreadParticipant>.LoadFromDto(
            ThreadParticipantInternalDTO dto, ICache cache)
        {
            ChannelCid = dto.ChannelCid;
            CreatedAt = dto.CreatedAt;
            LastReadAt = dto.LastReadAt;
            LastThreadMessageAt = dto.LastThreadMessageAt;
            LeftThreadAt = dto.LeftThreadAt;
            ThreadId = dto.ThreadId;
            UserId = dto.UserId;

            if (dto.User != null)
            {
                User = cache.TryCreateOrUpdate(dto.User);
                if (string.IsNullOrEmpty(UserId))
                {
                    UserId = dto.User.Id;
                }
            }

            _custom.Clear();
            if (dto.Custom != null)
            {
                foreach (var kv in dto.Custom)
                {
                    _custom[kv.Key] = kv.Value;
                }
            }

            return this;
        }

        private readonly Dictionary<string, object> _custom = new Dictionary<string, object>();
    }
}
