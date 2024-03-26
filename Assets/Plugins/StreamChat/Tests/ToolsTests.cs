#if STREAM_TESTS_ENABLED
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using StreamChat.EditorTools;
using StreamChat.Libs.Auth;
using StreamChat.Libs.Serialization;

namespace StreamChat.Tests
{
    internal class ToolsTests
    {
        [Test]
        public void when_provided_json_base64_test_auth_data_expect_valid_deserialization()
        {
            var list = new List<AuthCredentials>()
            {
                new AuthCredentials("TestApiKeyXYZ", "test-admin", "aaa.bbb.ccc"),
                new AuthCredentials("TestApiKeyXYZ", "test-admin-2", "aaa.bbb.ccc"),
                new AuthCredentials("TestApiKeyXYZ", "test-admin-3", "aaa.bbb.ccc"),
                new AuthCredentials("TestApiKeyXYZ", "test-admin-4", "aaa.bbb.ccc"),
                new AuthCredentials("TestApiKeyXYZ", "test-admin-5", "aaa.bbb.ccc"),
                new AuthCredentials("TestApiKeyXYZ", "test-admin-6", "aaa.bbb.ccc"),
                new AuthCredentials("TestApiKeyXYZ", "test-admin-7", "aaa.bbb.ccc"),
                new AuthCredentials("TestApiKeyXYZ", "test-admin-8", "aaa.bbb.ccc"),
            };

            var user = new AuthCredentials("TestApiKeyXYZ", "test-user",
                "aaa.yyy.zzz");

            var quest = new AuthCredentials("TestApiKeyXYZ", "tests-guest",
                "aaa.yyy.zzz");

            var dataSet = new TestAuthDataSet(list, user, quest);
            var serializer = new NewtonsoftJsonSerializer();

            var serializedDataSet = serializer.Serialize(dataSet);
            var encodedBase64DataSet = Convert.ToBase64String(Encoding.UTF8.GetBytes(serializedDataSet));

            var decodedBase64DataSet = Convert.FromBase64String(encodedBase64DataSet);
            var deserializedDataSet = serializer.Deserialize<TestAuthDataSet>(Encoding.UTF8.GetString(decodedBase64DataSet));

            Assert.NotNull(deserializedDataSet);
            Assert.NotNull(deserializedDataSet.TestAdminData);
            Assert.NotNull(deserializedDataSet.TestGuestData);
            Assert.NotNull(deserializedDataSet.TestUserData);

            Assert.AreEqual(deserializedDataSet.TestAdminData[2].UserId, "test-admin-3");

        }
    }
}
#endif