using System;
using System.Collections.Generic;
using System.Linq;
using StreamChat.Core;
using StreamChat.EditorTools.DefineSymbols;
using UnityEditor;
using UnityEngine;

namespace StreamChat.EditorTools
{
    public static class StreamEditorTools
    {
        public const string MenuPrefix = "Tools/" + StreamChatClient.MenuPrefix;

        public const string StreamTestsEnabledCompilerFlag = "STREAM_TESTS_ENABLED";

        [MenuItem(MenuPrefix + "Toggle Stream Integration & Unit Tests Enabled")]
        public static void ToggleStreamTestsEnabledCompilerFlag()
        {
            var unityDefineSymbols = new UnityDefineSymbolsFactory().CreateDefault();

            var activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;

            var symbols = unityDefineSymbols.GetScriptingDefineSymbols(activeBuildTarget).ToList();

            var nextState = !symbols.Contains(StreamTestsEnabledCompilerFlag);

            SetStreamTestsEnabledCompilerFlag(nextState);
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

        public static void ParseEnvArgs(string[] args, IDictionary<string, string> result)
            => ParseEnvArgs(args, _ => result.Add(_.Key, _.Value));

        public static void ParseEnvArgs(string[] args, Action<(string Key, string Value)> onArgumentParsed)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("-"))
                {
                    var key = args[i];
                    var value = i < args.Length - 1 ? args[i + 1] : "";

                    onArgumentParsed?.Invoke((key, value));
                }
            }
        }
    }
}

