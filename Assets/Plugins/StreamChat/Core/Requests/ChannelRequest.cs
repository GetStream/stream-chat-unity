using StreamChat.Core.DTO.Requests;
using StreamChat.Core.Helpers;

namespace StreamChat.Core.Requests
{
    public partial class ChannelRequest : RequestObjectBase, ISavableTo<ChannelRequestDTO>
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

        public System.Collections.Generic.ICollection<ChannelMemberRequest> Members { get; set; }

        public System.Collections.Generic.ICollection<double> OwnCapabilities { get; set; }

        /// <summary>
        /// Team the channel belongs to (if multi-tenant mode is enabled)
        /// </summary>
        public string Team { get; set; }

        public string Name { get; set; }

        ChannelRequestDTO ISavableTo<ChannelRequestDTO>.SaveToDto() =>
            new ChannelRequestDTO
            {
                AutoTranslationEnabled = AutoTranslationEnabled,
                AutoTranslationLanguage = AutoTranslationLanguage,
                ConfigOverrides = ConfigOverrides.TrySaveToDto(),
                CreatedBy = CreatedBy.TrySaveToDto(),
                Disabled = Disabled,
                Frozen = Frozen,
                Members = Members.TrySaveToDtoCollection<ChannelMemberRequest, ChannelMemberRequestDTO>(),
                OwnCapabilities = OwnCapabilities,
                Team = Team,
                Name = Name,
                AdditionalProperties = AdditionalProperties,
            };
    }
}