using System;
using System.Collections.Generic;
using System.Linq;
using StreamChat.Core.Helpers;
using StreamChat.Core.StatefulModels;
using StreamChat.SampleProject.Configs;
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

        public ViewFactory(IAppConfig config, Transform popupsContainer)
        {
            _appConfig = config ?? throw new ArgumentNullException(nameof(config));
            _config = config.ViewFactoryConfig ?? throw new ArgumentNullException(nameof(config.ViewFactoryConfig));
            _popupsContainer = popupsContainer ? popupsContainer : throw new ArgumentNullException(nameof(popupsContainer));

            _appConfig.Emojis.LoadEmojisSprites();
            
            _popupPrefabs.Add(typeof(CreateNewChannelFormPopup), _config.CreateNewChannelFormPopupPrefab);
            _popupPrefabs.Add(typeof(InviteChannelMembersPopup), _config.InviteChannelMembersPopupPrefab);
            _popupPrefabs.Add(typeof(InviteReceivedPopup), _config.InviteReceivedPopup);
            _popupPrefabs.Add(typeof(ErrorPopup), _config.ErrorPopupPrefab);
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
                //new MenuOptionEntry("Reply", () => throw new NotImplementedException("Reply")),
                new MenuOptionEntry("Pin", () => message.PinAsync().LogExceptionsOnFailed()),
            };

            if (isSelfMessage)
            {
                options.Add(new MenuOptionEntry("Edit", () => _viewContext.State.EditMessage(message)));
            }
            else
            {
                options.Add(new MenuOptionEntry("Flag message", () => message.FlagAsync().LogExceptionsOnFailed()));
                options.Add(new MenuOptionEntry("Flag user", () => message.User.FlagAsync().LogExceptionsOnFailed()));

                //StreamTodo: muted ? => show unmute instead
                var user = message.User;
                options.Add(new MenuOptionEntry("Mute User", () => user.MuteAsync().LogExceptionsOnFailed()));
            }

            options.Add(new MenuOptionEntry("Mark as read", () => message.MarkMessageAsLastReadAsync()));

            options.Add(new MenuOptionEntry("Delete",
                () => message.SoftDeleteAsync().LogExceptionsOnFailed()));

            var emojis = new List<EmojiOptionEntry>();

            AddReactionsEmojiOptions(emojis, message);

            var args = new MessageOptionsPopup.Args(hideOnPointerExit: true, hideOnButtonClicked: true, options, emojis);
            popup.Show(args);

            return popup;
        }

        public void CreateEmoji(Image prefab, Transform container, string key)
        {
            var sprite = _appConfig.Emojis.AllSprites.FirstOrDefault(_ => _.name == key);
            if (sprite == default)
            {
                //Debug.LogError($"Failed to find emoji entry with key: `{key}`. Available keys: " + string.Join(", ", _appConfig.Emojis.AllSprites.Select(_ => _.name)));
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

            //StreamTodo: fix this dependency, some popups don't need view context like ErrorPopup
            if (_viewContext != null)
            {
                popup.Init(_viewContext);
            }

            return popup;
        }

        private readonly IAppConfig _appConfig;
        private readonly IViewFactoryConfig _config;
        private readonly Transform _popupsContainer;

        private readonly Dictionary<Type, BaseFullscreenPopup> _popupPrefabs
            = new Dictionary<Type, BaseFullscreenPopup>();

        private IChatViewContext _viewContext;

        private void AddReactionsEmojiOptions(ICollection<EmojiOptionEntry> emojis, IStreamMessage message)
        {
            foreach (var sprite in _appConfig.Emojis.ReactionSprites)
            {
                var key = sprite.name;

                var isAdded = message.ReactionCounts.ContainsKey(key);

                 emojis.Add(new EmojiOptionEntry(key, sprite, isAdded, () =>
                 {
                     if (isAdded)
                     {
                         message.DeleteReactionAsync(key);
                     }
                     else
                     {
                         message.SendReactionAsync(key);
                     }
                 }));
            }
        }

        private BaseFullscreenPopup GetFullscreenPopupPrefab<TPopup>()
            where TPopup : BaseFullscreenPopup
        {
            var key = typeof(TPopup);
            if (!_popupPrefabs.ContainsKey(key))
            {
                throw new ArgumentOutOfRangeException(nameof(TPopup), typeof(TPopup), null);
            }

            return (TPopup)_popupPrefabs[key];
        }
    }
}