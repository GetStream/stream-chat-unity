namespace StreamChat.Core.State.Responses
{
    public readonly struct StreamFileUploadResponse
    {
        public readonly string FileUrl;

        internal StreamFileUploadResponse(string fileUrl)
        {
            FileUrl = fileUrl;
        }
    }
}