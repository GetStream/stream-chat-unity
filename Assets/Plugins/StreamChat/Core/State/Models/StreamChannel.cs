using System;
using System.Collections.Generic;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.State;

namespace StreamChat.Core.State.Models
{
    // TODO: change all fields to readonly

    /// <summary>
    /// Stream channel where a group of <see cref="StreamUser"/>'s can chat
    ///
    /// This object is tracked by <see cref="StreamChatStateClient"/> meaning its state will be automatically updated
    /// </summary>
    public class StreamChannel : StreamTrackedObjectBase<StreamChannel>,
        IUpdateableFrom<ChannelStateResponseInternalDTO, StreamChannel>,
        ILoadableFrom<ChannelStateResponseInternalDTO, StreamChannel>
    {
        #region Channel

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
        //public ChannelConfig Config { get; set; } // Re add

        /// <summary>
        /// Cooldown period after sending each message
        /// </summary>
        public int? Cooldown { get; set; }

        /// <summary>
        /// Date/time of creation
        /// </summary>
        public DateTimeOffset? CreatedAt { get; set; }

        /// <summary>
        /// Creator of the channel
        /// </summary>
        public StreamUser CreatedBy { get; set; }

        /// <summary>
        /// Date/time of deletion
        /// </summary>
        public DateTimeOffset? DeletedAt { get; set; }

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
        public DateTimeOffset? HideMessagesBefore { get; set; }

        /// <summary>
        /// Channel unique ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Date of the last message sent
        /// </summary>
        public DateTimeOffset? LastMessageAt { get; set; }

        /// <summary>
        /// Number of members in the channel
        /// </summary>
        public int? MemberCount { get; set; }

        /// <summary>
        /// List of channel members (max 100)
        /// </summary>
        public IReadOnlyList<StreamChannelMember> Members => _members;

        /// <summary>
        /// Date of mute expiration
        /// </summary>
        public DateTimeOffset? MuteExpiresAt { get; set; }

        /// <summary>
        /// Whether this channel is muted or not
        /// </summary>
        public bool? Muted { get; set; }

        /// <summary>
        /// List of channel capabilities of authenticated user
        /// </summary>
        public List<string> OwnCapabilities { get; set; }

        /// <summary>
        /// Team the channel belongs to (multi-tenant only)
        /// </summary>
        public string Team { get; set; }

        public DateTimeOffset? TruncatedAt { get; set; }

        public StreamUser TruncatedBy { get; set; }

        /// <summary>
        /// Type of the channel
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Date/time of the last update
        /// </summary>
        public DateTimeOffset? UpdatedAt { get; set; }

        public string Name { get; set; }

        #endregion

        #region ChannelState

        /// <summary>
        /// Whether this channel is hidden or not
        /// </summary>
        //public bool? Hidden { get; set; } // DUPLICATE

        /// <summary>
        /// Messages before this date are hidden from the user
        /// </summary>
        //public DateTimeOffset? HideMessagesBefore { get; set; } // DUPLICATE

        /// <summary>
        /// List of channel members
        /// </summary>
        //public List<StreamChannelMember> Members { get; set; } // DUPLICATE

        /// <summary>
        /// Current user membership object
        /// </summary>
        public StreamChannelMember Membership { get; set; }

        /// <summary>
        /// List of channel messages
        /// </summary>
        public IReadOnlyList<StreamMessage> Messages => _messages;

        /// <summary>
        /// Pending messages that this user has sent
        /// </summary>
        //public System.Collections.Generic.List<PendingMessage> PendingMessages { get; set; } //TODO: Re add

        /// <summary>
        /// List of pinned messages in the channel
        /// </summary>
        public IReadOnlyList<StreamMessage> PinnedMessages => _pinnedMessages;

        /// <summary>
        /// List of read states
        /// </summary>
        //public List<Read> Read { get; set; } //TODO: Re add

        /// <summary>
        /// Number of channel watchers
        /// </summary>
        public int? WatcherCount { get; set; }

        /// <summary>
        /// List of user who is watching the channel
        /// </summary>
        public List<StreamUser> Watchers { get; set; }

        #endregion

        public Dictionary<string, object> AdditionalProperties { get; set; }

        internal static StreamChannel Create(string uniqueId, IRepository<StreamChannel> repository)
            => new StreamChannel(uniqueId, repository);

        internal StreamChannel(string uniqueId, IRepository<StreamChannel> repository)
            : base(uniqueId, repository)
        {
        }

        void IUpdateableFrom<ChannelStateResponseInternalDTO, StreamChannel>.UpdateFromDto(
            ChannelStateResponseInternalDTO dto, ICache cache)
        {
            //Channel = Channel.TryLoadFromDto(dto.Channel);
            Hidden = dto.Hidden;
            HideMessagesBefore = dto.HideMessagesBefore;
            _members.TryReplaceTrackedItems(dto.Members, cache.ChannelMembers);
            Membership = cache.CreateOrUpdate(dto.Membership);
            _messages.TryReplaceTrackedItems(dto.Messages, cache.Messages);
            _pinnedMessages.TryReplaceTrackedItems(dto.PinnedMessages, cache.Messages);
            //Read = Read.TryLoadFromDtoCollection(dto.Read);
            WatcherCount = dto.WatcherCount;
            //Watchers = Watchers.TryLoadFromDtoCollection(dto.Watchers);
            AdditionalProperties = dto.AdditionalProperties;
        }

        private List<StreamChannelMember> UpdateList(IEnumerable<ChannelMemberInternalDTO> dtos,
            IRepository<StreamChannelMember> rep)
        {
            if (dtos == null)
            {
                return null;
            }

            var result = new List<StreamChannelMember>();
            foreach (var dto in dtos)
            {
                //get id?
                var id = dto.UserId;

                var trackedItem = rep.CreateOrUpdate<StreamChannelMember, ChannelMemberInternalDTO>(id, dto);
                result.Add(trackedItem);
            }

            return result;
        }

        private List<TTracked> UpdateList2<TTracked, TDto>(IEnumerable<TDto> dtos, IRepository<TTracked> rep)
            where TTracked : class, IStreamTrackedObject, IUpdateableFrom<TDto, TTracked>
        {
            if (dtos == null)
            {
                return null;
            }

            var result = new List<TTracked>();
            foreach (var dto in dtos)
            {
                //get id?

                var id = "dto.UserId";

                var trackedItem = rep.CreateOrUpdate<TTracked, TDto>(id, dto);
                result.Add(trackedItem);
            }

            return result;
        }

        internal void UpdateFrom(ChannelStateResponseInternalDTO dto, ICache cache)
        {
            //Channel = Channel.TryLoadFromDto(dto.Channel);
            Hidden = dto.Hidden;
            HideMessagesBefore = dto.HideMessagesBefore;
            // Members = Members.TryLoadFromDtoCollection(dto.Members);
            // Membership = Membership.TryLoadFromDto(dto.Membership);
            // Messages = Messages.TryLoadFromDtoCollection(dto.Messages);
            // PendingMessages = PendingMessages.TryLoadFromDtoCollection(dto.PendingMessages);
            // PinnedMessages = PinnedMessages.TryLoadFromDtoCollection(dto.PinnedMessages);
            // Read = Read.TryLoadFromDtoCollection(dto.Read);
            // WatcherCount = dto.WatcherCount;
            // Watchers = Watchers.TryLoadFromDtoCollection(dto.Watchers);
            // AdditionalProperties = dto.AdditionalProperties;
        }

        internal void AddMessage(StreamMessage message)
        {
        }

        internal void UpdateMessage(StreamMessage message)
        {
        }

        public void DeleteMessage(string messageId, bool isHardDelete)
        {
            for (int i = _messages.Count - 1; i >= 0; i--)
            {
                var message = _messages[i];
                if (message.Id != messageId)
                {
                    continue;
                }

                if (isHardDelete)
                {
                    _messages.RemoveAt(i);
                    return;
                }

                message.SoftDelete();
                return;
            }
        }

        StreamChannel ILoadableFrom<ChannelStateResponseInternalDTO, StreamChannel>.LoadFromDto(
            ChannelStateResponseInternalDTO dto)
        {
            throw new NotImplementedException();
        }

        protected override StreamChannel Self => this;

        protected override string InternalUniqueId
        {
            get => Cid;
            set => Cid = value;
        }

        private readonly List<StreamChannelMember> _members = new List<StreamChannelMember>();
        private readonly List<StreamMessage> _messages = new List<StreamMessage>();
        private readonly List<StreamMessage> _pinnedMessages = new List<StreamMessage>();
    }
}