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
        public const string TestUserId = "integration-tests-role-user";
        public const string TestAdminId = "integration-tests-role-admin";
        public const string TestGuestId = "integration-tests-role-guest";

        public static void GetTestAuthCredentials(out AuthCredentials guestAuthCredentials,
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