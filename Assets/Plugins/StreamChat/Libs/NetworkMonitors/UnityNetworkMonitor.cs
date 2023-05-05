using UnityEngine;

namespace StreamChat.Libs.NetworkMonitors
{
    public delegate void NetworkAvailabilityChangedEventHandler(bool isNetworkAvailable);

    public class UnityNetworkMonitor : INetworkMonitor
    {
        public event NetworkAvailabilityChangedEventHandler NetworkAvailabilityChanged;

        public bool IsNetworkAvailable { get; private set; }

        public void Update()
        {
            var currentState = Application.internetReachability;
            if (currentState == _lastState)
            {
                return;
            }

            var prevLastState = _lastState;

            var isReachable = IsReachable(currentState);
            IsNetworkAvailable = isReachable;

            _lastState = currentState;

            if (!prevLastState.HasValue)
            {
                return;
            }

            var wasReachable = IsReachable(_lastState.Value);

            if (wasReachable && !isReachable)
            {
                NetworkAvailabilityChanged?.Invoke(false);
            }

            if (!wasReachable && isReachable)
            {
                NetworkAvailabilityChanged?.Invoke(true);
            }
        }

        private NetworkReachability? _lastState;

        private static bool IsReachable(NetworkReachability reachability)
            => reachability == NetworkReachability.ReachableViaCarrierDataNetwork ||
               reachability == NetworkReachability.ReachableViaLocalAreaNetwork;
    }
}