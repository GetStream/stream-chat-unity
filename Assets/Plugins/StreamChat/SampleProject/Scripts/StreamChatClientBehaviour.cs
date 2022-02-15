using System;
using StreamChat.Core;
using StreamChat.Core.Auth;
using StreamChat.SampleProject.Scripts.Utils;
using StreamChat.SampleProject.Views;
using UnityEngine;

namespace StreamChat.SampleProject
{

    /// <summary>
    /// Stream Chat Client MonoBehaviour
    /// </summary>
    public class StreamChatClientBehaviour : MonoBehaviour
    {
        protected void Awake()
        {
            try
            {
                _client = StreamChatClient.CreateDefaultClient(_authCredentialsAsset.Credentials);
                _client.Connect();

                var viewFactory = new ViewFactory(_client, _viewFactoryConfig, _popupsContainer);
                var viewContext = new ChatViewContext(_client, new UnityImageWebLoader(), viewFactory);
                viewFactory.Init(viewContext);

                _rootView.Init(viewContext);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        protected void Update() => _client?.Update(Time.deltaTime);

        protected void OnDestroy() => _client?.Dispose();

        private IStreamChatClient _client;

        [SerializeField]
        private RootView _rootView;

        [SerializeField]
        private AuthCredentialsAsset _authCredentialsAsset;

        [SerializeField]
        private ViewFactoryConfig _viewFactoryConfig;

        [SerializeField]
        private Transform _popupsContainer;
    }
}