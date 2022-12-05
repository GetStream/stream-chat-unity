using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Requests
{
    public class ImageDataRequest : RequestObjectBase, ISavableTo<ImageDataRequestInternalDTO>
    {
        public string Frames { get; set; }

        public string Height { get; set; }

        public string Size { get; set; }

        public string Url { get; set; }

        public string Width { get; set; }

        ImageDataRequestInternalDTO ISavableTo<ImageDataRequestInternalDTO>.SaveToDto()
        {
            return new ImageDataRequestInternalDTO
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