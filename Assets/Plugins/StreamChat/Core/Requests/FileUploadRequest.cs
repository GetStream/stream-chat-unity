using StreamChat.Core.DTO.Requests;

namespace StreamChat.Core.Requests
{
    public partial class FileUploadRequest : RequestObjectBase, ISavableTo<FileUploadRequestDTO>
    {
        /// <summary>
        /// multipart/form-data file field
        /// </summary>
        public string File { get; set; }

        FileUploadRequestDTO ISavableTo<FileUploadRequestDTO>.SaveToDto() =>
            new FileUploadRequestDTO()
            {
                File = File,
                AdditionalProperties = AdditionalProperties
            };
    }
}