using StreamChat.Core.DTO.Requests;
using StreamChat.Core.Helpers;

namespace StreamChat.Core.Requests
{
    public class ImagesRequest : RequestObjectBase, ISavableTo<ImagesRequestDTO>
    {
        public ImageDataRequest FixedHeight { get; set; }

        public ImageDataRequest FixedHeightDownsampled { get; set; }

        public ImageDataRequest FixedHeightStill { get; set; }

        public ImageDataRequest FixedWidth { get; set; }

        public ImageDataRequest FixedWidthDownsampled { get; set; }

        public ImageDataRequest FixedWidthStill { get; set; }

        public ImageDataRequest Original { get; set; }

        ImagesRequestDTO ISavableTo<ImagesRequestDTO>.SaveToDto() =>
            new ImagesRequestDTO
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