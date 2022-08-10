using StreamChat.Core;
using StreamChat.Libs.Auth;
using StreamChat.SampleProjects.UIToolkit.Config;
using StreamChat.SampleProjects.UIToolkit.Views;
using UnityEngine;
using UnityEngine.UIElements;

namespace StreamChat.SampleProjects.UIToolkit
{
    /// <summary>
    /// Represents Application main entry point. Initializes State & RootView as well as
    /// </summary>
    public class AppLoader : MonoBehaviour
    {
        protected void Start()
        {
            var uiDocument = GetComponent<UIDocument>();

            var viewFactory = new ViewFactory(uiDocument.rootVisualElement, _viewConfig);

            var streamChatClient = StreamChatClient.CreateDefaultClient(_authCredentials.Credentials);
            streamChatClient.Connect();

            _state = new ChatState(streamChatClient);
            var chatWriter = new ChatWriter(streamChatClient, _state);

            _rootView = viewFactory.CreateRootView(_state, chatWriter);
        }

        protected void Update()
            => _state.Update(Time.deltaTime);

        protected void OnDestroy()
        {
            _state.Dispose();
            _state = null;
        }

        [SerializeField]
        private ViewConfig _viewConfig;

        [SerializeField]
        private AuthCredentialsAsset _authCredentials;

        private RootView _rootView;
        private ChatState _state;
    }
}



