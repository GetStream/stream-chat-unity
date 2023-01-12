using System;
using System.Linq;
using StreamChat.Core.LowLevelClient;
using StreamChat.EditorTools.Builders;
using StreamChat.EditorTools.DefineSymbols;
using UnityEditor;
using UnityEngine;

namespace StreamChat.EditorTools
{
    public static class StreamEditorTools
    {
        [MenuItem(MenuPrefix + "Toggle " + StreamTestsEnabledCompilerFlag + " compiler flag")]
        public static void ToggleStreamTestsEnabledCompilerFlag() => ToggleCompilerFlag(StreamTestsEnabledCompilerFlag);

        [MenuItem(MenuPrefix + "Toggle " + StreamDebugModeEnabledCompilerFlag + " compiler flag")]
        public static void ToggleStreamDebugModeCompilerFlag()
            => ToggleCompilerFlag(StreamDebugModeEnabledCompilerFlag);

        [MenuItem(MenuPrefix + "Open " + nameof(SpriteAtlasUtilityEditor))]
        public static void ShowSpriteAtlasUtilityEditorWindow()
        {
            var window = EditorWindow.GetWindow<SpriteAtlasUtilityEditor>();
            window.Show();
        }

        [MenuItem(MenuPrefix + "Open " + nameof(StreamPackageExportEditor))]
        public static void ShowStreamPackageExportEditorWindow()
        {
            var window = EditorWindow.GetWindow<StreamPackageExportEditor>();
            window.Show();
        }

        public static void BuildSampleApp()
        {
            var parser = new CommandLineParser();
            var builder = new StreamAppBuilder();

            var (buildSettings, authCredentials) = parser.GetParsedBuildArgs();

            builder.BuildSampleApp(buildSettings, authCredentials);
        }

        public static void EnableStreamTestsEnabledCompilerFlag()
            => SetStreamTestsEnabledCompilerFlag(StreamTestsEnabledCompilerFlag, true);

        private static void ToggleCompilerFlag(string flagKeyword)
        {
            var unityDefineSymbols = new UnityDefineSymbolsFactory().CreateDefault();

            var activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;

            var symbols = unityDefineSymbols.GetScriptingDefineSymbols(activeBuildTarget).ToList();

            var nextState = !symbols.Contains(flagKeyword);

            SetStreamTestsEnabledCompilerFlag(flagKeyword, nextState);
        }

        public static void SetStreamTestsEnabledCompilerFlag(string flagKeyword, bool enabled)
        {
            var unityDefineSymbols = new UnityDefineSymbolsFactory().CreateDefault();

            var activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;

            var symbols = unityDefineSymbols.GetScriptingDefineSymbols(activeBuildTarget).ToList();

            var prevCombined = string.Join(", ", symbols);

            if (enabled && !symbols.Contains(flagKeyword))
            {
                symbols.Add(flagKeyword);
            }

            if (!enabled && symbols.Contains(flagKeyword))
            {
                symbols.Remove(flagKeyword);
            }

            var currentCombined = string.Join(", ", symbols);

            unityDefineSymbols.SetScriptingDefineSymbols(activeBuildTarget, symbols);

            Debug.Log(
                $"Editor scripting define symbols have been modified from: `{prevCombined}` to: `{currentCombined}` for named build target: `{Enum.GetName(typeof(BuildTarget), activeBuildTarget)}`");
        }

        private const string MenuPrefix = "Tools/" + StreamChatLowLevelClient.MenuPrefix;

        private const string StreamTestsEnabledCompilerFlag = "STREAM_TESTS_ENABLED";
        private const string StreamDebugModeEnabledCompilerFlag = "STREAM_DEBUG_ENABLED";
    }
}