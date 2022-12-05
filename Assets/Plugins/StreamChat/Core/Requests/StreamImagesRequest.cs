using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.LowLevelClient;

namespace StreamChat.Core.Requests
{
    public class StreamImagesRequest : ISavableTo<ImagesRequestInternalDTO>
    {
        public StreamImageDataRequest FixedHeight { get; set; }

        public StreamImageDataRequest FixedHeightDownsampled { get; set; }

        public StreamImageDataRequest FixedHeightStill { get; set; }

        public StreamImageDataRequest FixedWidth { get; set; }

        public StreamImageDataRequest FixedWidthDownsampled { get; set; }

        public StreamImageDataRequest FixedWidthStill { get; set; }

        public StreamImageDataRequest Original { get; set; }

        ImagesRequestInternalDTO ISavableTo<ImagesRequestInternalDTO>.SaveToDto() =>
            new ImagesRequestInternalDTO
            {
                FixedHeight = FixedHeight.TrySaveToDto(),
                FixedHeightDownsampled = FixedHeightDownsampled.TrySaveToDto(),
                FixedHeightStill = FixedHeightStill.TrySaveToDto(),
                FixedWidth = FixedWidth.TrySaveToDto(),
                FixedWidthDownsampled = FixedWidthDownsampled.TrySaveToDto(),
                FixedWidthStill = FixedWidthStill.TrySaveToDto(),
                Original = Original.TrySaveToDto(),
            };
    }
}