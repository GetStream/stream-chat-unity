using StreamChat.Core.DTO.Models;

namespace StreamChat.Core.Models
{
    public class ImageData : ModelBase, ILoadableFrom<ImageDataDTO, ImageData>
    {
        public string Frames { get; set; }

        public string Height { get; set; }

        public string Size { get; set; }

        public string Url { get; set; }

        public string Width { get; set; }

        ImageData ILoadableFrom<ImageDataDTO, ImageData>.LoadFromDto(ImageDataDTO dto)
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