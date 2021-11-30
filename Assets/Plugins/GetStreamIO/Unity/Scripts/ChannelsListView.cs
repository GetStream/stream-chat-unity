using System.Collections.Generic;
using Plugins.GetStreamIO.Core.Models;
using UnityEngine;

namespace Plugins.GetStreamIO.Unity.Scripts
{
    /// <summary>
    /// Channels view - presents list of channels
    /// </summary>
    public class ChannelsListView : BaseView
    {
        protected override void OnInited()
        {
            base.OnInited();

            Client.ChannelsUpdated += OnChannelsUpdated;
        }

        protected override void OnDisposing()
        {
            Client.ChannelsUpdated -= OnChannelsUpdated;
            DestroyAll();

            base.OnDisposing();
        }

        [SerializeField]
        private Transform _listContainer;

        [SerializeField]
        private ChannelView _channelViewPrefab;

        private readonly List<ChannelView> _channels = new List<ChannelView>();

        private void OnChannelsUpdated()
        {
            DestroyAll();

            foreach (var c in Client.Channels)
            {
                var channelView = Instantiate(_channelViewPrefab, _listContainer);
                channelView.Init(c);
                channelView.Clicked += OnChannelClicked;
                _channels.Add(channelView);
            }
        }

        private void OnChannelClicked(Channel channel) => Client.OpenChannel(channel);

        private void DestroyAll()
        {
            foreach (var c in _channels)
            {
                c.Clicked -= OnChannelClicked;
                Destroy(c.gameObject);
            }

            _channels.Clear();
        }
    }
}