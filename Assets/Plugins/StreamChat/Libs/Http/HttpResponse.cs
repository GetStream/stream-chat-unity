using System.Net.Http;
using System.Threading.Tasks;
using StreamChat.Libs.Utils;
using UnityEngine.Networking;

namespace StreamChat.Libs.Http
{
    /// <summary>
    /// Response wrapper struct for an Http request
    /// </summary>
    public readonly struct HttpResponse
    {
        public bool IsSuccessStatusCode { get; }
        public int StatusCode { get; }
        public string Result { get; }

        public static async Task<HttpResponse> CreateFromHttpResponseMessageAsync(
            HttpResponseMessage httpResponseMessage)
        {
            var result = await httpResponseMessage.Content.ReadAsStringAsync();
            return new HttpResponse(httpResponseMessage.IsSuccessStatusCode, (int)httpResponseMessage.StatusCode, result);
        }

        public static HttpResponse CreateFromUnityWebRequest(UnityWebRequest unityWebRequest)
        {
            var result = unityWebRequest.downloadHandler?.text ?? string.Empty;
            return new HttpResponse(unityWebRequest.IsRequestSuccessful(), (int)unityWebRequest.responseCode, result);
        }

        public HttpResponse(bool isSuccessStatusCode, int statusCode, string result)
        {
            IsSuccessStatusCode = isSuccessStatusCode;
            StatusCode = statusCode;
            Result = result;
        }
    }
}