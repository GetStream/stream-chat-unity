#if STREAM_TESTS_ENABLED
using System.IO;
using StreamChat.EditorTools.CommandLineParsers;
using StreamChat.EditorTools;
using StreamChat.Libs.Auth;
using UnityEngine;

namespace StreamChat.Tests
{
    internal static class TestUtils
    {
        public static TestAuthDataSets GetTestAuthCredentials(out int? forceDataSetIndex)
        {
            forceDataSetIndex = default;
            const string TestAuthDataFilePath = "test_auth_data_xSpgxW.txt";

            if (Application.isBatchMode)
            {
                Debug.Log("Batch mode, expecting data injected through CLI args");

                var parser = new BuildSettingsCommandLineParser();
                var argsDict = parser.GetParsedCommandLineArguments();

                var testAuthDataSet = parser.ParseTestAuthDataSetArg(argsDict, out forceDataSetIndex);

                return testAuthDataSet;
            }

            if (File.Exists(TestAuthDataFilePath))
            {
                var parser = new BuildSettingsCommandLineParser();

                var base64TestData = File.ReadAllText(TestAuthDataFilePath);
                var testAuthDataSet = parser.DeserializeFromBase64(base64TestData);

                return testAuthDataSet;
            }
            
            Debug.LogWarning($"`{TestAuthDataFilePath}` File with credentials sets not found.");

            // Define manually
            const string apiKey = "";
            const string testUserId = "integration-tests-role-user";
            const string testAdminId = "integration-tests-role-admin";

            var userAuthCredentials = new AuthCredentials(
                apiKey: apiKey,
                userId: testUserId,
                userToken: "");

            var adminAuthCredentials = new AuthCredentials(
                apiKey: apiKey,
                userId: testAdminId,
                userToken: "");

            return new TestAuthDataSets(new[] { adminAuthCredentials }, new[] { userAuthCredentials });
        }
    }
}
#endif