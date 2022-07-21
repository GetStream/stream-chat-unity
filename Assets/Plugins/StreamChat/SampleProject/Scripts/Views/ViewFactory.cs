﻿using System;
using System.Collections.Generic;
using System.Linq;
using SampleProject.Scripts.Popups;
using StreamChat.Core;
using StreamChat.Core.Models;
using StreamChat.Core.Requests;
using StreamChat.SampleProject.Configs;
using StreamChat.SampleProject.Popups;
using StreamChat.SampleProject.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace StreamChat.SampleProject.Views
{
    /// <summary>
    /// Default implementation of <see cref="IViewFactory"/>
    /// </summary>
    public class ViewFactory : IViewFactory
    {
        public RectTransform PopupsContainer => (RectTransform)_popupsContainer;

        public ViewFactory(IAppConfig config, Transform popupsContainer)
        {
            _appConfig = config ?? throw new ArgumentNullException(nameof(config));
            _config = config.ViewFactoryConfig ?? throw new ArgumentNullException(nameof(config.ViewFactoryConfig));
            _popupsContainer = popupsContainer ? popupsContainer : throw new ArgumentNullException(nameof(popupsContainer));
        }

        public void Init(IChatViewContext viewContext)
        {
            _viewContext = viewContext ?? throw new ArgumentNullException(nameof(viewContext));
        }

        public MessageOptionsPopup CreateMessageOptionsPopup(MessageView messageView, IChatState state)
        {
            var client = state.Client;
            var message = messageView.Message;

            var isSelfMessage = client.IsLocalUser(message.User);

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
                        TargetIds = new List<string> { user.Id }
                    };

                    //Todo: we could take OwnUser from response, save it in ViewContext and from OwnUser retrieve muted users
                    client.ModerationApi.MuteUserAsync(muteUserRequest).LogStreamExceptionIfFailed();
                }));
            }

            options.Add(new MenuOptionEntry("Delete",
                () => client.MessageApi.DeleteMessageAsync(message.Id, hard: false).LogStreamExceptionIfFailed()));

            var emojis = new List<EmojiOptionEntry>();

            AddReactionsEmojiOptions(emojis, message, client);

            var args = new MessageOptionsPopup.Args(hideOnPointerExit: true, hideOnButtonClicked: true, options, emojis);
            popup.Show(args);

            return popup;
        }

        public void CreateEmoji(Image prefab, Transform container, string key)
        {
            var sprite = _appConfig.Emojis.AllSprites.FirstOrDefault(_ => _.name == key);

            if (sprite == default)
            {
                Debug.LogError($"Failed to find emoji entry with key: `{key}`. Available keys: " + string.Join(", ", _appConfig.Emojis.AllSprites.Select(_ => _.name)));
                return;
            }

            var reaction = GameObject.Instantiate(prefab, container);
            reaction.sprite = sprite;
        }

        public TPopup CreateFullscreenPopup<TPopup>()
            where TPopup : BaseFullscreenPopup
        {
            var prefab = GetFullscreenPopupPrefab<TPopup>();
            var instance = GameObject.Instantiate(prefab, _popupsContainer);
            var popup = instance.GetComponent<TPopup>();

            //Todo: fix this dependency, some popups don't need view context like ErrorPopup
            if (_viewContext != null)
            {
                popup.Init(_viewContext);
            }

            return popup;
        }

        private readonly IAppConfig _appConfig;
        private readonly IViewFactoryConfig _config;
        private readonly Transform _popupsContainer;

        private IChatViewContext _viewContext;

        private void AddReactionsEmojiOptions(ICollection<EmojiOptionEntry> emojis, Message message,
            IStreamChatClient client)
        {
            foreach (var sprite in _appConfig.Emojis.ReactionSprites)
            {
                var key = sprite.name;

                var isAdded = message.ReactionCounts.ContainsKey(key);

                 emojis.Add(new EmojiOptionEntry(key, sprite, isAdded, () =>
                 {
                     if (isAdded)
                     {
                         client.MessageApi.DeleteReactionAsync(message.Id, key);
                     }
                     else
                     {
                         client.MessageApi.SendReactionAsync(message.Id, new SendReactionRequest
                         {
                             Reaction = new ReactionRequest
                             {
                                 Type = key,
                             }
                         });
                     }
                 }));
            }
        }

        private BaseFullscreenPopup GetFullscreenPopupPrefab<TPopup>()
            where TPopup : BaseFullscreenPopup
        {
            switch (typeof(TPopup))
            {
                case Type createNewChannel when createNewChannel == typeof(CreateNewChannelFormPopup):
                    return _config.CreateNewChannelFormPopupPrefab;
                case Type createNewChannel when createNewChannel == typeof(ErrorPopup):
                    return _config.ErrorPopupPrefab;
                default:
                    throw new ArgumentOutOfRangeException(nameof(TPopup), typeof(TPopup), null);
            }
        }
    }
}