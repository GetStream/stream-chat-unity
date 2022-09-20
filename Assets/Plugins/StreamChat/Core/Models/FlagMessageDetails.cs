using StreamChat.Core.DTO.Models;

namespace StreamChat.Core.Models
{
    public partial class FlagMessageDetails : ModelBase, ILoadableFrom<FlagMessageDetailsDTO, FlagMessageDetails>
    {
        public bool? PinChanged { get; set; }

        public bool? ShouldEnrich { get; set; }

        public bool? SkipPush { get; set; }

        public string UpdatedById { get; set; }

        FlagMessageDetails ILoadableFrom<FlagMessageDetailsDTO, FlagMessageDetails>.LoadFromDto(FlagMessageDetailsDTO dto)
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