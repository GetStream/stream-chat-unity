namespace StreamChat.Libs.Http
{
    /// <summary>
    /// Base class for <see cref="IHttpRequestResponse"/>
    /// </summary>
    /// <typeparam name="TInnerResponse"></typeparam>
    public abstract class HttpRequestResponseBase<TInnerResponse> : IHttpRequestResponse
    {
        public bool IsSuccessStatusCode { get; }
        public string Result { get; }

        public TInnerResponse InnerResponse { get;}

        public HttpRequestResponseBase(bool isSuccessStatusCode, string result, TInnerResponse innerResponse)
        {
            IsSuccessStatusCode = isSuccessStatusCode;
            Result = result;
            InnerResponse = innerResponse;
        }
    }
}