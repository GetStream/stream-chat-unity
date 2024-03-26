using System;
using System.Threading.Tasks;
using StreamChat.Core.StatefulModels;
using StreamChat.Libs.Utils;
using StreamChat.SampleProject.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StreamChat.SampleProject.Views
{
    /// <summary>
    /// Represents a single member on the channel member list
    /// </summary>
    public class MemberView : MonoBehaviour
    {
        public void UpdateData(IStreamChannelMember member, IImageLoader imageLoader)
        {
            if (_member != null)
            {
                _member.User.PresenceChanged -= OnlineStatusChanged;
            }

            _member = member;
            _member.User.PresenceChanged += OnlineStatusChanged;

            _label.text = GetName(_member.User);
            UpdateOnlineStatus(_member.User.Online);
            
            ShowAvatarAsync(_member.User.Image, imageLoader).LogIfFailed();
        }

        protected void OnDestroy()
        {
            _isDestroyed = true;
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
        
        [SerializeField]
        private Image _avatar;

        private IStreamChannelMember _member;
        private bool _isDestroyed;

        private async Task ShowAvatarAsync(string url, IImageLoader imageLoader)
        {
            if (string.IsNullOrEmpty(url))
            {
                return;
            }

            var sprite = await imageLoader.LoadImageAsync(url);
            if (_isDestroyed || sprite == null)
            {
                return;
            }

            _avatar.sprite = sprite;
        }
        
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