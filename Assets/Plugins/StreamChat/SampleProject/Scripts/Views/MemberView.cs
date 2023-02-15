using System;
using StreamChat.Core.StatefulModels;
using StreamChat.Libs.Utils;
using TMPro;
using UnityEngine;

namespace StreamChat.SampleProject.Views
{
    /// <summary>
    /// Represents a single member on the channel member list
    /// </summary>
    public class MemberView : MonoBehaviour
    {
        public void UpdateData(IStreamChannelMember member)
        {
            if (_member != null)
            {
                _member.User.PresenceChanged -= OnlineStatusChanged;
            }

            _member = member;
            _member.User.PresenceChanged += OnlineStatusChanged;

            _label.text = GetName(_member.User);
            UpdateOnlineStatus(_member.User.Online);
        }

        protected void OnDestroy()
        {
            if (_member == null)
            {
                return;
            }

            _member.User.PresenceChanged -= OnlineStatusChanged;
        }

        [SerializeField]
        private TMP_Text _label;

        [SerializeField]
        private GameObject _statusIsOnline;

        [SerializeField]
        private GameObject _statusIsOffline;

        private IStreamChannelMember _member;

        private void UpdateOnlineStatus(bool online)
        {
            _statusIsOnline.SetActive(online);
            _statusIsOffline.SetActive(!online);
        }

        private void OnlineStatusChanged(IStreamUser user, bool isOnline, DateTimeOffset? lastActive)
            => UpdateOnlineStatus(isOnline);

        private string GetName(IStreamUser user) => user.Name.IsNullOrEmpty() ? user.Id : user.Name; // Name is optional
    }
}