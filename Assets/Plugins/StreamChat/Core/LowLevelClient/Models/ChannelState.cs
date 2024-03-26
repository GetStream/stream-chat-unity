using System;
using System.Collections.Generic;
using System.Linq;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Responses;

namespace StreamChat.Core.LowLevelClient.Models
{
    public class ChannelState : ModelBase, ILoadableFrom<ChannelStateResponseFieldsInternalDTO, ChannelState>,
        ILoadableFrom<ChannelStateResponseInternalDTO, ChannelState>
    {
        [Obsolete("This event is deprecated and will be removed in a future major release.")]
        public event Action<ChannelState, Message> NewMessageAdded;

        public Channel Channel { get; set; }

        /// <summary>
        /// Whether this channel is hidden or not
        /// </summary>
        public bool? Hidden { get; set; }

        /// <summary>
        /// Messages before this date are hidden from the user
        /// </summary>
        public DateTimeOffset? HideMessagesBefore { get; set; }

        /// <summary>
        /// List of channel members
        /// </summary>
        public List<ChannelMember> Members { get; set; }

        /// <summary>
        /// Current user membership object
        /// </summary>
        public ChannelMember Membership { get; set; }

        /// <summary>
        /// List of channel messages
        /// </summary>
        public List<Message> Messages { get; set; }

        /// <summary>
        /// Pending messages that this user has sent
        /// </summary>
        public List<PendingMessage> PendingMessages { get; set; }

        /// <summary>
        /// List of pinned messages in the channel
        /// </summary>
        public List<Message> PinnedMessages { get; set; }

        /// <summary>
        /// List of read states
        /// </summary>
        public List<Read> Read { get; set; }

        /// <summary>
        /// Number of channel watchers
        /// </summary>
        public int? WatcherCount { get; set; }

        /// <summary>
        /// List of user who is watching the channel
        /// </summary>
        public List<User> Watchers { get; set; }

        public bool IsDirectMessage => Channel.MemberCount == 2 && Members.Any(_ => _.User.Id == Channel.CreatedBy.Id);

        [Obsolete("This method is deprecated and will be removed in a future major release.")]
        public void AddMessage(Message message)
        {
            Messages.Add(message);
            NewMessageAdded?.Invoke(this, message);
        }

        ChannelState ILoadableFrom<ChannelStateResponseFieldsInternalDTO, ChannelState>.LoadFromDto(ChannelStateResponseFieldsInternalDTO dto)
        {
            Channel = Channel.TryLoadFromDto(dto.Channel);
            Hidden = dto.Hidden;
            HideMessagesBefore = dto.HideMessagesBefore;
            Members = Members.TryLoadFromDtoCollection(dto.Members);
            Membership = Membership.TryLoadFromDto(dto.Membership);
            Messages = Messages.TryLoadFromDtoCollection(dto.Messages);
            PendingMessages = PendingMessages.TryLoadFromDtoCollection(dto.PendingMessages);
            PinnedMessages = PinnedMessages.TryLoadFromDtoCollection(dto.PinnedMessages);
            Read = Read.TryLoadFromDtoCollection(dto.Read);
            WatcherCount = dto.WatcherCount;
            Watchers = Watchers.TryLoadFromDtoCollection(dto.Watchers);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }

        ChannelState ILoadableFrom<ChannelStateResponseInternalDTO, ChannelState>.LoadFromDto(ChannelStateResponseInternalDTO dto)
        {
            Channel = Channel.TryLoadFromDto(dto.Channel);
            Hidden = dto.Hidden;
            HideMessagesBefore = dto.HideMessagesBefore;
            Members = Members.TryLoadFromDtoCollection(dto.Members);
            Membership = Membership.TryLoadFromDto(dto.Membership);
            Messages = Messages.TryLoadFromDtoCollection(dto.Messages);
            PendingMessages = PendingMessages.TryLoadFromDtoCollection(dto.PendingMessages);
            PinnedMessages = PinnedMessages.TryLoadFromDtoCollection(dto.PinnedMessages);
            Read = Read.TryLoadFromDtoCollection(dto.Read);
            WatcherCount = dto.WatcherCount;
            Watchers = Watchers.TryLoadFromDtoCollection(dto.Watchers);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}