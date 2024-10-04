using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Requests
{
    public partial class ImageSizeRequest : RequestObjectBase, ISavableTo<ImageSizeRequestInternalDTO>
    {
        /// <summary>
        /// Crop mode
        /// </summary>
        public ImageCropType? Crop { get; set; }

        /// <summary>
        /// Target image height
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// Resize method
        /// </summary>
        public ImageResizeType? Resize { get; set; }

        /// <summary>
        /// Target image width
        /// </summary>
        public int? Width { get; set; }

        ImageSizeRequestInternalDTO ISavableTo<ImageSizeRequestInternalDTO>.SaveToDto() =>
            new ImageSizeRequestInternalDTO()
            {
                Crop = Crop.TrySaveNullableStructToDto<ImageCropType, ImageCropTypeInternalDTO>(),
                Height = Height,
                Resize = Resize.TrySaveNullableStructToDto<ImageResizeType, ImageResizeTypeInternalDTO>(),
                Width = Width,
                AdditionalProperties = AdditionalProperties
            };
    }
}