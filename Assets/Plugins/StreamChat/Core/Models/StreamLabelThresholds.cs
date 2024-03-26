using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.State;
using StreamChat.Core.State.Caches;

namespace StreamChat.Core.Models
{
    public class StreamLabelThresholds : IStateLoadableFrom<LabelThresholdsInternalDTO, StreamLabelThresholds>
    {
        /// <summary>
        /// Threshold for automatic message block
        /// </summary>
        public float? Block { get; private set; }

        /// <summary>
        /// Threshold for automatic message flag
        /// </summary>
        public float? Flag { get; private set; }

        StreamLabelThresholds IStateLoadableFrom<LabelThresholdsInternalDTO, StreamLabelThresholds>.LoadFromDto(LabelThresholdsInternalDTO dto, ICache cache)
        {
            Block = dto.Block;
            Flag = dto.Flag;

            return this;
        }
    }
}