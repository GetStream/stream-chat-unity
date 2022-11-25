using System;
using StreamChat.SampleProject.Popups;
using StreamChat.SampleProject.Views;
using UnityEngine;

namespace StreamChat.SampleProject.Configs
{
    [Serializable]
    public struct ViewFactoryConfig : IViewFactoryConfig
    {
        public MessageOptionsPopup MessageOptionsPopupPrefab => _messageOptionsPopupPrefab;
        public CreateNewChannelFormPopup CreateNewChannelFormPopupPrefab => _createNewChannelPopupPrefab;
        public ErrorPopup ErrorPopupPrefab => _errorPopupPrefab;

        [SerializeField]
        private MessageOptionsPopup _messageOptionsPopupPrefab;

        [SerializeField]
        private CreateNewChannelFormPopup _createNewChannelPopupPrefab;

        [SerializeField]
        private ErrorPopup _errorPopupPrefab;
    }
}