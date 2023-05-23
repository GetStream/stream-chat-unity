#if STREAM_TESTS_ENABLED
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using StreamChat.Core;
using StreamChat.Core.Configs;
using StreamChat.Core.LowLevelClient;
using StreamChat.Core.LowLevelClient.Models;
using StreamChat.Libs.Auth;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace StreamChat.Tests
{
    /// <summary>
    /// Maintains global instance of stream chat client to be shared across all tests and disposes them once all of the tests are finished
    /// </summary>
    internal class StreamTestClients
    {
        public static StreamTestClients Instance => _instance ??= new StreamTestClients();

        public void AddLock(object owner) => _locks.Add(owner);

        public async Task RemoveLockAsync(object owner)
        {
            _locks.Remove(owner);
            await TryDisposeInstancesAsync();
        }

        public IStreamChatLowLevelClient LowLevelClient
        {
            get
            {
                if (_lowLevelClient == null)
                {
                    InitLowLevelClient();
                }

                return _lowLevelClient;
            }
        }

        public StreamChatClient StateClient => _stateClient ??= CreateStateClient();

        public StreamChatClient OtherStateClient => _otherStateClient ??= CreateStateClient();

        public OwnUser LowLevelClientOwnUser { get; private set; }

        public string OtherUserId => _otherUserCredentials.UserId;
        public IEnumerable<AuthCredentials> OtherUserCredentials => _otherUsersCredentials;

        public IEnumerator ReconnectLowLevelClientClient()
        {
            DisposeLowLevelClient();
            InitLowLevelClient();

            yield return LowLevelClient.WaitForClientToConnect();
        }

        public Task ConnectStateClientAsync() => ConnectStateClientAsync(StateClient, _stateClientCredentials);

        public Task<IStreamChatClient> ConnectOtherStateClientAsync()
            => ConnectStateClientAsync(OtherStateClient, _otherUserCredentials);

        private static StreamTestClients _instance;

        private readonly HashSet<object> _locks = new HashSet<object>();

        private readonly AuthCredentials _lowLevelClientCredentials;
        private readonly AuthCredentials _stateClientCredentials;
        private readonly AuthCredentials _otherUserCredentials;
        private readonly List<AuthCredentials> _otherUsersCredentials;

        private IStreamChatLowLevelClient _lowLevelClient;
        private StreamChatClient _stateClient;
        private StreamChatClient _otherStateClient;

        private bool _runFinished;

        private StreamTestClients()
        {
            UnityTestRunnerCallbacks.RunFinishedCallback += OnRunFinishedCallback;

            var testAuthDataSet = TestUtils.GetTestAuthCredentials();
            if (testAuthDataSet.TestAdminData.Length < 3)
            {
                throw new ArgumentException("At least 3 admin credentials required");
            }

            var adminData = testAuthDataSet.TestAdminData.OrderBy(_ => Random.value).ToList();

            _lowLevelClientCredentials = adminData[0];
            _stateClientCredentials = adminData[1];
            _otherUserCredentials = adminData[2];
            _otherUsersCredentials = adminData.Skip(3).ToList();
        }

        private static async Task<IStreamChatClient> ConnectStateClientAsync(IStreamChatClient client,
            AuthCredentials credentials)
        {
            if (client.IsConnected)
            {
                return client;
            }

            const int timeout = 5000;
            var timer = new Stopwatch();
            timer.Start();
            
            var connectTask = client.ConnectUserAsync(credentials);
            while (!connectTask.IsCompleted)
            {
#if STREAM_DEBUG_ENABLED
                Debug.Log($"Wait for {nameof(StatefulClient)} to connect user with ID: {credentials.UserId}");
#endif

                await Task.Delay(1);

                if (timer.ElapsedMilliseconds > timeout)
                {
                    throw new TimeoutException($"Reached timeout when trying to connect user: {credentials.UserId}");
                }
            }
            
            timer.Stop();

            Debug.Log(
                $"------------ State client connection made: {client.ConnectionState}, user ID: {client.LocalUserData.User.Id} after {timer.Elapsed.TotalSeconds}");
            return client;
        }

        private void OnClientConnected(OwnUser localUser)
        {
            LowLevelClientOwnUser = localUser;
            Debug.Log(
                $"------------ State client connection made: {_lowLevelClient.ConnectionState}, user ID: {LowLevelClientOwnUser.Id}");
        }

        private void OnRunFinishedCallback(ITestResult obj)
        {
            _runFinished = true;
            TryDisposeInstancesAsync();
        }

        private Task TryDisposeInstancesAsync()
        {
            if (!_runFinished || _locks.Any())
            {
                return Task.CompletedTask;
            }

            Debug.Log("------------  Tests finished - dispose client instances");

            DisposeLowLevelClient();
            return DisposeStateClientsAsync();
        }

        private static StreamChatClient CreateStateClient()
            => (StreamChatClient)StreamChatClient.CreateDefaultClient(new StreamClientConfig
            {
                LogLevel = StreamLogLevel.Debug
            });

        private async Task DisposeStateClientsAsync()
        {
            if (_stateClient != null)
            {
                await _stateClient.DisconnectUserAsync();
                _stateClient.Dispose();
                _stateClient = null;
            }

            if (_otherStateClient != null)
            {
                await _otherStateClient.DisconnectUserAsync();
                _otherStateClient.Dispose();
                _otherStateClient = null;
            }
        }

        private void InitLowLevelClient()
        {
            _lowLevelClient = StreamChatLowLevelClient.CreateDefaultClient(_lowLevelClientCredentials);
            _lowLevelClient.Connected += OnClientConnected;
            _lowLevelClient.Connect();
        }

        private void DisposeLowLevelClient()
        {
            if (_lowLevelClient == null)
            {
                return;
            }

            _lowLevelClient.Connected -= OnClientConnected;
            _lowLevelClient.Dispose();
            _lowLevelClient = null;
        }
    }
}
#endif