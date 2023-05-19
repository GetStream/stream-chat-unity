using System;
using StreamChat.SampleProject.Popups;
using StreamChat.SampleProject.Views;
using UnityEngine;
using UnityEngine.Serialization;

namespace StreamChat.SampleProject.Configs
{
    [Serializable]
    public struct ViewFactoryConfig : IViewFactoryConfig
    {
        public MessageOptionsPopup MessageOptionsPopupPrefab => _messageOptionsPopupPrefab;
        public CreateNewChannelFormPopup CreateNewChannelFormPopupPrefab => _createNewChannelPopupPrefab;
        public InviteChannelMembersPopup InviteChannelMembersPopupPrefab => _inviteChannelMembersPopup;
        public InviteReceivedPopup InviteReceivedPopup => inviteReceivedPopup;
        public ErrorPopup ErrorPopupPrefab => _errorPopupPrefab;

        [SerializeField]
        private MessageOptionsPopup _messageOptionsPopupPrefab;

        [SerializeField]
        private CreateNewChannelFormPopup _createNewChannelPopupPrefab;
        
        [SerializeField]
        private InviteChannelMembersPopup _inviteChannelMembersPopup;
        
        [FormerlySerializedAs("_invitationReceivedPopup")]
        [SerializeField]
        private InviteReceivedPopup inviteReceivedPopup;

        [SerializeField]
        private ErrorPopup _errorPopupPrefab;
    }
}