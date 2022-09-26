using StreamChat.Libs.Http;
using StreamChat.Libs.Logs;
using StreamChat.Libs.Serialization;
using StreamChat.Libs.Time;
using StreamChat.Libs.Websockets;

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

        public static IHttpClient CreateDefaultHttpClient()
        {
#if UNITY_WEBGL
            return new UnityWebRequestHttpClient();
#endif
            return new HttpClientAdapter();
        }

        public static ISerializer CreateDefaultSerializer() => new NewtonsoftJsonSerializer();

        public static ITimeService CreateDefaultTimeService() => new UnityTime();
    }
}