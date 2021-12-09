using System;
using Plugins.GetStreamIO.Core;
using Plugins.GetStreamIO.Core.Auth;
using Plugins.GetStreamIO.Unity.Scripts;
using UnityEngine;

namespace Plugins.GetStreamIO.Unity
{
    /// <summary>
    /// GetStream chat client MonoBehaviour
    /// </summary>
    public class GetStreamChatClientBehaviour : MonoBehaviour
    {
        protected void Awake()
        {
            var authData = new AuthData(new Uri(_serverUri), _userToken, _apiKey, _userId);

            _client = GetStreamChatClient.CreateDefaultClient(authData);
            _client.Start();

            var viewContext = new ChatViewContext(_client, new UnityImageWebLoader());

            _rootView.Init(viewContext);
        }

        protected void Update() => _client?.Update(Time.deltaTime);

        protected void OnDestroy() => _client?.Dispose();

        private IGetStreamChatClient _client;

        [SerializeField]
        private RootView _rootView;

        [Header("Authorization Data:")]
        [SerializeField]
        private string _serverUri;

        [SerializeField]
        private string _userToken;

        [SerializeField]
        private string _apiKey;

        [SerializeField]
        private string _userId;
    }
}