using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Responses;

namespace StreamChat.Core.LowLevelClient.Models
{
    public class Channel : ModelBase, ILoadableFrom<ChannelResponseInternalDTO, Channel>
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
        public ChannelConfig Config { get; set; }

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
        public User CreatedBy { get; set; }

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
        public System.Collections.Generic.List<ChannelMember> Members { get; set; }

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

        public User TruncatedBy { get; set; }

        /// <summary>
        /// Type of the channel
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Date/time of the last update
        /// </summary>
        public System.DateTimeOffset? UpdatedAt { get; set; }

        public string Name { get; set; }

        Channel ILoadableFrom<ChannelResponseInternalDTO, Channel>.LoadFromDto(ChannelResponseInternalDTO dto)
        {
            AutoTranslationEnabled = dto.AutoTranslationEnabled;
            AutoTranslationLanguage = dto.AutoTranslationLanguage;
            Cid = dto.Cid;
            Config = Config.TryLoadFromDto(dto.Config);
            Cooldown = dto.Cooldown;
            CreatedAt = dto.CreatedAt;
            CreatedBy = CreatedBy.TryLoadFromDto<UserObjectInternalDTO, User>(dto.CreatedBy);
            DeletedAt = dto.DeletedAt;
            Disabled = dto.Disabled;
            Frozen = dto.Frozen;
            Hidden = dto.Hidden;
            HideMessagesBefore = dto.HideMessagesBefore;
            Id = dto.Id;
            LastMessageAt = dto.LastMessageAt;
            MemberCount = dto.MemberCount;
            Members = Members.TryLoadFromDtoCollection(dto.Members);
            MuteExpiresAt = dto.MuteExpiresAt;
            Muted = dto.Muted;
            OwnCapabilities = dto.OwnCapabilities;
            Team = dto.Team;
            TruncatedAt = dto.TruncatedAt;
            TruncatedBy = TruncatedBy.TryLoadFromDto<UserObjectInternalDTO, User>(dto.TruncatedBy);
            Type = dto.Type;
            UpdatedAt = dto.UpdatedAt;
            AdditionalProperties = dto.AdditionalProperties;

            //Not in API spec
            Name = dto.Name;

            return this;
        }
    }
}