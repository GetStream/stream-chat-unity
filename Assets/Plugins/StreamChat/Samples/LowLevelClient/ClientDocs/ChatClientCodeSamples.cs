using StreamChat.Core.LowLevelClient;
using StreamChat.Libs.Auth;

namespace StreamChat.Samples.LowLevelClient.ClientDocs
{
    /// <summary>
    /// Code samples for the <see cref="IStreamChatLowLevelClient"/> https://getstream.io/chat/docs/unity/init_and_users/?language=unity
    /// </summary>
    public class ChatClientCodeSamples
    {
        public void Initialize()
        {
            var authCredentials = new AuthCredentials(
                apiKey: "STREAM_CHAT_API_KEY",
                userId: "USER_ID",
                userToken: "USER_TOKEN");

            var client = StreamChatLowLevelClient.CreateDefaultClient(authCredentials);

            //Initialize connection with the Stream Chat server
            client.Connect();
        }

        // public async Task ConnectUser()
        // {
        //     //TodoL implement switching users
        // }

        public void Disconnect()
        {
            //StreamTodo: implement disconnecting user without disposing a client

            var authCredentials = new AuthCredentials(
                apiKey: "STREAM_CHAT_API_KEY",
                userId: "USER_ID",
                userToken: "USER_TOKEN");

            var client = StreamChatLowLevelClient.CreateDefaultClient(authCredentials);

            //Initialize connection with the Stream Chat server
            client.Connect();

            client.Dispose();
        }
    }
}