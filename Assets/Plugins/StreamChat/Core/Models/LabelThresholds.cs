using StreamChat.Core.DTO.Models;

namespace StreamChat.Core.Models
{
    public class LabelThresholds : ModelBase, ILoadableFrom<LabelThresholdsInternalDTO, LabelThresholds>
    {
        /// <summary>
        /// Threshold for automatic message block
        /// </summary>
        public float? Block { get; set; }

        /// <summary>
        /// Threshold for automatic message flag
        /// </summary>
        public float? Flag { get; set; }

        LabelThresholds ILoadableFrom<LabelThresholdsInternalDTO, LabelThresholds>.LoadFromDto(LabelThresholdsInternalDTO dto)
        {
            Block = dto.Block;
            Flag = dto.Flag;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}