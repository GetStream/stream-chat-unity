using StreamChat.Core;
using StreamChat.Libs.Auth;

namespace Plugins.StreamChat.Samples.ClientDocs
{
    /// <summary>
    /// Code samples for the <see cref="IStreamChatClient"/> https://getstream.io/chat/docs/unity/init_and_users/?language=unity
    /// </summary>
    public class ChatClientCodeSamples
    {
        public void Initialize()
        {
            var authCredentials = new AuthCredentials(
                apiKey: "STREAM_CHAT_API_KEY",
                userId: "USER_ID",
                userToken: "USER_TOKEN");

            var client = StreamChatClient.CreateDefaultClient(authCredentials);

            //Initialize connection with the Stream Chat server
            client.Connect();
        }

        // public async Task ConnectUser()
        // {
        //     //TodoL implement switching users
        // }

        public void Disconnect()
        {
            //Todo: implement disconnecting user without disposing a client

            var authCredentials = new AuthCredentials(
                apiKey: "STREAM_CHAT_API_KEY",
                userId: "USER_ID",
                userToken: "USER_TOKEN");

            var client = StreamChatClient.CreateDefaultClient(authCredentials);

            //Initialize connection with the Stream Chat server
            client.Connect();

            client.Dispose();
        }
    }
}