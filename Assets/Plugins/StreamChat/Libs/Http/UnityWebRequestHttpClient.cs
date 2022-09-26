using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace StreamChat.Libs.Http
{
    /// <summary>
    /// <see cref="IHttpClient"/> implementation using <see cref="UnityWebRequest"/>
    /// </summary>
    public class UnityWebRequestHttpClient : IHttpClient
    {
        public void SetDefaultAuthenticationHeader(string value) => _headers["Authorization"] = value;

        public void AddDefaultCustomHeader(string key, string value) => _headers[key] = value;

        public Task<IHttpRequestResponse> PostAsync(Uri uri, string content)
            => SendWebRequest(uri, UnityWebRequest.kHttpVerbPOST, content);

        public Task<IHttpRequestResponse> DeleteAsync(Uri uri) => SendWebRequest(uri, UnityWebRequest.kHttpVerbDELETE);

        public Task<IHttpRequestResponse> GetAsync(Uri uri) => SendWebRequest(uri, UnityWebRequest.kHttpVerbGET);

        public Task<IHttpRequestResponse> PatchAsync(Uri uri, string content) => SendWebRequest(uri, "PATCH", content);

        public async Task<IHttpRequestResponse> PostAsync(Uri uri, HttpContent content)
        {
            var body = await content.ReadAsStringAsync();
            return await SendWebRequest(uri, UnityWebRequest.kHttpVerbPOST, body);
        }

        private readonly Dictionary<string, string> _headers = new Dictionary<string, string>();

        private async Task<IHttpRequestResponse> SendWebRequest(Uri uri, string httpMethod,
            string optionalContent = null)
        {
            using (var unityWebRequest = new UnityWebRequest(uri, httpMethod))
            {
                if (optionalContent != null)
                {
                    unityWebRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(optionalContent));
                }

                unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
                unityWebRequest.timeout = 5;

                foreach (var (key, value) in _headers)
                {
                    unityWebRequest.SetRequestHeader(key, value);
                }

                var asyncOperation = unityWebRequest.SendWebRequest();

                while (!asyncOperation.isDone)
                {
                    await Task.Yield();
                }

                if (unityWebRequest.result != UnityWebRequest.Result.Success)
                {
                    throw new Exception(unityWebRequest.error);
                }

                return new UnityWebRequestResponse(unityWebRequest);
            }
        }
    }
}