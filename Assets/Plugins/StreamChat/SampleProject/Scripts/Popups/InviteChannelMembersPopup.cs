using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StreamChat.SampleProject.Popups
{
    /// <summary>
    /// Popup that allows to invite members to <see cref="IChatState.ActiveChannel"/>
    /// </summary>
    public class InviteChannelMembersPopup : BaseFullscreenPopup
    {
        protected override void OnInited()
        {
            base.OnInited();

            _inviteButton.onClick.AddListener(OnInviteButtonClicked);
        }

        protected override void OnShow()
        {
            base.OnShow();

            _usersIdInput.Select();
            _usersIdInput.ActivateInputField();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (InputSystem.WasEnteredPressedThisFrame && !_isProcessing)
            {
                OnInviteButtonClicked();
            }
        }

        protected override void OnDisposing()
        {
            _inviteButton.onClick.RemoveListener(OnInviteButtonClicked);

            base.OnDisposing();
        }

        [SerializeField]
        private TMP_InputField _usersIdInput;

        [SerializeField]
        private Button _inviteButton;

        private bool _isProcessing;

        private void OnInviteButtonClicked()
        {
            if (_isProcessing)
            {
                return;
            }

            if (State.ActiveChannel == null)
            {
                Debug.LogError($"Tried to invite users but there is not active channel.");
                return;
            }

            if (string.IsNullOrEmpty(_usersIdInput.text.Trim()))
            {
                Debug.LogError($"Tried to invite users but no user IDs where given.");
                return;
            }

            var ids = _usersIdInput.text.Split(',').Select(_ => _.Trim()).ToArray();
            InviteMembersAsync(ids).ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    Debug.LogException(t.Exception);
                    return;
                }

                Debug.Log("Sent invite to users with IDs: " + string.Join(", ", ids));
                Hide();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async Task InviteMembersAsync(IEnumerable<string> userIds)
        {
            _isProcessing = true;
            try
            {
                await State.ActiveChannel.InviteMembersAsync(userIds);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                _isProcessing = false;
            }
        }
    }
}