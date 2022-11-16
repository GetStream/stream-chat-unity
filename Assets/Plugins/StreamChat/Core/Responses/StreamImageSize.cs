using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.State;
using StreamChat.Core.State.Caches;

namespace StreamChat.Core.Responses
{
    public sealed class StreamImageSize  : IStateLoadableFrom<ImageSizeInternalDTO, StreamImageSize>
    {
        /// <summary>
        /// Crop mode
        /// </summary>
        public StreamImageCropType? Crop { get; private set; }

        /// <summary>
        /// Target image height
        /// </summary>
        public int? Height { get; private set; }

        /// <summary>
        /// Resize method
        /// </summary>
        public StreamImageResizeType? Resize { get; private set; }

        /// <summary>
        /// Target image width
        /// </summary>
        public int? Width { get; private set; }

        StreamImageSize IStateLoadableFrom<ImageSizeInternalDTO, StreamImageSize>.LoadFromDto(ImageSizeInternalDTO dto, ICache cache)
        {
            Crop = dto.Crop?.ToStreamImageCropType();
            Height = dto.Height;
            Resize = dto.Resize?.ToStreamImageResizeType();
            Width = dto.Width;

            return this;
        }
    }
}