using System.Text;
using StreamChat.Core.StatefulModels;
using TMPro;
using UnityEngine;

namespace StreamChat.SampleProject.Views
{
    /// <summary>
    /// Active channel header
    /// </summary>
    public class ChannelHeaderView : BaseView
    {
        protected void Awake()
        {
            _typingNotificationText.text = string.Empty;
        }

        protected override void OnInited()
        {
            base.OnInited();
            
            State.ActiveChanelChanged += OnActiveChanelChanged;
        }

        private void OnActiveChanelChanged(IStreamChannel activeChannel)
        {
            if (_activeChannel != null)
            {
                _activeChannel.UserStartedTyping -= OnTypingStarted;
                _activeChannel.UserStoppedTyping -= OnTypingStopped;
            }
            
            activeChannel.UserStartedTyping += OnTypingStarted;
            activeChannel.UserStoppedTyping += OnTypingStopped;

            _activeChannel = activeChannel;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            //for trailing dots animation
            if (Time.time - _lastUpdateTime > UpdateInterval)
            {
                UpdateTypingUsersPreview();
            }
        }

        protected override void OnDisposing()
        {
            State.ActiveChanelChanged -= OnActiveChanelChanged;
            
            if (_activeChannel != null)
            {
                _activeChannel.UserStartedTyping -= OnTypingStarted;
                _activeChannel.UserStoppedTyping -= OnTypingStopped;
            }

            base.OnDisposing();
        }

        private void OnTypingStopped(IStreamChannel channel, IStreamUser user) => UpdateTypingUsersPreview();

        private void OnTypingStarted(IStreamChannel channel, IStreamUser user) => UpdateTypingUsersPreview();

        private const float UpdateInterval = 0.3f;

        private readonly StringBuilder _sb = new StringBuilder();

        [SerializeField]
        private TMP_Text _typingNotificationText;

        private int _step;
        private float _lastUpdateTime;
        private IStreamChannel _activeChannel;

        private void UpdateTypingUsersPreview()
        {
            if (_activeChannel == null)
            {
                _typingNotificationText.text = string.Empty;
                return;
            }

            var typingUsers = _activeChannel.TypingUsers;
            var index = 0;
            var isSingle = typingUsers.Count == 1;
            foreach (var user in typingUsers)
            {
                var isLast = index == typingUsers.Count - 1;
                var isNextLast = !isLast && index == typingUsers.Count - 2;
                _sb.Append("<color=#F9AC17><b>");
                _sb.Append(user.Name);
                _sb.Append("</b></color>");

                if (!isLast && !isNextLast)
                {
                    _sb.Append(", ");
                }
                if (isNextLast)
                {
                    _sb.Append(" and ");
                }

                if (isLast)
                {
                    _sb.Append(isSingle ? " is typing" : " are typing");

                    var dotsCount = _step % 4;
                    _sb.Append(new string('.', dotsCount));
                }

                index++;
            }

            _lastUpdateTime = Time.time;
            _step++;

            _typingNotificationText.text = _sb.ToString();
            _sb.Clear();
        }
    }
}