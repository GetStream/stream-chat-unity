using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StreamChat.Libs.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace StreamChat.SampleProject.Utils
{
    /// <summary>
    /// Unity <see cref="IImageLoader"/>
    /// </summary>
    public class UnityImageWebLoader : IImageLoader
    {
        public async Task<Sprite> LoadImageAsync(string url)
        {
            //StreamTodo: validate url

            if (_cachedImages.ContainsKey(url))
            {
                return _cachedImages[url];
            }

            if (_pendingRequests.Contains(url))
            {
                var tcs = new TaskCompletionSource<Sprite>();

                if (!_subscribers.ContainsKey(url))
                {
                    _subscribers[url] = new List<TaskCompletionSource<Sprite>>();
                }

                _subscribers[url].Add(tcs);
                return await tcs.Task;
            }

            using (var webRequest = UnityWebRequestTexture.GetTexture(url))
            using (new TimeLogScope($"Download avatar `{url}`", Debug.Log))
            {
                var requestAsyncHandler = webRequest.SendWebRequest();
                _pendingRequests.Add(url);

                while (!requestAsyncHandler.isDone)
                {
                    await Task.Delay(1);
                }

                _pendingRequests.Remove(url);

                var request = requestAsyncHandler.webRequest;

                if (request.result != UnityWebRequest.Result.Success)
                {
                    if (request.responseCode == 404)
                    {
                        Debug.LogWarning($"Tried to download `{url}`, but resource is missing with 404 response code");
                        return null;
                    }

                    throw new Exception($"Failed to download image from `{url}` with error: `{request.error}`");
                }

                var texture = DownloadHandlerTexture.GetContent(request);
                var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

                if (_subscribers.ContainsKey(url))
                {
                    foreach (var tcs in _subscribers[url])
                    {
                        tcs.SetResult(sprite);
                    }

                    _subscribers[url].Clear();
                }

                return _cachedImages[url] = sprite;
            }
        }

        private static readonly HashSet<string> _pendingRequests = new HashSet<string>();

        private static readonly Dictionary<string, List<TaskCompletionSource<Sprite>>> _subscribers =
            new Dictionary<string, List<TaskCompletionSource<Sprite>>>();

        private static readonly Dictionary<string, Sprite> _cachedImages = new Dictionary<string, Sprite>();
    }
}