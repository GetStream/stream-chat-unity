#if STREAM_TESTS_ENABLED
using System;
using System.Collections;
using System.Threading.Tasks;
using NUnit.Framework;
using StreamChat.Core;
using StreamChat.Core.Configs;
using StreamChat.Core.LowLevelClient;
using StreamChat.Core.LowLevelClient.Models;
using UnityEngine.TestTools;

namespace StreamChat.Tests.LowLevelClient
{
    internal class LowLevelClientConnectionTests
    {
        [SetUp]
        public void Up()
        {
            _lowLevelClient = StreamChatLowLevelClient.CreateDefaultClient(StreamTestClients.Instance.LowLevelClientCredentials, new StreamClientConfig
            {
                LogLevel = StreamLogLevel.Debug
            });
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return TearDownAsync().RunAsIEnumerator();
        }
        
        private async Task TearDownAsync()
        {
            if (_lowLevelClient.ConnectionState == ConnectionState.Connected)
            {
                await _lowLevelClient.DisconnectAsync();
            }
            
            _lowLevelClient.Dispose();
            _lowLevelClient = null;
        }

        [UnityTest]
        public IEnumerator When_client_connects_and_disconnects_multiple_times_expect_client_to_have_a_correct_connection_state()
        {
            yield return When_client_connects_and_disconnects_multiple_times_expect_client_to_have_a_correct_connection_state_Async()
                .RunAsIEnumerator(_lowLevelClient);
        }

        private async Task When_client_connects_and_disconnects_multiple_times_expect_client_to_have_a_correct_connection_state_Async()
        {
            var credentials = StreamTestClients.Instance.LowLevelClientCredentials;

            async Task ConnectAsync()
            {
                var timeout = Task.Delay(TimeSpan.FromSeconds(10));
                var taskCompletion = new TaskCompletionSource<bool>();

                void OnUserConnected(OwnUser ownUser)
                {
                    _lowLevelClient.Connected -= OnUserConnected;
                    taskCompletion.SetResult(true);
                }

                _lowLevelClient.Connected -= OnUserConnected;
                _lowLevelClient.Connected += OnUserConnected;

                _lowLevelClient.ConnectUser(credentials);

                if (await Task.WhenAny(timeout, taskCompletion.Task) == timeout)
                {
                    throw new TimeoutException("Reached timeout when waiting for client to connect");
                }

                await taskCompletion.Task;
            }

            await ConnectAsync();
            Assert.AreEqual(ConnectionState.Connected, _lowLevelClient.ConnectionState);
            await _lowLevelClient.DisconnectAsync(permanent: true);
            Assert.AreEqual(ConnectionState.Disconnected, _lowLevelClient.ConnectionState);

            await ConnectAsync();
            Assert.AreEqual(ConnectionState.Connected, _lowLevelClient.ConnectionState);
            await _lowLevelClient.DisconnectAsync(permanent: true);
            Assert.AreEqual(ConnectionState.Disconnected, _lowLevelClient.ConnectionState);
            
            await ConnectAsync();
            Assert.AreEqual(ConnectionState.Connected, _lowLevelClient.ConnectionState);
            await _lowLevelClient.DisconnectAsync(permanent: true);
            Assert.AreEqual(ConnectionState.Disconnected, _lowLevelClient.ConnectionState);
            
            await ConnectAsync();
            Assert.AreEqual(ConnectionState.Connected, _lowLevelClient.ConnectionState);
            await _lowLevelClient.DisconnectAsync(permanent: true);
            Assert.AreEqual(ConnectionState.Disconnected, _lowLevelClient.ConnectionState);
            
            await ConnectAsync();
            Assert.AreEqual(ConnectionState.Connected, _lowLevelClient.ConnectionState);
            await _lowLevelClient.DisconnectAsync(permanent: true);
            Assert.AreEqual(ConnectionState.Disconnected, _lowLevelClient.ConnectionState);
        }

        //StreamTodo: Debug why this test is causing NullRef Exception. Check if this is happening outside of test
        // [UnityTest]
        // public IEnumerator When_consumer_cancels_connection_attempt_expect_client_to_terminate_connecting_state()
        // {
        //     yield return When_consumer_cancels_connection_attempt_expect_client_to_terminate_connecting_state_Async()
        //         .RunAsIEnumerator(_lowLevelClient);
        // }
        //
        // private async Task When_consumer_cancels_connection_attempt_expect_client_to_terminate_connecting_state_Async()
        // {
        //     var credentials = StreamTestClients.Instance.LowLevelClientCredentials;
        //
        //     _lowLevelClient.ConnectUser(credentials);
        //
        //     //await Task.Delay(500); // With this delay the Null ref will not occur
        //
        //     await _lowLevelClient.DisconnectAsync(permanent: true);
        //     Assert.AreEqual(ConnectionState.Disconnected, _lowLevelClient.ConnectionState);
        // }
        
        //StreamTodo: assert that the connection will timeout

        private IStreamChatLowLevelClient _lowLevelClient;
    }
}
#endif