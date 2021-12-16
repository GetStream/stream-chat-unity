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

            State.ChannelsUpdated += OnChannelsUpdated;
        }

        protected override void OnDisposing()
        {
            State.ChannelsUpdated -= OnChannelsUpdated;
            ClearAll();

            base.OnDisposing();
        }

        [SerializeField]
        private Transform _listContainer;

        [SerializeField]
        private ChannelView _channelViewPrefab;

        private readonly List<ChannelView> _channels = new List<ChannelView>();

        private void OnChannelsUpdated()
        {
            ClearAll();

            foreach (var c in State.Channels)
            {
                //Todo: move to ViewFactory
                var channelView = Instantiate(_channelViewPrefab, _listContainer);
                channelView.Init(c, ViewContext);
                channelView.Clicked += OnChannelClicked;
                _channels.Add(channelView);
            }
        }

        private void OnChannelClicked(Channel channel) => State.OpenChannel(channel);

        private void ClearAll()
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