﻿using StreamChat.Core.DTO.Models;
using StreamChat.Core.DTO.Requests;

namespace StreamChat.Core.Requests
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
                Crop = Crop,
                Height = Height,
                Resize = Resize,
                Width = Width,
                AdditionalProperties = AdditionalProperties
            };
    }
}