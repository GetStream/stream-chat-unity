using System;
using System.Linq;
using StreamChat.Core;
using StreamChat.Editor.DefineSymbols;
using UnityEditor;
using UnityEngine;

namespace StreamChat.EditorTools
{
    public static class EditorTools
    {
        public const string MenuPrefix = "Tools/" + StreamChatClient.MenuPrefix;

        public const string StreamTestsEnabledCompilerFlag = "STREAM_TESTS_ENABLED";

        [MenuItem(MenuPrefix + "Toggle Stream Integration & Unit Tests Enabled")]
        public static void ToggleStreamTestsEnabledCompilerFlag()
        {
            var unityDefineSymbols = new UnityDefineSymbolsFactory().CreateDefault();

            var activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;

            var symbols = unityDefineSymbols.GetScriptingDefineSymbols(activeBuildTarget).ToList();

            var prevCombined = string.Join(", ", symbols);

            if (symbols.Contains(StreamTestsEnabledCompilerFlag))
            {
                symbols.Remove(StreamTestsEnabledCompilerFlag);
            }
            else
            {
                symbols.Add(StreamTestsEnabledCompilerFlag);
            }

            var currentCombined = string.Join(", ", symbols);

            unityDefineSymbols.SetScriptingDefineSymbols(activeBuildTarget, symbols);

            Debug.Log($"Editor scripting define symbols have been modified from: `{prevCombined}` to: `{currentCombined}` for named build target: `{Enum.GetName(typeof(BuildTarget), activeBuildTarget)}`");
        }
    }
}

