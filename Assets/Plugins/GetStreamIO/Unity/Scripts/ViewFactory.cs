using System;
using Plugins.GetStreamIO.Core.Models;
using Plugins.GetStreamIO.Unity.Scripts.Popups;
using UnityEngine;

namespace Plugins.GetStreamIO.Unity.Scripts
{
    /// <summary>
    /// Default implementation of <see cref="IViewFactory"/>
    /// </summary>
    public class ViewFactory : IViewFactory
    {
        public ViewFactory(IChatViewContext context, IViewFactoryConfig config, Transform popupsContainer)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _popupsContainer = popupsContainer ? popupsContainer : throw new ArgumentNullException(nameof(popupsContainer));
        }

        public MessageOptionsPopup CreateMessageOptionsPopup(Message message)
        {
            var isSelfMessage = _context.Client.IsLocalUser(message.User);

            var popup = GameObject.Instantiate(_config.MessageOptionsPopupPrefab);

            //reply
            //pin

            if (!isSelfMessage)
            {
                //flag
                // mute/unmute
            }

            //edit
            //delete

            return null;
        }

        private readonly IChatViewContext _context;
        private readonly IViewFactoryConfig _config;
        private readonly Transform _popupsContainer;
    }
}