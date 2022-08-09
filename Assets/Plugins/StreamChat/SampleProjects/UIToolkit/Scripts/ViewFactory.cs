using System;
using StreamChat.Core.Models;
using StreamChat.SampleProjects.UIToolkit.Config;
using StreamChat.SampleProjects.UIToolkit.Views;
using UnityEngine.UIElements;

namespace StreamChat.SampleProjects.UIToolkit
{
    public class ViewFactory : IViewFactory
    {
        public ViewFactory(VisualElement rootVisualElement, IViewConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _rootVisualElement = rootVisualElement ?? throw new ArgumentNullException(nameof(rootVisualElement));
        }

        public RootView CreateRootView(IChatState chatState)
            => new RootView(chatState, _rootVisualElement, viewFactory: this, _config);

        public ChannelItemView CreateChannelItemView(ChannelState channelState)
        {
            var vs = _config.ChannelItemViewTemplate.Instantiate();

            var item = new ChannelItemView(vs, viewFactory: this, _config);
            item.SetData(channelState);

            return item;
        }

        public MessageItemView CreateMessageItemView(Message message)
        {
            var vs = _config.MessageItemViewTemplate.Instantiate();

            var item = new MessageItemView(vs, viewFactory: this, _config);
            item.SetData(message);

            return item;
        }

        public MessageInputFormView CreateMessageInputFormView(IChatWriter chatWriter)
        {
            var vs = _config.MessageInputFormViewTemplate.Instantiate();

            var item = new MessageInputFormView(chatWriter, vs, viewFactory: this, _config);

            return item;
        }

        private readonly VisualElement _rootVisualElement;
        private readonly IViewConfig _config;
    }
}