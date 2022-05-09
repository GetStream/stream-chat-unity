using StreamChat.Libs.Auth;
using UnityEngine;

namespace StreamChat.Core.Samples
{
    /// <summary>
    /// Sample MonoBehaviour that creates and updates instance of <see cref="StreamChatClient"/>
    /// </summary>
    public class ChatBehaviourSample : MonoBehaviour
    {
        public IStreamChatClient Client { get; private set; }

        protected void Awake()
        {
            //Create AuthCredentials or optionally you can set them in asset and use _authCredentialsAsset.Credentials
            var authCredentials = new AuthCredentials(
                apiKey: "STREAM_CHAT_API_KEY",
                userId: "USER_ID",
                userToken: "USER_TOKEN");

            Client = StreamChatClient.CreateDefaultClient(authCredentials);

            //Initialize connection with the Stream Chat server
            Client.Connect();
        }

        //Client needs to be updated per frame in order to maintain websocket connection and exchange data with API
        protected void Update() => Client?.Update(Time.deltaTime);

        //Client needs to be disposed once its no longer needed in order to close the connection
        protected void OnDestroy() => Client?.Dispose();

        //Optionally you can use this asset to store credentials
        [SerializeField]
        private AuthCredentialsAsset _authCredentialsAsset;
    }
}