﻿using System;
using System.Linq;
using System.Threading.Tasks;
using StreamChat.Core.Models;
using StreamChat.Core.StatefulModels;
using StreamChat.Libs.Utils;
using StreamChat.SampleProject.Popups;
using StreamChat.SampleProject.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

namespace StreamChat.SampleProject.Views
{
    /// <summary>
    /// Message view
    /// </summary>
    public class MessageView : BaseView, IPointerDownHandler
    {
        public IStreamMessage Message { get; private set; }

        public void UpdateData(IStreamMessage message, IImageLoader imageLoader)
        {
            imageLoader = imageLoader ?? throw new ArgumentNullException(nameof(imageLoader));
            Message = message ?? throw new ArgumentNullException(nameof(message));

            var isVideo = HasVideoAttachment(message, out var videoAttachment);

            _videoPlayer.gameObject.SetActive(isVideo);
            _text.gameObject.SetActive(!isVideo);

            if (isVideo)
            {
                if (_renderTexture != null)
                {
                    _renderTexture.Release();
                }

                _renderTexture = new RenderTexture(_sourceRenderTexture);

                _videoPlayer.playOnAwake = false;
                _videoPlayer.isLooping = false;
                _videoPlayer.url = videoAttachment.AssetUrl;

                _videoPlayer.targetTexture = _renderTexture;
                _videoPlayer.GetComponentInChildren<RawImage>().texture = _renderTexture;
            }

            _text.text = $"{GetMessageText(message)}<br>{Message.User.Name}";

            ShowAvatarAsync(Message.User.Image, imageLoader).LogIfFailed();

            ShowReactions(Message);
        }

        public void TryPlay()
        {
            if (!_videoPlayer.url.IsNullOrEmpty())
            {
                Play();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!InputSystem.GetMouseButton(1))
            {
                return;
            }

            SetOptionsMenuActive(true);
        }

        protected void Awake()
        {
            _playButton.onClick.AddListener(OnPlayButtonClicked);
            _videoPlayer.loopPointReached += OnVideoPlayerLoopPointReached;
        }

        protected override void OnDisposing()
        {
            _isDestroyed = true;

            if (_renderTexture != null)
            {
                _renderTexture.Release();
            }

            base.OnDisposing();
        }

        private bool _isDestroyed;
        private MessageOptionsPopup _activePopup;
        private RenderTexture _renderTexture;

        [SerializeField]
        private TMP_Text _text;

        [SerializeField]
        private VideoPlayer _videoPlayer;

        [SerializeField]
        private Button _playButton;

        [SerializeField]
        private Image _avatar;

        [SerializeField]
        private Transform _emojisContainer;

        [SerializeField]
        private Image _emojiPrefab;

        [SerializeField]
        private RenderTexture _sourceRenderTexture;

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

        private void ShowReactions(IStreamMessage message)
        {
            foreach (var reactionCount in message.ReactionCounts)
            {
                Factory.CreateEmoji(_emojiPrefab, _emojisContainer, reactionCount.Key);
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

        private static string GetMessageText(IStreamMessage message)
            => message.IsDeleted ? ChatState.MessageDeletedInfo : message.Text;

        private bool HasVideoAttachment(IStreamMessage message, out StreamMessageAttachment videoAttachment)
        {
            for (int i = 0; i < message.Attachments.Count; i++)
            {
                var attachment = message.Attachments[0];

                if (AllowedVideoFormats.Contains(attachment.Type))
                {
                    videoAttachment = attachment;
                    return true;
                }
            }

            videoAttachment = default;
            return false;
        }

        private void OnPlayButtonClicked()
        {
            Play();
        }

        private void Play()
        {
            _videoPlayer.Play();
            _playButton.gameObject.SetActive(false);
        }

        private void OnVideoPlayerLoopPointReached(VideoPlayer source)
        {
            _playButton.gameObject.SetActive(true);
        }
    }
}