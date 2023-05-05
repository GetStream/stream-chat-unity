namespace StreamChat.Libs.NetworkMonitors
{
    /// <summary>
    /// Provides information on network availability and notifies whenever this state changes
    /// </summary>
    public interface INetworkMonitor
    {
        event NetworkAvailabilityChangedEventHandler NetworkAvailabilityChanged;
        
        bool IsNetworkAvailable { get; }

        void Update();
    }
}