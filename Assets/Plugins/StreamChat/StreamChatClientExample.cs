using StreamChat.Core;
using StreamChat.Core.StatefulModels;
using StreamChat.Libs.Utils;
using UnityEngine;

namespace StreamChat
{
    /// <summary>
    /// Example showing how to create instance of <see cref="IStreamChatClient"/>
    /// </summary>
    public class StreamChatClientExample : MonoBehaviour
    {
        public IStreamChatClient Client { get; private set; }
        
        void Start()
        {
            // Init 
            Client = StreamChatClient.CreateDefaultClient();
            Client.Connected += OnConnected;

            // Get API_KEY from https://dashboard.getstream.io/
            // To get USER_ID and USER_TOKEN:
            // During Development - Enable "Developer Tokens" -> https://getstream.io/chat/docs/unity/tokens_and_authentication/?language=unity#developer-tokens
            // During Production - User Token Provider -> https://getstream.io/chat/docs/unity/tokens_and_authentication/?language=unity#how-to-refresh-expired-tokens
            Client.ConnectUserAsync("API_KEY", "USER_ID", "USER_TOKEN").LogIfFailed();
        }

        private void OnConnected(IStreamLocalUserData localUserData)
        {
            Debug.Log($"user {localUserData.User.Id} is now connected");
        }
    }
}

