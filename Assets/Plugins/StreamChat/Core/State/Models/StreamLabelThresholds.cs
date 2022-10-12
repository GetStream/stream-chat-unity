using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.State.Models
{
    public class StreamLabelThresholds : IStateLoadableFrom<LabelThresholdsInternalDTO, StreamLabelThresholds>
    {
        /// <summary>
        /// Threshold for automatic message block
        /// </summary>
        public float? Block { get; set; }

        /// <summary>
        /// Threshold for automatic message flag
        /// </summary>
        public float? Flag { get; set; }

        StreamLabelThresholds IStateLoadableFrom<LabelThresholdsInternalDTO, StreamLabelThresholds>.LoadFromDto(LabelThresholdsInternalDTO dto, ICache cache)
        {
            Block = dto.Block;
            Flag = dto.Flag;

            return this;
        }
    }
}