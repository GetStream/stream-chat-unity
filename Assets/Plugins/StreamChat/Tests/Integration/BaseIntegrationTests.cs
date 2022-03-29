using NUnit.Framework;
using StreamChat.Core;
using StreamChat.Core.Auth;

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
            Client.Dispose();
            Client = null;
        }

        protected const string TestUserId = "integration-tests-role-user";
        protected const string TestAdminId = "integration-tests-role-admin";
        protected const string TestGuestId = "integration-tests-role-guest";

        protected IStreamChatClient Client { get; private set; }
    }
}