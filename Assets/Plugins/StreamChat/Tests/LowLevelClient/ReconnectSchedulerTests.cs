using System;
using NSubstitute;
using NUnit.Framework;
using StreamChat.Core;
using StreamChat.Core.LowLevelClient;
using StreamChat.Libs.NetworkMonitors;
using StreamChat.Libs.Time;

#if STREAM_TESTS_ENABLED
namespace StreamChat.Tests.LowLevelClient
{
    internal class ReconnectSchedulerTests
    {
        [SetUp]
        public void Up()
        {
            _mockTimeService = Substitute.For<ITimeService>();
            _mockNetworkMonitor = Substitute.For<INetworkMonitor>();

            _clientMock = Substitute.For<IStreamChatLowLevelClient>();
            _timeScheduler = new ReconnectScheduler(_mockTimeService, _clientMock, _mockNetworkMonitor);
        }

        [TearDown]
        public void TearDown()
        {
            _timeScheduler?.Dispose();
        }

        [Test]
        public void when_scheduler_is_created_expect_next_reconnect_time_null()
        {
            Assert.IsNull(_timeScheduler.NextReconnectTime);
        }

        [Test]
        public void when_client_disconnects_expect_reconnect_scheduled()
        {
            _mockTimeService.Time.Returns(10);

            SetDisconnectedState();
            Assert.IsNotNull(_timeScheduler.NextReconnectTime);
        }

        [Test]
        public void when_client_disconnects_expect_precise_number_of_instant_reconnects()
        {
            _mockTimeService.Time.Returns(10);

            for (int i = 0; i < _timeScheduler.ReconnectMaxInstantTrials; i++)
            {
                SimulateFailedConnectAttempt();
                Assert.IsNotNull(_timeScheduler.NextReconnectTime);
                Assert.AreEqual(10, _timeScheduler.NextReconnectTime.Value);
            }

            SimulateFailedConnectAttempt();
            Assert.IsNotNull(_timeScheduler.NextReconnectTime);
            Assert.Greater(_timeScheduler.NextReconnectTime.Value, 10);
        }

        [Test]
        public void when_client_waits_to_reconnect_but_network_becomes_available_expect_instant_reconnect()
        {
            _mockTimeService.Time.Returns(10);

            // Simulate multiple reconnects so that NextReconnectTime becomes longer 
            #region SimulateMultipleAttempts

            for (int i = 0; i < _timeScheduler.ReconnectMaxInstantTrials; i++)
            {
                SimulateFailedConnectAttempt();
                Assert.IsNotNull(_timeScheduler.NextReconnectTime);
                Assert.AreEqual(10, _timeScheduler.NextReconnectTime.Value);
            }

            SimulateFailedConnectAttempt();
            SimulateFailedConnectAttempt();
            SimulateFailedConnectAttempt();
            Assert.IsNotNull(_timeScheduler.NextReconnectTime);
            Assert.Greater(_timeScheduler.NextReconnectTime.Value, 10);

            #endregion

            // Simulate network becoming available
            _mockNetworkMonitor.NetworkAvailabilityChanged += Raise.Event<NetworkAvailabilityChangedEventHandler>(true);
            
            // Expect instant reconnect due to network becoming available
            Assert.AreEqual(10, _timeScheduler.NextReconnectTime.Value);
        }

        private IStreamChatLowLevelClient _client;
        private IStreamChatLowLevelClient _clientMock;
        private ReconnectScheduler _timeScheduler;

        private ITimeService _mockTimeService;
        private INetworkMonitor _mockNetworkMonitor;

        private void SetDisconnectedState()
            => SimulateConnectionStateChanged(ConnectionState.Connected, ConnectionState.Disconnected);

        private void SimulateConnectionStateChanged(ConnectionState from, ConnectionState to)
        {
            _clientMock.ConnectionState.Returns(to);
            _clientMock.ConnectionStateChanged
                += Raise.Event<ConnectionStateChangeHandler>(from, to);
        }

        private void SimulateFailedConnectAttempt()
        {
            SimulateConnectionStateChanged(_clientMock.ConnectionState, ConnectionState.Connecting);
            _clientMock.Reconnecting += Raise.Event<Action>();
            SimulateConnectionStateChanged(_clientMock.ConnectionState, ConnectionState.Disconnected);
        }
    }
}
#endif