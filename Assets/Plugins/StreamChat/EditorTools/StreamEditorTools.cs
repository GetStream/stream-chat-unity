using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using StreamChat.EditorTools.Builders;
using StreamChat.Core;
using StreamChat.EditorTools.DefineSymbols;
using UnityEditor;
using UnityEngine;

namespace StreamChat.EditorTools
{
    public static class StreamEditorTools
    {
        [MenuItem(MenuPrefix + "Toggle Stream Integration & Unit Tests Enabled")]
        public static void ToggleStreamTestsEnabledCompilerFlag()
        {
            var unityDefineSymbols = new UnityDefineSymbolsFactory().CreateDefault();

            var activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;

            var symbols = unityDefineSymbols.GetScriptingDefineSymbols(activeBuildTarget).ToList();

            var nextState = !symbols.Contains(StreamTestsEnabledCompilerFlag);

            SetStreamTestsEnabledCompilerFlag(nextState);
        }

        public static void BuildSampleApp()
        {
            var parser = new CommandLineParser();
            var builder = new StreamAppBuilder();

            var (buildSettings, authCredentials) = parser.GetParsedBuildArgs();

            builder.BuildSampleApp(buildSettings, authCredentials);
        }

        public static void EnableStreamTestsEnabledCompilerFlag()
            => SetStreamTestsEnabledCompilerFlag(true);

        public static void SetStreamTestsEnabledCompilerFlag(bool enabled)
        {
            var unityDefineSymbols = new UnityDefineSymbolsFactory().CreateDefault();

            var activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;

            var symbols = unityDefineSymbols.GetScriptingDefineSymbols(activeBuildTarget).ToList();

            var prevCombined = string.Join(", ", symbols);

            if (enabled && !symbols.Contains(StreamTestsEnabledCompilerFlag))
            {
                symbols.Add(StreamTestsEnabledCompilerFlag);
            }

            if (!enabled && symbols.Contains(StreamTestsEnabledCompilerFlag))
            {
                symbols.Remove(StreamTestsEnabledCompilerFlag);
            }

            var currentCombined = string.Join(", ", symbols);

            unityDefineSymbols.SetScriptingDefineSymbols(activeBuildTarget, symbols);

            Debug.Log($"Editor scripting define symbols have been modified from: `{prevCombined}` to: `{currentCombined}` for named build target: `{Enum.GetName(typeof(BuildTarget), activeBuildTarget)}`");
        }

        private const string MenuPrefix = "Tools/" + StreamChatClient.MenuPrefix;

        private const string StreamTestsEnabledCompilerFlag = "STREAM_TESTS_ENABLED";

    }
}

