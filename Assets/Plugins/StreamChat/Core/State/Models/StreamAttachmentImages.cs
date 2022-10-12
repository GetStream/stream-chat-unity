using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.State.Models
{
    public class StreamAttachmentImages : IStateLoadableFrom<ImagesInternalDTO, StreamAttachmentImages>
    {
        public StreamImageData FixedHeight { get; set; }

        public StreamImageData FixedHeightDownsampled { get; set; }

        public StreamImageData FixedHeightStill { get; set; }

        public StreamImageData FixedWidth { get; set; }

        public StreamImageData FixedWidthDownsampled { get; set; }

        public StreamImageData FixedWidthStill { get; set; }

        public StreamImageData Original { get; set; }

        StreamAttachmentImages IStateLoadableFrom<ImagesInternalDTO, StreamAttachmentImages>.LoadFromDto(ImagesInternalDTO dto, ICache cache)
        {
            FixedHeight = FixedHeight.TryLoadFromDto(dto.FixedHeight, cache);
            FixedHeightDownsampled = FixedHeightDownsampled.TryLoadFromDto(dto.FixedHeightDownsampled, cache);
            FixedHeightStill = FixedHeightStill.TryLoadFromDto(dto.FixedHeightStill, cache);
            FixedWidth = FixedWidth.TryLoadFromDto(dto.FixedWidth, cache);
            FixedWidthDownsampled = FixedWidthDownsampled.TryLoadFromDto(dto.FixedWidthDownsampled, cache);
            FixedWidthStill = FixedWidthStill.TryLoadFromDto(dto.FixedWidthStill, cache);
            Original = Original.TryLoadFromDto(dto.Original, cache);

            return this;
        }
    }
}