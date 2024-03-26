using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Requests
{
    public partial class ImageUploadRequest : RequestObjectBase, ISavableTo<ImageUploadRequestInternalDTO>
    {
        /// <summary>
        /// multipart/form-data file field
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// multipart/form-data field with JSON-encoded array of image size configurations
        /// </summary>
        public System.Collections.Generic.List<ImageSizeRequest> UploadSizes { get; set; }

        ImageUploadRequestInternalDTO ISavableTo<ImageUploadRequestInternalDTO>.SaveToDto() =>
            new ImageUploadRequestInternalDTO()
            {
                File = File,
                AdditionalProperties = AdditionalProperties
            };
    }
}