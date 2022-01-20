using GetStreamIO.Core.DTO.Requests;
using Plugins.GetStreamIO.Core.Helpers;

namespace Plugins.GetStreamIO.Core.Requests
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

        public System.Collections.Generic.ICollection<ChannelMemberRequestDTO> Members { get; set; }

        public System.Collections.Generic.ICollection<double> OwnCapabilities { get; set; }

        /// <summary>
        /// Team the channel belongs to (if multi-tenant mode is enabled)
        /// </summary>
        public string Team { get; set; }

        public ChannelRequestDTO SaveToDto() =>
            new ChannelRequestDTO
            {
                AutoTranslationEnabled = AutoTranslationEnabled,
                AutoTranslationLanguage = AutoTranslationLanguage,
                ConfigOverrides = ConfigOverrides.TrySaveToDto(),
                CreatedBy = CreatedBy.TrySaveToDto(),
                Disabled = Disabled,
                Frozen = Frozen,
                Members = Members,
                OwnCapabilities = OwnCapabilities,
                Team = Team,
                AdditionalProperties = AdditionalProperties,
            };
    }
}