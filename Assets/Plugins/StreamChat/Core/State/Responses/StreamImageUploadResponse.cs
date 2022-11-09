using StreamChat.Core.InternalDTO.Responses;

namespace StreamChat.Core.State.Responses
{
    public partial class
        StreamImageUploadResponse : IStateLoadableFrom<ImageUploadResponseInternalDTO, StreamImageUploadResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        /// <summary>
        /// URL to the uploaded asset. Should be used to put to `asset_url` attachment field
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// URL of the file thumbnail for supported file formats. Should be put to `thumb_url` attachment field
        /// </summary>
        public string ThumbUrl { get; set; }

        public System.Collections.Generic.List<StreamImageSize> UploadSizes { get; set; }

        StreamImageUploadResponse IStateLoadableFrom<ImageUploadResponseInternalDTO, StreamImageUploadResponse>.
            LoadFromDto(ImageUploadResponseInternalDTO dto, ICache cache)
        {
            Duration = dto.Duration;
            File = dto.File;
            ThumbUrl = dto.ThumbUrl;
            UploadSizes = UploadSizes.TryLoadFromDtoCollection(dto.UploadSizes, cache);

            return this;
        }
    }
}