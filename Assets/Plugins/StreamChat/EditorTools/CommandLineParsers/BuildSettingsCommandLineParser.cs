using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using StreamChat.EditorTools.Builders;
using StreamChat.Libs.Auth;
using StreamChat.Libs.Serialization;
using UnityEditor;
using UnityEngine;

namespace StreamChat.EditorTools.CommandLineParsers
{
    public class
        BuildSettingsCommandLineParser : CommandLineParserBase<(BuildSettings buildSettings, AuthCredentials
        authCredentials)>
    {
        public const string ApiCompatibilityArgKey = "-apiCompatibility";
        public const string ScriptingBackendArgKey = "-scriptingBackend";
        public const string BuildTargetPlatformArgKey = "-buildTargetPlatform";
        public const string BuildTargetPathArgKey = "-buildTargetPath";

        public const string StreamBase64TestDataArgKey = "-streamBase64TestDataSet";
        public const string TestDataSetIndexArgKey = "-testDataSetIndex";

        protected override (BuildSettings buildSettings, AuthCredentials authCredentials) Parse(
            IDictionary<string, string> args)
        {
            if (IsAnyRequiredArgMissing(args, out var missingArgsInfo, BuildTargetPlatformArgKey,
                    ApiCompatibilityArgKey, ScriptingBackendArgKey, BuildTargetPathArgKey, StreamBase64TestDataArgKey))
            {
                throw new ArgumentException($"Missing argument: `{missingArgsInfo}`");
            }

            if (!TryParseEnum<BuildTargetPlatform>(args[BuildTargetPlatformArgKey], ignoreCase: true,
                    out var targetPlatform))
            {
                throw new ArgumentException(
                    $"Failed to parse argument: `{args[BuildTargetPlatformArgKey]}` to enum: {typeof(BuildTargetPlatform)}");
            }

            if (!TryParseEnum<ApiCompatibility>(args[ApiCompatibilityArgKey], ignoreCase: true,
                    out var apiCompatibility))
            {
                throw new ArgumentException(
                    $"Failed to parse argument: `{args[BuildTargetPlatformArgKey]}` to enum: {typeof(BuildTargetPlatform)}");
            }

            if (!TryParseEnum<ScriptingBackend>(args[ScriptingBackendArgKey], ignoreCase: true,
                    out var scriptingBackend))
            {
                throw new ArgumentException(
                    $"Failed to parse argument: `{args[BuildTargetPlatformArgKey]}` to enum: {typeof(BuildTargetPlatform)}");
            }

            var buildTargetGroup = GetBuildTargetGroup(targetPlatform);
            var apiCompatibilityLevel = GetApiCompatibilityLevel(apiCompatibility);
            var scriptingImplementation = GetScriptingImplementation(scriptingBackend);
            var targetPath = args[BuildTargetPathArgKey];

            var testAuthDataSet = ParseTestAuthDataSetArg(args, out var optionalTestDataIndex);

            return (new BuildSettings(buildTargetGroup, apiCompatibilityLevel, scriptingImplementation, targetPath),
                testAuthDataSet.GetAdminData(forceIndex: optionalTestDataIndex));
        }

        public TestAuthDataSets ParseTestAuthDataSetArg(IDictionary<string, string> args, out int? forceDataSetIndex)
        {
            if (!args.ContainsKey(StreamBase64TestDataArgKey))
            {
                throw new ArgumentException($"Tests CLI - Missing argument: `{StreamBase64TestDataArgKey}`");
            }

            if (!args.ContainsKey(TestDataSetIndexArgKey))
            {
                Debug.LogWarning($"Tests CLI - Missing argument: {nameof(TestDataSetIndexArgKey)}. Ignored");
            }

            forceDataSetIndex = GetOptionalTestDataIndex();
            var base64TestAuthDataSet = args[StreamBase64TestDataArgKey];

            return DeserializeFromBase64(base64TestAuthDataSet);

            int? GetOptionalTestDataIndex()
            {
                if (!args.TryGetValue(TestDataSetIndexArgKey, out var arg))
                {
                    return default;
                }

                return int.Parse(arg);
            }
        }

        public TestAuthDataSets DeserializeFromBase64(string urlSafeBase64)
        {
            Debug.Log($"Test Data Set. URL-safe Base 64 encoded length: {urlSafeBase64.Length}");

            var base64Set = UrlSafeBase64ToBase64(urlSafeBase64);
            var decodedBytes = Convert.FromBase64String(base64Set);

            var decodedString = DecompressString(decodedBytes);
            Debug.Log($"Test Data Set. Decompressed to UTF8 string length: {decodedString.Length}");

            var serializer = new NewtonsoftJsonSerializer();
            var testAuthDataSet = serializer.Deserialize<TestAuthDataSets>(decodedString);
            Debug.Log(
                $"Test Data Set. Admin sets: {testAuthDataSet.Admins.Length}, User sets: {testAuthDataSet.Users.Length}");

            return testAuthDataSet;
        }

        private static string UrlSafeBase64ToBase64(string urlSafeBase64)
        {
            var result = urlSafeBase64.Replace('_', '/').Replace('-', '+');
            switch (urlSafeBase64.Length % 4)
            {
                case 2:
                    result += "==";
                    break;
                case 3:
                    result += "=";
                    break;
            }

            return result;
        }

        private static string DecompressString(byte[] bytes)
        {
            using (var memoryStream = new MemoryStream(bytes))
            {
                using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    using (var reader = new StreamReader(gzipStream, Encoding.UTF8))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }

        private static BuildTargetGroup GetBuildTargetGroup(BuildTargetPlatform targetPlatform)
        {
            switch (targetPlatform)
            {
                case BuildTargetPlatform.Standalone: return BuildTargetGroup.Standalone;
                case BuildTargetPlatform.Mobile:
#if UNITY_STANDALONE_OSX
                    return BuildTargetGroup.iOS;
#else
                    return BuildTargetGroup.Android;
#endif
                case BuildTargetPlatform.Android: return BuildTargetGroup.Android;
                case BuildTargetPlatform.IOS: return BuildTargetGroup.iOS;
                default:
                    throw new ArgumentOutOfRangeException(nameof(targetPlatform), targetPlatform, null);
            }
        }

        private static ApiCompatibilityLevel GetApiCompatibilityLevel(ApiCompatibility apiCompatibility)
        {
#if UNITY_2021
            switch (apiCompatibility)
            {
                case ApiCompatibility.NET_4_x: return ApiCompatibilityLevel.NET_Unity_4_8;
                case ApiCompatibility.STANDARD_2_x: return ApiCompatibilityLevel.NET_Standard_2_0;
                default:
                    throw new ArgumentOutOfRangeException(nameof(apiCompatibility), apiCompatibility, null);
            }

#else
            switch (apiCompatibility)
            {
                case ApiCompatibility.NET_4_x: return ApiCompatibilityLevel.NET_4_6;
                case ApiCompatibility.STANDARD_2_x: return ApiCompatibilityLevel.NET_Standard_2_0;
                default:
                    throw new ArgumentOutOfRangeException(nameof(apiCompatibility), apiCompatibility, null);
            }

#endif
        }

        private static ScriptingImplementation GetScriptingImplementation(ScriptingBackend scriptingBackend)
        {
            switch (scriptingBackend)
            {
                case ScriptingBackend.Mono: return ScriptingImplementation.Mono2x;
                case ScriptingBackend.IL2CPP: return ScriptingImplementation.IL2CPP;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scriptingBackend), scriptingBackend, null);
            }
        }

        private static bool TryParseEnum<TEnum>(string rawValue, bool ignoreCase, out TEnum value) where TEnum : struct
        {
            Debug.Log($"Try parse `{rawValue}` to enum of type `{typeof(TEnum)}`");
            return Enum.TryParse(rawValue, ignoreCase, out value);
        }
    }
}