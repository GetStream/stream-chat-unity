using StreamChat.Core.Auth;
using UnityEngine;

namespace StreamChat.Core.Plugins.StreamChat.Core.Samples
{
    /// <summary>
    /// Sample MonoBehaviour that created and updates instance of <see cref="StreamChatClient"/>
    /// </summary>
    public class ChatBehaviourSample : MonoBehaviour
    {
        public IStreamChatClient Client { get; private set; }

        protected void Awake()
        {
            //Create AuthCredentials or optionally you can set them in asset and use _authCredentialsAsset.Credentials
            var authCredentials = new AuthCredentials(
                apiKey: "STREAM_CHAT_API_KEY",
                userToken: "USER_TOKEN",
                userId: "USER_ID");

            Client = StreamChatClient.CreateDefaultClient(authCredentials);

            //Initialize connection with the Stream Chat server
            Client.Connect();
        }

        //Client needs to updated per frame in order to maintain websockets connection
        protected void Update() => Client?.Update(Time.deltaTime);

        //Client needs to be disposed once its no longer needed in order to close the connection
        protected void OnDestroy() => Client?.Dispose();

        //Optionally you can use this asset to store credentials
        [SerializeField]
        private AuthCredentialsAsset _authCredentialsAsset;
    }
}