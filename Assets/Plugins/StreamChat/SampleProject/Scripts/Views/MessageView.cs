using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StreamChat.Core.DTO.Models;
using StreamChat.Core.Models;
using StreamChat.Libs.Utils;
using StreamChat.SampleProject.Utils;
using StreamChat.SampleProject.Popups;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

            ShowReactions(Message);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!InputSystem.GetMouseButton(1))
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

        [SerializeField]
        private Transform _emojisContainer;

        [SerializeField]
        private Image _emojiPrefab;

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

        private void ShowReactions(Message message)
        {
            foreach (var reactionCount in message.ReactionCounts)
            {
                Factory.CreateReactionEmoji(_emojiPrefab, _emojisContainer, reactionCount.Key);
            }
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
                var mousePosition = InputSystem.MousePosition;

                _activePopup = Factory.CreateMessageOptionsPopup(this, State);

                var rectTransform = ((RectTransform)_activePopup.transform);

                rectTransform.position = mousePosition + new Vector2(-10, 10);
            }
        }

        private static string GetMessageText(Message message)
            => message.Type == MessageType.Deleted ? ChatState.MessageDeletedInfo : message.Text;
    }
}