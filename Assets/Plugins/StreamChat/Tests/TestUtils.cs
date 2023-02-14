#if STREAM_TESTS_ENABLED
using System;
using System.IO;
using System.Text;
using StreamChat.EditorTools;
using StreamChat.Libs.Auth;
using StreamChat.Libs.Serialization;
using UnityEngine;

namespace StreamChat.Tests
{
    internal static class TestUtils
    {
        // StreamTodo: replace with admin ids fetched from loaded data set
        public const string TestUserId = "integration-tests-role-user";
        public const string TestAdminId = "integration-tests-role-admin";
        public const string TestGuestId = "integration-tests-role-guest";

        public static void GetTestAuthCredentials(out AuthCredentials guestAuthCredentials,
            out AuthCredentials userAuthCredentials, out AuthCredentials adminAuthCredentials,
            out AuthCredentials otherUserAuthCredentials, string forcedAdminId = null)
        {
            var testAuthDataSet = GetTestAuthCredentials();

            guestAuthCredentials = testAuthDataSet.TestGuestData;
            userAuthCredentials = testAuthDataSet.TestUserData;
            adminAuthCredentials = testAuthDataSet.GetAdminData(forcedAdminId);
            otherUserAuthCredentials = testAuthDataSet.GetOtherThan(adminAuthCredentials);
        }

        public static TestAuthDataSet GetTestAuthCredentials()
        {
            const string TestAuthDataFilePath = "test_auth_data_xSpgxW.txt";

            if (Application.isBatchMode)
            {
                Debug.Log("Batch mode, expecting data injected through CLI args");

                var parser = new CommandLineParser();
                var argsDict = parser.GetParsedCommandLineArguments();

                var testAuthDataSet = parser.ParseTestAuthDataSetArg(argsDict);

                Debug.Log("Data deserialized correctly. Sample: " + testAuthDataSet.TestAdminData[0].UserId);

                return testAuthDataSet;
            }

            if (File.Exists(TestAuthDataFilePath))
            {
                var serializer = new NewtonsoftJsonSerializer();

                var base64TestData = File.ReadAllText(TestAuthDataFilePath);
                var decodedJsonTestData = Convert.FromBase64String(base64TestData);

                var testAuthDataSet =
                    serializer.Deserialize<TestAuthDataSet>(Encoding.UTF8.GetString(decodedJsonTestData));

                Debug.Log("Data deserialized correctly. Sample: " + testAuthDataSet.TestAdminData[0].UserId);

                return testAuthDataSet;
            }

            //Define manually

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

            var otherUserAuthCredentials = new AuthCredentials(
                apiKey: ApiKey,
                userId: "",
                userToken: "");

            return new TestAuthDataSet(new[] { adminAuthCredentials }, userAuthCredentials, guestAuthCredentials);
        }
    }
}
#endif