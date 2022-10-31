using System;
using StreamChat.SampleProject_StateClient.Popups;
using StreamChat.SampleProject_StateClient.Views;
using UnityEngine;

namespace StreamChat.SampleProject_StateClient.Configs
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