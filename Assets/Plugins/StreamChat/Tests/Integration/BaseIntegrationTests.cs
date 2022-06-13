#if STREAM_TESTS_ENABLED
using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using StreamChat.Core;
using StreamChat.Core.Models;
using StreamChat.Core.Requests;
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
        public const string StreamTestDataArgKey = "-StreamTestDataSet";

        [OneTimeSetUp]
        public void Up()
        {
            Debug.Log("------------ Up");

            AuthCredentials guestAuthCredentials, userAuthCredentials, adminAuthCredentials = default;

            if (Application.isBatchMode)
            {
                //Get from environment

                var args = new Dictionary<string, string>();
                EditorTools.StreamEditorTools.ParseEnvArgs(Environment.GetCommandLineArgs(), args);

                if (!args.ContainsKey(StreamTestDataArgKey))
                {
                    throw new ArgumentException("Missing environment arg with key: " + StreamTestDataArgKey);
                }

                var serializer = new NewtonsoftJsonSerializer();
                var testAuthDataSet = serializer.Deserialize<TestAuthDataSet>(args[StreamTestDataArgKey]);

                guestAuthCredentials = testAuthDataSet.TestGuestData;
                userAuthCredentials = testAuthDataSet.TestUserData;
                adminAuthCredentials = testAuthDataSet.TestAdminData;

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
            }

            Client = StreamChatClient.CreateDefaultClient(adminAuthCredentials);
            Client.Connect();
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
        ///  Create temp channel with random id that will be removed in [TearDown]
        /// </summary>
        protected IEnumerator CreateTempUniqueChannel(string channelType,
            ChannelGetOrCreateRequest channelGetOrCreateRequest, Action<ChannelState> onChannelReturned)
        {
            var channelId = "random-channel-" + Guid.NewGuid();

            var createChannelTask =
                Client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId, channelGetOrCreateRequest);

            yield return createChannelTask.RunAsIEnumerator(response =>
            {
                _tempChannelsToDelete.Add((response.Channel.Type, response.Channel.Id));
                onChannelReturned(response);
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
    }
}
#endif