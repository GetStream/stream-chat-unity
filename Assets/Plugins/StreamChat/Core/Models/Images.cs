using StreamChat.Core.DTO.Models;
using StreamChat.Core.Helpers;

namespace StreamChat.Core.Models
{
    public class Images : ModelBase, ILoadableFrom<ImagesDTO, Images>
    {
        public ImageData FixedHeight { get; set; }

        public ImageData FixedHeightDownsampled { get; set; }

        public ImageData FixedHeightStill { get; set; }

        public ImageData FixedWidth { get; set; }

        public ImageData FixedWidthDownsampled { get; set; }

        public ImageData FixedWidthStill { get; set; }

        public ImageData Original { get; set; }


        Images ILoadableFrom<ImagesDTO, Images>.LoadFromDto(ImagesDTO dto)
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