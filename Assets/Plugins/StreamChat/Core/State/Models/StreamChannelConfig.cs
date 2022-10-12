﻿using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.State.Models
{
    public class StreamChannelConfig : IStateLoadableFrom<ChannelConfigWithInfoInternalDTO, StreamChannelConfig>
    {
        /// <summary>
        /// Enables automatic message moderation
        /// </summary>
        public AutomodType Automod { get; set; }

        /// <summary>
        /// Sets behavior of automatic moderation
        /// </summary>
        public AutomodBehaviourType? AutomodBehavior { get; set; }

        public StreamThresholds AutomodThresholds { get; set; }

        /// <summary>
        /// Name of the blocklist to use
        /// </summary>
        public string Blocklist { get; set; }

        /// <summary>
        /// Sets behavior of blocklist
        /// </summary>
        public AutomodBehaviourType? BlocklistBehavior { get; set; }

        /// <summary>
        /// List of commands that channel supports
        /// </summary>
        public System.Collections.Generic.List<StreamCommand> Commands { get; set; }

        /// <summary>
        /// Connect events support
        /// </summary>
        public bool? ConnectEvents { get; set; }

        /// <summary>
        /// Date/time of creation
        /// </summary>
        public System.DateTimeOffset? CreatedAt { get; set; }

        /// <summary>
        /// Enables custom events
        /// </summary>
        public bool? CustomEvents { get; set; }

        public System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> Grants { get; set; }

        /// <summary>
        /// Number of maximum message characters
        /// </summary>
        public int? MaxMessageLength { get; set; }

        /// <summary>
        /// Number of days to keep messages. 'infinite' disables retention
        /// </summary>
        public string MessageRetention { get; set; }

        /// <summary>
        /// Enables mutes
        /// </summary>
        public bool? Mutes { get; set; }

        /// <summary>
        /// Channel type name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Enables push notifications
        /// </summary>
        public bool? PushNotifications { get; set; }

        /// <summary>
        /// Enables message quotes
        /// </summary>
        public bool? Quotes { get; set; }

        /// <summary>
        /// Enables message reactions
        /// </summary>
        public bool? Reactions { get; set; }

        /// <summary>
        /// Read events support
        /// </summary>
        public bool? ReadEvents { get; set; }

        public bool? Reminders { get; set; }

        /// <summary>
        /// Enables message replies (threads)
        /// </summary>
        public bool? Replies { get; set; }

        /// <summary>
        /// Enables message search
        /// </summary>
        public bool? Search { get; set; }

        /// <summary>
        /// Typing events support
        /// </summary>
        public bool? TypingEvents { get; set; }

        /// <summary>
        /// Date/time of the last update
        /// </summary>
        public System.DateTimeOffset? UpdatedAt { get; set; }

        /// <summary>
        /// Enables file uploads
        /// </summary>
        public bool? Uploads { get; set; }

        /// <summary>
        /// Enables URL enrichment
        /// </summary>
        public bool? UrlEnrichment { get; set; }

        StreamChannelConfig IStateLoadableFrom<ChannelConfigWithInfoInternalDTO, StreamChannelConfig>.LoadFromDto(ChannelConfigWithInfoInternalDTO dto, ICache cache)
        {
            Automod = dto.Automod;
            AutomodBehavior = dto.AutomodBehavior;
            AutomodThresholds = AutomodThresholds.TryLoadFromDto(dto.AutomodThresholds, cache);
            Blocklist = dto.Blocklist;
            BlocklistBehavior = dto.BlocklistBehavior;
            Commands = Commands.TryLoadFromDtoCollection(dto.Commands, cache);
            ConnectEvents = dto.ConnectEvents;
            CreatedAt = dto.CreatedAt;
            CustomEvents = dto.CustomEvents;
            Grants = dto.Grants;
            MaxMessageLength = dto.MaxMessageLength;
            MessageRetention = dto.MessageRetention;
            Mutes = dto.Mutes;
            Name = dto.Name;
            PushNotifications = dto.PushNotifications;
            Quotes = dto.Quotes;
            Reactions = dto.Reactions;
            ReadEvents = dto.ReadEvents;
            Reminders = dto.Reminders;
            Replies = dto.Replies;
            Search = dto.Search;
            TypingEvents = dto.TypingEvents;
            UpdatedAt = dto.UpdatedAt;
            Uploads = dto.Uploads;
            UrlEnrichment = dto.UrlEnrichment;

            return this;
        }
    }
}