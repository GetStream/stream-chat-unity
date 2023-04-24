using System;
using StreamChat.Core.LowLevelClient.Models;
using StreamChat.Libs.NetworkMonitors;
using StreamChat.Libs.Time;

namespace StreamChat.Core.LowLevelClient
{
    /// <summary>
    /// Schedules next reconnection time based on the past attempts and network availability
    /// </summary>
    internal class ReconnectScheduler : IDisposable
    {
        public event Action ReconnectionScheduled;
        public ReconnectStrategy ReconnectStrategy { get; private set; } = ReconnectStrategy.Exponential;
        public float ReconnectConstantInterval { get; private set; } = 1;
        public float ReconnectExponentialMinInterval { get; private set; } = 0.01f;
        public float ReconnectExponentialMaxInterval { get; private set; } = 64;
        public int ReconnectMaxInstantTrials { get; private set; } = 5; //StreamTodo: allow to control this by user

        public double? NextReconnectTime
        {
            get => _nextReconnectTime;
            private set
            {
                var isEqual = _nextReconnectTime.HasValue && value.HasValue &&
                              Math.Abs(_nextReconnectTime.Value - value.Value) < float.Epsilon;
                if (isEqual)
                {
                    return;
                }

                _nextReconnectTime = value;

                if (_nextReconnectTime.HasValue)
                {
                    ReconnectionScheduled?.Invoke();
                }
            }
        }

        public ReconnectScheduler(ITimeService timeService, IStreamChatLowLevelClient lowLevelClient,
            INetworkMonitor networkMonitor)
        {
            _client = lowLevelClient ?? throw new ArgumentNullException(nameof(lowLevelClient));
            _timeService = timeService ?? throw new ArgumentNullException(nameof(timeService));
            _networkMonitor = networkMonitor ?? throw new ArgumentNullException(nameof(networkMonitor));

            _networkMonitor.NetworkAvailabilityChanged += OnNetworkAvailabilityChanged;

            _client.Connected += OnConnected;
            _client.Reconnecting += OnReconnecting;
            _client.ConnectionStateChanged += OnConnectionStateChanged;
        }
        
        public void Dispose()
        {
            if (_client != null)
            {
                _client.Connected -= OnConnected;
                _client.Reconnecting -= OnReconnecting;
                _client.ConnectionStateChanged -= OnConnectionStateChanged;
            }
        }

        public void SetReconnectStrategySettings(ReconnectStrategy reconnectStrategy, float? exponentialMinInterval,
            float? exponentialMaxInterval, float? constantInterval)
        {
            ReconnectStrategy = reconnectStrategy;

            void ThrowIfLessOrEqualToZero(float value, string name)
            {
                if (value <= 0)
                {
                    throw new ArgumentException($"{name} needs to be greater than zero, given: " + value);
                }
            }

            if (exponentialMinInterval.HasValue)
            {
                ThrowIfLessOrEqualToZero(exponentialMinInterval.Value, nameof(exponentialMinInterval));
                ReconnectExponentialMinInterval = exponentialMinInterval.Value;
            }

            if (exponentialMaxInterval.HasValue)
            {
                ThrowIfLessOrEqualToZero(exponentialMaxInterval.Value, nameof(exponentialMaxInterval));
                ReconnectExponentialMaxInterval = exponentialMaxInterval.Value;
            }

            if (constantInterval.HasValue)
            {
                ThrowIfLessOrEqualToZero(constantInterval.Value, nameof(constantInterval));
                ReconnectConstantInterval = constantInterval.Value;
            }
        }

        public void Stop()
        {
            NextReconnectTime = float.MaxValue;
            _isStopped = true;
        }

        //StreamTodo: connection info could be split to separate interface
        private readonly IStreamChatLowLevelClient _client;
        private readonly ITimeService _timeService;
        private readonly INetworkMonitor _networkMonitor;

        private int _reconnectAttempts;
        private bool _isStopped;
        private double? _nextReconnectTime;

        private void TryScheduleNextReconnectTime()
        {
            if (NextReconnectTime.HasValue && NextReconnectTime.Value > _timeService.Time)
            {
                return;
            }

            if (_isStopped || ReconnectStrategy == ReconnectStrategy.Never)
            {
                return;
            }

            double? GetNextReconnectTime()
            {
                if (_reconnectAttempts <= ReconnectMaxInstantTrials)
                {
                    return _timeService.Time;
                }

                switch (ReconnectStrategy)
                {
                    case ReconnectStrategy.Exponential:

                        var baseInterval = Math.Pow(2, _reconnectAttempts);
                        var interval = Math.Min(Math.Max(ReconnectExponentialMinInterval, baseInterval),
                            ReconnectExponentialMaxInterval);
                        return _timeService.Time + interval;
                    case ReconnectStrategy.Constant:
                        return _timeService.Time + ReconnectConstantInterval;
                    case ReconnectStrategy.Never:
                        return null;
                    default:
                        throw new ArgumentOutOfRangeException(
                            $"Unhandled {nameof(ReconnectStrategy)}: {ReconnectStrategy}");
                }
            }

            NextReconnectTime = GetNextReconnectTime();
        }

        private void OnConnectionStateChanged(ConnectionState previous, ConnectionState current)
        {
            switch (current)
            {
                case ConnectionState.Disconnected:

                    TryScheduleNextReconnectTime();

                    break;
                case ConnectionState.Connecting:

                    NextReconnectTime = default;

                    break;
                case ConnectionState.WaitToReconnect:
                    break;
                case ConnectionState.Connected:
                    break;
                case ConnectionState.Closing:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(current), current, null);
            }
        }

        private void OnNetworkAvailabilityChanged(bool isNetworkAvailable)
        {
            if (!isNetworkAvailable)
            {
                return;
            }

            if (_client.ConnectionState == ConnectionState.Connected ||
                _client.ConnectionState == ConnectionState.Connecting)
            {
                return;
            }

            if (_isStopped)
            {
                return;
            }

            NextReconnectTime = _timeService.Time;
        }

        private void OnReconnecting()
        {
            _reconnectAttempts++;
        }

        private void OnConnected(OwnUser localUser)
        {
            _reconnectAttempts = 0;
            NextReconnectTime = default;
        }
    }
}