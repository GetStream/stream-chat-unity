using UnityEngine.Networking;

namespace StreamChat.Libs.Http
{
    /// <summary>
    /// <see cref="IHttpRequestResponse"/> implementation for <see cref="UnityWebRequest"/>
    /// </summary>
    public class UnityWebRequestResponse : HttpRequestResponseBase<UnityWebRequest>
    {
        public UnityWebRequestResponse(UnityWebRequest innerResponse)
            : base(innerResponse.result == UnityWebRequest.Result.Success,
                innerResponse.downloadHandler?.text ?? string.Empty, innerResponse)
        {
        }
    }
}