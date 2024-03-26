using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Responses
{
    public partial class Event : ResponseObjectBase, ILoadableFrom<EventInternalDTO, Event>
    {
        /// <summary>
        /// Only applicable to `message.flagged` event.
        /// </summary>
        public bool? Automoderation { get; set; }

        /// <summary>
        /// Only applicable to `message.flagged` event.
        /// </summary>
        public ModerationResponse AutomoderationScores { get; set; }

        public Channel Channel { get; set; }

        public string ChannelId { get; set; }

        public string ChannelType { get; set; }

        /// <summary>
        /// Channel CID (&lt;type&gt;:&lt;id&gt;)
        /// </summary>
        public string Cid { get; set; }

        /// <summary>
        /// Only applicable to `health.check` event
        /// </summary>
        public string ConnectionId { get; set; }

        /// <summary>
        /// Date/time of creation
        /// </summary>
        public System.DateTimeOffset? CreatedAt { get; set; }

        /// <summary>
        /// User who issued moderation action. Only applicable to moderation-related events
        /// </summary>
        public User CreatedBy { get; set; }

        public OwnUser Me { get; set; }

        public ChannelMember Member { get; set; }

        public Message Message { get; set; }

        /// <summary>
        /// ID of thread. Used in typing events
        /// </summary>
        public string ParentId { get; set; }

        public Reaction Reaction { get; set; }

        /// <summary>
        /// Ban reason. Only applicable to `user.banned` event
        /// </summary>
        public string Reason { get; set; }

        public string Team { get; set; }

        /// <summary>
        /// Event type. To use custom event types see Custom Events documentation
        /// </summary>
        public string Type { get; set; }

        public User User { get; set; }

        public string UserId { get; set; }

        /// <summary>
        /// Number of watchers who received this event
        /// </summary>
        public int? WatcherCount { get; set; }

        Event ILoadableFrom<EventInternalDTO, Event>.LoadFromDto(EventInternalDTO dto)
        {
            Automoderation = dto.Automoderation;
            AutomoderationScores = AutomoderationScores.TryLoadFromDto(dto.AutomoderationScores);
            Channel = Channel.TryLoadFromDto(dto.Channel);
            ChannelId = dto.ChannelId;
            ChannelType = dto.ChannelType;
            Cid = dto.Cid;
            ConnectionId = dto.ConnectionId;
            CreatedAt = dto.CreatedAt;
            CreatedBy = CreatedBy.TryLoadFromDto<UserObjectInternalDTO, User>(dto.CreatedBy);
            Me = Me.TryLoadFromDto<OwnUserInternalDTO, OwnUser>(dto.Me);
            Member = Member.TryLoadFromDto(dto.Member);
            Message = Message.TryLoadFromDto<MessageInternalDTO, Message>(dto.Message);
            ParentId = dto.ParentId;
            Reaction = Reaction.TryLoadFromDto(dto.Reaction);
            Reason = dto.Reason;
            Team = dto.Team;
            Type = dto.Type;
            User = User.TryLoadFromDto<UserObjectInternalDTO, User>(dto.User);
            UserId = dto.UserId;
            WatcherCount = dto.WatcherCount;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}