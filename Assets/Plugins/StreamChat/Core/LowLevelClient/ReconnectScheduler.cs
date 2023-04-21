using System;
using System.Text;
using StreamChat.Core.LowLevelClient.Models;
using StreamChat.Libs.Logs;
using StreamChat.Libs.Time;

namespace StreamChat.Core.LowLevelClient
{
    public class ReconnectScheduler
    {
        public event Action ReconnectionScheduled;
        public ReconnectStrategy ReconnectStrategy { get; private set; } = ReconnectStrategy.Exponential;
        public float ReconnectConstantInterval { get; private set; } = 1;
        public float ReconnectExponentialMinInterval { get; private set; } = 0.01f;
        public float ReconnectExponentialMaxInterval { get; private set; } = 64;
        public int ReconnectMaxInstantTrials { get; private set; } = 5; //StreamTodo: allow to control this by user
        public double? NextReconnectTime { get; private set; }

        public ReconnectScheduler(ITimeService timeService, IStreamChatLowLevelClient lowLevelClient, ILogs logs)
        {
            _logs = logs;
            _lowLevelClient = lowLevelClient;
            _timeService = timeService;
            
            _lowLevelClient.Connected += OnConnected;
            _lowLevelClient.Reconnecting += OnReconnecting;
            
            _lowLevelClient.ConnectionStateChanged += OnConnectionStateChanged;
        }

        public void SetReconnectStrategySettings(ReconnectStrategy reconnectStrategy, float? exponentialMinInterval,
            float? exponentialMaxInterval, float? constantInterval)
        {
            ReconnectStrategy = reconnectStrategy;

            //StreamTodo: move to Assets library
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

        public void TryScheduleNextReconnectTime()
        {
            if (NextReconnectTime.HasValue && NextReconnectTime.Value > _timeService.Time)
            {
                return;
            }

            double? GetNextReconnectTime()
            {
                if (ReconnectStrategy != ReconnectStrategy.Never && _reconnectAttempt <= ReconnectMaxInstantTrials)
                {
                    return _timeService.Time;
                }

                switch (ReconnectStrategy)
                {
                    case ReconnectStrategy.Exponential:

                        var baseInterval = Math.Pow(2, _reconnectAttempt);
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

            if (NextReconnectTime.HasValue)
            {
                ReconnectionScheduled?.Invoke();
            }
        }

        public void Stop()
        {
            NextReconnectTime = float.MaxValue;
        }
        
        //StreamTodo: connection info could be split to separate interface
        private readonly IStreamChatLowLevelClient _lowLevelClient;
        private readonly ILogs _logs;
        private readonly ITimeService _timeService;
        
        private int _reconnectAttempt;

        private void OnConnectionStateChanged(ConnectionState previous, ConnectionState current)
        {
            switch (current)
            {
                case ConnectionState.Disconnected:

                    TryScheduleNextReconnectTime();
                    
                    break;
                case ConnectionState.Connecting:
                    
                    //Can we tell between connect and reconnect? Probably yes??

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

        private void OnReconnecting()
        {
            _reconnectAttempt++;
        }

        private void OnConnected(OwnUser localuser)
        {
            _reconnectAttempt = 0;
        }
    }
}