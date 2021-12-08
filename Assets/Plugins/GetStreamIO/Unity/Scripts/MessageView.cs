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

            ShowAvatarAsync(_message.User.ImageUrl, imageLoader)
                .ContinueWith(_ => Debug.LogError(_.Exception), TaskContinuationOptions.OnlyOnFaulted); //Todo: create extension LogIfFailed
        }

        private Message _message;

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
            var texture = await imageLoader.LoadImageAsync(url);

            _avatar.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }
    }
}