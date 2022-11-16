using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.LowLevelClient.Models
{
    public class ImageData : ModelBase, ILoadableFrom<ImageDataInternalDTO, ImageData>
    {
        public string Frames { get; set; }

        public string Height { get; set; }

        public string Size { get; set; }

        public string Url { get; set; }

        public string Width { get; set; }

        ImageData ILoadableFrom<ImageDataInternalDTO, ImageData>.LoadFromDto(ImageDataInternalDTO dto)
        {
            Frames = dto.Frames;
            Height = dto.Height;
            Size = dto.Size;
            Url = dto.Url;
            Width = dto.Width;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}