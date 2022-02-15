using System;
using System.Threading.Tasks;
using StreamChat.Core.DTO.Models;
using StreamChat.Core.Models;
using StreamChat.Libs.Utils;
using StreamChat.SampleProject.Scripts.Utils;
using StreamChat.SampleProject.Popups;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
//This requires StreamChat.Unity assembly definition to have a reference to UnityEngine.InputSystem package
using UnityEngine.InputSystem;
#endif

namespace StreamChat.SampleProject.Views
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
#if ENABLE_LEGACY_INPUT_MANAGER
            if (!Input.GetMouseButton(1))
#elif ENABLE_INPUT_SYSTEM
            //This requires StreamChat.Unity assembly definition to have a reference to UnityEngine.InputSystem package
            if (!Mouse.current.rightButton.wasPressedThisFrame)
#endif
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