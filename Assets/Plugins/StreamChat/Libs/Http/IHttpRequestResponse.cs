namespace StreamChat.Libs.Http
{
    /// <summary>
    /// Http request response
    /// </summary>
    public interface IHttpRequestResponse
    {
        bool IsSuccessStatusCode { get; }
        string Result { get; }
    }
}