using System;
using System.Collections.Generic;
using System.Linq;
using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Helpers;

namespace StreamChat.Core.Models
{
    public class ChannelState : ModelBase, ILoadableFrom<ChannelStateResponseFieldsDTO, ChannelState>,
        ILoadableFrom<ChannelStateResponseDTO, ChannelState>
    {
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
        public ICollection<ChannelMember> Members { get; set; }

        /// <summary>
        /// Current user membership object
        /// </summary>
        public ChannelMember Membership { get; set; }

        /// <summary>
        /// List of channel messages
        /// </summary>
        public ICollection<Message> Messages { get; set; }

        /// <summary>
        /// List of pinned messages in the channel
        /// </summary>
        public ICollection<Message> PinnedMessages { get; set; }

        /// <summary>
        /// List of read states
        /// </summary>
        public ICollection<Read> Read { get; set; }

        /// <summary>
        /// Number of channel watchers
        /// </summary>
        public int? WatcherCount { get; set; }

        /// <summary>
        /// List of user who is watching the channel
        /// </summary>
        public ICollection<User> Watchers { get; set; }

        public bool IsDirectMessage => Channel.MemberCount == 2 && Members.Any(_ => _.User.Id == Channel.CreatedBy.Id);

        public void AddMessage(Message message)
        {
            Messages.Add(message);
            NewMessageAdded?.Invoke(this, message);
        }

        ChannelState ILoadableFrom<ChannelStateResponseFieldsDTO, ChannelState>.LoadFromDto(ChannelStateResponseFieldsDTO dto)
        {
            Channel = Channel.TryLoadFromDto(dto.Channel);
            Hidden = dto.Hidden;
            HideMessagesBefore = dto.HideMessagesBefore;
            Members = Members.TryLoadFromDtoCollection(dto.Members);
            Membership = Membership.TryLoadFromDto(dto.Membership);
            Messages = Messages.TryLoadFromDtoCollection(dto.Messages);
            PinnedMessages = PinnedMessages.TryLoadFromDtoCollection(dto.PinnedMessages);
            Read = Read.TryLoadFromDtoCollection(dto.Read);
            WatcherCount = dto.WatcherCount;
            Watchers = Watchers.TryLoadFromDtoCollection(dto.Watchers);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }

        ChannelState ILoadableFrom<ChannelStateResponseDTO, ChannelState>.LoadFromDto(ChannelStateResponseDTO dto)
        {
            Channel = Channel.TryLoadFromDto(dto.Channel);
            Hidden = dto.Hidden;
            HideMessagesBefore = dto.HideMessagesBefore;
            Members = Members.TryLoadFromDtoCollection(dto.Members);
            Membership = Membership.TryLoadFromDto(dto.Membership);
            Messages = Messages.TryLoadFromDtoCollection(dto.Messages);
            PinnedMessages = PinnedMessages.TryLoadFromDtoCollection(dto.PinnedMessages);
            Read = Read.TryLoadFromDtoCollection(dto.Read);
            WatcherCount = dto.WatcherCount;
            Watchers = Watchers.TryLoadFromDtoCollection(dto.Watchers);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}