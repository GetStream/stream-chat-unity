using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Requests
{
    public partial class ChannelRequest : RequestObjectBase, ISavableTo<ChannelRequestInternalDTO>
    {
        /// <summary>
        /// Enable or disable auto translation
        /// </summary>
        public bool? AutoTranslationEnabled { get; set; }

        /// <summary>
        /// Switch auto translation language
        /// </summary>
        public string AutoTranslationLanguage { get; set; }

        public ChannelConfigRequest ConfigOverrides { get; set; }

        public UserObjectRequest CreatedBy { get; set; }

        public bool? Disabled { get; set; }

        /// <summary>
        /// Freeze or unfreeze the channel
        /// </summary>
        public bool? Frozen { get; set; }

        public System.Collections.Generic.List<ChannelMemberRequest> Members { get; set; }

        public System.Collections.Generic.List<string> OwnCapabilities { get; set; }

        /// <summary>
        /// Team the channel belongs to (if multi-tenant mode is enabled)
        /// </summary>
        public string Team { get; set; }

        public System.Collections.Generic.List<int> TruncatedAt { get; set; }

        public System.Collections.Generic.List<int> TruncatedBy { get; set; }

        public string TruncatedById { get; set; }

        public string Name { get; set; }

        ChannelRequestInternalDTO ISavableTo<ChannelRequestInternalDTO>.SaveToDto() =>
            new ChannelRequestInternalDTO
            {
                AutoTranslationEnabled = AutoTranslationEnabled,
                AutoTranslationLanguage = AutoTranslationLanguage,
                ConfigOverrides = ConfigOverrides.TrySaveToDto(),
                CreatedBy = CreatedBy.TrySaveToDto(),
                Disabled = Disabled,
                Frozen = Frozen,
                Members = Members.TrySaveToDtoCollection<ChannelMemberRequest, ChannelMemberRequestInternalDTO>(),
                OwnCapabilities = OwnCapabilities,
                Team = Team,
                TruncatedAt = TruncatedAt,
                TruncatedBy = TruncatedBy,
                TruncatedById = TruncatedById,
                Name = Name,
                AdditionalProperties = AdditionalProperties,
            };
    }
}