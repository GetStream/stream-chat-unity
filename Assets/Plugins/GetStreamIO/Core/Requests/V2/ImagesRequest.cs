using GetStreamIO.Core.DTO.Requests;

namespace Plugins.GetStreamIO.Core.Requests.V2
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

        public ImagesRequestDTO SaveToDto() =>
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