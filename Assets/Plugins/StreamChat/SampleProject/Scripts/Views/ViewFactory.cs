using System;
using System.Collections.Generic;
using StreamChat.Core;
using StreamChat.Core.Exceptions;
using StreamChat.Core.Requests;
using StreamChat.Libs.Utils;
using StreamChat.SampleProject.Popups;
using UnityEngine;

namespace StreamChat.SampleProject.Views
{
    /// <summary>
    /// Default implementation of <see cref="IViewFactory"/>
    /// </summary>
    public class ViewFactory : IViewFactory
    {
        public ViewFactory(IStreamChatClient client, IViewFactoryConfig config, Transform popupsContainer)
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

            if (isSelfMessage)
            {
                options.Add(new MenuOptionEntry("Edit", () => _viewContext.State.EditMessage(message)));
            }
            else
            {
                options.Add(new MenuOptionEntry("Flag", () => throw new NotImplementedException("Flag")));

                //Todo: muted ? => show unmute instead
                var user = message.User;
                options.Add(new MenuOptionEntry("Mute", () =>
                {
                    var muteUserRequest = new MuteUserRequest
                    {
                        TargetIds = new List<string>().AddFluent(user.Id)
                    };

                    //Todo: we could take OwnUser from response, save it in ViewContext and from OwnUser retrieve muted users
                    _client.ModerationApi.MuteUserAsync(muteUserRequest).LogStreamExceptionIfFailed();
                }));
            }

            options.Add(new MenuOptionEntry("Delete", () => _client.MessageApi.DeleteMessageAsync(message.Id, hard: false).LogStreamExceptionIfFailed()));

            var args = new MessageOptionsPopup.Args(hideOnPointerExit: true, hideOnButtonClicked: true, options);
            popup.Show(args);

            return popup;
        }

        private readonly IStreamChatClient _client;
        private readonly IViewFactoryConfig _config;
        private readonly Transform _popupsContainer;

        private IChatViewContext _viewContext;
    }
}