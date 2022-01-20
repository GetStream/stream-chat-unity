using StreamChat.Core;
using Plugins.GetStreamIO.Unity.Scripts.Popups;
using UnityEngine;

namespace Plugins.GetStreamIO.Unity.Scripts
{
    /// <summary>
    /// Asset to keep <see cref="IViewFactory"/> config
    /// </summary>
    [CreateAssetMenu(fileName = "ViewFactoryConfig", menuName = GetStreamChatClient.MenuPrefix + "View/Create view factory config asset", order = 1)]
    public class ViewFactoryConfig : ScriptableObject, IViewFactoryConfig
    {
        public MessageOptionsPopup MessageOptionsPopupPrefab => _messageOptionsPopupPrefab;

        [SerializeField]
        private MessageOptionsPopup _messageOptionsPopupPrefab;
    }
}