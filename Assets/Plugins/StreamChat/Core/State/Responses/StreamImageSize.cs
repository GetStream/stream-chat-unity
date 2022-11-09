using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.State.Responses
{
    public partial class StreamImageSize  : IStateLoadableFrom<ImageSizeInternalDTO, StreamImageSize>
    {
        /// <summary>
        /// Crop mode
        /// </summary>
        public ImageCropType? Crop { get; set; } //StreamTodo

        /// <summary>
        /// Target image height
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// Resize method
        /// </summary>
        public ImageResizeType? Resize { get; set; } //StreamTodo

        /// <summary>
        /// Target image width
        /// </summary>
        public int? Width { get; set; }

        StreamImageSize IStateLoadableFrom<ImageSizeInternalDTO, StreamImageSize>.LoadFromDto(ImageSizeInternalDTO dto, ICache cache)
        {
            Crop = dto.Crop;
            Height = dto.Height;
            Resize = dto.Resize;
            Width = dto.Width;

            return this;
        }
    }
}