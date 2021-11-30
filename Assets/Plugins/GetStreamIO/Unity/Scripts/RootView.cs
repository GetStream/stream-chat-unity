using UnityEngine;

namespace Plugins.GetStreamIO.Unity.Scripts
{
    /// <summary>
    /// Root view
    /// </summary>
    public class RootView : BaseView
    {
        protected override void OnInited()
        {
            base.OnInited();

            _channelsListView.Init(Client);
            _chatView.Init(Client);
            _createMessageView.Init(Client);
            _consoleView.Init(Client);
        }

        [SerializeField]
        private ChannelsListView _channelsListView;

        [SerializeField]
        private ChatView _chatView;

        [SerializeField]
        private CreateMessageView _createMessageView;

        [SerializeField]
        private ConsoleView _consoleView;
    }
}