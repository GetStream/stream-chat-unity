using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.State.Models
{
    /// <summary>
    /// Sets thresholds for AI moderation
    /// </summary>
    public class StreamThresholds : IStateLoadableFrom<ThresholdsInternalDTO, StreamThresholds>
    {
        /// <summary>
        /// Thresholds for explicit messages
        /// </summary>
        public StreamLabelThresholds Explicit { get; set; }

        /// <summary>
        /// Thresholds for spam
        /// </summary>
        public StreamLabelThresholds Spam { get; set; }

        /// <summary>
        /// Thresholds for toxic messages
        /// </summary>
        public StreamLabelThresholds Toxic { get; set; }

        StreamThresholds IStateLoadableFrom<ThresholdsInternalDTO, StreamThresholds>.LoadFromDto(ThresholdsInternalDTO dto, ICache cache)
        {
            Explicit = Explicit.TryLoadFromDto(dto.Explicit, cache);
            Spam = Explicit.TryLoadFromDto(dto.Spam, cache);
            Toxic = Explicit.TryLoadFromDto(dto.Toxic, cache);

            return this;
        }
    }
}