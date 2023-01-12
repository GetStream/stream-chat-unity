using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.LowLevelClient;

namespace StreamChat.Core.Requests
{
    public class StreamImageDataRequest : ISavableTo<ImageDataRequestInternalDTO>
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
            };
        }
    }
}