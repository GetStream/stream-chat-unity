using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.LowLevelClient.Models
{
    public class Images : ModelBase, ILoadableFrom<ImagesInternalDTO, Images>
    {
        public ImageData FixedHeight { get; set; }

        public ImageData FixedHeightDownsampled { get; set; }

        public ImageData FixedHeightStill { get; set; }

        public ImageData FixedWidth { get; set; }

        public ImageData FixedWidthDownsampled { get; set; }

        public ImageData FixedWidthStill { get; set; }

        public ImageData Original { get; set; }

        Images ILoadableFrom<ImagesInternalDTO, Images>.LoadFromDto(ImagesInternalDTO dto)
        {
            FixedHeight = FixedHeight.TryLoadFromDto(dto.FixedHeight);
            FixedHeightDownsampled = FixedHeightDownsampled.TryLoadFromDto(dto.FixedHeightDownsampled);
            FixedHeightStill = FixedHeightStill.TryLoadFromDto(dto.FixedHeightStill);
            FixedWidth = FixedWidth.TryLoadFromDto(dto.FixedWidth);
            FixedWidthDownsampled = FixedWidthDownsampled.TryLoadFromDto(dto.FixedWidthDownsampled);
            FixedWidthStill = FixedWidthStill.TryLoadFromDto(dto.FixedWidthStill);
            Original = Original.TryLoadFromDto(dto.Original);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}