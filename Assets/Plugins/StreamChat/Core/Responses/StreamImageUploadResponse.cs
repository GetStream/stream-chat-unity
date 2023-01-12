using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.State;
using StreamChat.Core.State.Caches;

namespace StreamChat.Core.Responses
{
    public sealed class StreamImageUploadResponse
        : IStateLoadableFrom<ImageUploadResponseInternalDTO, StreamImageUploadResponse>
    {
        /// <summary>
        /// URL to the uploaded asset. Should be used to put to `asset_url` attachment field
        /// </summary>
        public string FileUrl { get; private set; }

        /// <summary>
        /// URL of the file thumbnail for supported file formats. Should be put to `thumb_url` attachment field
        /// </summary>
        public string ThumbUrl { get; private set; }

        public System.Collections.Generic.List<StreamImageSize> UploadSizes { get; private set; }

        StreamImageUploadResponse IStateLoadableFrom<ImageUploadResponseInternalDTO, StreamImageUploadResponse>.
            LoadFromDto(ImageUploadResponseInternalDTO dto, ICache cache)
        {
            FileUrl = dto.File;
            ThumbUrl = dto.ThumbUrl;
            UploadSizes = UploadSizes.TryLoadFromDtoCollection(dto.UploadSizes, cache);

            return this;
        }
    }
}