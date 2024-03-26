using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.LowLevelClient.Models
{
    public partial class FlagMessageDetails : ModelBase, ILoadableFrom<FlagMessageDetailsInternalDTO, FlagMessageDetails>
    {
        public bool? PinChanged { get; set; }

        public bool? ShouldEnrich { get; set; }

        public bool? SkipPush { get; set; }

        public string UpdatedById { get; set; }

        FlagMessageDetails ILoadableFrom<FlagMessageDetailsInternalDTO, FlagMessageDetails>.LoadFromDto(FlagMessageDetailsInternalDTO dto)
        {
            PinChanged = dto.PinChanged;
            ShouldEnrich = dto.ShouldEnrich;
            SkipPush = dto.SkipPush;
            UpdatedById = dto.UpdatedById;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}