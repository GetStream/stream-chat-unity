using StreamChat.Core.DTO.Requests;

namespace StreamChat.Core.Requests
{
    public partial class ImageUploadRequest : RequestObjectBase, ISavableTo<ImageUploadRequestDTO>
    {
        /// <summary>
        /// multipart/form-data file field
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// multipart/form-data field with JSON-encoded array of image size configurations
        /// </summary>
        public System.Collections.Generic.List<ImageSizeRequest> UploadSizes { get; set; }

        ImageUploadRequestDTO ISavableTo<ImageUploadRequestDTO>.SaveToDto() =>
            new ImageUploadRequestDTO()
            {
                File = File,
                AdditionalProperties = AdditionalProperties
            };
    }
}