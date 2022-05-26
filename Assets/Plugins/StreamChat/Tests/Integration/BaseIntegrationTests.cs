using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using StreamChat.Core;
using StreamChat.Core.Models;
using StreamChat.Core.Requests;
using StreamChat.Libs;
using StreamChat.Libs.Auth;
using StreamChat.Libs.Utils;

namespace StreamChat.Tests.Integration
{
    /// <summary>
    /// Base class for integration tests that operate on API and make real API requests
    /// </summary>
    public abstract class BaseIntegrationTests
    {
        [SetUp]
        public void Up()
        {
            const string ApiKey = "";

            var guestAuthCredentials = new AuthCredentials(
                apiKey: ApiKey,
                userId: TestGuestId,
                userToken: "");

            var userAuthCredentials = new AuthCredentials(
                apiKey: ApiKey,
                userId: TestUserId,
                userToken: "");

            var adminAuthCredentials = new AuthCredentials(
                apiKey: ApiKey,
                userId: TestAdminId,
                userToken: "");

            Client = StreamChatClient.CreateDefaultClient(adminAuthCredentials);
            Client.Connect();
        }

        [TearDown]
        public void TearDown()
        {
            DeleteTempChannels();

            Client.Dispose();
            Client = null;
        }

        protected const string TestUserId = "integration-tests-role-user";
        protected const string TestAdminId = "integration-tests-role-admin";
        protected const string TestGuestId = "integration-tests-role-guest";

        protected IStreamChatClient Client { get; private set; }

        /// <summary>
        ///  Create temp channel with random name that will be removed in [TearDown]
        /// </summary>
        protected IEnumerator CreateTempUniqueChannel(string channelType, Action<ChannelState> onChannelReturned)
        {
            var channelId = "random-channel-" + Guid.NewGuid();

            var createChannelTask =
                Client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId, new ChannelGetOrCreateRequest());

            yield return createChannelTask.RunAsIEnumerator(response =>
            {
                _tempChannelsToDelete.Add((response.Channel.Type, response.Channel.Id));
                onChannelReturned(response);
            });
        }

        private readonly List<(string ChannelType, string ChannelId)> _tempChannelsToDelete = new List<(string ChannelType, string ChannelId)>();

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

            Client.ChannelApi.DeleteChannelsAsync(new DeleteChannelsRequest
            {
                Cids = cids,
                HardDelete = true
            }).LogIfFailed(unityLogs);
        }
    }
}