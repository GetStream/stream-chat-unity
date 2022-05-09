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
        public static ILogs CreateDefaultLogs() => new UnityLogs();
        public static IWebsocketClient CreateDefaultWebsocketClient(ILogs logs) => new WebsocketClient(logs);
        public static IHttpClient CreateDefaultHttpClient() => new HttpClientAdapter();
        public static ISerializer CreateDefaultSerializer() => new NewtonsoftJsonSerializer();
        public static ITimeService CreateDefaultTimeService() => new UnityTime();
    }
}