namespace StreamChat.SampleProject.Popups
{
    /// <summary>
    /// Args for <see cref="BasePopup{T}"/>
    /// </summary>
    public interface IPopupArgs
    {
        bool HideOnPointerExit { get; }
    }
}