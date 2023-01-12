using System.Threading.Tasks;
using StreamChat.Core.Helpers;
using StreamChat.Libs.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StreamChat.SampleProject.Views
{
    /// <summary>
    /// Create new channel form
    /// </summary>
    public class CreateNewChannelFormPopup : BaseFullscreenPopup
    {
        protected override void OnShow()
        {
            base.OnShow();

            _createButton.onClick.AddListener(OnCreateButtonClicked);

            _channelIdInput.Select();
            _channelIdInput.ActivateInputField();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (InputSystem.WasEnteredPressedThisFrame && !_isProcessing)
            {
                OnCreateButtonClicked();
            }
        }

        [SerializeField] private TMP_InputField _channelIdInput;

        [SerializeField] private Button _createButton;

        private bool _isProcessing;

        private void OnCreateButtonClicked()
        {
            if (_channelIdInput.text.IsNullOrEmpty())
            {
                Debug.LogError("Channel id is required");
                return;
            }

            if (_isProcessing)
            {
                return;
            }

            _isProcessing = true;

            State.CreateNewChannelAsync(_channelIdInput.text).ContinueWith(task =>
            {
                _isProcessing = false;

                if (task.IsFaulted)
                {
                    Debug.LogError("Adding new channel failed with exception");
                    Debug.LogException(task.Exception.InnerException);
                    return;
                }

                var channel = task.Result;

                Debug.Log("Added new channel with id: " + channel.Id);

                channel.AddMembersAsync(new[] {Client.LocalUserData.User}).ContinueWith(_ =>
                {
                    State.UpdateChannelsAsync().LogExceptionsOnFailed();

                    Hide();
                });
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}