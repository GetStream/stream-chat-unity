using Plugins.GetStreamIO.Libs.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Plugins.GetStreamIO.Unity.Scripts
{
    /// <summary>
    /// Message input view
    /// </summary>
    public class MessageInputView : BaseView
    {
        protected void Awake()
        {
            _sendButton.onClick.AddListener(OnSendButtonClicked);
        }

        protected void Update()
        {
            if (Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey("return"))
            {
                OnSendButtonClicked();
            }
        }

        [SerializeField]
        private TMP_InputField _messageInput;

        [SerializeField]
        private Button _sendButton;

        private void OnSendButtonClicked()
        {
            if (_messageInput.text.Length == 0)
            {
                return;
            }

            Client.SendMessage(_messageInput.text);

            _messageInput.text = "";

            _messageInput.Select();
            _messageInput.ActivateInputField();
        }
    }
}