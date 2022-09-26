using System.Net.Http;

namespace StreamChat.Libs.Http
{
    /// <summary>
    /// Implementation of <see cref="IHttpRequestResponse"/> for .NET <see cref="HttpClient"/>
    /// </summary>
    public class NetHttpRequestResponse : HttpRequestResponseBase<HttpResponseMessage>
    {
        public NetHttpRequestResponse(string result, HttpResponseMessage innerResponse)
            : base(innerResponse.IsSuccessStatusCode, result, innerResponse)
        {
        }
    }
}