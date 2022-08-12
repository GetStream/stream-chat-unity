using System.Collections.Generic;
using StreamChat.Core;
using TMPro;
using UnityEngine;

namespace StreamChat.SampleProject.Views
{
    /// <summary>
    /// Console view
    /// </summary>
    public class ConsoleView : BaseView
    {
        protected override void OnInited()
        {
            base.OnInited();

            Client.EventReceived += OnEventReceived;
            Client.ConnectionStateChanged += OnConnectionStateChanged;
        }

        protected override void OnDisposing()
        {
            Client.EventReceived -= OnEventReceived;
            Client.ConnectionStateChanged -= OnConnectionStateChanged;

            base.OnDisposing();
        }

        private const int MaxRecords = 10;

        private readonly List<string> _records = new List<string>();

        [SerializeField]
        private TMP_Text _text;

        private void OnEventReceived(string obj)
        {
            _records.Insert(0, obj);

            if (_records.Count > MaxRecords)
            {
                _records.RemoveRange(0, _records.Count - MaxRecords);
            }

            PrintRecords();
        }

        private void OnConnectionStateChanged(ConnectionState prev, ConnectionState current)
        {
            if (current == ConnectionState.Disconnected)
            {
                _records.Clear();
                PrintRecords();
            }
        }

        private void PrintRecords() => _text.text = "Received events:" + "<br>" + string.Join("<br>", _records);
    }
}