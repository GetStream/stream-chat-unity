using System;
using System.IO;
using StreamChat.Libs.Auth;
using UnityEditor;
using UnityEngine;

namespace StreamChat.EditorTools.Builders
{
    public class StreamAppBuilder
    {
        public void BuildSampleApp(BuildSettings settings, AuthCredentials authCredentials)
        {
            var buildTarget = GetBuildTargetFromGroup(settings.BuildTargetGroup);

            PlayerSettings.SetApiCompatibilityLevel(settings.BuildTargetGroup, settings.ApiCompatibilityLevel);
            PlayerSettings.SetScriptingBackend(settings.BuildTargetGroup, settings.ScriptingImplementation);
            EditorUserBuildSettings.SwitchActiveBuildTarget(settings.BuildTargetGroup, buildTarget);
            EditorUserBuildSettings.SetBuildLocation(buildTarget, settings.TargetPath);

            var sceneAssetPath = AssetDatabase.GUIDToAssetPath(SampleAppSceneGuid);
            if (!File.Exists(sceneAssetPath))
            {
                throw new ArgumentException($"Failed to find scene with guid: `{SampleAppSceneGuid}` and path: `{sceneAssetPath}`");
            }

            var configAssetPath = AssetDatabase.GUIDToAssetPath(SampleAppCredentialsAssetGuid);
            if (!File.Exists(configAssetPath))
            {
                throw new ArgumentException($"Failed to find scene with guid: `{SampleAppCredentialsAssetGuid}` and path: `{configAssetPath}`");
            }

            var configAsset = AssetDatabase.LoadAssetAtPath<AuthCredentialsAsset>(configAssetPath);
            configAsset.SetData(authCredentials);
            EditorUtility.SetDirty(configAsset);
            AssetDatabase.SaveAssets();

            var options = new BuildPlayerOptions
            {
                scenes = new string[]
                {
                    sceneAssetPath
                },
                locationPathName = settings.TargetPath,
                targetGroup = settings.BuildTargetGroup,
                target = buildTarget,
            };

            Debug.Log("Building sample app with settings: " + settings);

            BuildPipeline.BuildPlayer(options);
        }

        private const string SampleAppSceneGuid = "78fbad76b0116d442a58c1552d9de372";
        private const string SampleAppCredentialsAssetGuid = "aa176142597826141af2043db51cba28";

        private static BuildTarget GetBuildTargetFromGroup(BuildTargetGroup buildTargetGroup)
        {
            if (buildTargetGroup == BuildTargetGroup.Android)
            {
                return BuildTarget.Android;
            }
            if (buildTargetGroup == BuildTargetGroup.iOS)
            {
                return BuildTarget.iOS;
            }
            if (buildTargetGroup == BuildTargetGroup.Standalone)
            {
#if UNITY_EDITOR_WIN
                return BuildTarget.StandaloneWindows64;
#elif UNITY_EDITOR_OSX
                return BuildTarget.StandaloneOSX;

#elif UNITY_EDITOR_LINUX
                return BuildTarget.StandaloneLinux64;
#endif
            }

            throw new NotImplementedException(buildTargetGroup.ToString());
        }
    }
}