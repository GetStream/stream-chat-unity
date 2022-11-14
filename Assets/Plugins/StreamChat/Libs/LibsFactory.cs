using StreamChat.Libs.Http;
using StreamChat.Libs.Logs;
using StreamChat.Libs.ChatInstanceRunner;
using StreamChat.Libs.Serialization;
using StreamChat.Libs.Time;
using StreamChat.Libs.Websockets;
using UnityEngine;

namespace StreamChat.Libs
{
    /// <summary>
    /// Provides default implementation of Libs services
    /// </summary>
    public static class LibsFactory
    {
        public static ILogs CreateDefaultLogs(UnityLogs.LogLevel logLevel = UnityLogs.LogLevel.All)
            => new UnityLogs(logLevel);

        public static IWebsocketClient CreateDefaultWebsocketClient(ILogs logs, bool isDebugMode = false)
            => new WebsocketClient(logs, isDebugMode: isDebugMode);

        public static IHttpClient CreateDefaultHttpClient() => new HttpClientAdapter();

        public static ISerializer CreateDefaultSerializer() => new NewtonsoftJsonSerializer();

        public static ITimeService CreateDefaultTimeService() => new UnityTime();

        public static IStreamChatClientRunner CreateChatClientRunner()
        {
            var go = new GameObject
            {
                name = "Stream Chat Client Runner",
#if !STREAM_DEBUG_ENABLED
                hideFlags = HideFlags.DontSaveInEditor | HideFlags.HideAndDontSave
#endif
            };
            return go.AddComponent<StreamMonoBehaviourWrapper.UnityStreamChatClientRunner>();
        }
    }
}