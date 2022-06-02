using System.Linq;
using StreamChat.Core;
using UnityEditor;
using UnityEditor.Build;
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
            var activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;
            var group = BuildPipeline.GetBuildTargetGroup(activeBuildTarget);
            var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(group);

            var scriptingDefineSymbols = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);

            var symbols = scriptingDefineSymbols.Split(';').ToList();

            if (symbols.Contains(StreamTestsEnabledCompilerFlag))
            {
                symbols.Remove(StreamTestsEnabledCompilerFlag);
            }
            else
            {
                symbols.Add(StreamTestsEnabledCompilerFlag);
            }

            var newScriptingDefineSymbols = string.Join(";", symbols);

            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, newScriptingDefineSymbols);

            Debug.Log($"Editor scripting define symbols have been modified from: `{scriptingDefineSymbols}` to: `{newScriptingDefineSymbols}` for named build target: `{namedBuildTarget.TargetName}`");
        }
    }
}

