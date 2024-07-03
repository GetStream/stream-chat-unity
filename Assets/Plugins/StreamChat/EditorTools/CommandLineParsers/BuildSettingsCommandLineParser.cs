using System;
using System.Collections.Generic;
using System.Text;
using StreamChat.EditorTools;
using StreamChat.EditorTools.Builders;
using StreamChat.Libs.Auth;
using StreamChat.Libs.Serialization;
using UnityEditor;

namespace StreamChat.EditorTools.CommandLineParsers
{
    public class BuildSettingsCommandLineParser : CommandLineParserBase<(BuildSettings buildSettings, AuthCredentials authCredentials)>
    {
        public const string ApiCompatibilityArgKey = "-apiCompatibility";
        public const string ScriptingBackendArgKey = "-scriptingBackend";
        public const string BuildTargetPlatformArgKey = "-buildTargetPlatform";
        public const string BuildTargetPathArgKey = "-buildTargetPath";

        public const string StreamBase64TestDataArgKey = "-streamBase64TestDataSet";

        protected override (BuildSettings buildSettings, AuthCredentials authCredentials) Parse(
            IDictionary<string, string> args)
        {
            if (IsAnyRequiredArgMissing(args, out var missingArgsInfo, BuildTargetPlatformArgKey,
                    ApiCompatibilityArgKey, ScriptingBackendArgKey, BuildTargetPathArgKey, StreamBase64TestDataArgKey))
            {
                throw new ArgumentException($"Missing argument: `{missingArgsInfo}`");
            }

            if (!Enum.TryParse<BuildTargetPlatform>(args[BuildTargetPlatformArgKey], ignoreCase: true,
                    out var targetPlatform))
            {
                throw new ArgumentException(
                    $"Failed to parse argument: `{args[BuildTargetPlatformArgKey]}` to enum: {typeof(BuildTargetPlatform)}");
            }

            if (!Enum.TryParse<ApiCompatibility>(args[ApiCompatibilityArgKey], ignoreCase: true,
                    out var apiCompatibility))
            {
                throw new ArgumentException(
                    $"Failed to parse argument: `{args[BuildTargetPlatformArgKey]}` to enum: {typeof(BuildTargetPlatform)}");
            }

            if (!Enum.TryParse<ScriptingBackend>(args[ScriptingBackendArgKey], ignoreCase: true,
                    out var scriptingBackend))
            {
                throw new ArgumentException(
                    $"Failed to parse argument: `{args[BuildTargetPlatformArgKey]}` to enum: {typeof(BuildTargetPlatform)}");
            }

            var buildTargetGroup = GetBuildTargetGroup(targetPlatform);
            var apiCompatibilityLevel = GetApiCompatibilityLevel(apiCompatibility);
            var scriptingImplementation = GetScriptingImplementation(scriptingBackend);
            var targetPath = args[BuildTargetPathArgKey];

            var testAuthDataSet = ParseTestAuthDataSetArg(args);

            return (new BuildSettings(buildTargetGroup, apiCompatibilityLevel, scriptingImplementation, targetPath),
                testAuthDataSet.GetAdminData());
        }

        public TestAuthDataSet ParseTestAuthDataSetArg(IDictionary<string, string> args)
        {
            if (!args.ContainsKey(StreamBase64TestDataArgKey))
            {
                throw new ArgumentException($"Missing argument: `{StreamBase64TestDataArgKey}`");
            }

            var rawTestDataSet = GetTestDataSet(args);
            var serializer = new NewtonsoftJsonSerializer();
            return serializer.Deserialize<TestAuthDataSet>(rawTestDataSet);
        }

        private BuildTargetGroup GetBuildTargetGroup(BuildTargetPlatform targetPlatform)
        {
            if (targetPlatform == BuildTargetPlatform.Standalone)
            {
                return BuildTargetGroup.Standalone;
            }

#if UNITY_STANDALONE_OSX
            return BuildTargetGroup.iOS;
#else
            return BuildTargetGroup.Android;
#endif
        }

        private ApiCompatibilityLevel GetApiCompatibilityLevel(ApiCompatibility apiCompatibility)
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

        private ScriptingImplementation GetScriptingImplementation(ScriptingBackend scriptingBackend)
        {
            switch (scriptingBackend)
            {
                case ScriptingBackend.Mono: return ScriptingImplementation.Mono2x;
                case ScriptingBackend.IL2CPP: return ScriptingImplementation.IL2CPP;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scriptingBackend), scriptingBackend, null);
            }
        }

        private string GetTestDataSet(IDictionary<string, string> args)
        {
            var decodedBytes = Convert.FromBase64String(args[StreamBase64TestDataArgKey]);
            return Encoding.UTF8.GetString(decodedBytes);
        }
    }
}