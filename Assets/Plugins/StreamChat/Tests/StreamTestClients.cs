#if STREAM_TESTS_ENABLED
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using StreamChat.Core;
using StreamChat.Core.Configs;
using StreamChat.Core.LowLevelClient;
using StreamChat.Core.LowLevelClient.Models;
using StreamChat.Libs.Auth;
using UnityEngine;
using Random = UnityEngine.Random;

namespace StreamChat.Tests
{
    /// <summary>
    /// Maintains global instance of stream chat client to be shared across all tests and disposes them once all of the tests are finished
    /// </summary>
    internal class StreamTestClients
    {
        public static StreamTestClients Instance
        {
            get { return _instance ??= new StreamTestClients(); }
        }

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

        public StreamChatClient StateClient
        {
            get
            {
                if (_stateClient == null)
                {
                    InitStateClient();
                }

                return _stateClient;
            }
        }

        public OwnUser LowLevelClientOwnUser { get; private set; }

        public string OtherUserId => _otherUserCredentials.UserId;
        public IEnumerable<AuthCredentials> OtherUserCredentials => _otherUsersCredentials;

        public IEnumerator ReconnectLowLevelClientClient()
        {
            DisposeLowLevelClient();
            InitLowLevelClient();

            yield return LowLevelClient.WaitForClientToConnect();
        }

        public async Task TryConnectStateClientAsync()
        {
            if (StateClient.IsConnected)
            {
                return;
            }

            var connectTask = StateClient.ConnectUserAsync(_stateClientCredentials);
            while (!connectTask.IsCompleted)
            {
#if STREAM_DEBUG_ENABLED
                Debug.Log($"Wait for {nameof(StatefulClient)} to connect");
#endif

                await Task.Delay(1);
            }

            Debug.Log(
                $"------------ State client connection made: {StateClient.ConnectionState}, user ID: {StateClient.LocalUserData.User.Id}");
        }

        private static StreamTestClients _instance;

        private readonly HashSet<object> _locks = new HashSet<object>();

        private readonly AuthCredentials _lowLevelClientCredentials;
        private readonly AuthCredentials _stateClientCredentials;
        private readonly AuthCredentials _otherUserCredentials;
        private readonly List<AuthCredentials> _otherUsersCredentials;

        private IStreamChatLowLevelClient _lowLevelClient;
        private StreamChatClient _stateClient;

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
            return DisposeStateClientAsync();
        }

        private void InitStateClient()
        {
            _stateClient = (StreamChatClient)StreamChatClient.CreateDefaultClient(new StreamClientConfig
            {
                LogLevel = StreamLogLevel.Debug
            });
        }

        private async Task DisposeStateClientAsync()
        {
            if (_stateClient == null)
            {
                return;
            }

            await _stateClient.DisconnectUserAsync();

            _stateClient.Dispose();
            _stateClient = null;
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