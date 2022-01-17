using System;
using System.Threading.Tasks;
using GetStreamIO.Core.DTO.Models;
using Plugins.GetStreamIO.Core;
using Plugins.GetStreamIO.Core.Models.V2;
using Plugins.GetStreamIO.Libs.Utils;
using Plugins.GetStreamIO.Unity.Scripts.Popups;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Plugins.GetStreamIO.Unity.Scripts
{
    /// <summary>
    /// Message view
    /// </summary>
    public class MessageView : BaseView, IPointerDownHandler
    {
        public Message Message { get; private set; }

        public void UpdateData(Message message, IImageLoader imageLoader)
        {
            imageLoader = imageLoader ?? throw new ArgumentNullException(nameof(imageLoader));
            Message = message ?? throw new ArgumentNullException(nameof(message));

            _text.text = $"{GetMessageText(message)}<br>{Message.User.Name}";

            ShowAvatarAsync(Message.User.Image, imageLoader).LogIfFailed();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!Input.GetMouseButton(1))
            {
                return;
            }

            SetOptionsMenuActive(true);
        }

        protected override void OnDisposing()
        {
            _isDestroyed = true;

            base.OnDisposing();
        }

        private bool _isDestroyed;
        private MessageOptionsPopup _activePopup;

        [SerializeField]
        private TMP_Text _text;

        [SerializeField]
        private Image _avatar;

        private async Task ShowAvatarAsync(string url, IImageLoader imageLoader)
        {
            if (string.IsNullOrEmpty(url))
            {
                return;
            }

            Debug.Log("ShowAvatarAsync " + url);
            var sprite = await imageLoader.LoadImageAsync(url);

            if (_isDestroyed || sprite == null)
            {
                return;
            }

            _avatar.sprite = sprite;
        }

        private void SetOptionsMenuActive(bool active)
        {
            if (_activePopup != null)
            {
                Destroy(_activePopup.gameObject);
                _activePopup = null;
            }

            if (active)
            {
                var mousePosition = Input.mousePosition;

                _activePopup = Factory.CreateMessageOptionsPopup(this);

                ((RectTransform)_activePopup.transform).position = mousePosition;
            }
        }

        private static string GetMessageText(Message message)
            => message.Type == MessageType.Deleted ? ChatState.MessageDeletedInfo : message.Text;
    }
}