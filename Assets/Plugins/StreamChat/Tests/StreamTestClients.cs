#if STREAM_TESTS_ENABLED
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using StreamChat.Core;
using StreamChat.Core.Configs;
using StreamChat.Core.LowLevelClient;
using StreamChat.Core.LowLevelClient.Models;
using StreamChat.Libs.Auth;
using StreamChat.Libs.ChatInstanceRunner;
using Debug = UnityEngine.Debug;
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
            get
            {
                if (_instance == null)
                {
                    _instance = new StreamTestClients();
                }

                return _instance;
            }
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
                    _stateClient = CreateStateClient();
                }

                return _stateClient;
            }
        }

        public StreamChatClient OtherStateClient
        {
            get
            {
                if (_otherStateClient == null)
                {
                    _otherStateClient = CreateStateClient();
                }

                return _otherStateClient;
            }
        }

        public OwnUser LowLevelClientOwnUser { get; private set; }

        public AuthCredentials LowLevelClientCredentials => AdminPrimaryCredentials;

        public AuthCredentials AdminPrimaryCredentials { get; private set; }
        public AuthCredentials AdminSecondaryCredentials { get; private set; }

        public AuthCredentials UserPrimaryCredentials { get; private set; }
        public AuthCredentials UserSecondaryCredentials { get; private set; }

        public IEnumerator ReconnectLowLevelClientClient()
        {
            DisposeLowLevelClient();
            InitLowLevelClient();

            yield return LowLevelClient.WaitForClientToConnect();
        }

        public Task ConnectStateClientAsync() => ConnectStateClientAsync(StateClient, AdminPrimaryCredentials);

        public Task<StreamChatClient> ConnectOtherStateClientAsync()
            => ConnectStateClientAsync(OtherStateClient, AdminSecondaryCredentials);

        private static StreamTestClients _instance;

        private readonly HashSet<object> _locks = new HashSet<object>();

        private IStreamChatLowLevelClient _lowLevelClient;
        private StreamChatClient _stateClient;
        private StreamChatClient _otherStateClient;

        private bool _runFinished;
        
        private Task _updateTask;
        private CancellationTokenSource _updateTaskCts;

        private StreamTestClients()
        {
            UnityTestRunnerCallbacks.RunFinishedCallback += OnRunFinishedCallback;

            var testAuthSets = TestUtils.GetTestAuthCredentials(out var optionalTestDataIndex);
            if (testAuthSets.Admins.Length < 3)
            {
                throw new ArgumentException("At least 3 admin credentials are required");
            }

            // StreamTodo: pass this offset via CLI arg
            const int offset = 20;

            AdminPrimaryCredentials = GetCredentialsFromSet(testAuthSets.Admins, optionalTestDataIndex);
            AdminSecondaryCredentials = GetCredentialsFromSet(testAuthSets.Admins, optionalTestDataIndex + offset);
            UserPrimaryCredentials = GetCredentialsFromSet(testAuthSets.Admins, optionalTestDataIndex);
            UserSecondaryCredentials = GetCredentialsFromSet(testAuthSets.Admins, optionalTestDataIndex + offset);
            
            _updateTaskCts = new CancellationTokenSource();
            _updateTask = UpdateTaskAsync();
        }
        
        private async Task UpdateTaskAsync()
        {
            Debug.LogWarning("UpdateTaskAsync STARTED");
            while (!_updateTaskCts.Token.IsCancellationRequested)
            {
                try
                {
                    _updateTaskCts.Token.ThrowIfCancellationRequested();
                }
                catch (Exception)
                {
                    Debug.LogWarning("UpdateTaskAsync STOPPED");
                    throw;
                }
                
                ((IStreamChatClientEventsListener)_stateClient)?.Update();
                ((IStreamChatClientEventsListener)_otherStateClient)?.Update();
                _lowLevelClient?.Update(0.1f);

                await Task.Delay(1);
            }
        }

        private AuthCredentials GetCredentialsFromSet(AuthCredentials[] set, int? forcedIndex)
        {
            if (forcedIndex.HasValue)
            {
                if (forcedIndex.Value >= set.Length)
                {
                    Debug.LogError($"{nameof(forcedIndex)} is out of range -> given: {forcedIndex}, " +
                                   $"max available: {set.Length - 1}. Using random credentials data instead.");
                }
                else
                {
                    return set[forcedIndex.Value];
                }
            }

            var shuffledSets = set.OrderBy(_ => Random.value);
            return shuffledSets.First();
        }

        private static async Task<StreamChatClient> ConnectStateClientAsync(StreamChatClient client,
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

                client.LowLevelClient.Update(0.1f);
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

            _updateTaskCts.Cancel();
            
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
            _lowLevelClient = StreamChatLowLevelClient.CreateDefaultClient(LowLevelClientCredentials);
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