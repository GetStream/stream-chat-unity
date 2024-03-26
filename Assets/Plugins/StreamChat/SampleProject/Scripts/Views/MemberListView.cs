using System.Collections.Generic;
using StreamChat.Core.StatefulModels;
using StreamChat.SampleProject.Popups;
using StreamChat.SampleProject.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace StreamChat.SampleProject.Views
{
    /// <summary>
    /// Shows list of channel members
    /// </summary>
    public class MemberListView : BaseView
    {
        protected override void OnInited()
        {
            base.OnInited();

            State.ActiveChanelChanged += OnActiveChannelChanged;
            _inviteButton.onClick.AddListener(OnInviteButtonClicked);

            RebuildMembers();
        }

        protected override void OnDisposing()
        {
            State.ActiveChanelChanged -= OnActiveChannelChanged;

            ClearAll();

            base.OnDisposing();
        }

        private readonly List<MemberView> _members = new List<MemberView>();
        private readonly UnityImageWebLoader _imageLoader = new UnityImageWebLoader();

        [SerializeField]
        private MemberView _memberViewPrefab;

        [SerializeField]
        private Transform _membersContainer;
        
        [SerializeField]
        private Button _inviteButton;

        private void ClearAll()
        {
            foreach (var m in _members)
            {
                Destroy(m.gameObject);
            }

            _members.Clear();
        }
        
        private void OnInviteButtonClicked() => State.ShowPopup<InviteChannelMembersPopup>();

        private void OnActiveChannelChanged(IStreamChannel streamChannel) => RebuildMembers();

        private void RebuildMembers()
        {
            ClearAll();

            if (State.ActiveChannel == null)
            {
                return;
            }
            
            foreach (var m in State.ActiveChannel.Members)
            {
                var memberEntryView = Instantiate(_memberViewPrefab, _membersContainer);
                memberEntryView.UpdateData(m, _imageLoader);
                _members.Add(memberEntryView);
            }
        }
    }
}