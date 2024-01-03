using System.Collections.Generic;
using StreamChat.SampleProject.Popups;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StreamChat.SampleProject.Views
{
    /// <summary>
    /// Error popup
    /// </summary>
    public class ErrorPopup : BaseFullscreenPopup, IPointerClickHandler
    {
        public void SetData(string title, string message, IDictionary<string, string> linkIdToUrlMap = null)
        {
            _header.text = title;
            _message.text = message;
            _linkIdToUrlMap = linkIdToUrlMap;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            var linkIndex = TMP_TextUtilities.FindIntersectingLink(_message, InputSystem.MousePosition, camera: null);
            if (linkIndex != -1)
            {
                var linkInfo = _message.textInfo.linkInfo[linkIndex];
                var linkId = linkInfo.GetLinkID();

                if (_linkIdToUrlMap.TryGetValue(linkId, out var url))
                {
                    Application.OpenURL(url);
                }
            }
        }

        protected override void OnInited()
        {
            base.OnInited();

            _okButton.onClick.AddListener(OnOkButtonClicked);
        }

        protected override void OnDisposing()
        {
            _okButton.onClick.RemoveListener(OnOkButtonClicked);
            
            base.OnDisposing();
        }

        [SerializeField]
        private TMP_Text _header;

        [SerializeField]
        private TMP_Text _message;

        [SerializeField]
        private Button _okButton;

        private IDictionary<string, string> _linkIdToUrlMap;

        private void OnOkButtonClicked() => Hide();
    }
}