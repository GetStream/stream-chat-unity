using System;
using System.Collections.Generic;
using System.Text;
using StreamChat.Core;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace StreamChat.SampleProject.Views
{
    /// <summary>
    /// Console view
    /// </summary>
    public class ConsoleView : BaseView
    {
        protected void Awake()
        {
            _prevNetworkReachability = Application.internetReachability;
        }

        protected override void OnInited()
        {
            base.OnInited();

            //StreamTodo: figure out if want to somehow get this debug info
            //Client.EventReceived += OnEventReceived;
            Client.ConnectionStateChanged += OnConnectionStateChanged;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (!_connectionLogEnabled)
            {
                return;
            }

            if (Client.NextReconnectTime != _prevNextReconnectTime)
            {
                Debug.LogWarning(
                    $"{nameof(Client.NextReconnectTime)} changed from: `{_prevNextReconnectTime:0:00}` to: `{Client.NextReconnectTime:0:00}`");
                _prevNextReconnectTime = Client.NextReconnectTime;
                UpdateConnectionLog();
            }

            if (_prevNetworkReachability != Application.internetReachability)
            {
                var reachabilityName = Enum.GetName(typeof(NetworkReachability), Application.internetReachability);
                Debug.LogWarning(
                    $"{nameof(NetworkReachability)} changed from `{_prevNetworkReachability}` to `{reachabilityName}`");
                _prevNetworkReachability = Application.internetReachability;
                UpdateConnectionLog();
            }
        }

        protected override void OnDisposing()
        {
            //StreamTodo:
            //Client.EventReceived -= OnEventReceived;
            Client.ConnectionStateChanged -= OnConnectionStateChanged;

            base.OnDisposing();
        }

        private const int MaxRecords = 10;

        private readonly List<string> _records = new List<string>();
        private readonly StringBuilder _sb = new StringBuilder();

        [FormerlySerializedAs("_text")]
        [SerializeField]
        private TMP_Text _eventsLogText;

        [SerializeField]
        private TMP_Text _connectionLogText;

        private double? _prevNextReconnectTime;
        private NetworkReachability _prevNetworkReachability;
        private bool _connectionLogEnabled;

        private void OnEventReceived(string obj)
        {
            _records.Insert(0, obj);

            if (_records.Count > MaxRecords)
            {
                _records.RemoveRange(0, _records.Count - MaxRecords);
            }

            UpdateEventsLog();
        }

        private void OnConnectionStateChanged(ConnectionState prev, ConnectionState current)
        {
            if (current == ConnectionState.Disconnected)
            {
                _records.Clear();
                UpdateEventsLog();
            }

            Debug.LogWarning($"Connection changed from `{prev}` to `{current}`");

            UpdateConnectionLog();
        }

        private void UpdateEventsLog()
            => _eventsLogText.text = "Received events:" + "<br>" + string.Join("<br>", _records);

        private void UpdateConnectionLog()
        {
            if (!_connectionLogEnabled)
            {
                return;
            }

            _sb.AppendLine("Connection:");

            _sb.Append("State: ");
            _sb.Append(Client.ConnectionState);
            _sb.Append(Environment.NewLine);

            _sb.Append("Next Reconnect: ");

            if (Client.NextReconnectTime.HasValue)
            {
                var timeLeft = Client.NextReconnectTime.Value - Time.time;
                _sb.Append(Client.NextReconnectTime.Value.ToString("0.0"));
                _sb.Append(" (in: ");
                _sb.Append(timeLeft.ToString("0.0"));
                _sb.Append("s)");
            }
            else
            {
                _sb.Append("None");
            }

            _sb.Append(Environment.NewLine);

            _sb.Append(nameof(NetworkReachability));
            _sb.Append(": ");
            _sb.Append(Application.internetReachability);
            _sb.Append(Environment.NewLine);

            _connectionLogText.text = _sb.ToString();
            _sb.Clear();
        }
    }
}