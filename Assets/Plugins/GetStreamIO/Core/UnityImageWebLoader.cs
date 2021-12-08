using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Plugins.GetStreamIO.Core
{
    /// <summary>
    /// Unity <see cref="IImageLoader"/>
    /// </summary>
    public class UnityImageWebLoader : IImageLoader
    {
        public async Task<Texture2D> LoadImageAsync(string url)
        {
            //Todo: validate url

            if (_cachedImages.ContainsKey(url))
            {
                return _cachedImages[url];
            }

            if (_pendingRequests.Contains(url))
            {
                var tcs = new TaskCompletionSource<Texture2D>();

                if (!_activeRequests.ContainsKey(url))
                {
                    _activeRequests[url] = new List<TaskCompletionSource<Texture2D>>();
                }

                _activeRequests[url].Add(tcs);
                return await tcs.Task;
            }

            using (var webRequest = UnityWebRequestTexture.GetTexture(url))
            {
                var requestAsyncHandler = webRequest.SendWebRequest();
                _pendingRequests.Add(url);

                while (!requestAsyncHandler.isDone)
                {
                    await Task.Delay(1);
                }

                var request = requestAsyncHandler.webRequest;

                if (request.result != UnityWebRequest.Result.Success)
                {
                    throw new Exception($"Failed to download image from `{url}` with error: `{request.error}`");
                }

                _pendingRequests.Remove(url);

                var texture = DownloadHandlerTexture.GetContent(request);

                if (_activeRequests.ContainsKey(url))
                {
                    foreach (var tcs in _activeRequests[url])
                    {
                        tcs.SetResult(texture);
                    }

                    _activeRequests.Clear();
                }

                return _cachedImages[url] = texture;
            }
        }

        private static readonly HashSet<string> _pendingRequests = new HashSet<string>();

        private static readonly Dictionary<string, List<TaskCompletionSource<Texture2D>>> _activeRequests =
            new Dictionary<string, List<TaskCompletionSource<Texture2D>>>();

        private static readonly Dictionary<string, Texture2D> _cachedImages = new Dictionary<string, Texture2D>();
    }
}