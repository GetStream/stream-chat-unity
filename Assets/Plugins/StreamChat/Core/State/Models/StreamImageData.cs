using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.State.Models
{
    public class StreamImageData : IStateLoadableFrom<ImageDataInternalDTO, StreamImageData>
    {
        public string Frames { get; private set; }

        public string Height { get; private set; }

        public string Size { get; private set; }

        public string Url { get; private set; }

        public string Width { get; private set; }

        StreamImageData IStateLoadableFrom<ImageDataInternalDTO, StreamImageData>.LoadFromDto(ImageDataInternalDTO dto, ICache cache)
        {
            Frames = dto.Frames;
            Height = dto.Height;
            Size = dto.Size;
            Url = dto.Url;
            Width = dto.Width;

            return this;
        }
    }
}