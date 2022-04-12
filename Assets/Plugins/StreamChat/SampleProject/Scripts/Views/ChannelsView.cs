using StreamChat.SampleProject.Views;
using UnityEngine;
using UnityEngine.UI;

namespace StreamChat.SampleProject
{
    /// <summary>
    /// Channels view
    /// </summary>
    public class ChannelsView : BaseView
    {
        protected void Awake()
        {
            _channelCreateButton.onClick.AddListener(OnCreateChannelClicked);
        }

        protected void OnDestroy()
        {
            _channelCreateButton.onClick.RemoveListener(OnCreateChannelClicked);
        }

        [SerializeField]
        private Button _channelCreateButton;

        private void OnCreateChannelClicked()
            => State.ShowPopup<CreateNewChannelFormPopup>();
    }
}