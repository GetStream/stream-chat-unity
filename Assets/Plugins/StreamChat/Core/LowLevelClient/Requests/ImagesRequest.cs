using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Requests
{
    public class ImagesRequest : RequestObjectBase, ISavableTo<ImagesRequestInternalDTO>
    {
        public ImageDataRequest FixedHeight { get; set; }

        public ImageDataRequest FixedHeightDownsampled { get; set; }

        public ImageDataRequest FixedHeightStill { get; set; }

        public ImageDataRequest FixedWidth { get; set; }

        public ImageDataRequest FixedWidthDownsampled { get; set; }

        public ImageDataRequest FixedWidthStill { get; set; }

        public ImageDataRequest Original { get; set; }

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
                AdditionalProperties = AdditionalProperties,
            };
    }
}