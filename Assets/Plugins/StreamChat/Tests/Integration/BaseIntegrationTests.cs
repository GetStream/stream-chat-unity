#if STREAM_TESTS_ENABLED
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using StreamChat.Core;
using StreamChat.Core.Models;
using StreamChat.Core.Requests;
using StreamChat.Core.Responses;
using StreamChat.EditorTools;
using StreamChat.Libs;
using StreamChat.Libs.Auth;
using StreamChat.Libs.Serialization;
using StreamChat.Libs.Utils;
using UnityEditor;
using UnityEngine;

namespace StreamChat.Tests.Integration
{
    /// <summary>
    /// Base class for integration tests that operate on API and make real API requests
    /// </summary>
    public abstract class BaseIntegrationTests
    {
        [OneTimeSetUp]
        public void Up()
        {
            Debug.Log("------------ Up");

            InitClientAndConnect();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Debug.Log("------------ TearDown");

            DeleteTempChannels();

            Client.Dispose();
            Client = null;
        }

        protected const string TestUserId = "integration-tests-role-user";
        protected const string TestAdminId = "integration-tests-role-admin";
        protected const string TestGuestId = "integration-tests-role-guest";

        protected IStreamChatClient Client { get; private set; }

        /// <summary>
        /// Id of other user than currently logged one
        /// </summary>
        protected string OtherUserId { get; private set; }

        /// <summary>
        ///  Create temp channel with random id that will be removed in [TearDown]
        /// </summary>
        protected IEnumerator CreateTempUniqueChannel(string channelType,
            ChannelGetOrCreateRequest channelGetOrCreateRequest, Action<ChannelState> onChannelReturned = null)
        {
            var channelId = "random-channel-" + Guid.NewGuid();

            var createChannelTask =
                Client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId, channelGetOrCreateRequest);

            yield return createChannelTask.RunAsIEnumerator(response =>
            {
                _tempChannelsToDelete.Add((response.Channel.Type, response.Channel.Id));
                onChannelReturned?.Invoke(response);
            });
        }

        protected IEnumerator InternalWaitForSeconds(float seconds)
        {
            if (seconds <= 0)
            {
                yield break;
            }

            var currentTime = EditorApplication.timeSinceStartup;

            while ((EditorApplication.timeSinceStartup - currentTime) < seconds)
            {
                yield return null;
            }
        }

        protected IEnumerator SendTestMessages(ChannelState channelState, int count,
            Action<(int Index, MessageResponse MessageResponse)> onMessageSent, string messagePrefix = "")
        {
            for (int i = 0; i < count; i++)
            {
                var sendMessageTask = Client.MessageApi.SendNewMessageAsync(channelState.Channel.Type,
                    channelState.Channel.Id, new SendMessageRequest
                    {
                        Message = new MessageRequest
                        {
                            Text = messagePrefix + " #" + i
                        }
                    });

                yield return sendMessageTask.RunAsIEnumerator(messageResponse =>
                {
                    onMessageSent?.Invoke((i, messageResponse));
                });
            }
        }

        private readonly List<(string ChannelType, string ChannelId)> _tempChannelsToDelete =
            new List<(string ChannelType, string ChannelId)>();

        private void DeleteTempChannels()
        {
            if (_tempChannelsToDelete.Count == 0)
            {
                return;
            }

            var unityLogs = LibsFactory.CreateDefaultLogs();

            var cids = new List<string>();

            foreach (var (channelType, channelId) in _tempChannelsToDelete)
            {
                cids.Add($"{channelType}:{channelId}");
            }

            _tempChannelsToDelete.Clear();

            Client.ChannelApi.DeleteChannelsAsync(new DeleteChannelsRequest
            {
                Cids = cids,
                HardDelete = true
            }).LogIfFailed(unityLogs);
        }

        private void InitClientAndConnect(string forcedAdminId = null)
        {
            GetTestAuthCredentials(out var guestAuthCredentials, out var userAuthCredentials,
                out var adminAuthCredentials, out var otherUserAuthCredentials, forcedAdminId);

            OtherUserId = otherUserAuthCredentials.UserId;

            Client = StreamChatClient.CreateDefaultClient(adminAuthCredentials);
            Client.Connect();
        }

        protected IEnumerator ReconnectClient()
        {
            var userId = Client.UserId;
            Client.Dispose();
            InitClientAndConnect(userId);

            yield return Client.WaitForClientToConnect();
        }

        private static void GetTestAuthCredentials(out AuthCredentials guestAuthCredentials,
            out AuthCredentials userAuthCredentials, out AuthCredentials adminAuthCredentials,
            out AuthCredentials otherUserAuthCredentials, string forcedAdminId = null)
        {
            const string TestAuthDataFilePath = "test_auth_data_xSpgxW.txt";

            if (Application.isBatchMode)
            {
                Debug.Log("Batch mode, expecting data injected through CLI args");

                var parser = new CommandLineParser();
                var argsDict = parser.GetParsedCommandLineArguments();

                var testAuthDataSet = parser.ParseTestAuthDataSetArg(argsDict);

                Debug.Log("Data deserialized correctly. Sample: " + testAuthDataSet.TestAdminData[0].UserId);

                guestAuthCredentials = testAuthDataSet.TestGuestData;
                userAuthCredentials = testAuthDataSet.TestUserData;
                adminAuthCredentials = testAuthDataSet.GetAdminData(forcedAdminId);
                otherUserAuthCredentials = testAuthDataSet.GetOtherThan(adminAuthCredentials);
            }
            else if (File.Exists(TestAuthDataFilePath))
            {
                var serializer = new NewtonsoftJsonSerializer();

                var base64TestData = File.ReadAllText(TestAuthDataFilePath);
                var decodedJsonTestData = Convert.FromBase64String(base64TestData);

                var testAuthDataSet =
                    serializer.Deserialize<TestAuthDataSet>(Encoding.UTF8.GetString(decodedJsonTestData));

                Debug.Log("Data deserialized correctly. Sample: " + testAuthDataSet.TestAdminData[0].UserId);

                guestAuthCredentials = testAuthDataSet.TestGuestData;
                userAuthCredentials = testAuthDataSet.TestUserData;
                adminAuthCredentials = testAuthDataSet.GetAdminData(forcedAdminId);
                otherUserAuthCredentials = testAuthDataSet.GetOtherThan(adminAuthCredentials);
            }
            else
            {
                //Define manually

                const string ApiKey = "";

                guestAuthCredentials = new AuthCredentials(
                    apiKey: ApiKey,
                    userId: TestGuestId,
                    userToken: "");

                userAuthCredentials = new AuthCredentials(
                    apiKey: ApiKey,
                    userId: TestUserId,
                    userToken: "");

                adminAuthCredentials = new AuthCredentials(
                    apiKey: ApiKey,
                    userId: TestAdminId,
                    userToken: "");

                otherUserAuthCredentials = new AuthCredentials(
                    apiKey: ApiKey,
                    userId: "",
                    userToken: "");
                ;
            }
        }
    }
}
#endif