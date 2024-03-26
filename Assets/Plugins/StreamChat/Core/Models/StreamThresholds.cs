using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.State;
using StreamChat.Core.State.Caches;

namespace StreamChat.Core.Models
{
    /// <summary>
    /// Sets thresholds for AI moderation
    /// </summary>
    public class StreamThresholds : IStateLoadableFrom<ThresholdsInternalDTO, StreamThresholds>
    {
        /// <summary>
        /// Thresholds for explicit messages
        /// </summary>
        public StreamLabelThresholds Explicit { get; private set; }

        /// <summary>
        /// Thresholds for spam
        /// </summary>
        public StreamLabelThresholds Spam { get; private set; }

        /// <summary>
        /// Thresholds for toxic messages
        /// </summary>
        public StreamLabelThresholds Toxic { get; private set; }

        StreamThresholds IStateLoadableFrom<ThresholdsInternalDTO, StreamThresholds>.LoadFromDto(ThresholdsInternalDTO dto, ICache cache)
        {
            Explicit = Explicit.TryLoadFromDto(dto.Explicit, cache);
            Spam = Explicit.TryLoadFromDto(dto.Spam, cache);
            Toxic = Explicit.TryLoadFromDto(dto.Toxic, cache);

            return this;
        }
    }
}