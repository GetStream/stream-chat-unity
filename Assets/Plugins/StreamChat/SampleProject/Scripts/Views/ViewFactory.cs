using System;
using System.Collections.Generic;
using System.Linq;
using SampleProject.Scripts.Popups;
using StreamChat.Core;
using StreamChat.Core.Exceptions;
using StreamChat.Core.Models;
using StreamChat.Core.Requests;
using StreamChat.SampleProject.Popups;
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
                        TargetIds = new List<string> { user.Id }
                    };

                    //Todo: we could take OwnUser from response, save it in ViewContext and from OwnUser retrieve muted users
                    _client.ModerationApi.MuteUserAsync(muteUserRequest).LogStreamExceptionIfFailed();
                }));
            }

            options.Add(new MenuOptionEntry("Delete",
                () => _client.MessageApi.DeleteMessageAsync(message.Id, hard: false).LogStreamExceptionIfFailed()));

            var emojis = new List<EmojiOptionEntry>();

            AddEmojiOptions(emojis, message);

            var args = new MessageOptionsPopup.Args(hideOnPointerExit: true, hideOnButtonClicked: true, options, emojis);
            popup.Show(args);

            return popup;
        }

        public void CreateReactionEmoji(Image prefab, Transform container, string key)
        {
            var emojiEntry = _config.EmojiConfig.Emojis.FirstOrDefault(_ => _.Key == key);

            if (emojiEntry == default)
            {
                Debug.LogError($"Failed to find emoji entry with key: `{key}`. Available keys: " + string.Join(", ", _config.EmojiConfig.Emojis.Select(_ => _.Key)));
                return;
            }

            var reaction = GameObject.Instantiate(prefab, container);
            reaction.sprite = emojiEntry.Sprite;
        }

        private readonly IStreamChatClient _client;
        private readonly IViewFactoryConfig _config;
        private readonly Transform _popupsContainer;

        private IChatViewContext _viewContext;

        private void AddEmojiOptions(ICollection<EmojiOptionEntry> emojis, Message message)
        {
            foreach (var (key, sprite) in _config.EmojiConfig.Emojis)
            {
                var isAdded = message.ReactionCounts.ContainsKey(key);

                emojis.Add(new EmojiOptionEntry(key, sprite, isAdded, () =>
                {
                    if (isAdded)
                    {
                        _client.MessageApi.DeleteReactionAsync(message.Id, key);
                    }
                    else
                    {
                        _client.MessageApi.SendReactionAsync(message.Id, new SendReactionRequest
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
    }
}