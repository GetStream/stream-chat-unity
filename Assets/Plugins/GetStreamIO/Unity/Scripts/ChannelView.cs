using System;
using Plugins.GetStreamIO.Core.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Plugins.GetStreamIO.Unity.Scripts
{
    /// <summary>
    /// Single channel entry view
    /// </summary>
    public class ChannelView : MonoBehaviour
    {
        public event Action<Channel> Clicked;

        public void Init(Channel channel)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));

            _text.text = $"<b>{channel.Details.Name}</b> (Members: {channel.Details.MemberCount}) <br> Last update: {channel.Details.UpdatedAt}";
        }

        protected void Awake()
        {
            _button.onClick.AddListener(OnClicked);
        }

        private Channel _channel;

        [SerializeField]
        private TMP_Text _text;

        [SerializeField]
        private Button _button;

        private void OnClicked() => Clicked?.Invoke(_channel);
    }
}