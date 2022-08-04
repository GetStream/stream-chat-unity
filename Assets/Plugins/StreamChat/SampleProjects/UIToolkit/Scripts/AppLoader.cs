using StreamChat.Libs.Auth;
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

            _state = new ChatState(_authCredentials.Credentials);
            _rootView = viewFactory.CreateRootView(_state);
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



