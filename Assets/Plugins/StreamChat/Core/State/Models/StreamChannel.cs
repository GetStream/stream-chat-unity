using System.Collections.Generic;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.State;
using StreamChat.Core.StreamChat.Core.State;

namespace StreamChat.Core.State.Models
{
    /// <summary>
    /// Stream channel
    ///
    /// This object is tracked by <see cref="StreamChatStateClient"/> meaning its state will be automatically updated
    /// </summary>
    public class StreamChannel : StreamTrackedObjectBase<StreamChannel>, ILoadableFrom<ChannelStateResponseInternalDTO, StreamChannel>
    {
        /// <summary>
        /// Whether auto translation is enabled or not
        /// </summary>
        public bool? AutoTranslationEnabled { get; set; }

        /// <summary>
        /// Language to translate to when auto translation is active
        /// </summary>
        public string AutoTranslationLanguage { get; set; }

        /// <summary>
        /// Channel CID (&lt;type&gt;:&lt;id&gt;)
        /// </summary>
        public string Cid { get; set; }

        /// <summary>
        /// Channel configuration
        /// </summary>
        //public ChannelConfig Config { get; set; } //Todo: research if this is needed

        /// <summary>
        /// Cooldown period after sending each message
        /// </summary>
        public int? Cooldown { get; set; }

        /// <summary>
        /// Date/time of creation
        /// </summary>
        public System.DateTimeOffset? CreatedAt { get; set; }

        /// <summary>
        /// Creator of the channel
        /// </summary>
        public StreamUser CreatedBy { get; set; }

        /// <summary>
        /// Date/time of deletion
        /// </summary>
        public System.DateTimeOffset? DeletedAt { get; set; }

        public bool? Disabled { get; set; }

        /// <summary>
        /// Whether channel is frozen or not
        /// </summary>
        public bool? Frozen { get; set; }

        /// <summary>
        /// Whether this channel is hidden by current user or not
        /// </summary>
        public bool? Hidden { get; set; }

        /// <summary>
        /// Date since when the message history is accessible
        /// </summary>
        public System.DateTimeOffset? HideMessagesBefore { get; set; }

        /// <summary>
        /// Channel unique ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Date of the last message sent
        /// </summary>
        public System.DateTimeOffset? LastMessageAt { get; set; }

        /// <summary>
        /// Number of members in the channel
        /// </summary>
        public int? MemberCount { get; set; }

        /// <summary>
        /// List of channel members (max 100)
        /// </summary>
        public System.Collections.Generic.List<StreamChannelMember> Members { get; set; }

        /// <summary>
        /// Date of mute expiration
        /// </summary>
        public System.DateTimeOffset? MuteExpiresAt { get; set; }

        /// <summary>
        /// Whether this channel is muted or not
        /// </summary>
        public bool? Muted { get; set; }

        /// <summary>
        /// List of channel capabilities of authenticated user
        /// </summary>
        public System.Collections.Generic.List<string> OwnCapabilities { get; set; }

        /// <summary>
        /// Team the channel belongs to (multi-tenant only)
        /// </summary>
        public string Team { get; set; }

        public System.DateTimeOffset? TruncatedAt { get; set; }

        public StreamUser TruncatedBy { get; set; }

        /// <summary>
        /// Type of the channel
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Date/time of the last update
        /// </summary>
        public System.DateTimeOffset? UpdatedAt { get; set; }

        public string Name { get; set; }










        /// <summary>
        /// Whether this channel is hidden or not
        /// </summary>
        //public bool? Hidden { get; set; }

        /// <summary>
        /// Messages before this date are hidden from the user
        /// </summary>
        //public DateTimeOffset? HideMessagesBefore { get; set; }

        /// <summary>
        /// List of channel members
        /// </summary>
        //public List<StreamChannelMember> Members { get; set; }

        /// <summary>
        /// Current user membership object
        /// </summary>
        public StreamChannelMember Membership { get; set; }

        /// <summary>
        /// List of channel messages
        /// </summary>
        public List<StreamMessage> Messages { get; set; }

        /// <summary>
        /// Pending messages that this user has sent
        /// </summary>
        //public List<PendingMessage> PendingMessages { get; set; } //Todo: do we create StreamPendingMessage?

        /// <summary>
        /// List of pinned messages in the channel
        /// </summary>
        public List<StreamMessage> PinnedMessages { get; set; }

        /// <summary>
        /// List of read states
        /// </summary>
        public List<StreamReadState> Read { get; set; }

        /// <summary>
        /// Number of channel watchers
        /// </summary>
        public int? WatcherCount { get; set; }

        /// <summary>
        /// List of user who is watching the channel
        /// </summary>
        public List<StreamUser> Watchers { get; set; }

        internal static StreamChannel Create(string uniqueId, IRepository<StreamChannel> repository)
            => new StreamChannel(uniqueId, repository);

        internal StreamChannel(string uniqueId, IRepository<StreamChannel> repository)
            : base(uniqueId, repository)
        {
        }

        internal void UpdateFrom(ChannelStateResponseInternalDTO channelStateResponseInternalDto)
        {

        }

        internal void AddMessage(StreamMessage message)
        {

        }

        internal void UpdateMessage(StreamMessage message)
        {

        }

        public void DeleteMessage(string messageId, bool isHardDelete)
        {
            for (int i = Messages.Count - 1; i >= 0; i--)
            {
                var message = Messages[i];
                if (message.Id != messageId)
                {
                    continue;
                }

                if (isHardDelete)
                {
                    Messages.RemoveAt(i);
                    return;
                }

                message.SoftDelete();
                return;
            }
        }

        StreamChannel ILoadableFrom<ChannelStateResponseInternalDTO, StreamChannel>.LoadFromDto(ChannelStateResponseInternalDTO dto)
        {
            throw new System.NotImplementedException();
        }

        protected override StreamChannel Self => this;

        protected override string InternalUniqueId
        {
            get => Cid;
            set => Cid = value;
        }
    }
}