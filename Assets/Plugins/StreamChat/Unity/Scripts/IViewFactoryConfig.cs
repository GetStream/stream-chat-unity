using Plugins.GetStreamIO.Unity.Scripts.Popups;

namespace Plugins.GetStreamIO.Unity.Scripts
{
    /// <summary>
    /// Config for <see cref="IViewFactory"/>
    /// </summary>
    public interface IViewFactoryConfig
    {
        MessageOptionsPopup MessageOptionsPopupPrefab { get; }
    }
}