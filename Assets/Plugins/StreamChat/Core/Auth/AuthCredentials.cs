using UnityEngine;

namespace StreamChat.Core.Auth
{
    /// <summary>
    /// Asset to keep auth credentials
    /// </summary>
    [CreateAssetMenu(fileName = "AuthCredentials", menuName = StreamChatClient.MenuPrefix + "Config/Create auth credentials asset", order = 1)]
    public class AuthCredentials : ScriptableObject
    {
        public AuthData Data => new AuthData(_userToken, _apiKey, _userId);
        [SerializeField]
        private string _userToken;

        [SerializeField]
        private string _apiKey;

        [SerializeField]
        private string _userId;
    }
}