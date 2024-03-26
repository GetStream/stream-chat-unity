using System.Threading.Tasks;
using StreamChat.Core.Requests;
using StreamChat.Core.StatefulModels;
using StreamChat.Libs.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StreamChat.SampleProject.Views
{
    /// <summary>
    /// Show local user info like: ID, Online status, Invisible status
    /// </summary>
    public class OnlineStatusView : BaseView
    {
        protected override void OnInited()
        {
            base.OnInited();

            State.Client.Connected += OnConnected;

            // StreamTodo: remove once the invisibility issue is resolved
            _toggleInvisibleButton.gameObject.SetActive(false);

            _toggleInvisibleButton.onClick.AddListener(OnToggleInvisible);
        }

        protected override void OnDisposing()
        {
            State.Client.Connected -= OnConnected;
            _toggleInvisibleButton.onClick.RemoveListener(OnToggleInvisible);

            base.OnDisposing();
        }

        [SerializeField]
        private TMP_Text _statusLabel;

        [SerializeField]
        private Button _toggleInvisibleButton;

        [SerializeField]
        private TMP_Text _toggleInvisibleButtonLabel;

        [SerializeField]
        private Image _toggleInvisibleButtonImage;

        private void OnConnected(IStreamLocalUserData localUserData)
        {
            UpdateInvisibilityToggleButton();
            UpdateOnlineStatus();
        }

        private void UpdateOnlineStatus()
        {
            var localUser = State.Client.LocalUserData.User;
            _statusLabel.text
                = $"Logged in: <color=#F9AC17><b>{localUser.Id}</b></color>, Online: <color=#F9AC17><b>{localUser.Online}</b></color>, Invisible: <color=#F9AC17><b>{localUser.Invisible}</b></color>";
        }

        private void UpdateInvisibilityToggleButton()
        {
            var localUser = State.Client.LocalUserData.User;
            _toggleInvisibleButtonLabel.text = localUser.Invisible ? "Set Visible" : "Set Invisible";
            _toggleInvisibleButtonImage.color = localUser.Invisible ? Color.gray : Color.green;
        }

        private void OnToggleInvisible() => ToggleInvisibleAsync().LogIfFailed();

        private Task ToggleInvisibleAsync()
        {
            var localUser = Client.LocalUserData.User;
            return localUser.Invisible ? localUser.MarkVisibleAsync() : localUser.MarkInvisibleAsync();
        }
    }
}