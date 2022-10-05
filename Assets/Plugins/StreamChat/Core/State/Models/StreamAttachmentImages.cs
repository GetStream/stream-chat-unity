using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.State.Models
{
    public class StreamAttachmentImages : ILoadableFrom<ImagesInternalDTO, StreamAttachmentImages>
    {
        public StreamImageData FixedHeight { get; set; }

        public StreamImageData FixedHeightDownsampled { get; set; }

        public StreamImageData FixedHeightStill { get; set; }

        public StreamImageData FixedWidth { get; set; }

        public StreamImageData FixedWidthDownsampled { get; set; }

        public StreamImageData FixedWidthStill { get; set; }

        public StreamImageData Original { get; set; }

        StreamAttachmentImages ILoadableFrom<ImagesInternalDTO, StreamAttachmentImages>.LoadFromDto(ImagesInternalDTO dto)
        {
            FixedHeight = FixedHeight.TryLoadFromDto(dto.FixedHeight);
            FixedHeightDownsampled = FixedHeightDownsampled.TryLoadFromDto(dto.FixedHeightDownsampled);
            FixedHeightStill = FixedHeightStill.TryLoadFromDto(dto.FixedHeightStill);
            FixedWidth = FixedWidth.TryLoadFromDto(dto.FixedWidth);
            FixedWidthDownsampled = FixedWidthDownsampled.TryLoadFromDto(dto.FixedWidthDownsampled);
            FixedWidthStill = FixedWidthStill.TryLoadFromDto(dto.FixedWidthStill);
            Original = Original.TryLoadFromDto(dto.Original);

            return this;
        }
    }
}