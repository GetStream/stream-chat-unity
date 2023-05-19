using StreamChat.SampleProject.Popups;
using UnityEngine;
using UnityEngine.UI;

namespace StreamChat.SampleProject.Views
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

        protected override void OnDisposing()
        {
            _channelCreateButton.onClick.RemoveListener(OnCreateChannelClicked);

            base.OnDisposing();
        }

        [SerializeField]
        private Button _channelCreateButton;

        private void OnCreateChannelClicked()
            => State.ShowPopup<CreateNewChannelFormPopup>();
    }
}