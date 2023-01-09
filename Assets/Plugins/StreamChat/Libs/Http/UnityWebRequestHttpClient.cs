using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
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

        public Task<HttpResponse> GetAsync(Uri uri) => SendWebRequest(uri, UnityWebRequest.kHttpVerbGET);

        public Task<HttpResponse> PostAsync(Uri uri, object content)
            => SendWebRequest(uri, UnityWebRequest.kHttpVerbPOST, content);

        public Task<HttpResponse> PutAsync(Uri uri, object content)
            => SendWebRequest(uri, UnityWebRequest.kHttpVerbPUT, content);

        public Task<HttpResponse> PatchAsync(Uri uri, object content) => SendWebRequest(uri, "PATCH", content);

        public Task<HttpResponse> DeleteAsync(Uri uri) => SendWebRequest(uri, UnityWebRequest.kHttpVerbDELETE);

        public Task<HttpResponse> SendHttpRequestAsync(HttpMethodType methodType, Uri uri,
            object optionalRequestContent)
        {
            var httpMethodKey = GetHttpMethodKey(methodType);
            return SendWebRequest(uri, httpMethodKey, optionalRequestContent);
        }

        private readonly Dictionary<string, string> _headers = new Dictionary<string, string>();

        private static string GetHttpMethodKey(HttpMethodType methodType)
        {
            switch (methodType)
            {
                case HttpMethodType.Get: return UnityWebRequest.kHttpVerbGET;
                case HttpMethodType.Post: return UnityWebRequest.kHttpVerbPOST;
                case HttpMethodType.Put: return UnityWebRequest.kHttpVerbPUT;
                case HttpMethodType.Patch: return "PATCH";
                case HttpMethodType.Delete: return UnityWebRequest.kHttpVerbDELETE;
                default:
                    throw new ArgumentOutOfRangeException(nameof(methodType), methodType, null);
            }
        }

        private async Task<HttpResponse> SendWebRequest(Uri uri, string httpMethod,
            object optionalContent = null)
        {
            if (optionalContent is FileWrapper fileWrapper2)
            {
                var formData = new List<IMultipartFormSection>
                {
                    new MultipartFormFileSection("file", fileWrapper2.FileContent, fileWrapper2.FileName,
                        "multipart/form-data")
                };

                var unityWebRequest = UnityWebRequest.Post(uri, formData);

                unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
                unityWebRequest.timeout = 5;

                foreach (var pair in _headers)
                {
                    unityWebRequest.SetRequestHeader(pair.Key, pair.Value);
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

                return HttpResponse.CreateFromUnityWebRequest(unityWebRequest);
            }

            using (var unityWebRequest = new UnityWebRequest(uri, httpMethod))
            {
                if (optionalContent == null)
                {
                }
                else if (optionalContent is string stringContent)
                {
                    unityWebRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(stringContent));
                }
                else
                {
                    throw new NotImplementedException(
                        $"Not implemented support for body object type of {optionalContent.GetType()}");
                }

                unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
                unityWebRequest.timeout = 5;

                foreach (var pair in _headers)
                {
                    unityWebRequest.SetRequestHeader(pair.Key, pair.Value);
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

                return HttpResponse.CreateFromUnityWebRequest(unityWebRequest);
            }
        }
    }
}