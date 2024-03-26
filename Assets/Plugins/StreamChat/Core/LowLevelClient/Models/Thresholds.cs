using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.LowLevelClient.Models
{
    /// <summary>
    /// Sets thresholds for AI moderation
    /// </summary>
    public class Thresholds : ModelBase, ILoadableFrom<ThresholdsInternalDTO, Thresholds>
    {
        /// <summary>
        /// Thresholds for explicit messages
        /// </summary>
        public LabelThresholds Explicit { get; set; }

        /// <summary>
        /// Thresholds for spam
        /// </summary>
        public LabelThresholds Spam { get; set; }

        /// <summary>
        /// Thresholds for toxic messages
        /// </summary>
        public LabelThresholds Toxic { get; set; }

        Thresholds ILoadableFrom<ThresholdsInternalDTO, Thresholds>.LoadFromDto(ThresholdsInternalDTO dto)
        {
            Explicit = Explicit.TryLoadFromDto(dto.Explicit);
            Spam = Explicit.TryLoadFromDto(dto.Spam);
            Toxic = Explicit.TryLoadFromDto(dto.Toxic);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}