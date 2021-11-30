using System;
using Plugins.GetStreamIO.Core.Models;
using TMPro;
using UnityEngine;

namespace Plugins.GetStreamIO.Unity.Scripts
{
    /// <summary>
    /// Message view
    /// </summary>
    public class MessageView : MonoBehaviour
    {
        public void Init(Message message)
        {
            _message = message ?? throw new ArgumentNullException(nameof(message));

            _text.text = $"{_message.Text}<br>{_message.User.Name}";
        }

        private Message _message;

        [SerializeField]
        private TMP_Text _text;
    }
}