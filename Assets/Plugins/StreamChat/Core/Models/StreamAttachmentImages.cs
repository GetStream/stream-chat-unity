using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.State;
using StreamChat.Core.State.Caches;

namespace StreamChat.Core.Models
{
    public class StreamAttachmentImages : IStateLoadableFrom<ImagesInternalDTO, StreamAttachmentImages>
    {
        public StreamImageData FixedHeight { get; private set; }

        public StreamImageData FixedHeightDownsampled { get; private set; }

        public StreamImageData FixedHeightStill { get; private set; }

        public StreamImageData FixedWidth { get; private set; }

        public StreamImageData FixedWidthDownsampled { get; private set; }

        public StreamImageData FixedWidthStill { get; private set; }

        public StreamImageData Original { get; private set; }

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