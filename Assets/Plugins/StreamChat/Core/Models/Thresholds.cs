using StreamChat.Core.DTO.Models;
using StreamChat.Core.Helpers;

namespace StreamChat.Core.Models
{
    /// <summary>
    /// Sets thresholds for AI moderation
    /// </summary>
    public class Thresholds : ModelBase, ILoadableFrom<ThresholdsDTO, Thresholds>
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

        Thresholds ILoadableFrom<ThresholdsDTO, Thresholds>.LoadFromDto(ThresholdsDTO dto)
        {
            Explicit = Explicit.TryLoadFromDto(dto.Explicit);
            Spam = Explicit.TryLoadFromDto(dto.Spam);
            Toxic = Explicit.TryLoadFromDto(dto.Toxic);

            return this;
        }
    }
}