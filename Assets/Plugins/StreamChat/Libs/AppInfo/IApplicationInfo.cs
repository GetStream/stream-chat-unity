namespace StreamChat.Libs.AppInfo
{
    /// <summary>
    /// Provides basic information on the running platform
    /// </summary>
    public interface IApplicationInfo
    {
        string Engine { get; }
        string EngineVersion { get; }
        string Platform { get; }
        string OperatingSystem { get; }
        int MemorySize { get; }
        int GraphicsMemorySize { get; }
        string ScreenSize { get; }
    }
}