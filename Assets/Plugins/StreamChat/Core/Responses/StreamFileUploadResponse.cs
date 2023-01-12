namespace StreamChat.Core.Responses
{
    public readonly struct StreamFileUploadResponse
    {
        public readonly string FileUrl { get; }

        internal StreamFileUploadResponse(string fileUrl)
        {
            FileUrl = fileUrl;
        }
    }
}