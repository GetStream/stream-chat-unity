using System;
using System.Threading.Tasks;
using Plugins.GetStreamIO.Core;
using Plugins.GetStreamIO.Core.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Plugins.GetStreamIO.Unity.Scripts
{
    /// <summary>
    /// Message view
    /// </summary>
    public class MessageView : MonoBehaviour
    {
        public void Init(Message message, IImageLoader imageLoader)
        {
            imageLoader = imageLoader ?? throw new ArgumentNullException(nameof(imageLoader));
            _message = message ?? throw new ArgumentNullException(nameof(message));

            _text.text = $"{_message.Text}<br>{_message.User.Name}";

            ShowAvatarAsync(_message.User.Image, imageLoader)
                .ContinueWith(_ => Debug.LogError(_.Exception), TaskContinuationOptions.OnlyOnFaulted); //Todo: create extension LogIfFailed
        }

        protected void OnDestroy()
        {
            _isDestroyed = true;
        }

        private Message _message;
        private bool _isDestroyed;

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
    }
}