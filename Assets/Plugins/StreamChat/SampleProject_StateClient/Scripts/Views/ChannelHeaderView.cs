using System.Collections.Generic;
using System.Text;
using StreamChat.Core.Events;
using TMPro;
using UnityEngine;

namespace StreamChat.SampleProject_StateClient.Views
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

            Client.TypingStarted += OnTypingStarted;
            Client.TypingStopped += OnTypingStopped;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (Time.time - _lastUpdateTime > UpdateInterval)
            {
                UpdateTypingUsersPreview();
            }
        }

        protected override void OnDisposing()
        {
            Client.TypingStarted -= OnTypingStarted;
            Client.TypingStopped -= OnTypingStopped;

            base.OnDisposing();
        }

        private void OnTypingStopped(EventTypingStop obj)
        {
            if (!_channelCidToTypingUserIds.ContainsKey(obj.Cid))
            {
                return;
            }

            _channelCidToTypingUserIds[obj.Cid].Remove(obj.User.Id);

            if (!_channelCidToTypingUserIds.TryGetValue(obj.Cid, out var users))
            {
                _channelCidToTypingUserIds[obj.Cid] = new HashSet<string>();
            }
        }

        private void OnTypingStarted(EventTypingStart obj)
        {
            if (!_channelCidToTypingUserIds.TryGetValue(obj.Cid, out var userIds))
            {
                _channelCidToTypingUserIds[obj.Cid] = userIds = new HashSet<string>();
            }

            userIds.Add(obj.User.Id);
        }

        private const float UpdateInterval = 0.3f;

        private readonly StringBuilder _sb = new StringBuilder();
        private readonly Dictionary<string, HashSet<string>> _channelCidToTypingUserIds =
            new Dictionary<string, HashSet<string>>();

        [SerializeField]
        private TMP_Text _typingNotificationText;

        private int _step;
        private float _lastUpdateTime;

        private void UpdateTypingUsersPreview()
        {
            if (State.ActiveChannel == null ||
                !_channelCidToTypingUserIds.TryGetValue(State.ActiveChannel.Channel.Cid, out var typingUsers))
            {
                return;
            }

            var index = 0;
            var isSingle = typingUsers.Count == 1;
            foreach (var userId in typingUsers)
            {
                var isLast = index == typingUsers.Count - 1;
                var isNextLast = !isLast && index == typingUsers.Count - 2;
                _sb.Append(userId);

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