using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Responses
{
    public partial class ImageUploadResponse : ResponseObjectBase, ILoadableFrom<ImageUploadResponseInternalDTO, ImageUploadResponse>
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

        public System.Collections.Generic.List<ImageSize> UploadSizes { get; set; }

        ImageUploadResponse ILoadableFrom<ImageUploadResponseInternalDTO, ImageUploadResponse>.LoadFromDto(ImageUploadResponseInternalDTO dto)
        {
            Duration = dto.Duration;
            File = dto.File;
            ThumbUrl = dto.ThumbUrl;
            UploadSizes = UploadSizes.TryLoadFromDtoCollection(dto.UploadSizes);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}