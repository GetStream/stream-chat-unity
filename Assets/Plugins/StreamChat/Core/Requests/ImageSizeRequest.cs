using StreamChat.Core.DTO.Models;
using StreamChat.Core.DTO.Requests;
using StreamChat.Core.Requests;

namespace StreamChat.Core.Plugins.StreamChat.Core.Requests
{
    public partial class ImageSizeRequest : RequestObjectBase, ISavableTo<ImageSizeRequestDTO>
    {
        /// <summary>
        /// Crop mode
        /// </summary>
        public ImageCropType? Crop { get; set; }

        /// <summary>
        /// Target image height
        /// </summary>
        public double? Height { get; set; }

        /// <summary>
        /// Resize method
        /// </summary>
        public ImageResizeType? Resize { get; set; }

        /// <summary>
        /// Target image width
        /// </summary>
        public double? Width { get; set; }

        ImageSizeRequestDTO ISavableTo<ImageSizeRequestDTO>.SaveToDto() =>
            new ImageSizeRequestDTO()
            {
                Crop = Crop,
                Height = Height,
                Resize = Resize,
                Width = Width,
                AdditionalProperties = AdditionalProperties
            };
    }
}