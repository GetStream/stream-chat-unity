using System;
using System.Collections.Generic;
using Plugins.GetStreamIO.Core;
using Plugins.GetStreamIO.Unity.Scripts.Popups;
using UnityEngine;

namespace Plugins.GetStreamIO.Unity.Scripts
{
    /// <summary>
    /// Default implementation of <see cref="IViewFactory"/>
    /// </summary>
    public class ViewFactory : IViewFactory
    {
        public ViewFactory(IGetStreamChatClient client, IViewFactoryConfig config, Transform popupsContainer)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _popupsContainer =
                popupsContainer ? popupsContainer : throw new ArgumentNullException(nameof(popupsContainer));
        }

        public void Init(IChatViewContext viewContext)
        {
            _viewContext = viewContext ?? throw new ArgumentNullException(nameof(viewContext));
        }

        public MessageOptionsPopup CreateMessageOptionsPopup(MessageView messageView)
        {
            var message = messageView.Message;
            var isSelfMessage = _client.IsLocalUser(message.User);

            var popup = GameObject.Instantiate(_config.MessageOptionsPopupPrefab, _popupsContainer);
            popup.Init(_viewContext);

            var options = new List<MenuOptionEntry>
            {
                new MenuOptionEntry("Reply", () => throw new NotImplementedException("Reply")),
                new MenuOptionEntry("Pin", () => throw new NotImplementedException("Pin")),

            };

            if (!isSelfMessage)
            {
                options.Add(new MenuOptionEntry("Flag", () => throw new NotImplementedException("Flag")));

                //Todo: muted ? => show unmute instead
                options.Add(new MenuOptionEntry("Mute", () => throw new NotImplementedException("Mute")));
            }

            options.Add(new MenuOptionEntry("Edit", () => throw new NotImplementedException("Edit")));
            options.Add(new MenuOptionEntry("Delete", () => _client.DeleteMessage(message, hard: false)));

            var args = new MessageOptionsPopup.Args(hideOnPointerExit: true, hideOnButtonClicked: true, options);
            popup.Show(args);

            return popup;
        }

        private readonly IGetStreamChatClient _client;
        private readonly IViewFactoryConfig _config;
        private readonly Transform _popupsContainer;
        private IChatViewContext _viewContext;
    }
}