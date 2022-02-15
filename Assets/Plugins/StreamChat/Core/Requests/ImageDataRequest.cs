using StreamChat.Core.DTO.Requests;

namespace StreamChat.Core.Requests
{
    public class ImageDataRequest : RequestObjectBase, ISavableTo<ImageDataRequestDTO>
    {
        public string Frames { get; set; }

        public string Height { get; set; }

        public string Size { get; set; }

        public string Url { get; set; }

        public string Width { get; set; }

        ImageDataRequestDTO ISavableTo<ImageDataRequestDTO>.SaveToDto()
        {
            return new ImageDataRequestDTO
            {
                Frames = Frames,
                Height = Height,
                Size = Size,
                Url = Url,
                Width = Width,
                AdditionalProperties = AdditionalProperties
            };
        }
    }
}